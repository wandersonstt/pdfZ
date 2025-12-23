using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Drawing;
using PdfSharp.Drawing;

namespace pdfZ
{
    public partial class Form1 : Form
    {
        private string arquivoDeEntrada = "";

        public Form1()
        {
            InitializeComponent();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            // Preenche o ComboBox de Rotação
            cmbAngulo.Items.Add("90° (Horário)");
            cmbAngulo.Items.Add("180°");
            cmbAngulo.Items.Add("270° (Anti-horário)");
            cmbAngulo.SelectedIndex = 0;

            // Preenche o Tutorial na aba "Ajuda"
            PreencherTutorial();
        }

        private void PreencherTutorial()
        {
            string tutorial = "Bem-vindo ao pdfZ!\r\n\r\n";
            tutorial += "Aqui está uma explicação simples de cada função:\r\n";
            tutorial += "------------------------------------------------------\r\n\r\n";
            tutorial += "### COMPRESSÃO ###\r\n";
            tutorial += "Reduz o tamanho (peso) de um arquivo PDF. É ótimo para quando você precisa enviar um PDF por e-mail e ele está muito grande.\r\n";
            tutorial += "1. Clique em 'Selecionar PDF'.\r\n";
            tutorial += "2. Clique em 'Comprimir Agora!' e escolha onde salvar o novo arquivo.\r\n\r\n";

            tutorial += "### JUNTAR ###\r\n";
            tutorial += "Pega dois ou mais arquivos PDF e os transforma em um único arquivo, na ordem que você selecionou.\r\n";
            tutorial += "1. Clique em 'Juntar PDFs...'.\r\n";
            tutorial += "2. Selecione 2 ou mais arquivos (segure CTRL para selecionar vários).\r\n";
            tutorial += "3. Escolha onde salvar o arquivo final.\r\n\r\n";

            tutorial += "### DIVIDIR ###\r\n";
            tutorial += "Pega um PDF com várias páginas e salva cada página individualmente como um novo PDF (ex: 'arquivo_pagina_1.pdf', 'arquivo_pagina_2.pdf', etc.).\r\n";
            tutorial += "1. Clique em 'Dividir PDF...'.\r\n";
            tutorial += "2. Selecione o PDF que quer dividir.\r\n";
            tutorial += "3. Escolha a PASTA onde os novos arquivos serão salvos.\r\n\r\n";

            tutorial += "### ROTACIONAR ###\r\n";
            tutorial += "Gira as páginas de um PDF. Útil para documentos que foram digitalizados de lado ou de cabeça para baixo.\r\n";
            tutorial += "1. Escolha oângulo na caixa (ex: 90°).\r\n";
            tutorial += "2. Clique em 'Rotacionar PDF...'.\r\n";
            tutorial += "3. Escolha o arquivo e onde salvar a cópia rotacionada.\r\n\r\n";

            tutorial += "### IMAGEM P/ PDF ###\r\n";
            tutorial += "Converte uma ou várias imagens (JPG, PNG) em um único arquivo PDF, com uma imagem por página.\r\n";
            tutorial += "1. Clique em 'Imagem para PDF...'.\r\n";
            tutorial += "2. Selecione uma ou mais imagens.\r\n";
            tutorial += "3. Escolha onde salvar o PDF final.\r\n";

            txtAjuda.Text = tutorial;
        }


        // --- FUNÇÕES DE COMPRESSÃO (ATUALIZADA) ---
        #region Compressão
        private void btnSelecionar_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Arquivos PDF (*.pdf)|*.pdf";
            dialog.Title = "Selecione um arquivo PDF para comprimir";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                arquivoDeEntrada = dialog.FileName;
                lblArquivo.Text = Path.GetFileName(arquivoDeEntrada);
                lblStatusPrincipal.Text = "Arquivo selecionado: " + Path.GetFileName(arquivoDeEntrada);
            }
        }

        private void btnComprimir_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(arquivoDeEntrada))
            {
                MessageBox.Show("Por favor, selecione um arquivo PDF primeiro (usando o botão 'Selecionar PDF').", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SaveFileDialog dialogSalvar = new SaveFileDialog();
            dialogSalvar.Filter = "Arquivo PDF Comprimido (*.pdf)|*.pdf";
            dialogSalvar.Title = "Salvar PDF comprimido como...";
            dialogSalvar.FileName = Path.GetFileNameWithoutExtension(arquivoDeEntrada) + "_comprimido.pdf";

            if (dialogSalvar.ShowDialog() == DialogResult.OK)
            {
                string arquivoDeSaida = dialogSalvar.FileName;
                string pastaDoApp = AppDomain.CurrentDomain.BaseDirectory;
                string caminhoGhostscript = Path.Combine(pastaDoApp, "motor_gs", "gswin64c.exe");

                if (!File.Exists(caminhoGhostscript))
                {
                    MessageBox.Show("Motor Ghostscript (gswin64c.exe) não encontrado na pasta 'motor_gs'.\n\nVerifique se você copiou os arquivos para o projeto e marcou 'Copiar se for mais novo'.", "Erro de Motor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    lblStatusPrincipal.Text = "Comprimindo, por favor aguarde...";
                    this.Update();

                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = caminhoGhostscript;
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.Arguments = string.Format(
                        "-sDEVICE=pdfwrite -dCompatibilityLevel=1.4 -dPDFSETTINGS=/ebook " +
                        "-dNOPAUSE -dQUIET -dBATCH -sOutputFile=\"{0}\" \"{1}\"",
                        arquivoDeSaida,
                        arquivoDeEntrada
                    );

                    Process processo = Process.Start(startInfo);
                    processo.WaitForExit();

                    lblStatusPrincipal.Text = "Compressão Concluída!";
                    MessageBox.Show("Arquivo comprimido salvo com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ocorreu um erro na compressão: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (File.Exists(arquivoDeEntrada))
                        lblArquivo.Text = Path.GetFileName(arquivoDeEntrada);
                    else
                        lblArquivo.Text = "Nenhum arquivo selecionado";

                    lblStatusPrincipal.Text = "Pronto.";
                }
            }
        }
        #endregion

        // --- FUNÇÃO JUNTAR PDF (COM A CORREÇÃO do 'dialogSalvert') ---
        #region Juntar
        private void btnJuntar_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Arquivos PDF (*.pdf)|*.pdf";
            dialog.Title = "Selecione 2 ou mais PDFs para juntar";
            dialog.Multiselect = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (dialog.FileNames.Length < 2)
                {
                    MessageBox.Show("Você precisa selecionar pelo menos 2 arquivos PDF para juntar.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog dialogSalvar = new SaveFileDialog();
                dialogSalvar.Filter = "PDF Juntado (*.pdf)|*.pdf";
                dialogSalvar.Title = "Salvar arquivo juntado como...";
                dialogSalvar.FileName = "documento_juntado.pdf";

                if (dialogSalvar.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        lblStatusPrincipal.Text = "Juntando PDFs...";
                        this.Update();

                        PdfDocument outputDocument = new PdfDocument();
                        foreach (string arquivo in dialog.FileNames)
                        {
                            PdfDocument inputDocument = PdfReader.Open(arquivo, PdfDocumentOpenMode.Import);
                            foreach (PdfPage pagina in inputDocument.Pages)
                            {
                                outputDocument.AddPage(pagina);
                            }
                        }

                        outputDocument.Save(dialogSalvar.FileName);

                        lblStatusPrincipal.Text = "Arquivos juntados com sucesso!";
                        MessageBox.Show("PDFs juntados e salvos com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ocorreu um erro ao juntar os PDFs: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        lblStatusPrincipal.Text = "Pronto.";
                        arquivoDeEntrada = "";
                    }
                }
            }
        }
        #endregion

        // --- FUNÇÃO DIVIDIR PDF (ATUALIZADA) ---
        #region Dividir
        private void btnDividir_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialogAbrir = new OpenFileDialog();
            dialogAbrir.Filter = "Arquivos PDF (*.pdf)|*.pdf";
            dialogAbrir.Title = "Selecione o PDF que você quer dividir";

            if (dialogAbrir.ShowDialog() == DialogResult.OK)
            {
                string arquivoParaDividir = dialogAbrir.FileName;
                string nomeBase = Path.GetFileNameWithoutExtension(arquivoParaDividir);

                FolderBrowserDialog dialogPasta = new FolderBrowserDialog();
                dialogPasta.Description = "Selecione a pasta para salvar as páginas divididas";

                if (dialogPasta.ShowDialog() == DialogResult.OK)
                {
                    string pastaDeSaida = dialogPasta.SelectedPath;

                    try
                    {
                        lblStatusPrincipal.Text = "Dividindo PDF...";
                        this.Update();

                        PdfDocument inputDocument = PdfReader.Open(arquivoParaDividir, PdfDocumentOpenMode.Import);

                        for (int i = 0; i < inputDocument.PageCount; i++)
                        {
                            PdfDocument outputDocument = new PdfDocument();
                            outputDocument.AddPage(inputDocument.Pages[i]);

                            string nomeArquivoSaida = string.Format("{0}_pagina_{1}.pdf", nomeBase, i + 1);
                            string caminhoCompletoSaida = Path.Combine(pastaDeSaida, nomeArquivoSaida);

                            outputDocument.Save(caminhoCompletoSaida);
                        }

                        lblStatusPrincipal.Text = "PDF dividido com sucesso!";
                        MessageBox.Show("PDF dividido com sucesso! Cada página foi salva como um arquivo separado.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ocorreu um erro ao dividir o PDF: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        lblStatusPrincipal.Text = "Pronto.";
                        arquivoDeEntrada = "";
                    }
                }
            }
        }
        #endregion

        // --- FUNÇÃO IMAGEM PARA PDF (COM CORREÇÕES) ---
        #region ImagemParaPdf
        private void btnImagemParaPdf_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialogAbrir = new OpenFileDialog();
            dialogAbrir.Filter = "Arquivos de Imagem (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
            dialogAbrir.Title = "Selecione uma ou mais imagens para converter";
            dialogAbrir.Multiselect = true;

            if (dialogAbrir.ShowDialog() == DialogResult.OK)
            {
                SaveFileDialog dialogSalvar = new SaveFileDialog();
                dialogSalvar.Filter = "Arquivo PDF (*.pdf)|*.pdf";
                dialogSalvar.Title = "Salvar PDF como...";
                dialogSalvar.FileName = "documento_convertido.pdf";

                if (dialogSalvar.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        lblStatusPrincipal.Text = "Convertendo imagens...";
                        this.Update();

                        PdfDocument outputDocument = new PdfDocument();

                        foreach (string arquivoImagem in dialogAbrir.FileNames)
                        {
                            PdfPage pagina = outputDocument.AddPage();
                            XImage imagem = XImage.FromFile(arquivoImagem);
                            XGraphics gfx = XGraphics.FromPdfPage(pagina);

                            double paginaLargura = pagina.Width.Point;
                            double paginaAltura = pagina.Height.Point;

                            double ratioX = paginaLargura / (double)imagem.PixelWidth;
                            double ratioY = paginaAltura / (double)imagem.PixelHeight;
                            double ratio = Math.Min(ratioX, ratioY);

                            double imgWidth = (double)imagem.PixelWidth * ratio;
                            double imgHeight = (double)imagem.PixelHeight * ratio;

                            double imgX = (paginaLargura - imgWidth) / 2;
                            double imgY = (paginaAltura - imgHeight) / 2;

                            gfx.DrawImage(imagem, imgX, imgY, imgWidth, imgHeight);
                        }

                        outputDocument.Save(dialogSalvar.FileName);

                        lblStatusPrincipal.Text = "Imagens convertidas!";
                        MessageBox.Show("Imagens convertidas para PDF com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ocorreu um erro ao converter as imagens: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        lblStatusPrincipal.Text = "Pronto.";
                        arquivoDeEntrada = "";
                    }
                }
            }
        }
        #endregion

        // --- FUNÇÃO ROTACIONAR PDF (ATUALIZADA) ---
        #region Rotacionar
        private void btnRotacionar_Click(object sender, EventArgs e)
        {
            int angulo = 0;
            switch (cmbAngulo.SelectedIndex)
            {
                case 0: angulo = 90; break;
                case 1: angulo = 180; break;
                case 2: angulo = 270; break;
            }

            OpenFileDialog dialogAbrir = new OpenFileDialog();
            dialogAbrir.Filter = "Arquivos PDF (*.pdf)|*.pdf";
            dialogAbrir.Title = "Selecione o PDF que você quer rotacionar";

            if (dialogAbrir.ShowDialog() == DialogResult.OK)
            {
                SaveFileDialog dialogSalvar = new SaveFileDialog();
                dialogSalvar.Filter = "PDF Rotacionado (*.pdf)|*.pdf";
                dialogSalvar.Title = "Salvar PDF rotacionado como...";
                dialogSalvar.FileName = Path.GetFileNameWithoutExtension(dialogAbrir.FileName) + "_rotacionado.pdf";

                if (dialogSalvar.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        lblStatusPrincipal.Text = "Rotacionando PDF...";
                        this.Update();

                        PdfDocument inputDocument = PdfReader.Open(dialogAbrir.FileName, PdfDocumentOpenMode.Import);
                        PdfDocument outputDocument = new PdfDocument();

                        foreach (PdfPage pagina in inputDocument.Pages)
                        {
                            pagina.Rotate = (pagina.Rotate + angulo) % 360;
                            outputDocument.AddPage(pagina);
                        }

                        outputDocument.Save(dialogSalvar.FileName);

                        lblStatusPrincipal.Text = "PDF rotacionado!";
                        MessageBox.Show("PDF rotacionado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ocorreu um erro ao rotacionar o PDF: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        lblStatusPrincipal.Text = "Pronto.";
                        arquivoDeEntrada = "";
                    }
                }
            }
        }
        #endregion
    }
}