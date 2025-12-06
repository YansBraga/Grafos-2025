using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrabalhoGrafos.Interfaces;

namespace TrabalhoGrafos.Classes
{
    public class Representacao
    {
        public static IGrafo CriarGrafo(int numVertices, int numArestasPreistas)
        {            
            //Perto de 1, é denso. perto de 0, é esparso.
            double densidade = (double)numArestasPreistas / (numVertices * (numVertices - 1));

            if (densidade > 0.5)
            {
                return new GrafoMatriz(numVertices);
            }
            else
            {
                return new GrafoLista(numVertices);
            }
        }
    }
}
