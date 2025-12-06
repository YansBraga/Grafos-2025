using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrabalhoGrafos.Classes;

namespace TrabalhoGrafos.Interfaces
{
    public interface IGrafo
    {
        int NumeroVertices { get; }

        void AdicionarAresta(int origem, int destino, int peso, int capacidade);
        List<Aresta> ObterAdjacentes(int vertice);
        bool ExisteAresta(int origem, int destino);
        Aresta ObterAresta(int origem, int destino);        
    }
}
