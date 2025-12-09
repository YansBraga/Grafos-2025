using System;
using System.Collections.Generic;
using System.Text;

class URI
{
    static int N, M;                      // N pares, M = 2N províncias
    static int[][] grafoSaida;            // grafoSaida[i] = 6 vizinhos alcançáveis a partir de i (cauda da cláusula i)
    static int[][] clausulasDaCaudaPorVertice; // clausulasDaCaudaPorVertice[v] = lista de cláusulas em que v aparece na cauda
    static bool[] diretorioEscolhido;     // diretorioEscolhido[i] = província i escolhida para receber diretório
    static int[] pontuacaoClausula;       // pontuacaoClausula[i] = # verdadeiros na cauda + 2 se cabeça verdadeira
    static int[] clausulasRuins;          // lista de cláusulas com pontuacaoClausula <= 1
    static int[] posicaoNaListaRuins;     // posicaoNaListaRuins[i] = índice de i em clausulasRuins ou -1
    static int quantidadeRuins;
    static int[] caudaNaoSatisfeita = new int[6]; // buffer para literais falsos da cauda
    static Random gerador = new Random(123456);

    // província gêmea (1,2) (3,4)
    static int Gemea(int v)
    {
        return ((v & 1) == 1) ? v + 1 : v - 1;
    }

    static void AdicionaClausulaRuim(int c)
    {
        if (posicaoNaListaRuins[c] != -1) return;
        posicaoNaListaRuins[c] = quantidadeRuins;
        clausulasRuins[quantidadeRuins++] = c;
    }

    static void RemoveClausulaRuim(int c)
    {
        int idx = posicaoNaListaRuins[c];
        if (idx == -1) return;
        quantidadeRuins--;
        int ultima = clausulasRuins[quantidadeRuins];
        clausulasRuins[idx] = ultima;
        posicaoNaListaRuins[ultima] = idx;
        posicaoNaListaRuins[c] = -1;
    }

    static void RecalculaPontuacoes()
    {
        quantidadeRuins = 0;
        for (int i = 1; i <= M; i++)
            posicaoNaListaRuins[i] = -1;

        for (int i = 1; i <= M; i++)
        {
            int cont = 0;
            var vizinhos = grafoSaida[i];
            for (int j = 0; j < 6; j++)
            {
                if (diretorioEscolhido[vizinhos[j]]) cont++;
            }
            if (diretorioEscolhido[i]) cont += 2; // cabeça satisfeita
            pontuacaoClausula[i] = cont;
            if (cont <= 1) AdicionaClausulaRuim(i);
        }
    }

    // Faz a inversão da variável cujo literal envolve a província "a"
    static void InverteVariavelDoPar(int a)
    {
        if (a < 1 || a > M) return;

        int u = a;
        int v = Gemea(a);

        int antigoEscolhido, novoEscolhido;

        if (diretorioEscolhido[u] && !diretorioEscolhido[v])
        {
            antigoEscolhido = u;
            novoEscolhido = v;
        }
        else if (diretorioEscolhido[v] && !diretorioEscolhido[u])
        {
            antigoEscolhido = v;
            novoEscolhido = u;
        }
        else
        {
            // Se tem alguma inconsistência, arruma o par e recalcula tudo
            if (gerador.Next(2) == 0)
            {
                diretorioEscolhido[u] = true;
                diretorioEscolhido[v] = false;
            }
            else
            {
                diretorioEscolhido[v] = true;
                diretorioEscolhido[u] = false;
            }
            RecalculaPontuacoes();
            return;
        }

        if (antigoEscolhido == novoEscolhido) return;

        // aplica a reversão
        diretorioEscolhido[antigoEscolhido] = false;
        diretorioEscolhido[novoEscolhido] = true;

        // atualiza cabeças das cláusulas antigoEscolhido e novoEscolhido 
        pontuacaoClausula[antigoEscolhido] -= 2;  // cabeça deixa de ser satisfeita
        if (pontuacaoClausula[antigoEscolhido] <= 1) AdicionaClausulaRuim(antigoEscolhido);
        else RemoveClausulaRuim(antigoEscolhido);

        pontuacaoClausula[novoEscolhido] += 2;    // cabeça passa a ser satisfeita
        if (pontuacaoClausula[novoEscolhido] <= 1) AdicionaClausulaRuim(novoEscolhido);
        else RemoveClausulaRuim(novoEscolhido);

        // atualiza caudas em que antigoEscolhido aparece 
        var listaAntigo = clausulasDaCaudaPorVertice[antigoEscolhido];
        for (int i = 0; i < listaAntigo.Length; i++)
        {
            int c = listaAntigo[i];
            pontuacaoClausula[c]--;  // literal deixa de ser verdadeiro
            if (pontuacaoClausula[c] <= 1) AdicionaClausulaRuim(c);
            else RemoveClausulaRuim(c);
        }

        // atualiza caudas em que novoEscolhido aparece
        var listaNovo = clausulasDaCaudaPorVertice[novoEscolhido];
        for (int i = 0; i < listaNovo.Length; i++)
        {
            int c = listaNovo[i];
            pontuacaoClausula[c]++;  // literal passa a ser verdadeiro
            if (pontuacaoClausula[c] <= 1) AdicionaClausulaRuim(c);
            else RemoveClausulaRuim(c);
        }
    }

    static bool SolucaoValida()
    {
        // só pode ter no máximo 1 diretório por par de gêmeos
        for (int p = 1; p <= N; p++)
        {
            int u = 2 * p - 1;
            int v = 2 * p;
            if (diretorioEscolhido[u] && diretorioEscolhido[v]) return false;
        }

        // para cada província sem diretório, pelo menos 2 vizinhos com diretório
        for (int i = 1; i <= M; i++)
        {
            if (diretorioEscolhido[i]) continue;

            int cont = 0;
            var vizinhos = grafoSaida[i];
            for (int j = 0; j < 6; j++)
            {
                if (diretorioEscolhido[vizinhos[j]]) cont++;
            }
            if (cont < 2) return false;
        }

        return true;
    }

    static void ImprimeSolucao()
    {
        var sb = new StringBuilder();
        for (int i = 1; i <= M; i++)
        {
            if (diretorioEscolhido[i])
                sb.AppendLine(i.ToString());
        }
        Console.Write(sb.ToString());
    }

    static void Main()
    {
        string linha = Console.ReadLine();
        if (string.IsNullOrEmpty(linha))
            return;

        N = int.Parse(linha.Trim());
        M = 2 * N;

        grafoSaida = new int[M + 1][];
        var listasClausulasPorVertice = new List<int>[M + 1];
        for (int i = 1; i <= M; i++)
            listasClausulasPorVertice[i] = new List<int>(6);

        // Leitura das 2N linhas de adjacência
        for (int i = 1; i <= M; i++)
        {
            string l = Console.ReadLine();
            while (l != null && l.Trim().Length == 0)
                l = Console.ReadLine();
            if (l == null) return;

            string[] partes = l.Split(new char[] { ' ', '\t' },
                                      StringSplitOptions.RemoveEmptyEntries);
            int[] vizinhos = new int[6];
            for (int j = 0; j < 6; j++)
            {
                int v = int.Parse(partes[j]);
                vizinhos[j] = v;
                listasClausulasPorVertice[v].Add(i); // v aparece na cauda da cláusula i
            }
            grafoSaida[i] = vizinhos;
        }

        // convertendo listas em arrays
        clausulasDaCaudaPorVertice = new int[M + 1][];
        for (int v = 1; v <= M; v++)
            clausulasDaCaudaPorVertice[v] = listasClausulasPorVertice[v].ToArray();

        diretorioEscolhido = new bool[M + 1];
        pontuacaoClausula = new int[M + 1];
        clausulasRuins = new int[M + 1];
        posicaoNaListaRuins = new int[M + 1];

        const int MAX_REINICIOS = 5;
        const int MAX_ITERACOES = 200000;

        for (int tentativa = 0; tentativa < MAX_REINICIOS; tentativa++)
        {
            // sorteia valoração inicial exatamente 1 diretório por par de províncias-irmãs 
            for (int i = 1; i <= M; i++)
                diretorioEscolhido[i] = false;

            for (int p = 1; p <= N; p++)
            {
                int u = 2 * p - 1;
                int v = 2 * p;
                if (gerador.Next(2) == 0)
                {
                    diretorioEscolhido[u] = true;
                    diretorioEscolhido[v] = false;
                }
                else
                {
                    diretorioEscolhido[v] = true;
                    diretorioEscolhido[u] = false;
                }
            }

            // calcula pontuações e identifica cláusulas ruins
            RecalculaPontuacoes();

            for (int it = 0; it < MAX_ITERACOES; it++)
            {
                if (quantidadeRuins == 0)
                {
                    if (SolucaoValida())
                    {
                        ImprimeSolucao();
                        return;
                    }
                    // se não for válida, recalcula
                    RecalculaPontuacoes();
                    if (quantidadeRuins == 0)
                    {
                        ImprimeSolucao();
                        return;
                    }
                }

                // escolhe cláusula ruim aleatória
                int clausula = clausulasRuins[gerador.Next(quantidadeRuins)];

                // se probabilidade p = 1/6, entao inverter variável da cabeça
                if (gerador.Next(6) == 0)
                {
                    InverteVariavelDoPar(clausula);
                }
                else
                {
                    // ou escolhe literal falso da cauda de forma uniforme
                    var vizinhos = grafoSaida[clausula];
                    int naoSatisfeitos = 0;
                    for (int j = 0; j < 6; j++)
                    {
                        int v = vizinhos[j];
                        if (!diretorioEscolhido[v])
                            caudaNaoSatisfeita[naoSatisfeitos++] = v;
                    }

                    if (naoSatisfeitos == 0)
                    {
                        // se tiver inconsistência numérica, recalcula tudo
                        RecalculaPontuacoes();
                        continue;
                    }

                    int verticeEscolhido = caudaNaoSatisfeita[gerador.Next(naoSatisfeitos)];
                    InverteVariavelDoPar(verticeEscolhido);
                }
            }
            // se não resolveu, recomeça outro sorteio inicial
        }

        ImprimeSolucao();
    }
}
