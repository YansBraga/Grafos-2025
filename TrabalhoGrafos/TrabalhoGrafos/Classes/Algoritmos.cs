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
        public static string FluxoMaximoEdmondsKarp(IGrafo grafo, int s, int t, int idArquivo)
        {
            int n = grafo.NumeroVertices;

            // inicializando o fluxo e construindo rede residual inicial 
            int[,] residual = new int[n + 1, n + 1];

            for (int u = 1; u <= n; u++)
            {
                var adj = grafo.ObterAdjacentes(u);
                foreach (var e in adj)
                {
                    int v = e.Destino;
                    int cap = e.Capacidade;
                    residual[u, v] += cap;    // considerando arestas paralelas
                }
            }

            int fluxoMaximo = 0;
            int[] pai = new int[n + 1];
            int iteracoes = 0; // qtde de caminhos aumentantes encontrados

            // enquanto existir caminho aumentante P em G'(f)
            while (BfsCaminhoAumentante(residual, n, s, t, pai))
            {
                iteracoes++;

                int delta = int.MaxValue;
                int v = t;

                while (v != s)
                {
                    int u = pai[v];
                    delta = Math.Min(delta, residual[u, v]);
                    v = u;
                }

                // para cada aresta (u, v) em P
                v = t;
                while (v != s)
                {
                    int u = pai[v];

                    // atualiza rede residual
                    // aresta direta (u->v): reduz capacidade
                    residual[u, v] -= delta;
                    // aresta reversa (v->u): aumenta capacidade
                    residual[v, u] += delta;

                    v = u;
                }

                // G'(f) atualizada, então acumula o fluxo
                fluxoMaximo += delta;
            }

            // log de saíde

            string resultadoFinal =
                $"Origem (fonte S): {s} | Destino (sorvedouro T): {t} | " +
                $"Fluxo Máximo: {fluxoMaximo} | Caminhos aumentantes usados: {iteracoes}";

            Log.Escrever("Capacidade Máxima de Escoamento (Fluxo Máximo)", resultadoFinal, idArquivo);

            return resultadoFinal;
        }

        /// BFS na rede residual para encontrar caminho aumentante de s ate t, ele preenche pai[] e retorna true se encontrou caminho
        private static bool BfsCaminhoAumentante(int[,] residual, int n, int s, int t, int[] pai)
        {
            for (int i = 1; i <= n; i++)
                pai[i] = -1;

            Queue<int> fila = new Queue<int>();
            fila.Enqueue(s);
            pai[s] = -2; // marca fonte como visitada

            while (fila.Count > 0)
            {
                int u = fila.Dequeue();

                for (int v = 1; v <= n; v++)
                {
                    if (pai[v] == -1 && residual[u, v] > 0)
                    {
                        pai[v] = u;
                        if (v == t)
                            return true;  // chegou no destino, entao ja achou caminho

                        fila.Enqueue(v);
                    }
                }
            }

            return false; // nao existe caminho aumentante
        }


        public static string ArvoreGeradoraMinima(IGrafo grafo, int idArquivo)
        {
            int n = grafo.NumeroVertices;

            bool[] visitado = new bool[n + 1];
            int[] chave = new int[n + 1];      // guarda o menor peso para cada vértice
            int[] pai = new int[n + 1];        // guarda a ligação MST

            // Inicialização
            for (int i = 1; i <= n; i++)
            {
                chave[i] = int.MaxValue;
                pai[i] = -1;
            }

            chave[1] = 0; // começa do vértice 1

            // Algoritmo de Prim
            for (int count = 1; count <= n; count++)
            {
                // encontra o vértice com menor chave que não foi visitado
                int u = -1;
                int min = int.MaxValue;
                for (int v = 1; v <= n; v++)
                {
                    if (!visitado[v] && chave[v] < min)
                    {
                        min = chave[v];
                        u = v;
                    }
                }

                if (u == -1) break; // caso desconexo
                visitado[u] = true;

                // atualiza os vértices adjacentes
                foreach (var aresta in grafo.ObterAdjacentes(u))
                {
                    int v = aresta.Destino;
                    int peso = aresta.Peso;

                    if (!visitado[v] && peso < chave[v])
                    {
                        chave[v] = peso;
                        pai[v] = u;
                    }
                }
            }

            // Calcula peso total da AGM
            int pesoTotal = 0;
            for (int i = 2; i <= n; i++)
                if (pai[i] != -1)
                    pesoTotal += chave[i];

            // Prepara log detalhado
            StringBuilder logDetalhado = new StringBuilder();
            logDetalhado.AppendLine($"Árvore Geradora Mínima do arquivo grafo0{idArquivo}.dimacs:");
            logDetalhado.AppendLine($"Vértices: {n} | Peso Total da AGM: {pesoTotal} | Arestas na AGM: {n - 1}");
            logDetalhado.AppendLine("Arestas da AGM:");

            // Lista todas as arestas da AGM
            for (int i = 2; i <= n; i++)
            {
                if (pai[i] != -1)
                {
                    logDetalhado.AppendLine($"Aresta: {pai[i]} - {i} | Peso: {chave[i]}");
                }
                else
                {
                    // Para casos desconexos
                    logDetalhado.AppendLine($"Vértice {i} não conectado à AGM");
                }
            }

            // Escreve log no arquivo
            Log.Escrever("Árvore Geradora Mínima (MST)", logDetalhado.ToString(), idArquivo);

            // Output conciso no terminal
            string resultadoFinal = $"Vértices: {n} | Peso Total da AGM: {pesoTotal} | Arestas na AGM: {n - 1}";

            return resultadoFinal;
        }



        public static string Coloracao(IGrafo grafo, int idArquivo)
        {
            int n = grafo.NumeroVertices;
            int[] cores = new int[n + 1]; // vetor para armazenar a cor de cada vértice (1..n)
            for (int i = 1; i <= n; i++) cores[i] = 0;

            for (int vertice = 1; vertice <= n; vertice++)
            {
                bool[] coresAdjacentes = new bool[n + 1];

                foreach (var aresta in grafo.ObterAdjacentes(vertice))
                {
                    int corVizinho = cores[aresta.Destino];
                    if (corVizinho != 0)
                        coresAdjacentes[corVizinho] = true;
                }

                // Atribui a menor cor disponível
                for (int cor = 1; cor <= n; cor++)
                {
                    if (!coresAdjacentes[cor])
                    {
                        cores[vertice] = cor;
                        break;
                    }
                }
            }

            // Determinar número de cores usadas
            int coresUsadas = 0;
            for (int i = 1; i <= n; i++)
                coresUsadas = Math.Max(coresUsadas, cores[i]);

            // Log detalhado
            string logDetalhado = $"Arquivo grafo0{idArquivo}.dimacs colorido com {coresUsadas} cores.\n";
            for (int i = 1; i <= n; i++)
                logDetalhado += $"Vértice {i} -> Cor {cores[i]}\n";

            Log.Escrever("Coloração de Vértices", logDetalhado, idArquivo);

            // Resultado resumido para o terminal (uma linha, como no fluxo máximo)
            string resultadoFinal = $"Coloração de Vértices: Número de cores usadas = {coresUsadas}";
            return resultadoFinal;
        }






        public static string RotaInspecao(IGrafo grafo, int idArquivo, int verticeOrigem)
        {
            int n = grafo.NumeroVertices;

            // =========================
            // CENÁRIO A – EULERIANO
            // =========================

            // Verifica se todos os vértices têm grau de entrada igual ao grau de saída
            bool euleriano = true;
            for (int u = 1; u <= n; u++)
            {
                int grauSaida = grafo.ObterAdjacentes(u).Count;
                int grauEntrada = 0;
                for (int v = 1; v <= n; v++)
                    grauEntrada += grafo.ObterAdjacentes(v).Count(a => a.Destino == u);

                if (grauEntrada != grauSaida)
                {
                    euleriano = false;
                    break;
                }
            }

            // Verifica conectividade a partir do vértice escolhido
            if (euleriano)
            {
                bool[] visitado = new bool[n + 1];
                DFS(grafo, verticeOrigem, visitado);
                if (visitado.Skip(1).Any(v => !v)) euleriano = false;
            }

            List<int> rotaEuler = new List<int>();
            int pesoEuler = 0;

            if (euleriano)
            {
                rotaEuler = ConstruirCicloEulerianoHierholzer(grafo, verticeOrigem, out pesoEuler);
            }

            // =========================
            // CENÁRIO B – HAMILTONIANO (heurística)
            // =========================

            bool[] visitadoHamilton = new bool[n + 1];
            List<int> rotaHamilton = new List<int>();
            int pesoHamilton = 0;

            int atualH = verticeOrigem;
            rotaHamilton.Add(atualH);
            visitadoHamilton[atualH] = true;

            while (rotaHamilton.Count < n)
            {
                bool encontrou = false;
                foreach (var a in grafo.ObterAdjacentes(atualH))
                {
                    int v = a.Destino;
                    if (!visitadoHamilton[v])
                    {
                        rotaHamilton.Add(v);
                        pesoHamilton += a.Peso;
                        visitadoHamilton[v] = true;
                        atualH = v;
                        encontrou = true;
                        break;
                    }
                }

                if (!encontrou) break;
            }

            bool rotaHamiltonCompleta = rotaHamilton.Count == n;

            // =========================
            // LOG DETALHADO
            // =========================

            StringBuilder logDetalhado = new StringBuilder();
            logDetalhado.AppendLine($"Rota de Inspeção do arquivo grafo0{idArquivo}.dimacs:");
            logDetalhado.AppendLine($"Vértices: {n}");
            logDetalhado.AppendLine($"Vértice de origem escolhido: {verticeOrigem}");
            logDetalhado.AppendLine("");

            // Cenário A
            if (euleriano)
            {
                logDetalhado.AppendLine("Cenário A – Ciclo Euleriano encontrado:");
                logDetalhado.AppendLine($"Peso total: {pesoEuler}");
                logDetalhado.AppendLine("Rota (arestas percorridas):");
                for (int i = 0; i < rotaEuler.Count - 1; i++)
                {
                    int u = rotaEuler[i];
                    int v = rotaEuler[i + 1];
                    int peso = grafo.ObterAdjacentes(u).First(a => a.Destino == v).Peso;
                    logDetalhado.AppendLine($"Aresta: {u} - {v} | Peso: {peso}");
                }
            }
            else
            {
                logDetalhado.AppendLine("Cenário A – Ciclo Euleriano NÃO existe");
            }

            logDetalhado.AppendLine("");

            // Cenário B
            logDetalhado.AppendLine("Cenário B – Percurso de Hubs:");
            if (rotaHamiltonCompleta)
                logDetalhado.AppendLine($"Hamiltoniano heurístico completo | Peso total: {pesoHamilton}");
            else
                logDetalhado.AppendLine($"Hamiltoniano heurístico parcial (não visitou todos os vértices) | Peso total: {pesoHamilton}");

            logDetalhado.AppendLine("Rota (arestas percorridas):");
            for (int i = 0; i < rotaHamilton.Count - 1; i++)
            {
                int u = rotaHamilton[i];
                int v = rotaHamilton[i + 1];
                int peso = grafo.ObterAdjacentes(u).First(a => a.Destino == v).Peso;
                logDetalhado.AppendLine($"Aresta: {u} - {v} | Peso: {peso}");
            }

            // Escreve log no arquivo
            Log.Escrever("Rota de Inspeção", logDetalhado.ToString(), idArquivo);

            // =========================
            // OUTPUT CONCISO
            // =========================

            string resultadoFinal = "";
            resultadoFinal += euleriano ? $"Ciclo Euleriano encontrado | Peso: {pesoEuler}" : "Ciclo Euleriano NÃO existe";
            resultadoFinal += " | ";
            resultadoFinal += rotaHamiltonCompleta ? $"Hamiltoniano heurístico completo | Peso: {pesoHamilton}" :
                                                      $"Hamiltoniano heurístico parcial | Peso: {pesoHamilton}";

            return resultadoFinal;
        }

        // =========================
        // MÉTODO AUXILIAR – Hierholzer para ciclo Euleriano
        // =========================

        private static List<int> ConstruirCicloEulerianoHierholzer(IGrafo grafo, int inicio, out int pesoTotal)
        {
            pesoTotal = 0;

            // Copia a lista de adjacência para manipular arestas
            Dictionary<int, Queue<(int destino, int peso)>> adj = new Dictionary<int, Queue<(int, int)>>();
            for (int u = 1; u <= grafo.NumeroVertices; u++)
                adj[u] = new Queue<(int, int)>(grafo.ObterAdjacentes(u).Select(a => (a.Destino, a.Peso)));

            List<int> ciclo = new List<int>();
            Stack<int> stack = new Stack<int>();
            stack.Push(inicio);

            while (stack.Count > 0)
            {
                int u = stack.Peek();
                if (adj[u].Count == 0)
                {
                    ciclo.Add(u);
                    stack.Pop();
                }
                else
                {
                    var (v, peso) = adj[u].Dequeue();
                    stack.Push(v);
                    pesoTotal += peso;
                }
            }

            ciclo.Reverse();
            return ciclo;
        }

        // =========================
        // MÉTODO AUXILIAR – DFS para conectividade
        // =========================

        private static void DFS(IGrafo grafo, int u, bool[] visitado)
        {
            visitado[u] = true;
            foreach (var a in grafo.ObterAdjacentes(u))
            {
                if (!visitado[a.Destino])
                    DFS(grafo, a.Destino, visitado);
            }
        }

    }

}







