using System;
using System.Collections.Generic;

class Program
{
    static int N;
    static int M;
    static bool[,] adj;          // adj[i, j] = existe aresta de i para j
    static List<int> solution;   // guarda a solução encontrada

    static void Main()
    {
        Console.Write("Defina N:");
        string line = Console.ReadLine();
        if (string.IsNullOrEmpty(line)) return;

        N = int.Parse(line);
        M = 2 * N;

        // Matriz de adjacência 
        adj = new bool[M + 1, M + 1];

        for (int i = 1; i <= M; i++)
        {
            string[] p = Console.ReadLine()
                                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // cada linha tem exatamente 6 vizinhos
            for (int j = 0; j < p.Length; j++)
            {
                int neighbor = int.Parse(p[j]);
                adj[i, neighbor] = true;
            }
        }

        solution = null;

        // começa tentando escolher 1 vértice do 1º par 
        List<int> current = new List<int>();
        SolveByPairs(1, current);

        Console.WriteLine("Solução encontrada:");
        if (solution != null)
        {
            // imprime um vértice por linha
            foreach (int v in solution)
                Console.WriteLine(v);
        }
        else
        {
            Console.WriteLine("Nenhuma solucao encontrada");
        }
    }

    // Tenta escolher um vértice para cada par 
    // pairIndex = índice do par (1..N)
    static void SolveByPairs(int pairIndex, List<int> chosen)
    {
        // se já achamos uma solução, não precisa continuar explorando
        if (solution != null) return;

        // se já escolhemos 1 de cada par, verifica se a seleção é válida
        if (pairIndex > N)
        {
            if (ValidSolution(chosen))
                solution = new List<int>(chosen);
            return;
        }

        int u = 2 * pairIndex - 1; // primeiro vértice do par
        int v = 2 * pairIndex;     // segundo vértice do par

        // Tenta escolher u
        chosen.Add(u);
        SolveByPairs(pairIndex + 1, chosen);
        chosen.RemoveAt(chosen.Count - 1);

        // Se já achou solução com u, não precisa testar v
        if (solution != null) return;

        // Tenta escolher v
        chosen.Add(v);
        SolveByPairs(pairIndex + 1, chosen);
        chosen.RemoveAt(chosen.Count - 1);
    }

    // Avalia a condição de existencia que é para cada vértice não pertencente ao conjunto S, existem pelo menos 4 vizinhos em S alcançáveis a partir dele.
    static bool ValidSolution(List<int> S)
    {
        bool[] inS = new bool[M + 1];
        foreach (int v in S)
            inS[v] = true;

        // Para cada vértice que NÃO está em S
        for (int i = 1; i <= M; i++)
        {
            if (inS[i]) continue;

            int count = 0;

            // conta quantos vizinhos de i pertencem a S
            for (int j = 1; j <= M; j++)
            {
                if (adj[i, j] && inS[j])
                    count++;
            }

            if (count < 4)
                return false;  // falhou para este vértice
        }

        return true;
    }
}
