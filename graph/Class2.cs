using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graph
{
    // Класс для реализации Union-Find
    public class UnionFind
    {
        private Dictionary<string, string> parent;
        private Dictionary<string, int> rank;

        public UnionFind()
        {
            parent = new Dictionary<string, string>();
            rank = new Dictionary<string, int>();
        }

        public void Add(string item)
        {
            if (!parent.ContainsKey(item))
            {
                parent[item] = item; 
                rank[item] = 0;      
            }
        }

        public string Find(string item)
        {
            if (!parent.ContainsKey(item))
            {
                throw new ArgumentException($"Элемент {item} не найден в UnionFind.");
            }

            if (parent[item] != item)
            {
                parent[item] = Find(parent[item]);
            }
            return parent[item];
        }

        public void Union(string u, string v)
        {
            string rootU = Find(u);
            string rootV = Find(v);

            if (rootU != rootV)
            {
                
                if (rank[rootU] > rank[rootV])
                {
                    parent[rootV] = rootU;
                }
                else if (rank[rootU] < rank[rootV])
                {
                    parent[rootU] = rootV;
                }
                else
                {
                    parent[rootV] = rootU;
                    rank[rootU]++;
                }
            }
        }
    }
}
