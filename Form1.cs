using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using AutoUpdaterDotNET;

namespace pdfZ
{
    public partial class Form1 : Form
    {
        // --- VARIÁVEIS GLOBAIS ---
        private readonly PdfService _service = new PdfService();
        private ToolStripProgressBar barraProgresso;
        private const string URL_UPDATE_XML = "https://raw.githubusercontent.com/wandersonstt/pdfZ/refs/heads/main/update.xml";

        // Controles Dinâmicos (Interface)
        private Label lblArquivoCompressao;
        private Button btnSelecionarCompressao, btnExecutarCompressao;
        private ComboBox cmbQualidade;
        private string arquivoCompressao = "";

        private ListBox lstArquivosJuntar;
        private Button btnJuntarAdd, btnJuntarRemove, btnJuntarUp, btnJuntarDown, btnJuntarExecutar;

        private TextBox txtSenhaProtecao;
        private Button btnProtegerSelecionar, btnProtegerExecutar;
        private Label lblArquivoProteger;
        private string arquivoParaProteger = "";

        private TextBox txtMarcaTexto;
        private Button btnMarcaSelecionar, btnMarcaExecutar;
        private Label lblArquivoMarca;
        private string arquivoParaMarca = "";

        private ComboBox cmbAnguloDinamico;
        private string arquivoDeEntrada = "";

        public Form1()
        {
            InitializeComponent();

            // Configurações Básicas
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ConfigurarInterfaceBase();
            ConfigurarAutoUpdater();

            // --- RECONSTRUÇÃO SEGURA DA INTERFACE ---
            // 1. Limpa tudo para garantir que não haja duplicatas ou lixo
            if (tabControlPrincipal.TabPages.Count > 0) tabControlPrincipal.TabPages.Clear();

            // 2. Recria TODAS as abas na ordem correta
            CriarAba("Compressão", ConfigurarAbaCompressao);
            CriarAba("Juntar PDFs", ConfigurarAbaJuntar);
            CriarAba("Dividir PDF", ConfigurarAbaDividir);
            CriarAba("Rotacionar", ConfigurarAbaRotacionar);
            CriarAba("Imagem p/ PDF", ConfigurarAbaImagem);
            CriarAba("Proteger", ConfigurarAbaProteger);       // Traz de volta a aba Proteger
            CriarAba("Marca D'água", ConfigurarAbaMarcaDagua); // Traz de volta a aba Marca D'água
            CriarAba("Ajuda", ConfigurarAbaAjuda);

            // 3. Configurações Finais
            ConfigurarDragDrop();
            CriarBotaoTema();
        }

        // ====================================================================================
        // --- GERENCIADOR DE ABAS ---
        // ====================================================================================
        private void CriarAba(string titulo, Action<TabPage> configurarConteudo)
        {
            TabPage tab = new TabPage(titulo);
            tab.BackColor = Color.WhiteSmoke;
            // Permite rolagem se a tela for pequena
            tab.AutoScroll = true;
            configurarConteudo(tab);
            tabControlPrincipal.TabPages.Add(tab);
        }

        // ====================================================================================
        // --- CONFIGURAÇÃO DETALHADA DE CADA ABA ---
        // ====================================================================================

        private void ConfigurarAbaCompressao(TabPage tab)
        {
            GroupBox gb = new GroupBox { Text = "Compressão (Reduzir Tamanho)", Location = new Point(15, 15), Size = new Size(460, 220) };

            btnSelecionarCompressao = new Button { Text = "Selecionar PDF", Location = new Point(20, 30), Size = new Size(150, 35) };
            lblArquivoCompressao = new Label { Text = "Nenhum arquivo...", Location = new Point(180, 40), AutoSize = true };

            Label lblQ = new Label { Text = "Qualidade:", Location = new Point(20, 80), AutoSize = true };
            cmbQualidade = new ComboBox { Location = new Point(90, 77), Size = new Size(350, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbQualidade.Items.AddRange(new object[] {
                "EXTREMA (50 DPI - Mínima)",
                "Baixa / Tela (72 DPI)",
                "Média / eBook (150 DPI)",
                "Alta / Impressão (300 DPI)"
            });
            cmbQualidade.SelectedIndex = 0;

            btnExecutarCompressao = new Button { Text = "Comprimir Agora!", Location = new Point(20, 130), Size = new Size(420, 45), BackColor = Color.LightGreen };

            btnSelecionarCompressao.Click += (s, e) => SelecionarArquivo(path => {
                arquivoCompressao = path;
                lblArquivoCompressao.Text = Path.GetFileName(path);
            });
            btnExecutarCompressao.Click += BtnExecutarCompressao_Click;

            gb.Controls.AddRange(new Control[] { btnSelecionarCompressao, lblArquivoCompressao, lblQ, cmbQualidade, btnExecutarCompressao });
            tab.Controls.Add(gb);
        }

        private void ConfigurarAbaJuntar(TabPage tab)
        {
            GroupBox gb = new GroupBox { Text = "Lista de Arquivos", Location = new Point(15, 15), Size = new Size(460, 220) };

            lstArquivosJuntar = new ListBox { Location = new Point(15, 25), Size = new Size(350, 130), HorizontalScrollbar = true };
            btnJuntarAdd = new Button { Text = "+", Location = new Point(375, 25), Size = new Size(35, 35) };
            btnJuntarRemove = new Button { Text = "-", Location = new Point(375, 65), Size = new Size(35, 35) };
            btnJuntarUp = new Button { Text = "▲", Location = new Point(415, 25), Size = new Size(35, 35) };
            btnJuntarDown = new Button { Text = "▼", Location = new Point(415, 65), Size = new Size(35, 35) };
            btnJuntarExecutar = new Button { Text = "Juntar PDFs", Location = new Point(15, 170), Size = new Size(435, 40), BackColor = Color.LightBlue };

            btnJuntarAdd.Click += (s, e) => {
                using (var op = new OpenFileDialog { Filter = "PDF|*.pdf", Multiselect = true })
                    if (op.ShowDialog() == DialogResult.OK) lstArquivosJuntar.Items.AddRange(op.FileNames);
            };
            btnJuntarRemove.Click += (s, e) => { if (lstArquivosJuntar.SelectedIndex >= 0) lstArquivosJuntar.Items.RemoveAt(lstArquivosJuntar.SelectedIndex); };
            btnJuntarUp.Click += (s, e) => MoverItemLista(-1);
            btnJuntarDown.Click += (s, e) => MoverItemLista(1);
            btnJuntarExecutar.Click += BtnJuntarExecutar_Click;

            gb.Controls.AddRange(new Control[] { lstArquivosJuntar, btnJuntarAdd, btnJuntarRemove, btnJuntarUp, btnJuntarDown, btnJuntarExecutar });
            tab.Controls.Add(gb);
        }

        private void ConfigurarAbaDividir(TabPage tab)
        {
            GroupBox gb = new GroupBox { Text = "Dividir PDF em Páginas", Location = new Point(15, 15), Size = new Size(460, 150) };
            Label lblInfo = new Label { Text = "Separa cada página do PDF em um arquivo novo.", Location = new Point(20, 30), AutoSize = true };
            Button btnDividir = new Button { Text = "Selecionar PDF e Dividir...", Location = new Point(20, 60), Size = new Size(420, 50) };

            btnDividir.Click += async (s, e) => {
                using (var open = new OpenFileDialog { Filter = "PDF|*.pdf" })
                    if (open.ShowDialog() == DialogResult.OK)
                        using (var folder = new FolderBrowserDialog())
                            if (folder.ShowDialog() == DialogResult.OK)
                            {
                                try { SetCarregando(true, "Dividindo..."); await _service.DividirArquivoAsync(open.FileName, folder.SelectedPath); MessageBox.Show("Sucesso!"); }
                                catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
                                finally { SetCarregando(false); }
                            }
            };
            gb.Controls.AddRange(new Control[] { lblInfo, btnDividir });
            tab.Controls.Add(gb);
        }

        private void ConfigurarAbaRotacionar(TabPage tab)
        {
            GroupBox gb = new GroupBox { Text = "Girar Páginas", Location = new Point(15, 15), Size = new Size(460, 180) };
            Label lblA = new Label { Text = "Ângulo:", Location = new Point(20, 40), AutoSize = true };
            cmbAnguloDinamico = new ComboBox { Location = new Point(80, 37), Size = new Size(360, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbAnguloDinamico.Items.AddRange(new object[] { "90° (Horário)", "180°", "270° (Anti-horário)" });
            cmbAnguloDinamico.SelectedIndex = 0;

            Button btnRot = new Button { Text = "Selecionar e Rotacionar", Location = new Point(20, 90), Size = new Size(420, 50) };

            btnRot.Click += async (s, e) => {
                int ang = (cmbAnguloDinamico.SelectedIndex + 1) * 90;
                if (ang == 360) ang = 270;
                using (var open = new OpenFileDialog { Filter = "PDF|*.pdf" })
                    if (open.ShowDialog() == DialogResult.OK)
                        using (var save = new SaveFileDialog { Filter = "PDF|*.pdf", FileName = Path.GetFileNameWithoutExtension(open.FileName) + "_rot.pdf" })
                            if (save.ShowDialog() == DialogResult.OK)
                            {
                                try { SetCarregando(true, "Rotacionando..."); await _service.RotacionarArquivoAsync(open.FileName, save.FileName, ang); MessageBox.Show("Sucesso!"); }
                                catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
                                finally { SetCarregando(false); }
                            }
            };
            gb.Controls.AddRange(new Control[] { lblA, cmbAnguloDinamico, btnRot });
            tab.Controls.Add(gb);
        }

        private void ConfigurarAbaImagem(TabPage tab)
        {
            GroupBox gb = new GroupBox { Text = "Converter Imagens para PDF", Location = new Point(15, 15), Size = new Size(460, 150) };
            Button btnImg = new Button { Text = "Selecionar Imagens...", Location = new Point(20, 40), Size = new Size(420, 60) };

            btnImg.Click += async (s, e) => {
                using (var open = new OpenFileDialog { Filter = "Imagens|*.jpg;*.png", Multiselect = true })
                    if (open.ShowDialog() == DialogResult.OK)
                        using (var save = new SaveFileDialog { Filter = "PDF|*.pdf", FileName = "album.pdf" })
                            if (save.ShowDialog() == DialogResult.OK)
                            {
                                try { SetCarregando(true, "Convertendo..."); await _service.ImagensParaPdfAsync(open.FileNames, save.FileName); MessageBox.Show("Sucesso!"); }
                                catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
                                finally { SetCarregando(false); }
                            }
            };
            gb.Controls.Add(btnImg);
            tab.Controls.Add(gb);
        }

        // --- ABA PROTEGER (GARANTIDA) ---
        private void ConfigurarAbaProteger(TabPage tab)
        {
            GroupBox gb = new GroupBox { Text = "Adicionar Senha", Location = new Point(15, 15), Size = new Size(460, 180) };
            btnProtegerSelecionar = new Button { Text = "Selecionar PDF", Location = new Point(20, 30), Size = new Size(150, 30) };
            lblArquivoProteger = new Label { Text = "...", Location = new Point(180, 35), AutoSize = true };

            Label lblS = new Label { Text = "Senha:", Location = new Point(20, 80), AutoSize = true };
            txtSenhaProtecao = new TextBox { Location = new Point(80, 77), Size = new Size(360, 23), PasswordChar = '*' };

            btnProtegerExecutar = new Button { Text = "Proteger PDF", Location = new Point(20, 120), Size = new Size(420, 40) };

            btnProtegerSelecionar.Click += (s, e) => SelecionarArquivo(p => { arquivoParaProteger = p; lblArquivoProteger.Text = Path.GetFileName(p); });
            btnProtegerExecutar.Click += async (s, e) => {
                if (string.IsNullOrEmpty(arquivoParaProteger) || string.IsNullOrEmpty(txtSenhaProtecao.Text)) { MessageBox.Show("Preencha tudo."); return; }
                using (var save = new SaveFileDialog { Filter = "PDF|*.pdf", FileName = Path.GetFileNameWithoutExtension(arquivoParaProteger) + "_protegido.pdf" })
                    if (save.ShowDialog() == DialogResult.OK)
                    {
                        try { SetCarregando(true, "Protegendo..."); await _service.ProtegerPdfAsync(arquivoParaProteger, save.FileName, txtSenhaProtecao.Text); MessageBox.Show("Sucesso!"); }
                        catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
                        finally { SetCarregando(false); }
                    }
            };
            gb.Controls.AddRange(new Control[] { btnProtegerSelecionar, lblArquivoProteger, lblS, txtSenhaProtecao, btnProtegerExecutar });
            tab.Controls.Add(gb);
        }

        // --- ABA MARCA D'ÁGUA (GARANTIDA) ---
        private void ConfigurarAbaMarcaDagua(TabPage tab)
        {
            GroupBox gb = new GroupBox { Text = "Adicionar Texto (Marca D'água)", Location = new Point(15, 15), Size = new Size(460, 180) };
            btnMarcaSelecionar = new Button { Text = "Selecionar PDF", Location = new Point(20, 30), Size = new Size(150, 30) };
            lblArquivoMarca = new Label { Text = "...", Location = new Point(180, 35), AutoSize = true };

            Label lblT = new Label { Text = "Texto:", Location = new Point(20, 80), AutoSize = true };
            txtMarcaTexto = new TextBox { Text = "CONFIDENCIAL", Location = new Point(80, 77), Size = new Size(360, 23) };

            btnMarcaExecutar = new Button { Text = "Aplicar Marca", Location = new Point(20, 120), Size = new Size(420, 40) };

            btnMarcaSelecionar.Click += (s, e) => SelecionarArquivo(p => { arquivoParaMarca = p; lblArquivoMarca.Text = Path.GetFileName(p); });
            btnMarcaExecutar.Click += async (s, e) => {
                if (string.IsNullOrEmpty(arquivoParaMarca) || string.IsNullOrEmpty(txtMarcaTexto.Text)) { MessageBox.Show("Preencha tudo."); return; }
                using (var save = new SaveFileDialog { Filter = "PDF|*.pdf", FileName = Path.GetFileNameWithoutExtension(arquivoParaMarca) + "_marca.pdf" })
                    if (save.ShowDialog() == DialogResult.OK)
                    {
                        try { SetCarregando(true, "Aplicando..."); await _service.AdicionarMarcaDaguaAsync(arquivoParaMarca, save.FileName, txtMarcaTexto.Text); MessageBox.Show("Sucesso!"); }
                        catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
                        finally { SetCarregando(false); }
                    }
            };
            gb.Controls.AddRange(new Control[] { btnMarcaSelecionar, lblArquivoMarca, lblT, txtMarcaTexto, btnMarcaExecutar });
            tab.Controls.Add(gb);
        }

        private void ConfigurarAbaAjuda(TabPage tab)
        {
            TextBox txtAjuda = new TextBox { Multiline = true, ReadOnly = true, Dock = DockStyle.Fill, ScrollBars = ScrollBars.Vertical, Font = new Font("Consolas", 10) };
            var v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            txtAjuda.Text = $"pdfZ - Versão {v}\r\n\r\n" +
                            "GUIA DE FUNÇÕES:\r\n" +
                            "1. COMPRESSÃO: Reduz tamanho do arquivo.\r\n" +
                            "2. Juntar: Combina vários PDFs em um.\r\n" +
                            "3. Dividir: Salva cada página separadamente.\r\n" +
                            "4. Rotacionar: Gira as páginas permanentemente.\r\n" +
                            "5. Imagem p/ PDF: Cria álbum a partir de fotos.\r\n" +
                            "6. Proteger: Adiciona senha de abertura.\r\n" +
                            "7. Marca D'água: Insere texto diagonal nas páginas.\r\n\r\n" +
                            "Gostou do projeto? Faça uma Doação via Pix ou siga nas redes!";

            FlowLayoutPanel pnlSocial = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 60, FlowDirection = FlowDirection.LeftToRight, Padding = new Padding(10), BackColor = Color.Transparent };

            Button btnDoar = new Button { Text = "❤ Doar (Pix)", Size = new Size(120, 35), BackColor = Color.LightGreen, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            Button btnGit = new Button { Text = "GitHub", Size = new Size(100, 35), BackColor = Color.LightGray, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            Button btnInsta = new Button { Text = "Instagram", Size = new Size(100, 35), BackColor = Color.PeachPuff, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Font = new Font("Segoe UI", 9, FontStyle.Bold) };

            string chavePix = "98815f34-e9bf-4bad-835f-17daf1b8fe47";
            string nomePix = "Wanderson Saraiva Torres";
            string cidadePix = "Barcarena";

            btnDoar.Click += (s, e) => ExibirQrCodePix(chavePix, nomePix, cidadePix, "Doacao pdfZ");
            btnGit.Click += (s, e) => AbrirLink("https://github.com/wandersonstt/pdfz");
            btnInsta.Click += (s, e) => AbrirLink("https://instagram.com/wandersonsaraivatorres");

            pnlSocial.Controls.AddRange(new Control[] { btnDoar, btnGit, btnInsta });
            tab.Controls.Add(txtAjuda);
            tab.Controls.Add(pnlSocial);
            pnlSocial.BringToFront();
        }

        // ====================================================================================
        // --- FUNÇÕES AUXILIARES ---
        // ====================================================================================
        private void SelecionarArquivo(Action<string> onSelect)
        {
            using (var op = new OpenFileDialog { Filter = "PDF|*.pdf" })
                if (op.ShowDialog() == DialogResult.OK) onSelect(op.FileName);
        }

        private void MoverItemLista(int direcao)
        {
            if (lstArquivosJuntar.SelectedItem == null || lstArquivosJuntar.SelectedIndex < 0) return;
            int current = lstArquivosJuntar.SelectedIndex;
            int newIdx = current + direcao;
            if (newIdx < 0 || newIdx >= lstArquivosJuntar.Items.Count) return;
            object item = lstArquivosJuntar.SelectedItem;
            lstArquivosJuntar.Items.RemoveAt(current);
            lstArquivosJuntar.Items.Insert(newIdx, item);
            lstArquivosJuntar.SetSelected(newIdx, true);
        }

        private async void BtnExecutarCompressao_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(arquivoCompressao)) { MessageBox.Show("Selecione um arquivo."); return; }
            string configGS = "/ebook";
            switch (cmbQualidade.SelectedIndex)
            {
                case 0: configGS = "EXTREME"; break;
                case 1: configGS = "/screen"; break;
                case 2: configGS = "/ebook"; break;
                case 3: configGS = "/printer"; break;
            }
            using (var save = new SaveFileDialog { Filter = "PDF|*.pdf", FileName = Path.GetFileNameWithoutExtension(arquivoCompressao) + "_otimizado.pdf" })
                if (save.ShowDialog() == DialogResult.OK)
                {
                    string gs = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "motor_gs", "gswin64c.exe");
                    try { SetCarregando(true, "Comprimindo..."); await _service.ComprimirArquivoAsync(arquivoCompressao, save.FileName, gs, configGS); MessageBox.Show("Sucesso!"); }
                    catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
                    finally { SetCarregando(false); }
                }
        }

        private async void BtnJuntarExecutar_Click(object sender, EventArgs e)
        {
            if (lstArquivosJuntar.Items.Count < 2) { MessageBox.Show("Adicione pelo menos 2 arquivos."); return; }
            using (var save = new SaveFileDialog { Filter = "PDF|*.pdf", FileName = "unido.pdf" })
                if (save.ShowDialog() == DialogResult.OK)
                {
                    try { SetCarregando(true, "Unindo..."); await _service.JuntarArquivosAsync(lstArquivosJuntar.Items.Cast<string>().ToArray(), save.FileName); MessageBox.Show("Sucesso!"); }
                    catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
                    finally { SetCarregando(false); }
                }
        }

        // ====================================================================================
        // --- PIX & UTILITÁRIOS ---
        // ====================================================================================
        private void ExibirQrCodePix(string chave, string nome, string cidade, string descricao)
        {
            try
            {
                string chaveL = LimparChavePix(chave);
                string nomeL = NormalizarTexto(nome, 25);
                string cidadeL = NormalizarTexto(cidade, 15);

                string payload = GerarPayloadPix(chaveL, nomeL, cidadeL, "***");
                string url = $"https://api.qrserver.com/v1/create-qr-code/?size=250x250&data={Uri.EscapeDataString(payload)}";

                Form frm = new Form { Text = "Apoie o Projeto ❤", Size = new Size(340, 520), StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false };
                PictureBox pb = new PictureBox { ImageLocation = url, Size = new Size(250, 250), Location = new Point(35, 15), SizeMode = PictureBoxSizeMode.StretchImage, BorderStyle = BorderStyle.FixedSingle };
                Label lblC = new Label { Text = "Pix Copia e Cola:", Location = new Point(35, 280), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
                TextBox txt = new TextBox { Text = payload, Location = new Point(35, 300), Size = new Size(250, 23), ReadOnly = true };
                Button btn = new Button { Text = "Copiar Código Pix", Location = new Point(35, 335), Size = new Size(250, 40), BackColor = Color.LightGreen, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
                Label lblInfo = new Label { Text = $"Nome: {nomeL}\nChave: {chaveL}\nCidade: {cidadeL}", Location = new Point(35, 390), Size = new Size(250, 60), TextAlign = ContentAlignment.TopCenter, ForeColor = Color.Gray, Font = new Font("Segoe UI", 8) };

                btn.Click += (s, e) => { Clipboard.SetText(payload); MessageBox.Show("Código copiado! Abra o app do seu banco e escolha 'Pix Copia e Cola'."); };
                frm.Controls.AddRange(new Control[] { pb, lblC, txt, btn, lblInfo });
                frm.ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private string GerarPayloadPix(string k, string n, string c, string t)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("000201");
            sb.Append(FormatarCampo("26", "0014br.gov.bcb.pix" + FormatarCampo("01", k)));
            sb.Append("52040000").Append("5303986").Append("5802BR");
            sb.Append(FormatarCampo("59", n)).Append(FormatarCampo("60", c));
            sb.Append(FormatarCampo("62", FormatarCampo("05", t))).Append("6304");
            return sb.ToString() + CalcularCRC16(sb.ToString());
        }

        private string FormatarCampo(string id, string v) => id + Encoding.UTF8.GetByteCount(v).ToString("D2") + v;

        private string CalcularCRC16(string s)
        {
            byte[] data = Encoding.UTF8.GetBytes(s);
            int crc = 0xFFFF;
            foreach (byte b in data)
            {
                for (int i = 0; i < 8; i++)
                {
                    bool bit = ((b >> (7 - i) & 1) == 1);
                    bool c15 = ((crc >> 15 & 1) == 1);
                    crc <<= 1;
                    if (c15 ^ bit) crc ^= 0x1021;
                }
            }
            return (crc & 0xFFFF).ToString("X4");
        }

        private string LimparChavePix(string c)
        {
            if (string.IsNullOrEmpty(c)) return "";
            c = c.Trim();
            if ((c.Contains("-") && c.Length > 30) || c.Contains("@")) return c.ToLower();
            return Regex.Replace(c, "[^0-9]", "");
        }

        private string NormalizarTexto(string t, int l)
        {
            if (string.IsNullOrEmpty(t)) return "";
            var s = new StringBuilder();
            foreach (char c in t.Normalize(NormalizationForm.FormD))
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark) s.Append(c);
            string r = s.ToString().ToUpper().Trim();
            return r.Length > l ? r.Substring(0, l) : r;
        }

        private void AbrirLink(string url) { try { Process.Start(new ProcessStartInfo(url) { UseShellExecute = true }); } catch { MessageBox.Show("Erro ao abrir link."); } }

        // --- INTERFACE GERAL ---
        private void ConfigurarInterfaceBase()
        {
            barraProgresso = new ToolStripProgressBar { Size = new Size(200, 16), Style = ProgressBarStyle.Marquee, Visible = false };
            statusRodape.Items.Add(barraProgresso);
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        private void SetCarregando(bool c, string t = "") { tabControlPrincipal.Enabled = !c; barraProgresso.Visible = c; lblStatusPrincipal.Text = c ? t : "Concluído."; Cursor = c ? Cursors.WaitCursor : Cursors.Default; }
        private void ConfigurarAutoUpdater() { AutoUpdater.RunUpdateAsAdmin = false; AutoUpdater.Start(URL_UPDATE_XML); }

        // --- DRAG & DROP CORRIGIDO (O método que faltava) ---
        private void ConfigurarDragDrop() { AllowDrop = true; DragEnter += (s, e) => e.Effect = DragDropEffects.Copy; DragDrop += Form1_DragDrop; }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] arquivos = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (arquivos == null || arquivos.Length == 0) return;

            string nomeAba = tabControlPrincipal.SelectedTab.Text;

            if (nomeAba.Contains("Compressão"))
            {
                if (Path.GetExtension(arquivos[0]).ToLower() == ".pdf")
                {
                    arquivoCompressao = arquivos[0];
                    lblArquivoCompressao.Text = Path.GetFileName(arquivoCompressao);
                    arquivoDeEntrada = arquivoCompressao;
                }
            }
            else if (nomeAba.Contains("Juntar"))
            {
                foreach (var arq in arquivos)
                    if (Path.GetExtension(arq).ToLower() == ".pdf")
                        lstArquivosJuntar.Items.Add(arq);
            }
            else if (nomeAba.Contains("Proteger"))
            {
                if (Path.GetExtension(arquivos[0]).ToLower() == ".pdf")
                {
                    arquivoParaProteger = arquivos[0];
                    lblArquivoProteger.Text = Path.GetFileName(arquivoParaProteger);
                }
            }
            else if (nomeAba.Contains("Marca"))
            {
                if (Path.GetExtension(arquivos[0]).ToLower() == ".pdf")
                {
                    arquivoParaMarca = arquivos[0];
                    lblArquivoMarca.Text = Path.GetFileName(arquivoParaMarca);
                }
            }
        }

        private void CriarBotaoTema() { ToolStripButton b = new ToolStripButton("🌙 Tema"); b.Click += (s, e) => AlternarTema(); statusRodape.Items.Insert(0, b); }

        private void AlternarTema()
        {
            bool dark = BackColor.R > 100;
            BackColor = dark ? Color.FromArgb(45, 45, 48) : Color.WhiteSmoke;
            Color txt = dark ? Color.White : Color.Black;
            Color ctrl = dark ? Color.FromArgb(30, 30, 30) : Color.White;
            tabControlPrincipal.BackColor = BackColor;
            statusRodape.BackColor = ctrl; statusRodape.ForeColor = txt;

            foreach (TabPage t in tabControlPrincipal.TabPages)
            {
                t.BackColor = BackColor; t.ForeColor = txt;
                foreach (Control c in t.Controls) if (c is GroupBox gb)
                    {
                        gb.ForeColor = txt;
                        foreach (Control s in gb.Controls)
                        {
                            if (s is Button bn) { bn.BackColor = ctrl; bn.ForeColor = txt; }
                            else if (s is TextBox tx) { tx.BackColor = ctrl; tx.ForeColor = txt; }
                            else if (s is ListBox lx) { lx.BackColor = ctrl; lx.ForeColor = txt; }
                            else if (s is ComboBox cx) { cx.BackColor = ctrl; cx.ForeColor = txt; }
                        }
                    }
            }
        }

        // Stubs de Compatibilidade
        private void btnSelecionar_Click(object sender, EventArgs e) { }
        private void btnComprimir_Click(object sender, EventArgs e) { }
        private void btnJuntar_Click(object sender, EventArgs e) { }
        private void btnDividir_Click(object sender, EventArgs e) { }
        private void btnRotacionar_Click(object sender, EventArgs e) { }
        private void btnImagemParaPdf_Click(object sender, EventArgs e) { }
        private void txtAjuda_TextChanged(object sender, EventArgs e) { }
    }
}