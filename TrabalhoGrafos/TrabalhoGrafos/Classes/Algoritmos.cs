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
            int n = grafo.NumeroVertices;

      
            var dist = new Dictionary<int, int>();
            var pred = new Dictionary<int, int>();
            var S = new HashSet<int>(); 
            
            var pq = new PriorityQueue<int, int>();

            
            dist[origem] = 0;
            pq.Enqueue(origem, 0);

            while (pq.Count > 0)
            {
                if (!pq.TryDequeue(out int v, out int distanciaAtual)) break;

                if (S.Contains(v)) continue;

                S.Add(v);

                if (v == destino) break;

                foreach (var aresta in grafo.ObterAdjacentes(v)) 
                {
                    int w = aresta.Destino;
                    int pesoAresta = aresta.Peso; 

                    if (S.Contains(w)) continue;

                    int distW = dist.ContainsKey(w) ? dist[w] : int.MaxValue;

                    if (distanciaAtual + pesoAresta < distW)
                    {
                        dist[w] = distanciaAtual + pesoAresta;
                        pred[w] = v; 

                        pq.Enqueue(w, dist[w]);
                    }
                }
            }


            if (!dist.ContainsKey(destino))
            {
                string msgErro = $"Não existe caminho entre {origem} e {destino}.";
                Log.Escrever("Roteamento de Menor Custo", msgErro, idArquivo);
                return msgErro;
            }

            var caminho = new List<int>();
            int atual = destino;
            while (atual != 0) 
            {
                caminho.Add(atual);

                if (atual == origem) break; 

                if (pred.ContainsKey(atual))
                    atual = pred[atual];
                else
                    break; 
            }
            caminho.Reverse(); 

            string caminhoTexto = string.Join(" -> ", caminho);
            int custoTotal = dist[destino];

            string resultadoFinal = $"Custo Total: {custoTotal} | Caminho: {caminhoTexto}";

            Log.Escrever("Roteamento de Menor Custo", resultadoFinal, idArquivo);

            return resultadoFinal;
        }

        public static string FluxoMaximoEdmondsKarp(IGrafo grafo, int s, int t, int idArquivo)
        {
            int n = grafo.NumeroVertices;

            int[,] residual = new int[n + 1, n + 1];

            for (int u = 1; u <= n; u++)
            {
                var adj = grafo.ObterAdjacentes(u);
                foreach (var e in adj)
                {
                    int v = e.Destino;
                    int cap = e.Capacidade;
                    residual[u, v] += cap;    
                }
            }

            int fluxoMaximo = 0;
            int[] pai = new int[n + 1];
            int iteracoes = 0; 

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

                v = t;
                while (v != s)
                {
                    int u = pai[v];

                    residual[u, v] -= delta;
                    residual[v, u] += delta;

                    v = u;
                }

                fluxoMaximo += delta;
            }

            
            if (iteracoes == 0)
            {
                string semCaminho =
                    $"Não existe caminho da origem {s} até o destino {t} na rede residual (Fluxo máximo = 0).";

                Log.Escrever("Capacidade Máxima de Escoamento (Fluxo Máximo)", semCaminho, idArquivo);
                return semCaminho;
            }

            string resultadoFinal =
                $"Origem (fonte S): {s} | Destino (sorvedouro T): {t} | " +
                $"Fluxo Máximo: {fluxoMaximo} | Caminhos aumentantes usados: {iteracoes}";

            Log.Escrever("Capacidade Máxima de Escoamento (Fluxo Máximo)", resultadoFinal, idArquivo);

            return resultadoFinal;
        }

        private static bool BfsCaminhoAumentante(int[,] residual, int n, int s, int t, int[] pai)
        {
            for (int i = 1; i <= n; i++)
                pai[i] = -1;

            Queue<int> fila = new Queue<int>();
            fila.Enqueue(s);
            pai[s] = -2; 
            while (fila.Count > 0)
            {
                int u = fila.Dequeue();

                for (int v = 1; v <= n; v++)
                {
                    if (pai[v] == -1 && residual[u, v] > 0)
                    {
                        pai[v] = u;
                        if (v == t)
                            return true;  

                        fila.Enqueue(v);
                    }
                }
            }

            return false; 
        }


public static string ArvoreGeradoraMinima(IGrafo grafo, int idArquivo)
{
    int n = grafo.NumeroVertices;

    bool[] visitado = new bool[n + 1];
    int[] chave = new int[n + 1];      
    int[] pai = new int[n + 1];        

    for (int i = 1; i <= n; i++)
    {
        chave[i] = int.MaxValue;
        pai[i] = -1;
    }

    chave[1] = 0; 

    for (int count = 1; count <= n; count++)
    {
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

        if (u == -1) break; 
        visitado[u] = true;

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

    int pesoTotal = 0;
    for (int i = 2; i <= n; i++)
        if (pai[i] != -1)
            pesoTotal += chave[i];

    
    StringBuilder logDetalhado = new StringBuilder();
    logDetalhado.AppendLine("--------------------------------------------------");
    logDetalhado.AppendLine($"[Hora Atual] - EXPANSÃO DA REDE DE COMUNICAÇÃO (AGM / MST)");
    logDetalhado.AppendLine("--------------------------------------------------");
    logDetalhado.AppendLine($"Árvore Geradora Mínima do arquivo grafo0{idArquivo}.dimacs:");
    logDetalhado.AppendLine("Objetivo: Conectar todos os hubs com o menor custo total possível.");
    logDetalhado.AppendLine($"Vértices: {n} | Peso Total da AGM: {pesoTotal} | Número de Arestas na AGM: {n - 1}");
    logDetalhado.AppendLine("");
    logDetalhado.AppendLine("Arestas selecionadas para a instalação da rede:");

    for (int i = 2; i <= n; i++)
    {
        if (pai[i] != -1)
        {
            logDetalhado.AppendLine($"- Hub {pai[i]} → Hub {i} | Custo: {chave[i]}");
        }
        else
        {
            logDetalhado.AppendLine($"- Hub {i} não conectado à AGM");
        }
    }

    logDetalhado.AppendLine("");
    logDetalhado.AppendLine("Observação: Esta configuração garante conectividade total da malha logística sem criar ciclos desnecessários.");

    Log.Escrever("Árvore Geradora Mínima (MST)", logDetalhado.ToString(), idArquivo);

    string resultadoFinal = $"Peso Total da AGM: {pesoTotal} | Número de Arestas: {n - 1}";

    return resultadoFinal;
}

        public static string Coloracao(IGrafo grafo, int idArquivo)
        {
            int n = grafo.NumeroVertices;

            var conflitos = new Dictionary<int, List<int>>();
            for (int i = 1; i <= n; i++) conflitos[i] = new List<int>();

            for (int u = 1; u <= n; u++)
            {
                foreach (var aresta in grafo.ObterAdjacentes(u))
                {
                    int v = aresta.Destino;
                    conflitos[u].Add(v);
                    if (v <= n) conflitos[v].Add(u);
                }
            }

            int[] cores = new int[n + 1];
            for (int i = 1; i <= n; i++) cores[i] = 0;

            for (int vertice = 1; vertice <= n; vertice++)
            {
                bool[] coresIndisponiveis = new bool[n + 1];

                foreach (var vizinho in conflitos[vertice])
                {
                    int corVizinho = cores[vizinho];
                    if (corVizinho != 0)
                    {
                        coresIndisponiveis[corVizinho] = true;
                    }
                }

                for (int cor = 1; cor <= n; cor++)
                {
                    if (!coresIndisponiveis[cor])
                    {
                        cores[vertice] = cor;
                        break;
                    }
                }
            }

            int totalCores = 0;
            for (int i = 1; i <= n; i++)
                if (cores[i] > totalCores) totalCores = cores[i];

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Resultado da Coloração:");
            sb.AppendLine($"Total de Turnos (Cores) necessários: {totalCores}");
            sb.AppendLine("--------------------------------------------------");
            sb.AppendLine("Cronograma Sugerido:");

            for (int i = 1; i <= n; i++)
                sb.AppendLine($"Hub {i}: Turno {cores[i]}");

            Log.Escrever("Agendamento de Manutenções (Coloração)", sb.ToString(), idArquivo);

            return $"Mínimo de Turnos: {totalCores}";
        }

   

public static string RotaInspecao(IGrafo grafo, int idArquivo, int verticeOrigem)
{
    int n = grafo.NumeroVertices;


    bool euleriano = true;

    Dictionary<int, int> inDegree = new Dictionary<int, int>();
    Dictionary<int, int> outDegree = new Dictionary<int, int>();

    for (int u = 1; u <= n; u++)
    {
        outDegree[u] = grafo.ObterAdjacentes(u).Count;
        inDegree[u] = 0;
    }

    for (int u = 1; u <= n; u++)
        foreach (var a in grafo.ObterAdjacentes(u))
            inDegree[a.Destino]++;

    for (int u = 1; u <= n; u++)
        if (inDegree[u] != outDegree[u])
        {
            euleriano = false;
            break;
        }

    if (euleriano)
    {
        bool[] vis1 = new bool[n + 1];
        DFS(grafo, verticeOrigem, vis1);

        if (vis1.Skip(1).Any(v => !v))
            euleriano = false;
        else
        {
            var reverso = GrafoReverso(grafo, n);

            bool[] vis2 = new bool[n + 1];
            DFSReverso(reverso, verticeOrigem, vis2);

            if (vis2.Skip(1).Any(v => !v))
                euleriano = false;
        }
    }

    List<int> rotaEuler = new List<int>();
    int pesoEuler = 0;

    if (euleriano)
        rotaEuler = ConstruirCicloEulerianoHierholzer(grafo, verticeOrigem, out pesoEuler);


    bool[] visitadoH = new bool[n + 1];
    List<int> rotaHamilton = new List<int>();
    int pesoHamilton = 0;

    int atualH = verticeOrigem;
    rotaHamilton.Add(atualH);
    visitadoH[atualH] = true;

    while (rotaHamilton.Count < n)
    {
        bool encontrou = false;

        foreach (var a in grafo.ObterAdjacentes(atualH))
        {
            int v = a.Destino;
            if (!visitadoH[v])
            {
                rotaHamilton.Add(v);
                pesoHamilton += a.Peso;
                visitadoH[v] = true;
                atualH = v;
                encontrou = true;
                break;
            }
        }

        if (!encontrou) break;
    }

    bool rotaHamiltonCompleta = (rotaHamilton.Count == n);


    StringBuilder sb = new StringBuilder();
    sb.AppendLine($"--------------------------------------------------");
    sb.AppendLine($"[Hora Atual] - ROTA DE INSPEÇÃO");
    sb.AppendLine($"--------------------------------------------------");
    sb.AppendLine($"Arquivo: grafo0{idArquivo}.dimacs");
    sb.AppendLine($"Objetivo: Verificar percursos de inspeção na malha logística");
    sb.AppendLine($"Vértices (Hubs): {n}");
    sb.AppendLine($"Vértice de origem escolhido: {verticeOrigem}");
    sb.AppendLine("");

    if (euleriano)
    {
        sb.AppendLine("Cenário A — Percurso de Rotas (Ciclo Euleriano):");
        sb.AppendLine($"Descrição: O inspetor consegue percorrer todas as rotas exatamente uma vez, retornando ao hub inicial.");
        sb.AppendLine($"Peso total do percurso: {pesoEuler}");
        sb.AppendLine("Sequência de rotas percorridas:");
        for (int i = 0; i < rotaEuler.Count - 1; i++)
        {
            int u = rotaEuler[i];
            int v = rotaEuler[i + 1];
            int peso = grafo.ObterAdjacentes(u).First(a => a.Destino == v).Peso;
            sb.AppendLine($"  Hub {u} -> Hub {v} | Custo: {peso}");
        }
    }
    else
    {
        sb.AppendLine("Cenário A — Percurso de Rotas (Ciclo Euleriano): NÃO existe");
        sb.AppendLine("Observação: Não é possível percorrer todas as rotas uma única vez retornando ao hub inicial devido a grau de entrada/saída desigual ou falta de conectividade forte.");
    }

    sb.AppendLine("");

    sb.AppendLine("Cenário B — Percurso de Hubs (Heurística Hamiltoniana):");
    sb.AppendLine("Descrição: O inspetor tenta visitar todos os hubs exatamente uma vez, retornando ao ponto de origem se possível.");
    sb.AppendLine(rotaHamiltonCompleta ?
                  $"Resultado: Completo | Peso total do percurso: {pesoHamilton}" :
                  $"Resultado: Parcial | Peso total do percurso: {pesoHamilton}");
    sb.AppendLine("Sequência de hubs percorridos:");
    for (int i = 0; i < rotaHamilton.Count - 1; i++)
    {
        int u = rotaHamilton[i];
        int v = rotaHamilton[i + 1];
        int peso = grafo.ObterAdjacentes(u).First(a => a.Destino == v).Peso;
        sb.AppendLine($"  Hub {u} -> Hub {v} | Custo: {peso}");
    }
    sb.AppendLine("");
    sb.AppendLine("Observação: Este percurso auxilia o planejamento logístico a identificar uma rota de inspeção eficiente pelos hubs.");

    Log.Escrever("Rota de Inspeção", sb.ToString(), idArquivo);


    string txt = "";
    txt += euleriano ? $"Ciclo Euleriano encontrado | Peso {pesoEuler}"
                     : "Ciclo Euleriano NÃO existe";

    txt += " | ";

    txt += rotaHamiltonCompleta ? $"Hamiltoniano heurístico completo | Peso {pesoHamilton}"
                                : $"Hamiltoniano heurístico parcial | Peso {pesoHamilton}";

    return txt;
}


        private static void DFS(IGrafo grafo, int u, bool[] visitado)
        {
            visitado[u] = true;
            foreach (var a in grafo.ObterAdjacentes(u))
                if (!visitado[a.Destino])
                    DFS(grafo, a.Destino, visitado);
        }

        private static Dictionary<int, List<int>> GrafoReverso(IGrafo grafo, int n)
        {
            var rev = new Dictionary<int, List<int>>();
            for (int i = 1; i <= n; i++)
                rev[i] = new List<int>();

            for (int u = 1; u <= n; u++)
                foreach (var a in grafo.ObterAdjacentes(u))
                    rev[a.Destino].Add(u);

            return rev;
        }

        private static void DFSReverso(Dictionary<int, List<int>> rev, int u, bool[] visitado)
        {
            visitado[u] = true;
            foreach (var v in rev[u])
                if (!visitado[v])
                    DFSReverso(rev, v, visitado);
        }

        private static List<int> ConstruirCicloEulerianoHierholzer(IGrafo grafo, int inicio, out int pesoTotal)
        {
            pesoTotal = 0;

            Dictionary<int, Queue<(int v, int peso)>> adj = new Dictionary<int, Queue<(int, int)>>();

            for (int u = 1; u <= grafo.NumeroVertices; u++)
                adj[u] = new Queue<(int, int)>(grafo.ObterAdjacentes(u)
                                                .Select(a => (a.Destino, a.Peso)));

            Stack<int> stack = new Stack<int>();
            List<int> ciclo = new List<int>();

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
                    pesoTotal += peso;
                    stack.Push(v);
                }
            }

            ciclo.Reverse();
            return ciclo;
        }


    }

}







