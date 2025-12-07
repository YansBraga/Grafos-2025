using System;
using System.Collections.Generic;
using TrabalhoGrafos.Interfaces;

namespace TrabalhoGrafos.Classes
{
    public class GrafoMatriz : IGrafo
    {
        private Aresta[,] matriz;
        private int _numeroVertices;

        public int NumeroVertices => _numeroVertices;

        public GrafoMatriz(int numVertices)
        {
            _numeroVertices = numVertices;
            matriz = new Aresta[numVertices + 1, numVertices + 1];
        }

        public void AdicionarAresta(int origem, int destino, int peso, int capacidade)
        {            
            if (origem > _numeroVertices || destino > _numeroVertices)
            {                
                throw new Exception($"Vértice inválido para Matriz: {origem}->{destino}. Máximo: {_numeroVertices}");
            }
            
            matriz[origem, destino] = new Aresta(origem, destino, peso, capacidade);
        }

        public bool ExisteAresta(int origem, int destino)
        {
            if (origem > _numeroVertices || destino > _numeroVertices)
                return false;

            return matriz[origem, destino] != null;
        }

        public List<Aresta> ObterAdjacentes(int vertice)
        {
            var listaAdj = new List<Aresta>();

            if (vertice > _numeroVertices) return listaAdj;
            
            for (int i = 1; i <= _numeroVertices; i++)
            {
                if (matriz[vertice, i] != null)
                {
                    listaAdj.Add(matriz[vertice, i]);
                }
            }
            return listaAdj;
        }

        public Aresta ObterAresta(int origem, int destino)
        {
            if (origem > _numeroVertices || destino > _numeroVertices) return null;
            return matriz[origem, destino];
        }

        public override string ToString()
        {
            return "Matriz de Adjacência";
        }
    }
}