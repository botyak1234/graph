using System;
using System.Collections.Generic;
using System.IO;

namespace graph
{
    internal class Graph
    {
        private Dictionary<int, List<Tuple<int, int>>> adjacencyList;
        private bool isDirected;
        private bool isWeighted;
        // Конструктор по умолчанию: создаёт пустой граф, ориентированный или неориентированный
        public Graph(bool isDirected = true)
        {
            adjacencyList = new Dictionary<int, List<Tuple<int, int>>>();
            this.isDirected = isDirected;
        }

        public Graph(bool isDirected, bool isWeighted)
        {
            adjacencyList = new Dictionary<int, List<Tuple<int, int>>>();
            this.isDirected = isDirected;
            this.isWeighted = isWeighted;
        }

        // Конструктор, заполняющий данные графа из файла
        // Конструктор для графа из файла
        public Graph(string input)
        {
            adjacencyList = new Dictionary<int, List<Tuple<int, int>>>();

            using (StreamReader file = new StreamReader(input))
            {
                string firstLine = file.ReadLine();
                if (firstLine == null)
                {
                    Console.WriteLine("Ошибка: Пустой файл.");
                    return;
                }

                // Первая строка файла определяет тип графа: ориентированный или нет, взвешенный или нет
                string[] graphTypeInfo = firstLine.Split(' ');
                if (graphTypeInfo.Length != 2)
                {
                    Console.WriteLine("Ошибка: Некорректный формат первой строки. Ожидается 2 значения (ориентированность и взвешенность).");
                    return;
                }

                // Ориентированный граф (y/n)
                isDirected = graphTypeInfo[0].ToLower() == "y";

                // Взвешенный граф (y/n)
                isWeighted = graphTypeInfo[1].ToLower() == "y";

                string line;
                while ((line = file.ReadLine()) != null)
                {
                    var parts = line.Split(' ');

                    if (parts.Length < 2)
                    {
                        Console.WriteLine($"Некорректная строка в файле: {line}");
                        continue;
                    }

                    int startVertex = int.Parse(parts[0]);
                    int endVertex = int.Parse(parts[1]);

                    int weight = isWeighted ? int.Parse(parts[2]) : 1;

                    // Добавляем ребро
                    addEdge(startVertex, endVertex, weight);
                }
            }
        }

        // Конструктор-копия
        public Graph(Graph otherGraph)
        {
            adjacencyList = new Dictionary<int, List<Tuple<int, int>>>();
            this.isDirected = otherGraph.isDirected;
            foreach (var vertex in otherGraph.adjacencyList)
            {
                adjacencyList[vertex.Key] = new List<Tuple<int, int>>(vertex.Value);
            }
        }

        // Добавить ребро (дугу)
        // Добавить ребро (дугу)
        public void addEdge(int startVertex, int endVertex, int weight = 1)
        {
            if (!isWeighted)
            {
                weight = 1; // Если граф невзвешенный, устанавливаем вес в 1
            }

            if (!adjacencyList.ContainsKey(startVertex))
            {
                if (!adjacencyList.ContainsKey(endVertex))
                {
                    Console.WriteLine("Несуществующая конечная вершина");
                }
                else
                {
                    adjacencyList[startVertex] = new List<Tuple<int, int>>();
                }
            }
            if (!adjacencyList.ContainsKey(endVertex))
            {
                Console.WriteLine("Несуществующая конечная вершина");
            }
            else
            {
                adjacencyList[startVertex].Add(new Tuple<int, int>(endVertex, weight));
            }
            
            
            // Если граф неориентированный, добавляем обратное ребро
            if (!isDirected)
            {
                if (!adjacencyList.ContainsKey(endVertex))
                {
                    if (!adjacencyList.ContainsKey(endVertex))
                    {
                        Console.WriteLine("Несуществующая конечная вершина");
                    }

                    else
                    {
                        adjacencyList[endVertex] = new List<Tuple<int, int>>();
                    }

                }
                if (!adjacencyList.ContainsKey(endVertex))
                {
                    Console.WriteLine("Несуществующая конечная вершина");
                }
                else
                {
                    adjacencyList[endVertex].Add(new Tuple<int, int>(startVertex, weight));
                }
            }
        }


        public void addEdge_const(int startVertex, int endVertex, int weight = 1)
        {
            if (!isWeighted)
            {
                weight = 1; // Если граф невзвешенный, устанавливаем вес в 1
            }

            if (!adjacencyList.ContainsKey(startVertex))
            {
                adjacencyList[startVertex] = new List<Tuple<int, int>>();
            }
            adjacencyList[startVertex].Add(new Tuple<int, int>(endVertex, weight));
            // Если граф неориентированный, добавляем обратное ребро
            if (!isDirected)
            {
                if (!adjacencyList.ContainsKey(endVertex))
                {
                    adjacencyList[endVertex] = new List<Tuple<int, int>>();
                }
                adjacencyList[endVertex].Add(new Tuple<int, int>(startVertex, weight));
            }
        }


        // Добавить вершину
        public void addVertex(int vertex, List<Tuple<int, int>> edges = null)
        {
            if (!adjacencyList.ContainsKey(vertex))
            {
                adjacencyList[vertex] = edges ?? new List<Tuple<int, int>>();
            }
            else
            {
                Console.WriteLine($"Вершина {vertex} уже существует.");
            }
        }

        

        // Удаление ребра
        public bool removeEdge(int startVertex, int endVertex, int weight)
        {
            if (adjacencyList.ContainsKey(startVertex))
            {
                Tuple<int, int> edge = new Tuple<int, int>(endVertex, weight);
                bool removed = adjacencyList[startVertex].Remove(edge);

                // Если граф неориентированный, удаляем и обратное ребро
                if (!isDirected && removed)
                {
                    adjacencyList[endVertex].Remove(new Tuple<int, int>(startVertex, weight));
                }

                return removed;
            }
            return false;
        }
        // Удаление вершины
        public bool removeVertex(int vertex)
        {
            if (!adjacencyList.ContainsKey(vertex))
            {
                Console.WriteLine($"Вершина {vertex} не найдена.");
                return false;
            }

            adjacencyList.Remove(vertex);

            // Удаляем все ребра, связанные с этой вершиной
            foreach (var vertexEdges in adjacencyList.Values)
            {
                vertexEdges.RemoveAll(edge => edge.Item1 == vertex);
            }

            return true;
        }

        // Вывод списка смежности в файл
        public void saveToFile(string outputFilePath)
        {
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                foreach (var vertex in adjacencyList)
                {
                    foreach (var edge in vertex.Value)
                    {
                        writer.WriteLine($"{vertex.Key} {edge.Item1} {edge.Item2}");
                    }
                }
            }
        }

        // Вывод графа в консоль
        public void printGraph()
        {
            foreach (var vertex in adjacencyList)
            {
                Console.Write($"{vertex.Key}: ");
                foreach (var edge in vertex.Value)
                {
                    Console.Write($"({edge.Item1}, {edge.Item2}) ");
                }
                Console.WriteLine();
            }
        }
    }
}
