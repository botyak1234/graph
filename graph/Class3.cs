using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graph
{
    public class Edge
    {
        public string U;
        public string V;
        public int Weight;

        public Edge(string u, string v, int weight)
        {
            U = u;
            V = v;
            Weight = weight;
        }
    }
}
