using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrabalhoGrafos.Interfaces;

namespace TrabalhoGrafos.Classes
{
    public class Algoritmos
    {
        public static string Dijkstra(IGrafo grafo, int origem, int destino, int idArquivo)
        {
            // --- PASSO 1: INICIALIZAÇÃO (Pseudocódigo item 1) ---
            // dist[v] <- infinito
            // pred[v] <- null (0 no nosso caso)
            int n = grafo.NumeroVertices;

            // Usamos Dicionário para garantir que funciona mesmo se os IDs forem espalhados (ex: 1, 5, 900)
            // Se fosse garantido sequencial, poderíamos usar int[n + 1]
            var dist = new Dictionary<int, int>();
            var pred = new Dictionary<int, int>();
            var S = new HashSet<int>(); // Conjunto de explorados (Item 2)

            // Fila de Prioridade para realizar o passo 4a eficientemente
            // Ela guarda <Vertice, DistanciaAcumulada> e sempre entrega o menor
            var pq = new PriorityQueue<int, int>();

            // Inicializa tudo
            // Nota: Não precisamos varrer todos os vértices agora, inicializamos sob demanda (lazy)
            // mas conceitualmente todos são infinito.

            // --- PASSO 2 e 3: Raiz (Pseudocódigo item 2 e 3) ---
            dist[origem] = 0;
            pq.Enqueue(origem, 0);

            // --- PASSO 4: Loop Principal ---
            while (pq.Count > 0)
            {
                // PASSO 4a: Encontrar vértice com menor distância (que não está em S)
                if (!pq.TryDequeue(out int v, out int distanciaAtual)) break;

                // Se v já está em S (explorado), ignora
                if (S.Contains(v)) continue;

                // PASSO 4c: Adicionar v ao conjunto S
                S.Add(v);

                // Se chegamos ao destino, podemos parar antes (otimização)
                if (v == destino) break;

                // Analisa os vizinhos para atualizar distâncias (Relaxamento)
                foreach (var aresta in grafo.ObterAdjacentes(v)) // Aqui pegamos as arestas de 'v'
                {
                    int w = aresta.Destino;
                    int pesoAresta = aresta.Peso; // d_vw

                    // Se w já foi explorado (está em S), pula
                    if (S.Contains(w)) continue;

                    // Recupera distância atual de w (se não existir, é infinito)
                    int distW = dist.ContainsKey(w) ? dist[w] : int.MaxValue;

                    // PASSO 4b: Atualizar dist[w] e pred[w] se achou caminho melhor
                    if (distanciaAtual + pesoAresta < distW)
                    {
                        dist[w] = distanciaAtual + pesoAresta;
                        pred[w] = v; // v é o pai de w

                        // Recoloca na fila para ser avaliado no futuro
                        pq.Enqueue(w, dist[w]);
                    }
                }
            }

            // --- PÓS-PROCESSAMENTO: Montar a String de Retorno e Logar ---

            // Verifica se chegou no destino
            if (!dist.ContainsKey(destino))
            {
                string msgErro = $"Não existe caminho entre {origem} e {destino}.";
                Log.Escrever("Roteamento de Menor Custo", msgErro, idArquivo);
                return msgErro;
            }

            // Recupera o caminho andando para trás pelo array 'pred'
            var caminho = new List<int>();
            int atual = destino;
            while (atual != 0) // 0 significa null/sem pai
            {
                caminho.Add(atual);

                if (atual == origem) break; // Chegou no início

                if (pred.ContainsKey(atual))
                    atual = pred[atual];
                else
                    break; // Segurança
            }
            caminho.Reverse(); // Inverte para ficar Origem -> Destino

            // Formata a saída
            string caminhoTexto = string.Join(" -> ", caminho);
            int custoTotal = dist[destino];

            string resultadoFinal = $"Custo Total: {custoTotal} | Caminho: {caminhoTexto}";

            // Grava o log
            Log.Escrever("Roteamento de Menor Custo", resultadoFinal, idArquivo);

            return resultadoFinal;
        }
    }
}
