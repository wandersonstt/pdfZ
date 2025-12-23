using System;
using System.IO;
using System.Windows.Forms;
using PdfSharp.Fonts; // Importante para o IFontResolver

namespace pdfZ
{
    static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 1. REGISTRA O RESOLVADOR DE FONTES ANTES DE TUDO
            // Isso ensina o PDFsharp a encontrar a fonte Arial no Windows
            GlobalFontSettings.FontResolver = new WindowsFontResolver();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    // --- CLASSE AUXILIAR PARA CORRIGIR O ERRO DE FONTE ---
    public class WindowsFontResolver : IFontResolver
    {
        public string DefaultFontName => "Arial";

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            // Se pedir Arial (ou qualquer outra coisa, como fallback), usa a Arial do Windows
            string nomeArquivo = "arial.ttf";

            if (familyName.Equals("Arial", StringComparison.OrdinalIgnoreCase))
            {
                if (isBold && isItalic) nomeArquivo = "arialbi.ttf";
                else if (isBold) nomeArquivo = "arialbd.ttf";
                else if (isItalic) nomeArquivo = "ariali.ttf";
            }

            // Retorna uma referência interna para o método GetFont usar depois
            return new FontResolverInfo(nomeArquivo);
        }

        public byte[] GetFont(string faceName)
        {
            // Procura a fonte na pasta padrão do Windows
            string caminhoFonte = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), faceName);

            if (File.Exists(caminhoFonte))
            {
                return File.ReadAllBytes(caminhoFonte);
            }

            // Se não achar (ex: Linux ou erro), tenta retornar a Arial padrão como salvaguarda
            // ou lança erro se realmente não existir.
            return null;
        }
    }
}