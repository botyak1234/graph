using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace graph
{
    internal class Graph
    {
        private Dictionary<string, List<Tuple<string, int>>> adjacencyList;
        public int V => adjacencyList.Count; // Количество вершин
        public bool isDirected;
        public bool isWeighted;
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
            List<string> allVertices = new List<string>();

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
                    string[] parts = line.Split();

                    if (parts.Length == 1) // Изолированная вершина
                    {
                        string vertex = parts[0];
                        if (!adjacencyList.ContainsKey(vertex))
                        {
                            adjacencyList[vertex] = new List<Tuple<string, int>>();
                        }
                        if (!allVertices.Contains(vertex))
                        {
                            allVertices.Add(vertex); // Добавляем вершину
                        }
                    }
                    else if (parts.Length == 2) // Невзвешенное ребро
                    {
                        string startVertex = parts[0];
                        string endVertex = parts[1];

                        addEdge_const(startVertex, endVertex, -1);
                        if (!allVertices.Contains(startVertex))
                        {
                            allVertices.Add(startVertex);
                        }
                        if (!allVertices.Contains(endVertex))
                        {
                            allVertices.Add(endVertex);
                        }
                    }
                    else if (parts.Length == 3 && isWeighted) // Взвешенное ребро
                    {
                        string startVertex = parts[0];
                        string endVertex = parts[1];
                        int weight = int.Parse(parts[2]);

                        addEdge_const(startVertex, endVertex, weight);
                        if (!allVertices.Contains(startVertex))
                        {
                            allVertices.Add(startVertex);
                        }
                        if (!allVertices.Contains(endVertex))
                        {
                            allVertices.Add(endVertex);
                        }
                    }
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
                weight = -1; // Если граф невзвешенный, устанавливаем вес в -1
            }

            // Добавляем начальную вершину и её ребро
            if (!adjacencyList.ContainsKey(startVertex))
            {
                adjacencyList[startVertex] = new List<Tuple<string, int>>();
            }
            adjacencyList[startVertex].Add(new Tuple<string, int>(endVertex, weight));

            // Убедимся, что конечная вершина присутствует в списке смежности
            if (!adjacencyList.ContainsKey(endVertex))
            {
                adjacencyList[endVertex] = new List<Tuple<string, int>>();
            }

            // Если граф неориентированный, добавляем обратное ребро
            if (!isDirected && startVertex != endVertex)
            {
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
            if (!adjacencyList.ContainsKey(startVertex))
            {
                Console.WriteLine($"Вершина {startVertex} не существует.");
                return false;
            }
            if (!isWeighted)
            {
                var edge = adjacencyList[startVertex].Find(e => e.Item1 == endVertex);
                if (edge != null)
                {
                    adjacencyList[startVertex].Remove(edge);
                    Console.WriteLine($"Ребро {startVertex} -> {endVertex} удалено.");
                    if (!isDirected)
                    {
                        var reverseEdge = adjacencyList[endVertex].Find(e => e.Item1 == startVertex);
                        adjacencyList[endVertex].Remove(reverseEdge);
                    }
                    return true;
                }
                else
                {
                    Console.WriteLine($"Ребро {startVertex} -> {endVertex} не существует.");
                    return false;
                }
            }


            var weightedEdge = adjacencyList[startVertex].Find(e => e.Item1 == endVertex && e.Item2 == weight);
            if (weightedEdge != null)
            {
                adjacencyList[startVertex].Remove(weightedEdge);
                Console.WriteLine($"Ребро {startVertex} -> {endVertex} с весом {weight} удалено.");
                if (!isDirected)
                {
                    var reverseWeightedEdge = adjacencyList[endVertex].Find(e => e.Item1 == startVertex && e.Item2 == weight);
                    adjacencyList[endVertex].Remove(reverseWeightedEdge);
                }
                return true;
            }
            else
            {
                Console.WriteLine($"Ребро {startVertex} -> {endVertex} с весом {weight} не существует.");
                return false;
            }
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
                // Записываем тип графа: ориентированный/неориентированный и взвешенный/невзвешенный
                string graphType = (isDirected ? "y" : "n") + " " + (isWeighted ? "y" : "n");
                writer.WriteLine(graphType);

                // Множество для хранения уникальных рёбер (для неориентированного графа)
                HashSet<string> visitedEdges = new HashSet<string>();

                foreach (var vertex in adjacencyList)
                {
                    foreach (var edge in vertex.Value)
                    {
                        string start = vertex.Key;
                        string end = edge.Item1;
                        int weight = edge.Item2;

                        // Для неориентированного графа убедимся, что ребро не дублируется
                        if (!isDirected)
                        {
                            // Упорядочиваем вершины ребра по алфавиту, чтобы избежать дублирования
                            string edgeKey = start.CompareTo(end) < 0 ? $"{start} {end}" : $"{end} {start}";

                            if (!visitedEdges.Contains(edgeKey))
                            {
                                writer.WriteLine(isWeighted ? $"{start} {end} {weight}" : $"{start} {end}");
                                visitedEdges.Add(edgeKey);
                            }
                        }
                        else
                        {
                            // Для ориентированного графа просто записываем ребро
                            writer.WriteLine(isWeighted ? $"{start} {end} {weight}" : $"{start} {end}");
                        }
                    }
                }
            }

            Console.WriteLine($"Граф успешно сохранён в файл {fileName}.");
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

        public int GetInDegree(string vertex)
        {
            if (!adjacencyList.ContainsKey(vertex))
            {
                throw new ArgumentException($"Вершина {vertex} не существует в графе.");
            }

            int inDegree = 0;

            foreach (var adj in adjacencyList)
            {
                foreach (var edge in adj.Value)
                {
                    if (edge.Item1 == vertex)
                    {
                        inDegree++;
                    }
                }
            }

            return inDegree;
        }
        public string FindIntermediateVertex(string u, string v)
        {
            if (!adjacencyList.ContainsKey(u))
            {
                throw new ArgumentException($"Вершина {u} не существует в графе.");
            }
            if (!adjacencyList.ContainsKey(v))
            {
                throw new ArgumentException($"Вершина {v} не существует в графе.");
            }

            foreach (var edgeFromU in adjacencyList[u])
            {
                string w = edgeFromU.Item1;


                if (adjacencyList.ContainsKey(w) && adjacencyList[w].Exists(edge => edge.Item1 == v))
                {
                    return w;
                }
            }

            return null;
        }


        public Graph RemoveEdgesLeadingToLeafVertices()
        {
            // Создаём новый граф, чтобы не модифицировать исходный
            Graph newGraph = new Graph(isDirected, isWeighted);

            // Считаем количество входящих и исходящих рёбер для каждой вершины
            Dictionary<string, int> inDegree = new Dictionary<string, int>();
            Dictionary<string, int> outDegree = new Dictionary<string, int>();

            // Инициализируем словари для всех вершин
            foreach (var vertex in adjacencyList.Keys)
            {
                inDegree[vertex] = 0;
                outDegree[vertex] = 0;
            }

            // Заполняем количество входящих и исходящих рёбер
            foreach (var vertex in adjacencyList)
            {
                foreach (var edge in vertex.Value)
                {
                    string endVertex = edge.Item1;

                    // Увеличиваем исходящие рёбра для текущей вершины
                    outDegree[vertex.Key]++;

                    // Увеличиваем входящие рёбра для конечной вершины
                    if (inDegree.ContainsKey(endVertex))
                        inDegree[endVertex]++;
                    else
                        inDegree[endVertex] = 1;
                }
            }

            // Переносим все рёбра, кроме тех, которые ведут в висячие вершины
            foreach (var vertex in adjacencyList)
            {
                foreach (var edge in vertex.Value)
                {
                    string endVertex = edge.Item1;

                    // Для неориентированного графа: пропускаем ребро, если конечная вершина — висячая
                    if (!isDirected)
                    {
                        // В неориентированном графе висячая вершина — это вершина с одним ребром
                        if (outDegree[vertex.Key] > 1 && inDegree[endVertex] > 1)
                        {
                            newGraph.addEdge_const(vertex.Key, endVertex, edge.Item2);
                        }
                    }
                    // Для ориентированного графа: пропускаем рёбра, ведущие к вершинам с одним входящим или одним исходящим ребром
                    else
                    {
                        if ((inDegree.ContainsKey(vertex.Key) && outDegree.ContainsKey(vertex.Key) && inDegree[vertex.Key] + outDegree[vertex.Key] > 1 && inDegree.ContainsKey(endVertex) && outDegree.ContainsKey(endVertex) && inDegree[endVertex] + outDegree[endVertex] > 1))
                        {
                            newGraph.addEdge_const(vertex.Key, endVertex, edge.Item2);
                        }
                    }
                }
            }

            // Добавляем изолированные вершины (если есть)
            foreach (var vertex in adjacencyList.Keys)
            {
                if (adjacencyList[vertex].Count == 0 || (inDegree[vertex] == 0 && outDegree[vertex] == 0))
                {
                    newGraph.addVertex(vertex, new List<Tuple<string, int>>());
                }
            }

            return newGraph;
        }

        public Graph RemoveVertex(string vertex)
        {
            Graph newGraph = new Graph(isDirected, isWeighted);

            foreach (var v in adjacencyList.Keys)
            {
                if (v != vertex)
                {
                    foreach (var edge in adjacencyList[v])
                    {
                        if (edge.Item1 != vertex)
                        {
                            newGraph.addEdge_const(v, edge.Item1, edge.Item2);
                        }
                    }
                }
            }

            return newGraph;
        }



        public Dictionary<string, int> ShortestPathsBFS(string u)
        {
            // Словарь для хранения расстояний
            Dictionary<string, int> distances = new Dictionary<string, int>();

            // Инициализация всех расстояний как бесконечность
            foreach (var vertex in adjacencyList.Keys)
            {
                distances[vertex] = int.MaxValue;
            }

            // Очередь для BFS
            Queue<string> queue = new Queue<string>();

            // Инициализируем расстояние до вершины u как 0
            distances[u] = 0;
            queue.Enqueue(u);

            // Пока очередь не пуста
            while (queue.Count > 0)
            {
                string currentVertex = queue.Dequeue();

                // Проходим по всем соседям текущей вершины
                foreach (var neighbor in adjacencyList[currentVertex])
                {
                    string nextVertex = neighbor.Item1;

                    // Если вершина ещё не посещена
                    if (distances[nextVertex] == int.MaxValue)
                    {
                        // Обновляем расстояние и добавляем её в очередь
                        distances[nextVertex] = distances[currentVertex] + 1;
                        queue.Enqueue(nextVertex);
                    }
                }
            }

            return distances;
        }



        public List<Edge> Kruskal(ref bool flag)
        {
            List<Edge> edges = new List<Edge>();

            foreach (var vertex in adjacencyList.Keys)
            {
                foreach (var neighbor in adjacencyList[vertex])
                {
                    string u = vertex;
                    string v = neighbor.Item1;
                    int weight = neighbor.Item2;

                    if (string.Compare(u, v) < 0)
                    {
                        edges.Add(new Edge(u, v, weight));
                    }
                }
            }

            edges.Sort((a, b) => a.Weight.CompareTo(b.Weight));

            UnionFind uf = new UnionFind();

            foreach (var vertex in adjacencyList.Keys)
            {
                uf.Add(vertex);
            }

            List<Edge> result = new List<Edge>();

            foreach (var edge in edges)
            {
                string u = edge.U;
                string v = edge.V;

                if (uf.Find(u) != uf.Find(v))
                {
                    uf.Union(u, v);
                    result.Add(edge);
                }
            }

            int vertexCount = adjacencyList.Keys.Count;
            if (result.Count == vertexCount - 1 && IsConnected())
            {
                return result;
            }
            else
            {
        
                flag = false;
                return null;
            }
        }


        public int CountEdges()
        {
            int edgeCount = 0;

            foreach (var vertex in adjacencyList)
            {
                
                edgeCount += vertex.Value.Count;
            }

            
            if (!isDirected)
            {
                edgeCount /= 2;
            }

            return edgeCount;
        }

        public bool IsConnected()
        {
            
            if (adjacencyList.Count == 0)
                return false;

            
            string startVertex = adjacencyList.Keys.First();
            HashSet<string> visited = new HashSet<string>();

            DFS(startVertex, visited);

            return visited.Count == adjacencyList.Count;
        }

        private void DFS(string vertex, HashSet<string> visited)
        {
            visited.Add(vertex);

            foreach (var neighbor in adjacencyList[vertex])
            {
                string neighborVertex = neighbor.Item1;

                if (!visited.Contains(neighborVertex))
                {
                    DFS(neighborVertex, visited);
                }
            }
        }

        public void ADDEdge(string from, string to, int weight)
        {
            if (!adjacencyList.ContainsKey(from))
                adjacencyList[from] = new List<Tuple<string, int>>();
            if (!adjacencyList.ContainsKey(to))
                adjacencyList[to] = new List<Tuple<string, int>>();

            adjacencyList[from].Add(new Tuple<string, int>(to, weight));
            adjacencyList[to].Add(new Tuple<string, int>(from, weight)); // Для неориентированного графа
        }

        public bool CanRemoveVertexToFormTree()
        {
            
            foreach (var vertex in adjacencyList.Keys.ToList())
            {
                
                var newGraph = RemoveVertex(vertex);

                
                int edgeCount = newGraph.CountEdges();

                
                if (!isDirected)
                {
                    edgeCount /= 2;
                }

                
                int vertexCount = newGraph.adjacencyList.Count;

                
                if (edgeCount == vertexCount - 1 && newGraph.IsConnected())
                {
                    return true; 
                }
            }

            return false; 
        }



        public Dictionary<string, int> Dijkstra(string startVertex)
        {
            if (!adjacencyList.ContainsKey(startVertex))
                throw new ArgumentException($"Вершина {startVertex} отсутствует в графе.");

            
            var distances = new Dictionary<string, int>();
            var priorityQueue = new SortedSet<(int, string)>();
            var visited = new HashSet<string>();

            
            foreach (var vertex in adjacencyList.Keys)
                distances[vertex] = int.MaxValue;

            
            foreach (var vertexList in adjacencyList.Values)
            {
                foreach (var edge in vertexList)
                {
                    string neighbor = edge.Item1;
                    if (!distances.ContainsKey(neighbor))
                        distances[neighbor] = int.MaxValue;
                }
            }

            
            distances[startVertex] = 0;
            priorityQueue.Add((0, startVertex));

            
            while (priorityQueue.Count > 0)
            {
                
                var (currentDistance, currentVertex) = priorityQueue.Min;
                priorityQueue.Remove(priorityQueue.Min);

                
                if (visited.Contains(currentVertex))
                    continue;

                visited.Add(currentVertex);

                
                if (adjacencyList.ContainsKey(currentVertex))
                {
                    foreach (var edge in adjacencyList[currentVertex])
                    {
                        string neighbor = edge.Item1;
                        int weight = edge.Item2;

                        if (visited.Contains(neighbor))
                            continue;

                        int newDistance = currentDistance + weight;

                        
                        if (newDistance < distances[neighbor])
                        {
                            
                            priorityQueue.Remove((distances[neighbor], neighbor));
                            distances[neighbor] = newDistance;
                            priorityQueue.Add((newDistance, neighbor));
                        }
                    }
                }
            }

            return distances;
        }




        public bool minNWay(int N)
        {
            foreach (var vertex in adjacencyList.Keys)
            {
                try
                {
                    var distances = Dijkstra(vertex);

                    Console.WriteLine($"Расстояния от вершины {vertex}:");
                    foreach (var kvp in distances)
                    {
                        Console.WriteLine($"До вершины {kvp.Key}: {kvp.Value}");
                    }

                    if (distances.Values.All(distance => distance <= N))
                    {
                        Console.WriteLine($"Вершина {vertex} удовлетворяет условию: все минимальные пути <= {N}");
                        return true;
                    }
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Console.WriteLine("Такой вершины нет.");
            return false;
        }


        public Dictionary<string, int> BellmanFord(string startVertex, out Dictionary<string, string> predecessors)
        {
            var distances = new Dictionary<string, int>();
            predecessors = new Dictionary<string, string>();

            
            foreach (var vertex in adjacencyList.Keys)
            {
                distances[vertex] = int.MaxValue;  
                predecessors[vertex] = null;       
            }

            distances[startVertex] = 0;  

            
            for (int i = 0; i < V - 1; i++)
            {
                foreach (var vertex in adjacencyList.Keys)
                {
                    
                    if (distances[vertex] == int.MaxValue)
                        continue;

                    foreach (var edge in adjacencyList[vertex])
                    {
                        if (!distances.ContainsKey(edge.Item1))
                        {
                            distances[edge.Item1] = int.MaxValue;  
                            predecessors[edge.Item1] = null;       
                        }
                        if (distances[vertex] != int.MaxValue && distances[vertex] + edge.Item2 < distances[edge.Item1])
                        {
                            distances[edge.Item1] = distances[vertex] + edge.Item2;
                            predecessors[edge.Item1] = vertex;
                        }
                    }
                }
            }

            foreach (var vertex in adjacencyList.Keys)
            {
                foreach (var edge in adjacencyList[vertex])
                {
                    if (distances[vertex] != int.MaxValue && distances[vertex] + edge.Item2 < distances[edge.Item1])
                    {
                        throw new InvalidOperationException("Граф содержит отрицательный цикл.");
                    }
                }
            }

            return distances;
        }
        public List<string> GetPath(string startVertex, string endVertex, Dictionary<string, string> predecessors)
        {
            var path = new List<string>();
            for (var at = endVertex; at != null; at = predecessors[at])
            {
                path.Add(at);
            }
            path.Reverse();
            return path;
        }


        public Dictionary<string, Dictionary<string, int>> FloydWarshallWithCycleCheck(out bool hasNegativeCycle)
        {
            hasNegativeCycle = false;

            var distances = new Dictionary<string, Dictionary<string, int>>();
            foreach (var vertex in adjacencyList.Keys)
            {
                distances[vertex] = new Dictionary<string, int>();
                foreach (var otherVertex in adjacencyList.Keys)
                {
                    if (vertex == otherVertex)
                        distances[vertex][otherVertex] = 0; 
                    else
                        distances[vertex][otherVertex] = int.MaxValue; 
                }
            }

            
            foreach (var from in adjacencyList.Keys)
            {
                foreach (var edge in adjacencyList[from])
                {
                    var to = edge.Item1;
                    var weight = edge.Item2;
                    distances[from][to] = weight; 
                }
            }

            
            foreach (var k in adjacencyList.Keys)
            {
                foreach (var i in adjacencyList.Keys)
                {
                    foreach (var j in adjacencyList.Keys)
                    {
                        if (distances[i][k] != int.MaxValue && distances[k][j] != int.MaxValue &&
                            distances[i][k] + distances[k][j] < distances[i][j])
                        {
                            distances[i][j] = distances[i][k] + distances[k][j];
                        }
                    }
                }
            }

            
            foreach (var vertex in adjacencyList.Keys)
            {
                if (distances[vertex][vertex] < 0)
                {
                    hasNegativeCycle = true;
                    Console.WriteLine($"Обнаружен отрицательный цикл с участием вершины {vertex}.");
                    break;
                }
            }

            return distances;
        }





        public HashSet<string> NPeripheria(string vertex, int N)
        {
            var nPeripheras = new HashSet<string>();
            bool hasNegativeCycle;

            var distances = FloydWarshallWithCycleCheck(out hasNegativeCycle);

            if (hasNegativeCycle)
            {
                Console.WriteLine("Обнаружен отрицательный цикл. Невозможно корректно вычислить N-периферию.");
                return nPeripheras;
            }

            if (!distances.ContainsKey(vertex))
            {
                Console.WriteLine($"Ошибка: Вершина {vertex} отсутствует в графе.");
                return nPeripheras;
            }

            foreach (var otherVertex in distances[vertex].Keys)
            {
                var distance = distances[vertex][otherVertex];

                if (distance > N && distance != int.MaxValue)
                {
                    nPeripheras.Add(otherVertex);
                    Console.WriteLine($"Добавлено в N-периферию: {otherVertex}");
                }
            }

            return nPeripheras;
        }



        public int MaxFlowEdmondsKarp(string source, string sink)
        {
            var capacity = new Dictionary<string, Dictionary<string, int>>();
            foreach (var from in adjacencyList.Keys)
            {
                capacity[from] = new Dictionary<string, int>();
                foreach (var (to, cap) in adjacencyList[from])
                {
                    capacity[from][to] = cap;

                    if (!capacity.ContainsKey(to))
                        capacity[to] = new Dictionary<string, int>();
                    if (!capacity[to].ContainsKey(from))
                        capacity[to][from] = 0;
                }
            }

            var flow = new Dictionary<string, Dictionary<string, int>>();
            foreach (var from in capacity.Keys)
            {
                flow[from] = new Dictionary<string, int>();
                foreach (var to in capacity[from].Keys)
                    flow[from][to] = 0;
            }

            int maxFlow = 0;

            while (true)
            {
                var parent = new Dictionary<string, string>();
                var augmentingPathFlow = Bfs(source, sink, capacity, flow, parent);

                if (augmentingPathFlow == 0)
                    break;

                maxFlow += augmentingPathFlow;

                string v = sink;
                while (v != source)
                {
                    string u = parent[v];

                    if (!flow[v].ContainsKey(u))
                        flow[v][u] = 0;

                    flow[u][v] += augmentingPathFlow;
                    flow[v][u] -= augmentingPathFlow;
                    v = u;
                }
            }

            return maxFlow;
        }

        private int Bfs(string source, string sink, Dictionary<string, Dictionary<string, int>> capacity,
                        Dictionary<string, Dictionary<string, int>> flow, Dictionary<string, string> parent)
        {
            var visited = new HashSet<string>();
            var queue = new Queue<(string, int)>();
            queue.Enqueue((source, int.MaxValue));

            while (queue.Count > 0)
            {
                var (u, pathFlow) = queue.Dequeue();

                foreach (var v in capacity[u].Keys)
                {
                    int residualCapacity = capacity[u][v] - flow[u][v];
                    if (!visited.Contains(v) && residualCapacity > 0)
                    {
                        parent[v] = u;
                        int newPathFlow = Math.Min(pathFlow, residualCapacity);
                        if (v == sink)
                            return newPathFlow;

                        visited.Add(v);
                        queue.Enqueue((v, newPathFlow));
                    }
                }
            }

            return 0;
        }














    }
}
