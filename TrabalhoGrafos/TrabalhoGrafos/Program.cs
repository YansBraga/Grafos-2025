using System;
using TrabalhoGrafos.Classes;
using TrabalhoGrafos.Interfaces;

namespace TrabalhoGrafos
{
    internal class Program
    {
        // Variável global para manter o grafo carregado na memória
        public static IGrafo grafo;

        static void Main(string[] args)
        {
            int opcao = -1;

            while (opcao != 0)
            {
                Console.Clear();
                Console.WriteLine("=== SISTEMA DE OTIMIZAÇÃO LOGÍSTICA (SORL) ===");
                Console.WriteLine("1 - Importar Arquivo DIMACS");
                Console.WriteLine("0 - Sair");
                Console.Write("\nEscolha uma opção: ");

                if (!int.TryParse(Console.ReadLine(), out opcao))
                {
                    Console.WriteLine("Entrada inválida! Pressione qualquer tecla...");
                    Console.ReadKey();
                    continue;
                }

                switch (opcao)
                {
                    case 1:
                        Console.Write("\nDigite o ID do arquivo (1 a 7): ");
                        if (int.TryParse(Console.ReadLine(), out int idArquivo) && idArquivo >= 1 && idArquivo <= 7)
                        {
                            try
                            {
                                Console.WriteLine($"Carregando grafo0{idArquivo}.dimacs...");

                                grafo = Arquivo.ImportarArquivo(idArquivo);

                                Console.WriteLine("Grafo carregado com sucesso! Pressione algo para continuar.");
                                Console.ReadKey();

                                MenuOpcoes(idArquivo);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Erro ao importar: {ex.Message}");
                                Console.ReadKey();
                            }
                        }
                        else
                        {
                            Console.WriteLine("ID inválido! Deve ser entre 1 e 7.");
                            Console.ReadKey();
                        }
                        break;

                    case 0:
                        Console.WriteLine("Encerrando sistema...");
                        break;

                    default:
                        Console.WriteLine("Opção inválida.");
                        break;
                }
            }
        }


        public static void MenuOpcoes(int idArquivo)
        {
            int opcao = -1;
            while (opcao != 0)
            {
                Console.Clear();
                Console.WriteLine($"=== ANÁLISES DO GRAFO {idArquivo} ===");
                Console.WriteLine($"Vértices: {grafo.NumeroVertices} | Tipo: {grafo.GetType().Name}");
                Console.WriteLine("---------------------------------------------");

                Console.WriteLine("1 - Roteamento de Menor Custo (Dijkstra)");
                Console.WriteLine("2 - Capacidade Máxima (Fluxo Máximo)");
                Console.WriteLine("3 - Expansão da Rede (Árvore Geradora Mínima)");
                Console.WriteLine("4 - Agendamento de Manutenções (Coloração)");
                Console.WriteLine("5 - Rota de Inspeção (Euleriano/Hamiltoniano)");
                Console.WriteLine("---------------------------------------------");


                Console.WriteLine("0 - Voltar / Trocar Arquivo");

                Console.Write("\nEscolha o algoritmo: ");

                if (!int.TryParse(Console.ReadLine(), out opcao)) continue;

                Console.Clear();

                switch (opcao)
                {
                    case 1:
                        Console.Write("Informe o HUB de origem: ");
                        int origem = int.Parse(Console.ReadLine());

                        Console.Write("Informe o destino final: ");
                        int destino = int.Parse(Console.ReadLine());

                        Console.WriteLine(Algoritmos.Dijkstra(grafo, origem, destino, idArquivo));
                        Console.ReadKey();
                        break;
                    case 2:
                        Console.Write("Informe o HUB origem (fonte S): ");
                        int s = int.Parse(Console.ReadLine());

                        Console.Write("Informe o HUB destino (sorvedouro T): ");
                        int t = int.Parse(Console.ReadLine());

                        Console.WriteLine(Algoritmos.FluxoMaximoEdmondsKarp(grafo, s, t, idArquivo));
                        Console.ReadKey();
                        break;
                    case 3:
                        Console.WriteLine(Algoritmos.ArvoreGeradoraMinima(grafo, idArquivo));                        
                        Console.ReadKey();
                        break;
                    case 4:
                        Console.WriteLine(Algoritmos.Coloracao(grafo, idArquivo));                        
                        Console.ReadKey();
                        break;
                    case 5:
                        Console.Write("Informe o HUB de origem: ");
                        int verticeOrigem = int.Parse(Console.ReadLine());
                        Console.WriteLine(Algoritmos.RotaInspecao(grafo, idArquivo, verticeOrigem));
                        Console.ReadKey();
                        break;
                    case 0:
                        return; // Sai do método e volta para o Main
                    default:
                        Console.WriteLine("Opção inválida!");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}
