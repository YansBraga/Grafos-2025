using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrabalhoGrafos.Interfaces;

namespace TrabalhoGrafos.Classes
{
    public class GrafoMatriz : IGrafo
    {
        int[,] matriz;

        public GrafoMatriz(int numVertices) 
        {
            matriz = new int[numVertices, numVertices];
        }

        public int NumeroVertices => throw new NotImplementedException();

        public void AdicionarAresta(int origem, int destino, int peso, int capacidade)
        {
            throw new NotImplementedException();
        }

        public bool ExisteAresta(int origem, int destino)
        {
            throw new NotImplementedException();
        }

        public List<Aresta> ObterAdjacentes(int vertice)
        {
            throw new NotImplementedException();
        }

        public Aresta ObterAresta(int origem, int destino)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "Matriz de Adjacência";
        }
    }
}
