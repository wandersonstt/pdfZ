namespace pdfZ
{
    partial class Form1
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tabControlPrincipal = new System.Windows.Forms.TabControl();
            this.tabCompressao = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnComprimir = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblArquivo = new System.Windows.Forms.Label();
            this.btnSelecionar = new System.Windows.Forms.Button();
            this.tabJuntar = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnJuntar = new System.Windows.Forms.Button();
            this.tabDividir = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnDividir = new System.Windows.Forms.Button();
            this.tabRotacionar = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbAngulo = new System.Windows.Forms.ComboBox();
            this.btnRotacionar = new System.Windows.Forms.Button();
            this.tabImagem = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnImagemParaPdf = new System.Windows.Forms.Button();
            this.tabAjuda = new System.Windows.Forms.TabPage();
            this.txtAjuda = new System.Windows.Forms.TextBox();
            this.statusRodape = new System.Windows.Forms.StatusStrip();
            this.lblStatusPrincipal = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblNomeAutor = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControlPrincipal.SuspendLayout();
            this.tabCompressao.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabJuntar.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabDividir.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabRotacionar.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabImagem.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tabAjuda.SuspendLayout();
            this.statusRodape.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlPrincipal
            // 
            this.tabControlPrincipal.Controls.Add(this.tabCompressao);
            this.tabControlPrincipal.Controls.Add(this.tabJuntar);
            this.tabControlPrincipal.Controls.Add(this.tabDividir);
            this.tabControlPrincipal.Controls.Add(this.tabRotacionar);
            this.tabControlPrincipal.Controls.Add(this.tabImagem);
            this.tabControlPrincipal.Controls.Add(this.tabAjuda);
            this.tabControlPrincipal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlPrincipal.Location = new System.Drawing.Point(0, 0);
            this.tabControlPrincipal.Name = "tabControlPrincipal";
            this.tabControlPrincipal.SelectedIndex = 0;
            this.tabControlPrincipal.Size = new System.Drawing.Size(776, 401);
            this.tabControlPrincipal.TabIndex = 0;
            // 
            // tabCompressao
            // 
            this.tabCompressao.Controls.Add(this.groupBox2);
            this.tabCompressao.Controls.Add(this.groupBox1);
            this.tabCompressao.Location = new System.Drawing.Point(4, 24);
            this.tabCompressao.Name = "tabCompressao";
            this.tabCompressao.Padding = new System.Windows.Forms.Padding(10);
            this.tabCompressao.Size = new System.Drawing.Size(768, 373);
            this.tabCompressao.TabIndex = 0;
            this.tabCompressao.Text = "Compressão";
            this.tabCompressao.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnComprimir);
            this.groupBox2.Location = new System.Drawing.Point(13, 118);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(188, 79);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Passo 2: Comprimir";
            // 
            // btnComprimir
            // 
            this.btnComprimir.Location = new System.Drawing.Point(19, 29);
            this.btnComprimir.Name = "btnComprimir";
            this.btnComprimir.Size = new System.Drawing.Size(150, 40);
            this.btnComprimir.TabIndex = 1;
            this.btnComprimir.Text = "Comprimir Agora!";
            this.btnComprimir.UseVisualStyleBackColor = true;
            this.btnComprimir.Click += new System.EventHandler(this.btnComprimir_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblArquivo);
            this.groupBox1.Controls.Add(this.btnSelecionar);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(188, 99);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Passo 1: Selecionar";
            // 
            // lblArquivo
            // 
            this.lblArquivo.AutoSize = true;
            this.lblArquivo.Location = new System.Drawing.Point(16, 76);
            this.lblArquivo.Name = "lblArquivo";
            this.lblArquivo.Size = new System.Drawing.Size(163, 15);
            this.lblArquivo.TabIndex = 2;
            this.lblArquivo.Text = "Nenhum arquivo selecionado";
            // 
            // btnSelecionar
            // 
            this.btnSelecionar.Location = new System.Drawing.Point(19, 29);
            this.btnSelecionar.Name = "btnSelecionar";
            this.btnSelecionar.Size = new System.Drawing.Size(150, 40);
            this.btnSelecionar.TabIndex = 0;
            this.btnSelecionar.Text = "Selecionar PDF";
            this.btnSelecionar.UseVisualStyleBackColor = true;
            this.btnSelecionar.Click += new System.EventHandler(this.btnSelecionar_Click);
            // 
            // tabJuntar
            // 
            this.tabJuntar.Controls.Add(this.groupBox3);
            this.tabJuntar.Location = new System.Drawing.Point(4, 24);
            this.tabJuntar.Name = "tabJuntar";
            this.tabJuntar.Padding = new System.Windows.Forms.Padding(10);
            this.tabJuntar.Size = new System.Drawing.Size(768, 373);
            this.tabJuntar.TabIndex = 1;
            this.tabJuntar.Text = "Juntar";
            this.tabJuntar.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.btnJuntar);
            this.groupBox3.Location = new System.Drawing.Point(13, 13);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(450, 106);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Juntar Documentos";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 75);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(280, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "Selecione 2 ou mais PDFs para uni-los em um único";
            // 
            // btnJuntar
            // 
            this.btnJuntar.Location = new System.Drawing.Point(19, 29);
            this.btnJuntar.Name = "btnJuntar";
            this.btnJuntar.Size = new System.Drawing.Size(150, 40);
            this.btnJuntar.TabIndex = 4;
            this.btnJuntar.Text = "Juntar PDFs...";
            this.btnJuntar.UseVisualStyleBackColor = true;
            this.btnJuntar.Click += new System.EventHandler(this.btnJuntar_Click);
            // 
            // tabDividir
            // 
            this.tabDividir.Controls.Add(this.groupBox4);
            this.tabDividir.Location = new System.Drawing.Point(4, 24);
            this.tabDividir.Name = "tabDividir";
            this.tabDividir.Padding = new System.Windows.Forms.Padding(10);
            this.tabDividir.Size = new System.Drawing.Size(768, 373);
            this.tabDividir.TabIndex = 2;
            this.tabDividir.Text = "Dividir";
            this.tabDividir.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.btnDividir);
            this.groupBox4.Location = new System.Drawing.Point(13, 13);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(450, 106);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Dividir Documento";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(332, 15);
            this.label2.TabIndex = 7;
            this.label2.Text = "Pega um PDF e salva cada página como um arquivo separado";
            // 
            // btnDividir
            // 
            this.btnDividir.Location = new System.Drawing.Point(19, 29);
            this.btnDividir.Name = "btnDividir";
            this.btnDividir.Size = new System.Drawing.Size(150, 40);
            this.btnDividir.TabIndex = 6;
            this.btnDividir.Text = "Dividir PDF (por página)";
            this.btnDividir.UseVisualStyleBackColor = true;
            this.btnDividir.Click += new System.EventHandler(this.btnDividir_Click);
            // 
            // tabRotacionar
            // 
            this.tabRotacionar.Controls.Add(this.groupBox5);
            this.tabRotacionar.Location = new System.Drawing.Point(4, 24);
            this.tabRotacionar.Name = "tabRotacionar";
            this.tabRotacionar.Padding = new System.Windows.Forms.Padding(10);
            this.tabRotacionar.Size = new System.Drawing.Size(768, 373);
            this.tabRotacionar.TabIndex = 3;
            this.tabRotacionar.Text = "Rotacionar";
            this.tabRotacionar.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this.cmbAngulo);
            this.groupBox5.Controls.Add(this.btnRotacionar);
            this.groupBox5.Location = new System.Drawing.Point(13, 13);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(450, 122);
            this.groupBox5.TabIndex = 1;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Rotacionar Documento";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(285, 15);
            this.label3.TabIndex = 9;
            this.label3.Text = "Gira as páginas do PDF no ângulo selecionado acima";
            // 
            // cmbAngulo
            // 
            this.cmbAngulo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAngulo.FormattingEnabled = true;
            this.cmbAngulo.Location = new System.Drawing.Point(175, 38);
            this.cmbAngulo.Name = "cmbAngulo";
            this.cmbAngulo.Size = new System.Drawing.Size(150, 23);
            this.cmbAngulo.TabIndex = 8;
            // 
            // btnRotacionar
            // 
            this.btnRotacionar.Location = new System.Drawing.Point(19, 29);
            this.btnRotacionar.Name = "btnRotacionar";
            this.btnRotacionar.Size = new System.Drawing.Size(150, 40);
            this.btnRotacionar.TabIndex = 7;
            this.btnRotacionar.Text = "Rotacionar PDF";
            this.btnRotacionar.UseVisualStyleBackColor = true;
            this.btnRotacionar.Click += new System.EventHandler(this.btnRotacionar_Click);
            // 
            // tabImagem
            // 
            this.tabImagem.Controls.Add(this.groupBox6);
            this.tabImagem.Location = new System.Drawing.Point(4, 24);
            this.tabImagem.Name = "tabImagem";
            this.tabImagem.Padding = new System.Windows.Forms.Padding(10);
            this.tabImagem.Size = new System.Drawing.Size(768, 373);
            this.tabImagem.TabIndex = 4;
            this.tabImagem.Text = "Imagem p/ PDF";
            this.tabImagem.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.label4);
            this.groupBox6.Controls.Add(this.btnImagemParaPdf);
            this.groupBox6.Location = new System.Drawing.Point(13, 13);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(450, 106);
            this.groupBox6.TabIndex = 1;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Converter Imagem para PDF";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 75);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(300, 15);
            this.label4.TabIndex = 11;
            this.label4.Text = "Converte uma ou mais imagens (JPG, PNG) em um PDF";
            // 
            // btnImagemParaPdf
            // 
            this.btnImagemParaPdf.Location = new System.Drawing.Point(19, 29);
            this.btnImagemParaPdf.Name = "btnImagemParaPdf";
            this.btnImagemParaPdf.Size = new System.Drawing.Size(150, 40);
            this.btnImagemParaPdf.TabIndex = 10;
            this.btnImagemParaPdf.Text = "Imagem para PDF...";
            this.btnImagemParaPdf.UseVisualStyleBackColor = true;
            this.btnImagemParaPdf.Click += new System.EventHandler(this.btnImagemParaPdf_Click);
            // 
            // tabAjuda
            // 
            this.tabAjuda.Controls.Add(this.txtAjuda);
            this.tabAjuda.Location = new System.Drawing.Point(4, 24);
            this.tabAjuda.Name = "tabAjuda";
            this.tabAjuda.Padding = new System.Windows.Forms.Padding(3);
            this.tabAjuda.Size = new System.Drawing.Size(768, 373);
            this.tabAjuda.TabIndex = 5;
            this.tabAjuda.Text = "Ajuda";
            this.tabAjuda.UseVisualStyleBackColor = true;
            // 
            // txtAjuda
            // 
            this.txtAjuda.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAjuda.Location = new System.Drawing.Point(3, 3);
            this.txtAjuda.Multiline = true;
            this.txtAjuda.Name = "txtAjuda";
            this.txtAjuda.ReadOnly = true;
            this.txtAjuda.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAjuda.Size = new System.Drawing.Size(762, 367);
            this.txtAjuda.TabIndex = 0;
            this.txtAjuda.TextChanged += new System.EventHandler(this.txtAjuda_TextChanged);
            // 
            // statusRodape
            // 
            this.statusRodape.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.statusRodape.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatusPrincipal,
            this.lblNomeAutor});
            this.statusRodape.Location = new System.Drawing.Point(0, 401);
            this.statusRodape.Name = "statusRodape";
            this.statusRodape.Size = new System.Drawing.Size(776, 22);
            this.statusRodape.TabIndex = 1;
            this.statusRodape.Text = "statusStrip1";
            // 
            // lblStatusPrincipal
            // 
            this.lblStatusPrincipal.Name = "lblStatusPrincipal";
            this.lblStatusPrincipal.Size = new System.Drawing.Size(619, 17);
            this.lblStatusPrincipal.Spring = true;
            this.lblStatusPrincipal.Text = "Pronto.";
            this.lblStatusPrincipal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblNomeAutor
            // 
            this.lblNomeAutor.Name = "lblNomeAutor";
            this.lblNomeAutor.Size = new System.Drawing.Size(142, 17);
            this.lblNomeAutor.Text = "Wanderson Saraiva Torres";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(776, 423);
            this.Controls.Add(this.tabControlPrincipal);
            this.Controls.Add(this.statusRodape);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "pdfZ - Ferramentas PDF";
            this.tabControlPrincipal.ResumeLayout(false);
            this.tabCompressao.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabJuntar.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabDividir.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tabRotacionar.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tabImagem.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.tabAjuda.ResumeLayout(false);
            this.tabAjuda.PerformLayout();
            this.statusRodape.ResumeLayout(false);
            this.statusRodape.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlPrincipal;
        private System.Windows.Forms.TabPage tabCompressao;
        private System.Windows.Forms.TabPage tabJuntar;
        private System.Windows.Forms.StatusStrip statusRodape;
        private System.Windows.Forms.Label lblArquivo;
        private System.Windows.Forms.Button btnSelecionar;
        private System.Windows.Forms.Button btnComprimir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnJuntar;
        private System.Windows.Forms.TabPage tabDividir;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnDividir;
        private System.Windows.Forms.TabPage tabRotacionar;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbAngulo;
        private System.Windows.Forms.Button btnRotacionar;
        private System.Windows.Forms.TabPage tabImagem;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnImagemParaPdf;
        private System.Windows.Forms.TabPage tabAjuda;
        private System.Windows.Forms.TextBox txtAjuda;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusPrincipal;
        private System.Windows.Forms.ToolStripStatusLabel lblNomeAutor;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox6;
    }
}