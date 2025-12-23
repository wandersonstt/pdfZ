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
        // 1. Comprimir (COM MODO EXTREMO 50 DPI)
        public async Task ComprimirArquivoAsync(string entrada, string saida, string caminhoGS, string nivelQualidade)
        {
            await Task.Run(() =>
            {
                if (!File.Exists(caminhoGS))
                    throw new FileNotFoundException("O arquivo 'gswin64c.exe' não foi encontrado na pasta motor_gs.");

                string configSettings;

                // Se for o modo EXTREMO, usamos configurações manuais agressivas
                if (nivelQualidade == "EXTREME")
                {
                    // Força 50 DPI (abaixo de /screen que é 72)
                    configSettings = "-dPDFSETTINGS=/screen " +
                                     "-dColorImageDownsampleType=/Bicubic -dColorImageResolution=50 " +
                                     "-dGrayImageDownsampleType=/Bicubic -dGrayImageResolution=50 " +
                                     "-dMonoImageDownsampleType=/Bicubic -dMonoImageResolution=50";
                }
                else
                {
                    // Usa os perfis padrão (/ebook, /printer, etc)
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
                                outputDocument.AddPage(inputDocument.Pages[idx]);
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
                        using (var xImage = XImage.FromFile(imgFile))
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

        // 6. Proteger com Senha
        public async Task ProtegerPdfAsync(string entrada, string saida, string senha)
        {
            await Task.Run(() =>
            {
                using (var doc = PdfReader.Open(entrada, PdfDocumentOpenMode.Modify))
                {
                    var securitySettings = doc.SecuritySettings;
                    securitySettings.UserPassword = senha;
                    securitySettings.OwnerPassword = senha;

                    securitySettings.PermitExtractContent = false;
                    securitySettings.PermitModifyDocument = false;

                    doc.Save(saida);
                }
            });
        }

        // 7. Marca D'água
        public async Task AdicionarMarcaDaguaAsync(string entrada, string saida, string texto)
        {
            await Task.Run(() =>
            {
                using (var doc = PdfReader.Open(entrada, PdfDocumentOpenMode.Modify))
                {
                    foreach (var page in doc.Pages)
                    {
                        using (var gfx = XGraphics.FromPdfPage(page))
                        {
                            var font = new XFont("Arial", 40);
                            var color = XColor.FromArgb(128, 255, 0, 0);
                            var brush = new XSolidBrush(color);

                            var center = new XPoint(page.Width.Point / 2, page.Height.Point / 2);
                            gfx.TranslateTransform(center.X, center.Y);
                            gfx.RotateTransform(-45);

                            var size = gfx.MeasureString(texto, font);
                            gfx.DrawString(texto, font, brush, -(size.Width / 2), -(size.Height / 2));
                        }
                    }
                    doc.Save(saida);
                }
            });
        }
    }
}