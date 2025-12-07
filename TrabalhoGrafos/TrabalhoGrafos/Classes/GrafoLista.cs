using System;
using System.Collections.Generic;
using System.Linq;
using TrabalhoGrafos.Interfaces;

namespace TrabalhoGrafos.Classes
{
    public class GrafoLista : IGrafo
    {
        private int _numeroVertices;
        public int NumeroVertices => _numeroVertices;

        private Dictionary<int, List<Aresta>> adjacencia;

        public GrafoLista(int numVertices)
        {
            _numeroVertices = numVertices;
            adjacencia = new Dictionary<int, List<Aresta>>();
        }

        public void AdicionarAresta(int origem, int destino, int peso, int capacidade)
        {
            // so para garantir que os vértices existam no dicionário
            if (!adjacencia.ContainsKey(origem))
            {
                adjacencia[origem] = new List<Aresta>();
            }

            if (!adjacencia.ContainsKey(destino))
            {
                adjacencia[destino] = new List<Aresta>();
            }
            
            var aresta = new Aresta(origem, destino, peso, capacidade);
            adjacencia[origem].Add(aresta);
        }

        public bool ExisteAresta(int origem, int destino)
        {            
            if (!adjacencia.ContainsKey(origem)) 
                return false;

            return adjacencia[origem].Any(a => a.Destino == destino);
        }

        public List<Aresta> ObterAdjacentes(int vertice)
        {            
            if (adjacencia.ContainsKey(vertice))
                return adjacencia[vertice];

            return new List<Aresta>();
        }

        public Aresta ObterAresta(int origem, int destino)
        {
            if (!adjacencia.ContainsKey(origem)) return null;
            return adjacencia[origem].FirstOrDefault(a => a.Destino == destino);
        }

        public override string ToString()
        {
            return "Lista de Adjacência (Dicionário)";
        }
    }
}