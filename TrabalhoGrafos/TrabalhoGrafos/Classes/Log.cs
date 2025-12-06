using System;
using System.IO;

namespace TrabalhoGrafos.Classes
{
    public static class Log
    {
        public static void Escrever(string titulo, string mensagem, string idDimacs)
        {
            try
            {                
                var diretorioBase = AppDomain.CurrentDomain.BaseDirectory;
                var caminhoProjeto = Directory.GetParent(diretorioBase).Parent.Parent.FullName;
                string caminhoPastaLogs = Path.Combine(caminhoProjeto, "Logs");
               
                if (!Directory.Exists(caminhoPastaLogs))
                {
                    Directory.CreateDirectory(caminhoPastaLogs);
                }

                string nomeArquivo = Path.Combine(caminhoPastaLogs, $"log_grafo0{idDimacs}.txt");

                using (StreamWriter sw = new StreamWriter(nomeArquivo, true))
                {
                    sw.WriteLine("--------------------------------------------------");
                    sw.WriteLine($"[{DateTime.Now:HH:mm:ss}] - {titulo.ToUpper()}");
                    sw.WriteLine("--------------------------------------------------");
                    sw.WriteLine(mensagem);
                    sw.WriteLine("");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao gravar log: {ex.Message}");
            }
        }

        public static void LimparLog(string idDimacs)
        {
            try
            {
                var diretorioBase = AppDomain.CurrentDomain.BaseDirectory;
                var caminhoProjeto = Directory.GetParent(diretorioBase).Parent.Parent.FullName;
                string caminhoPastaLogs = Path.Combine(caminhoProjeto, "Logs");
                string nomeArquivo = Path.Combine(caminhoPastaLogs, $"log_grafo0{idDimacs}.txt");

                if (File.Exists(nomeArquivo))
                {
                    File.Delete(nomeArquivo);
                }
            }
            catch 
            {
                
            }
        }
    }
}