using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;

namespace pdfZ
{
    public class PdfService
    {
        // 1. Comprimir
        public async Task ComprimirArquivoAsync(string entrada, string saida, string caminhoGS, string nivelQualidade)
        {
            await Task.Run(() =>
            {
                if (!File.Exists(caminhoGS))
                    throw new FileNotFoundException("O executável 'gswin64c.exe' não foi encontrado. Verifique a pasta 'motor_gs'.");

                string configSettings;
                if (nivelQualidade == "EXTREME")
                {
                    configSettings = "-dPDFSETTINGS=/screen " +
                                     "-dColorImageDownsampleType=/Bicubic -dColorImageResolution=50 " +
                                     "-dGrayImageDownsampleType=/Bicubic -dGrayImageResolution=50 " +
                                     "-dMonoImageDownsampleType=/Bicubic -dMonoImageResolution=50";
                }
                else
                {
                    configSettings = $"-dPDFSETTINGS={nivelQualidade}";
                }

                var args = $"-sDEVICE=pdfwrite -dCompatibilityLevel=1.4 {configSettings} -dNOPAUSE -dQUIET -dBATCH -sOutputFile=\"{saida}\" \"{entrada}\"";

                var psi = new ProcessStartInfo(caminhoGS, args)
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                using (var p = Process.Start(psi)) p.WaitForExit();
            });
        }

        // 2. Juntar PDFs
        public async Task JuntarArquivosAsync(string[] arquivos, string saida)
        {
            await Task.Run(() =>
            {
                using (var outputDocument = new PdfDocument())
                {
                    foreach (var arquivo in arquivos)
                    {
                        using (var inputDocument = PdfReader.Open(arquivo, PdfDocumentOpenMode.Import))
                        {
                            int count = inputDocument.PageCount;
                            for (int idx = 0; idx < count; idx++)
                            {
                                outputDocument.AddPage(inputDocument.Pages[idx]);
                            }
                        }
                    }
                    outputDocument.Save(saida);
                }
            });
        }

        // 3. Dividir PDF
        public async Task DividirArquivoAsync(string entrada, string pastaSaida)
        {
            await Task.Run(() =>
            {
                string nomeBase = Path.GetFileNameWithoutExtension(entrada);
                using (var inputDocument = PdfReader.Open(entrada, PdfDocumentOpenMode.Import))
                {
                    for (int i = 0; i < inputDocument.PageCount; i++)
                    {
                        using (var outputDocument = new PdfDocument())
                        {
                            outputDocument.AddPage(inputDocument.Pages[i]);
                            outputDocument.Save(Path.Combine(pastaSaida, $"{nomeBase}_pag_{i + 1}.pdf"));
                        }
                    }
                }
            });
        }

        // 4. Rotacionar
        public async Task RotacionarArquivoAsync(string entrada, string saida, int angulo)
        {
            await Task.Run(() =>
            {
                using (var inputDocument = PdfReader.Open(entrada, PdfDocumentOpenMode.Import))
                using (var outputDocument = new PdfDocument())
                {
                    foreach (var page in inputDocument.Pages)
                    {
                        page.Rotate = (page.Rotate + angulo) % 360;
                        outputDocument.AddPage(page);
                    }
                    outputDocument.Save(saida);
                }
            });
        }

        // 5. Imagem para PDF
        public async Task ImagensParaPdfAsync(string[] imagens, string saida)
        {
            await Task.Run(() =>
            {
                using (var doc = new PdfDocument())
                {
                    foreach (var imgFile in imagens)
                    {
                        var page = doc.AddPage();
                        using (var fs = new FileStream(imgFile, FileMode.Open, FileAccess.Read))
                        using (var xImage = XImage.FromStream(fs))
                        using (var gfx = XGraphics.FromPdfPage(page))
                        {
                            double ratio = Math.Min(page.Width.Point / xImage.PixelWidth, page.Height.Point / xImage.PixelHeight);
                            double w = xImage.PixelWidth * ratio;
                            double h = xImage.PixelHeight * ratio;

                            gfx.DrawImage(xImage, (page.Width.Point - w) / 2, (page.Height.Point - h) / 2, w, h);
                        }
                    }
                    doc.Save(saida);
                }
            });
        }

        // 6. Proteger com Senha (CORRIGIDO PARA VERSÃO 6.x)
        public async Task ProtegerPdfAsync(string entrada, string saida, string senha)
        {
            await Task.Run(() =>
            {
                string senhaLimpa = senha.Trim();
                if (string.IsNullOrEmpty(senhaLimpa)) throw new Exception("A senha não pode ser vazia.");

                using (var docEntrada = PdfReader.Open(entrada, PdfDocumentOpenMode.Import))
                using (var docSaida = new PdfDocument())
                {
                    // --- CORREÇÃO APLICADA AQUI ---
                    // Forçamos a versão 1.7 (valor inteiro 17) para garantir compatibilidade
                    // com a criptografia AES padrão do PDFSharp 6.x.
                    docSaida.Version = 17;

                    foreach (var page in docEntrada.Pages)
                    {
                        docSaida.AddPage(page);
                    }

                    var securitySettings = docSaida.SecuritySettings;

                    // Definimos senhas diferentes para evitar que o leitor bloqueie o arquivo.
                    // Senha para abrir: O que o usuário digitou.
                    // Senha para editar (Owner): A senha do usuário + "_admin".
                    securitySettings.UserPassword = senhaLimpa;
                    securitySettings.OwnerPassword = senhaLimpa + "_admin";

                    securitySettings.PermitExtractContent = false;
                    securitySettings.PermitModifyDocument = false;
                    securitySettings.PermitPrint = true;

                    docSaida.Save(saida);
                }
            });
        }

        // 7. Marca D'água (Mantido funcional)
        public async Task AdicionarMarcaDaguaAsync(string entrada, string saida, string texto)
        {
            await Task.Run(() =>
            {
                using (var docEntrada = PdfReader.Open(entrada, PdfDocumentOpenMode.Import))
                using (var docSaida = new PdfDocument())
                {
                    docSaida.Version = docEntrada.Version;

                    foreach (var pageEntrada in docEntrada.Pages)
                    {
                        var pageSaida = docSaida.AddPage(pageEntrada);

                        using (var gfx = XGraphics.FromPdfPage(pageSaida))
                        {
                            // Usa construtor padrão da fonte
                            var font = new XFont("Arial", 40);

                            var color = XColor.FromArgb(128, 255, 0, 0);
                            var brush = new XSolidBrush(color);

                            var center = new XPoint(pageSaida.Width.Point / 2, pageSaida.Height.Point / 2);
                            var state = gfx.Save();

                            gfx.TranslateTransform(center.X, center.Y);
                            gfx.RotateTransform(-45);

                            var size = gfx.MeasureString(texto, font);
                            gfx.DrawString(texto, font, brush, -(size.Width / 2), -(size.Height / 2));

                            gfx.Restore(state);
                        }
                    }
                    docSaida.Save(saida);
                }
            });
        }
    }
}