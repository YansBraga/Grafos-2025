using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrabalhoGrafos.Interfaces;

namespace TrabalhoGrafos.Classes
{
    public class Arquivo
    {
        public static IGrafo ImportarArquivo(int opc)
        {
            try
            {
                Log.LimparLog(opc.ToString());

                List<Aresta> arestas = new List<Aresta>();                

                var projetoDir = Directory.GetParent(AppContext.BaseDirectory)      // ...\bin\Debug\net8.0
                         .Parent // ...\bin\Debug
                         .Parent // ...\bin
                         .Parent // ...\<pasta do projeto>
                         .FullName;

                string path = Path.Combine(projetoDir, $"grafo0{opc}.dimacs");
                string[] linhas = File.ReadAllLines(path);

                string[] primeiraLinha = linhas[0].Split(' ');
                int numVertices = int.Parse(primeiraLinha[0]);
                int numArestas = int.Parse(primeiraLinha[1]);


                // Processa as linhas das arestas
                for (int i = 1; i < linhas.Length; i++)
                {

                    string[] dadosAresta = linhas[i].Split(' ');
                    int origem = int.Parse(dadosAresta[0]);
                    int destino = int.Parse(dadosAresta[1]);
                    int peso = int.Parse(dadosAresta[2]);
                    int capacidade = int.Parse(dadosAresta[3]);

                    arestas.Add(new Aresta(origem, destino, peso, capacidade));
                }

                IGrafo grafo = Representacao.CriarGrafo(numVertices, numArestas);
                Log.Escrever("Importação de Arquivo", $"Arquivo grafo0{opc}.dimacs importado com sucesso.\n\n Representação definida: {grafo.ToString()}", opc.ToString());

                return grafo;
            }
            catch (FileNotFoundException)
            {
                throw new Exception("ERRO: O arquivo não foi encontrado. Verifique o caminho e tente novamente.");
            }
            catch (Exception ex)
            {
                throw new Exception($"ERRO inesperado ao ler o arquivo: {ex.Message}");
            }
        }
    }
}
