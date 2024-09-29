using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace graph
{
    internal class Graph
    {
        private Dictionary<string, List<Tuple<string, int>>> adjacencyList;
        private bool isDirected;
        private bool isWeighted;
        // Конструктор по умолчанию: создаёт пустой граф, ориентированный или неориентированный
        public Graph(bool isDirected = true)
        {
            adjacencyList = new Dictionary<string, List<Tuple<string, int>>>();
            this.isDirected = isDirected;
        }

        public Graph(bool isDirected, bool isWeighted)
        {
            adjacencyList = new Dictionary<string, List<Tuple<string, int>>>();
            this.isDirected = isDirected;
            this.isWeighted = isWeighted;
        }

        // Конструктор, заполняющий данные графа из файла
        // Конструктор для графа из файла
        public Graph(string input)
        {
            adjacencyList = new Dictionary<string, List<Tuple<string, int>>>();

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

                    string startVertex = parts[0];
                    string endVertex = parts[1];

                    int weight = isWeighted ? int.Parse(parts[2]) : 1;

                    // Добавляем ребро
                    addEdge_const(startVertex, endVertex, weight);
                }
            }
        }

        // Конструктор-копия
        public Graph(Graph otherGraph)
        {
            adjacencyList = new Dictionary<string, List<Tuple<string, int>>>();
            this.isDirected = otherGraph.isDirected;
            foreach (var vertex in otherGraph.adjacencyList)
            {
                adjacencyList[vertex.Key] = new List<Tuple<string, int>>(vertex.Value);
            }
        }

        private bool edgeExists(string startVertex, string endVertex)
        {
            if (adjacencyList.ContainsKey(startVertex))
            {
                foreach (var edge in adjacencyList[startVertex])
                {
                    if (edge.Item1 == endVertex)
                    {
                        return true; // Ребро уже существует
                    }
                }
            }
            return false; // Ребро не найдено
        }

        // Добавить ребро (дугу)
        // Добавить ребро (дугу)
        public void addEdge(string startVertex, string endVertex, int weight)
        {
            if (edgeExists(startVertex, endVertex))
            {
                Console.WriteLine($"Ребро между вершинами {startVertex} и {endVertex} уже существует. Добавление нового ребра запрещено.");
                return;
            }
            // Если граф взвешенный и вес не указан, выбрасываем ошибку
            if (isWeighted && weight == null)
            {
                Console.WriteLine("Для взвешенного графа необходимо указать вес ребра.");
                return;
            }

            if (!isWeighted)
            {
                weight = -1; // Если граф невзвешенный, устанавливаем вес в 1
            }

            if (!adjacencyList.ContainsKey(startVertex))
            {
                if (!adjacencyList.ContainsKey(endVertex))
                {
                    Console.WriteLine("Несуществующая конечная вершина");
                }
                else
                {
                    adjacencyList[startVertex] = new List<Tuple<string, int>>();
                }
            }
            if (!adjacencyList.ContainsKey(endVertex))
            {
                Console.WriteLine("Несуществующая конечная вершина");
            }
            else
            {
                adjacencyList[startVertex].Add(new Tuple<string, int>(endVertex, weight));
            }
            
            
            // Если граф неориентированный, добавляем обратное ребро
            if (!isDirected && startVertex != endVertex)
            {
                if (!adjacencyList.ContainsKey(endVertex))
                {
                    if (!adjacencyList.ContainsKey(endVertex))
                    {
                        Console.WriteLine("Несуществующая конечная вершина");
                    }

                    else
                    {
                        adjacencyList[endVertex] = new List<Tuple<string, int>>();
                    }

                }
                if (!adjacencyList.ContainsKey(endVertex))
                {
                    Console.WriteLine("Несуществующая конечная вершина");
                }
                else
                {
                    adjacencyList[endVertex].Add(new Tuple<string, int>(startVertex, weight));
                }
            }
        }


        public void addEdge_const(string startVertex, string endVertex, int weight = 1)
        {
            if (!isWeighted)
            {
                weight = -1; // Если граф невзвешенный, устанавливаем вес в 1
            }

            if (!adjacencyList.ContainsKey(startVertex))
            {
                adjacencyList[startVertex] = new List<Tuple<string, int>>();
            }
            adjacencyList[startVertex].Add(new Tuple<string, int>(endVertex, weight));
            // Если граф неориентированный, добавляем обратное ребро
            if (!isDirected && startVertex != endVertex)
            {
                if (!adjacencyList.ContainsKey(endVertex))
                {
                    adjacencyList[endVertex] = new List<Tuple<string, int>>();
                }
                adjacencyList[endVertex].Add(new Tuple<string, int>(startVertex, weight));
            }
        }


        // Добавить вершину
        public void addVertex(string vertex, List<Tuple<string, int>> edges = null)
        {
            if (!adjacencyList.ContainsKey(vertex))
            {
                adjacencyList[vertex] = edges ?? new List<Tuple<string, int>>();
            }
            else
            {
                Console.WriteLine($"Вершина {vertex} уже существует.");
            }
        }

        

        // Удаление ребра
        public bool removeEdge(string startVertex, string endVertex, int weight)
        {
            if (adjacencyList.ContainsKey(startVertex))
            {
                Tuple<string, int> edge = new Tuple<string, int>(endVertex, weight);
                bool removed = adjacencyList[startVertex].Remove(edge);

                // Если граф неориентированный, удаляем и обратное ребро
                if (!isDirected && removed)
                {
                    adjacencyList[endVertex].Remove(new Tuple<string, int>(startVertex, weight));
                }

                return removed;
            }
            return false;
        }
        // Удаление вершины
        public bool removeVertex(string vertex)
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
        public void saveToFile(string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                // Записываем тип графа: ориентированный или нет, взвешенный или нет
                writer.WriteLine($"isDirected:{isDirected}");
                writer.WriteLine($"isWeighted:{isWeighted}");

                // Записываем рёбра графа
                foreach (var vertex in adjacencyList)
                {
                    foreach (var edge in vertex.Value)
                    {
                        if (isWeighted)
                        {
                            // Формат записи для взвешенного графа: стартовая вершина, конечная вершина, вес
                            writer.WriteLine($"{vertex.Key} {edge.Item1} {edge.Item2}");
                        }
                        else
                        {
                            // Формат записи для невзвешенного графа: стартовая вершина, конечная вершина
                            writer.WriteLine($"{vertex.Key} {edge.Item1}");
                        }
                    }
                }

                // Находим изолированные вершины, которых нет в списке смежности или у которых нет рёбер
                var allVertices = adjacencyList.Keys.Union(adjacencyList.Values.SelectMany(v => v.Select(e => e.Item1))).Distinct();
                var isolatedVertices = allVertices.Where(v => !adjacencyList.ContainsKey(v) || adjacencyList[v].Count == 0);

                // Записываем информацию об изолированных вершинах
                if (isolatedVertices.Any())
                {
                    writer.WriteLine("Isolated vertices:");
                    foreach (var isolatedVertex in isolatedVertices)
                    {
                        writer.WriteLine(isolatedVertex);
                    }
                }
            }
            Console.WriteLine($"Граф успешно сохранён в файл {fileName}");
        }

        // Вывод графа в консоль
        public void printGraph()
        {
            // Получаем список всех вершин (ключей) из adjacencyList
            var allVertices = adjacencyList.Keys.ToList();

            // Добавляем вершины, которые есть только как конечные точки рёбер
            foreach (var vertexList in adjacencyList.Values)
            {
                foreach (var edge in vertexList)
                {
                    if (!allVertices.Contains(edge.Item1))
                    {
                        allVertices.Add(edge.Item1);
                    }
                }
            }

            allVertices.Sort();

            // Выводим список смежности для всех вершин
            foreach (var vertex in allVertices)
            {
                Console.Write($"{vertex}: ");
                if (adjacencyList.ContainsKey(vertex))
                {
                    foreach (var edge in adjacencyList[vertex])
                    {
                        Console.Write($"({edge.Item1}, {edge.Item2}) ");
                    }
                }
                Console.WriteLine(); // Переход на новую строку после каждой вершины
            }
        }
    }
}
