using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using AutoUpdaterDotNET;

namespace pdfZ
{
    public partial class Form1 : Form
    {
        // Variáveis Globais
        private readonly PdfService _service = new PdfService();
        private ToolStripProgressBar barraProgresso;
        private const string URL_UPDATE_XML = "https://raw.githubusercontent.com/wandersonstt/pdfZ/main/update.xml";

        // Controles Dinâmicos
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

        // Compatibilidade
        private string arquivoDeEntrada = "";

        public Form1()
        {
            InitializeComponent();

            // Segurança GitHub
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ConfigurarInterfaceBase();
            ConfigurarAutoUpdater();

            // --- CONSTRUÇÃO OBRIGATÓRIA DA INTERFACE ---
            // Removemos o 'try/catch' para garantir que as abas apareçam mesmo se houver erro antes
            RecriarAbaCompressao();
            RecriarAbaJuntar();
            CriarAbaProteger();       // <--- AQUI CRIA A ABA DE SENHA
            CriarAbaMarcaDagua();     // <--- AQUI CRIA A ABA DE MARCA

            ConfigurarDragDrop();
            CriarBotaoTema();
            PreencherTutorial();
        }

        // --- ABA 1: COMPRESSÃO (MODO EXTREMO) ---
        private void RecriarAbaCompressao()
        {
            if (tabControlPrincipal.TabPages.Count == 0) return;
            TabPage tab = tabControlPrincipal.TabPages[0];
            tab.Controls.Clear();

            GroupBox gbArquivo = new GroupBox { Text = "1. Selecionar Arquivo", Location = new Point(15, 15), Size = new Size(460, 80) };
            btnSelecionarCompressao = new Button { Text = "Selecionar PDF", Location = new Point(15, 25), Size = new Size(150, 35) };
            lblArquivoCompressao = new Label { Text = "Nenhum arquivo selecionado", Location = new Point(180, 35), AutoSize = true };

            btnSelecionarCompressao.Click += (s, e) => {
                using (var op = new OpenFileDialog { Filter = "PDF|*.pdf" })
                    if (op.ShowDialog() == DialogResult.OK)
                    {
                        arquivoCompressao = op.FileName;
                        lblArquivoCompressao.Text = Path.GetFileName(arquivoCompressao);
                        arquivoDeEntrada = arquivoCompressao;
                        lblArquivo.Text = Path.GetFileName(arquivoDeEntrada);
                    }
            };
            gbArquivo.Controls.AddRange(new Control[] { btnSelecionarCompressao, lblArquivoCompressao });

            GroupBox gbConfig = new GroupBox { Text = "2. Configurar Compressão", Location = new Point(15, 105), Size = new Size(460, 70) };
            Label lblQ = new Label { Text = "Qualidade:", Location = new Point(15, 30), AutoSize = true };
            cmbQualidade = new ComboBox { Location = new Point(90, 27), Size = new Size(350, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbQualidade.Items.Add("EXTREMA (50 DPI - Qualidade Mínima)");
            cmbQualidade.Items.Add("Baixa / Tela (72 DPI)");
            cmbQualidade.Items.Add("Média / eBook (150 DPI)");
            cmbQualidade.Items.Add("Alta / Impressão (300 DPI)");
            cmbQualidade.SelectedIndex = 0;

            gbConfig.Controls.AddRange(new Control[] { lblQ, cmbQualidade });
            btnExecutarCompressao = new Button { Text = "Comprimir Agora!", Location = new Point(15, 190), Size = new Size(460, 40), BackColor = Color.LightGreen };
            btnExecutarCompressao.Click += BtnExecutarCompressao_Click;
            tab.Controls.AddRange(new Control[] { gbArquivo, gbConfig, btnExecutarCompressao });
        }

        private async void BtnExecutarCompressao_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(arquivoCompressao)) { MessageBox.Show("Selecione um arquivo primeiro."); return; }
            string configGS = "/ebook";
            switch (cmbQualidade.SelectedIndex)
            {
                case 0: configGS = "EXTREME"; break;
                case 1: configGS = "/screen"; break;
                case 2: configGS = "/ebook"; break;
                case 3: configGS = "/printer"; break;
            }
            using (var save = new SaveFileDialog { Filter = "PDF|*.pdf", FileName = Path.GetFileNameWithoutExtension(arquivoCompressao) + "_otimizado.pdf" })
            {
                if (save.ShowDialog() == DialogResult.OK)
                {
                    string gs = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "motor_gs", "gswin64c.exe");
                    try
                    {
                        SetCarregando(true, "Comprimindo...");
                        await _service.ComprimirArquivoAsync(arquivoCompressao, save.FileName, gs, configGS);
                        MessageBox.Show("Sucesso!");
                    }
                    catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
                    finally { SetCarregando(false); }
                }
            }
        }

        // --- ABA 2: JUNTAR ---
        private void RecriarAbaJuntar()
        {
            // Busca a aba correta
            TabPage tab = null;
            foreach (TabPage t in tabControlPrincipal.TabPages) if (t.Text.Contains("Juntar") || t.Name == "tabJuntar") tab = t;

            // Se não achar a aba Juntar, para aqui para não travar
            if (tab == null) return;

            Control groupBoxOriginal = (tab.Controls.Count > 0) ? tab.Controls[0] : null;
            if (groupBoxOriginal != null)
            {
                groupBoxOriginal.Controls.Clear();
                groupBoxOriginal.Text = "Lista de Arquivos para Juntar";
                groupBoxOriginal.Size = new System.Drawing.Size(460, 200);
            }
            else
            {
                groupBoxOriginal = new GroupBox { Text = "Lista de Arquivos para Juntar", Location = new Point(10, 10), Size = new Size(460, 200) };
                tab.Controls.Add(groupBoxOriginal);
            }

            lstArquivosJuntar = new ListBox { Location = new Point(15, 25), Size = new Size(350, 100), HorizontalScrollbar = true };
            btnJuntarAdd = new Button { Text = "+", Location = new Point(375, 25), Size = new Size(30, 30) };
            btnJuntarRemove = new Button { Text = "-", Location = new Point(375, 60), Size = new Size(30, 30) };
            btnJuntarUp = new Button { Text = "▲", Location = new Point(410, 25), Size = new Size(30, 30) };
            btnJuntarDown = new Button { Text = "▼", Location = new Point(410, 60), Size = new Size(30, 30) };
            btnJuntarExecutar = new Button { Text = "Juntar PDFs Agora", Location = new Point(15, 135), Size = new Size(390, 35), BackColor = Color.LightBlue };

            btnJuntarAdd.Click += (s, e) => {
                using (var op = new OpenFileDialog { Filter = "PDF|*.pdf", Multiselect = true })
                    if (op.ShowDialog() == DialogResult.OK) lstArquivosJuntar.Items.AddRange(op.FileNames);
            };
            btnJuntarRemove.Click += (s, e) => { if (lstArquivosJuntar.SelectedIndex >= 0) lstArquivosJuntar.Items.RemoveAt(lstArquivosJuntar.SelectedIndex); };
            btnJuntarUp.Click += (s, e) => MoverItemLista(-1);
            btnJuntarDown.Click += (s, e) => MoverItemLista(1);
            btnJuntarExecutar.Click += BtnJuntarExecutar_Click;

            groupBoxOriginal.Controls.AddRange(new Control[] { lstArquivosJuntar, btnJuntarAdd, btnJuntarRemove, btnJuntarUp, btnJuntarDown, btnJuntarExecutar });
        }

        private void MoverItemLista(int direcao)
        {
            if (lstArquivosJuntar.SelectedItem == null || lstArquivosJuntar.SelectedIndex < 0) return;
            int newIndex = lstArquivosJuntar.SelectedIndex + direcao;
            if (newIndex < 0 || newIndex >= lstArquivosJuntar.Items.Count) return;
            object selected = lstArquivosJuntar.SelectedItem;
            lstArquivosJuntar.Items.Remove(selected);
            lstArquivosJuntar.Items.Insert(newIndex, selected);
            lstArquivosJuntar.SetSelected(newIndex, true);
        }

        private async void BtnJuntarExecutar_Click(object sender, EventArgs e)
        {
            if (lstArquivosJuntar.Items.Count < 2) { MessageBox.Show("Adicione pelo menos 2 arquivos."); return; }
            using (var save = new SaveFileDialog { Filter = "PDF|*.pdf", FileName = "unido.pdf" })
            {
                if (save.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        SetCarregando(true, "Unindo arquivos...");
                        string[] arquivos = lstArquivosJuntar.Items.Cast<string>().ToArray();
                        await _service.JuntarArquivosAsync(arquivos, save.FileName);
                        MessageBox.Show("Sucesso!");
                    }
                    catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
                    finally { SetCarregando(false); }
                }
            }
        }

        // --- ABA: PROTEGER (CRIADA VIA CÓDIGO) ---
        private void CriarAbaProteger()
        {
            foreach (TabPage t in tabControlPrincipal.TabPages) if (t.Text == "Proteger") return; // Evita duplicar

            TabPage tabProteger = new TabPage("Proteger");
            GroupBox gb = new GroupBox { Text = "Adicionar Senha", Location = new Point(13, 13), Size = new Size(450, 150) };

            lblArquivoProteger = new Label { Text = "Arraste o arquivo aqui ou selecione", Location = new Point(15, 65), AutoSize = true };
            btnProtegerSelecionar = new Button { Text = "Selecionar PDF", Location = new Point(15, 25), Size = new Size(150, 30) };
            Label lblSenha = new Label { Text = "Senha:", Location = new Point(180, 30), AutoSize = true };
            txtSenhaProtecao = new TextBox { Location = new Point(230, 27), Size = new Size(150, 23), PasswordChar = '*' };
            btnProtegerExecutar = new Button { Text = "Proteger PDF", Location = new Point(15, 100), Size = new Size(420, 35) };

            btnProtegerSelecionar.Click += (s, e) => {
                using (var op = new OpenFileDialog { Filter = "PDF|*.pdf" })
                    if (op.ShowDialog() == DialogResult.OK) { arquivoParaProteger = op.FileName; lblArquivoProteger.Text = Path.GetFileName(arquivoParaProteger); }
            };
            btnProtegerExecutar.Click += async (s, e) => {
                if (string.IsNullOrEmpty(arquivoParaProteger)) { MessageBox.Show("Selecione um arquivo."); return; }
                if (string.IsNullOrEmpty(txtSenhaProtecao.Text)) { MessageBox.Show("Digite uma senha."); return; }
                using (var save = new SaveFileDialog { Filter = "PDF|*.pdf", FileName = Path.GetFileNameWithoutExtension(arquivoParaProteger) + "_protegido.pdf" })
                {
                    if (save.ShowDialog() == DialogResult.OK)
                    {
                        try { SetCarregando(true, "Aplicando senha..."); await _service.ProtegerPdfAsync(arquivoParaProteger, save.FileName, txtSenhaProtecao.Text); MessageBox.Show("Sucesso!"); }
                        catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
                        finally { SetCarregando(false); }
                    }
                }
            };
            gb.Controls.AddRange(new Control[] { btnProtegerSelecionar, lblArquivoProteger, lblSenha, txtSenhaProtecao, btnProtegerExecutar });
            tabProteger.Controls.Add(gb);

            // Adiciona a aba antes da última (Ajuda)
            if (tabControlPrincipal.TabPages.Count > 0) tabControlPrincipal.TabPages.Insert(tabControlPrincipal.TabPages.Count - 1, tabProteger);
            else tabControlPrincipal.TabPages.Add(tabProteger);
        }

        // --- ABA: MARCA D'ÁGUA (CRIADA VIA CÓDIGO) ---
        private void CriarAbaMarcaDagua()
        {
            foreach (TabPage t in tabControlPrincipal.TabPages) if (t.Text == "Marca D'água") return; // Evita duplicar

            TabPage tabMarca = new TabPage("Marca D'água");
            GroupBox gb = new GroupBox { Text = "Configurar Marca", Location = new Point(13, 13), Size = new Size(450, 150) };
            lblArquivoMarca = new Label { Text = "Arraste o arquivo aqui ou selecione", Location = new Point(15, 65), AutoSize = true };
            btnMarcaSelecionar = new Button { Text = "Selecionar PDF", Location = new Point(15, 25), Size = new Size(150, 30) };
            Label lblTexto = new Label { Text = "Texto:", Location = new Point(180, 30), AutoSize = true };
            txtMarcaTexto = new TextBox { Text = "CONFIDENCIAL", Location = new Point(230, 27), Size = new Size(150, 23) };
            btnMarcaExecutar = new Button { Text = "Aplicar Marca D'água", Location = new Point(15, 100), Size = new Size(420, 35) };

            btnMarcaSelecionar.Click += (s, e) => {
                using (var op = new OpenFileDialog { Filter = "PDF|*.pdf" })
                    if (op.ShowDialog() == DialogResult.OK) { arquivoParaMarca = op.FileName; lblArquivoMarca.Text = Path.GetFileName(arquivoParaMarca); }
            };
            btnMarcaExecutar.Click += async (s, e) => {
                if (string.IsNullOrEmpty(arquivoParaMarca)) { MessageBox.Show("Selecione um arquivo."); return; }
                if (string.IsNullOrEmpty(txtMarcaTexto.Text)) { MessageBox.Show("Digite o texto."); return; }
                using (var save = new SaveFileDialog { Filter = "PDF|*.pdf", FileName = Path.GetFileNameWithoutExtension(arquivoParaMarca) + "_marca.pdf" })
                {
                    if (save.ShowDialog() == DialogResult.OK)
                    {
                        try { SetCarregando(true, "Aplicando marca..."); await _service.AdicionarMarcaDaguaAsync(arquivoParaMarca, save.FileName, txtMarcaTexto.Text); MessageBox.Show("Sucesso!"); }
                        catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
                        finally { SetCarregando(false); }
                    }
                }
            };
            gb.Controls.AddRange(new Control[] { btnMarcaSelecionar, lblArquivoMarca, lblTexto, txtMarcaTexto, btnMarcaExecutar });
            tabMarca.Controls.Add(gb);

            // Adiciona a aba antes da última
            if (tabControlPrincipal.TabPages.Count > 0) tabControlPrincipal.TabPages.Insert(tabControlPrincipal.TabPages.Count - 1, tabMarca);
            else tabControlPrincipal.TabPages.Add(tabMarca);
        }

        // --- AJUDA ---
        private void PreencherTutorial()
        {
            var v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            txtAjuda.Text = $"pdfZ - Versão {v}\r\n\r\n" +
                            "GUIA DE FUNÇÕES:\r\n\r\n" +
                            "1. COMPRESSÃO: Reduz PDF (Modo Extremo = 50 DPI).\r\n" +
                            "2. JUNTAR: Combina PDFs.\r\n" +
                            "3. DIVIDIR: Separa páginas.\r\n" +
                            "4. ROTACIONAR: Gira páginas.\r\n" +
                            "5. IMAGEM P/ PDF: Converte fotos.\r\n" +
                            "6. PROTEGER: Adiciona senha.\r\n" +
                            "7. MARCA D'ÁGUA: Adiciona texto diagonal.";
        }

        // --- CORREÇÃO DO ERRO DO DESIGNER ---
        // ESTA FUNÇÃO É OBRIGATÓRIA PORQUE O DESIGNER ESTÁ PROCURANDO POR ELA
        private void txtAjuda_TextChanged(object sender, EventArgs e)
        {
            // Não precisa fazer nada, só existir para o Designer não travar.
        }
        // ------------------------------------

        // AUXILIARES
        private void ConfigurarInterfaceBase()
        {
            barraProgresso = new ToolStripProgressBar { Size = new Size(200, 16), Style = ProgressBarStyle.Marquee, Visible = false };
            statusRodape.Items.Add(barraProgresso);
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            cmbAngulo.Items.Clear(); cmbAngulo.Items.AddRange(new object[] { "90° (Horário)", "180°", "270° (Anti-horário)" }); cmbAngulo.SelectedIndex = 0;
        }

        private void ConfigurarDragDrop() { this.AllowDrop = true; this.DragEnter += (s, e) => { if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy; }; this.DragDrop += Form1_DragDrop; }
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] arquivos = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (arquivos.Length == 0) return;
            string ext = Path.GetExtension(arquivos[0]).ToLower();
            if (ext != ".pdf") return;
            string nomeAba = tabControlPrincipal.SelectedTab.Text;
            if (tabControlPrincipal.SelectedTab == tabControlPrincipal.TabPages[0]) { arquivoCompressao = arquivos[0]; lblArquivoCompressao.Text = Path.GetFileName(arquivoCompressao); arquivoDeEntrada = arquivoCompressao; lblArquivo.Text = Path.GetFileName(arquivoDeEntrada); }
            else if (tabControlPrincipal.SelectedTab == tabJuntar || nomeAba.Contains("Juntar")) { foreach (var arq in arquivos) if (Path.GetExtension(arq).ToLower() == ".pdf") lstArquivosJuntar.Items.Add(arq); }
            else if (nomeAba == "Proteger") { arquivoParaProteger = arquivos[0]; lblArquivoProteger.Text = Path.GetFileName(arquivoParaProteger); }
            else if (nomeAba == "Marca D'água") { arquivoParaMarca = arquivos[0]; lblArquivoMarca.Text = Path.GetFileName(arquivoParaMarca); }
            else { arquivoDeEntrada = arquivos[0]; lblArquivo.Text = Path.GetFileName(arquivoDeEntrada); }
        }

        private void CriarBotaoTema() { ToolStripButton btnTema = new ToolStripButton("🌙 Tema"); btnTema.Click += (s, e) => AlternarTema(); statusRodape.Items.Insert(0, btnTema); }
        private void AlternarTema()
        {
            bool escuro = this.BackColor.R > 100;
            Color fundo = escuro ? Color.FromArgb(45, 45, 48) : Color.WhiteSmoke;
            Color fundoControle = escuro ? Color.FromArgb(30, 30, 30) : Color.White;
            Color texto = escuro ? Color.White : Color.Black;
            this.BackColor = fundo; tabControlPrincipal.BackColor = fundo; statusRodape.BackColor = fundoControle; statusRodape.ForeColor = texto;
            foreach (TabPage tab in tabControlPrincipal.TabPages)
            {
                tab.BackColor = fundo; tab.ForeColor = texto;
                foreach (Control c in tab.Controls) if (c is GroupBox gb) { gb.ForeColor = texto; foreach (Control sub in gb.Controls) { if (sub is Button btn) { btn.BackColor = fundoControle; btn.ForeColor = texto; btn.FlatStyle = FlatStyle.Flat; } if (sub is TextBox txt) { txt.BackColor = fundoControle; txt.ForeColor = texto; } if (sub is ListBox lst) { lst.BackColor = fundoControle; lst.ForeColor = texto; } if (sub is ComboBox cmb) { cmb.BackColor = fundoControle; cmb.ForeColor = texto; } if (sub is Label lbl) lbl.ForeColor = texto; } }
            }
        }

        private void ConfigurarAutoUpdater() { AutoUpdater.RunUpdateAsAdmin = false; AutoUpdater.Start(URL_UPDATE_XML); }
        private void SetCarregando(bool carregando, string texto = "Pronto.") { tabControlPrincipal.Enabled = !carregando; barraProgresso.Visible = carregando; lblStatusPrincipal.Text = carregando ? texto : "Concluído."; Cursor = carregando ? Cursors.WaitCursor : Cursors.Default; }

        // Placeholders e funções antigas para manter compatibilidade
        private void btnSelecionar_Click(object sender, EventArgs e) { }
        private void btnComprimir_Click(object sender, EventArgs e) { }
        private void btnJuntar_Click(object sender, EventArgs e) { }
        private async void btnDividir_Click(object sender, EventArgs e) { using (var open = new OpenFileDialog { Filter = "PDF|*.pdf" }) if (open.ShowDialog() == DialogResult.OK) using (var folder = new FolderBrowserDialog()) if (folder.ShowDialog() == DialogResult.OK) { try { SetCarregando(true, "Dividindo..."); await _service.DividirArquivoAsync(open.FileName, folder.SelectedPath); MessageBox.Show("Sucesso!"); } catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); } finally { SetCarregando(false); } } }
        private async void btnRotacionar_Click(object sender, EventArgs e) { int ang = (cmbAngulo.SelectedIndex + 1) * 90; if (ang == 360) ang = 270; using (var open = new OpenFileDialog { Filter = "PDF|*.pdf" }) if (open.ShowDialog() == DialogResult.OK) using (var save = new SaveFileDialog { Filter = "PDF|*.pdf", FileName = Path.GetFileNameWithoutExtension(open.FileName) + "_rot.pdf" }) if (save.ShowDialog() == DialogResult.OK) { try { SetCarregando(true, "Rotacionando..."); await _service.RotacionarArquivoAsync(open.FileName, save.FileName, ang); MessageBox.Show("Sucesso!"); } catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); } finally { SetCarregando(false); } } }
        private async void btnImagemParaPdf_Click(object sender, EventArgs e) { using (var open = new OpenFileDialog { Filter = "Imagens|*.jpg;*.png", Multiselect = true }) if (open.ShowDialog() == DialogResult.OK) using (var save = new SaveFileDialog { Filter = "PDF|*.pdf", FileName = "album.pdf" }) if (save.ShowDialog() == DialogResult.OK) { try { SetCarregando(true, "Convertendo..."); await _service.ImagensParaPdfAsync(open.FileNames, save.FileName); MessageBox.Show("Sucesso!"); } catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); } finally { SetCarregando(false); } } }
    }
}