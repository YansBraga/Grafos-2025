using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrabalhoGrafos.Classes
{
    public class Aresta
    {
        public int Origem { get; set; }
        public int Destino { get; set; }
        public int Peso { get; set; }      
        public int Capacidade { get; set; }
        public int FluxoAtual { get; set; } // Propriedade para o algoritmo de Fluxo (Ford-Fulkerson)

        public Aresta(int origem, int destino, int peso, int capacidade)
        {
            Origem = origem;
            Destino = destino;
            Peso = peso;
            Capacidade = capacidade;
            FluxoAtual = 0;
        }
    }
}
