using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graph
{
    internal class Program
    {
        static void CalculateInDegree(Graph graph)
        {
            if (graph == null)
            {
                Console.WriteLine("Граф не инициализирован.");
                return;
            }

            Console.Write("Введите вершину для расчета полустепени захода (тип string): ");
            string vertex = Console.ReadLine();

            try
            {
                int inDegree = graph.GetInDegree(vertex);
                Console.WriteLine($"Полустепень захода вершины {vertex}: {inDegree}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void RemoveEdgesToLeafVertices(Graph graph)
        {
            if (graph == null)
            {
                Console.WriteLine("Граф не инициализирован.");
                return;
            }

            Graph new_graph = graph.RemoveEdgesLeadingToLeafVertices();
            new_graph.saveToFile("new_one.txt");
            Console.WriteLine("Операция завершена.");
        }

        static void RemoveEdge(Graph graph)
        {
            if (graph == null)
            {
                Console.WriteLine("Граф не инициализирован.");
                return;
            }

            Console.Write("Введите начальную вершину: ");
            string startVertex = Console.ReadLine();

            Console.Write("Введите конечную вершину: ");
            string endVertex = Console.ReadLine();

            if (graph.isWeighted)
            {
                Console.Write("Введите вес ребра (тип int): ");
                int weight = int.Parse(Console.ReadLine());
                graph.removeEdge(startVertex, endVertex, weight);
            }
            else
            {
                graph.removeEdge(startVertex, endVertex, -1);
            }
        }

        static void FindPathThroughIntermediateVertex(Graph graph)
        {
            if (graph == null)
            {
                Console.WriteLine("Граф не инициализирован.");
                return;
            }

            Console.Write("Введите вершину u: ");
            string u = Console.ReadLine();
            Console.Write("Введите вершину v: ");
            string v = Console.ReadLine();

            try
            {
                string intermediateVertex = graph.FindIntermediateVertex(u, v);
                if (intermediateVertex != null)
                {
                    Console.WriteLine($"Можно попасть из вершины {u} в вершину {v} через вершину {intermediateVertex}.");
                }
                else
                {
                    Console.WriteLine($"Невозможно попасть из вершины {u} в вершину {v} через одну промежуточную вершину.");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void FindMinimumSpanningTree(Graph graph)
        {
            bool flag = true;
            if (graph == null)
            {
                Console.WriteLine("Граф не инициализирован.");
                return;
            }

            List<Edge> mst = graph.Kruskal(ref flag);
            if (flag)
            {
                Console.WriteLine("Минимальное остовное дерево:");
                foreach (var edge in mst)
                {
                    Console.WriteLine($"{edge.U} -- {edge.V} == {edge.Weight}");
                }
            }
            else
            {
                Console.WriteLine("Не удалось создать дерево");
            }
        }

        public static void Main(string[] args)
        {
            // Ввод пользователя: выбор создания графа с нуля или считывания из файла
            Console.WriteLine("Вы хотите создать новый граф (1) или считать граф из файла (2)? Введите 1 или 2: ");
            string choice = Console.ReadLine();

            Graph graph = null;

            if (choice == "1")
            {
                Console.WriteLine("Вы хотите работать с ориентированным графом? (y/n): ");
                string directedChoice = Console.ReadLine();
                bool isDirected = directedChoice.ToLower() == "y";

                Console.WriteLine("Вы хотите работать с взвешенным графом? (y/n): ");
                string weightedChoice = Console.ReadLine();
                bool isWeighted = weightedChoice.ToLower() == "y";

                graph = new Graph(isDirected, isWeighted);

                Console.WriteLine("Введите ребра в формате: начало конец (вес для взвешенных графов). Введите 'stop' для завершения:");
                while (true)
                {
                    string edgeInput = Console.ReadLine();
                    if (edgeInput.ToLower() == "stop") break;

                    string[] edgeParts = edgeInput.Split(' ');
                    if (edgeParts.Length < 2 || (isWeighted && edgeParts.Length != 3))
                    {
                        Console.WriteLine("Некорректный ввод. Попробуйте еще раз.");
                        continue;
                    }

                    string startVertex = edgeParts[0];
                    string endVertex = edgeParts[1];
                    int weight = isWeighted ? int.Parse(edgeParts[2]) : 1;

                    graph.addEdge_const(startVertex, endVertex, weight);
                }
            }
            else if (choice == "2")
            {
                Console.WriteLine("Введите имя файла для загрузки (например, graph_data.txt): ");
                string fileName = Console.ReadLine();
                string inputFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
                graph = new Graph(inputFile);
            }
            else
            {
                Console.WriteLine("Некорректный ввод. Программа завершена.");
                return;
            }

            while (true)
            {
                Console.WriteLine("1. Добавить вершину");
                Console.WriteLine("2. Добавить ребро");
                Console.WriteLine("3. Удалить вершину");
                Console.WriteLine("4. Удалить ребро");
                Console.WriteLine("5. Вывести граф");
                Console.WriteLine("6. Вывести полустепень захода вершины");
                Console.WriteLine("7. Сохранить в файл");
                Console.WriteLine("8. можно ли попасть из вершины u в вершину v через одну какую-либо вершину орграфа");
                Console.WriteLine("9. Без висячих вершин");
                Console.WriteLine("10. А можно ли сделать дерево?");
                Console.WriteLine("11. Кратчайшие пути");
                Console.WriteLine("12. Найти циклы в графе");
                Console.WriteLine("13. Найти минимальное остовное дерево (алгоритм Краскала)");
                Console.WriteLine("14. Минимальный путь не превосходящий N");
                Console.WriteLine("15. Путь от u до всех остальных");
                Console.WriteLine("16. N-периферия");
                Console.WriteLine("17. Потоки");
                Console.WriteLine("0. Выйти");

                string cases = Console.ReadLine();

                switch (cases)
                {
                    case "1":
                        Console.WriteLine("Введите номер вершины:");
                        string vertex = Console.ReadLine();
                        graph.addVertex(vertex);
                        break;

                    case "2":
                        Console.WriteLine("Введите начальную вершину, конечную вершину и вес через пробел:");
                        var edgeParts = Console.ReadLine().Split(' ');
                        string start = edgeParts[0];
                        string end = edgeParts[1];
                        if (graph.isWeighted)
                        {
                            int weight = int.Parse(edgeParts[2]);
                            graph.addEdge(start, end, weight);
                        }
                        else
                        {
                            graph.addEdge(start, end, -1);
                        }
                        break;

                    case "3":
                        Console.WriteLine("Введите номер вершины:");
                        string removeVertex = Console.ReadLine();
                        graph.removeVertex(removeVertex);
                        break;

                    case "4":
                        RemoveEdge(graph);
                        break;

                    case "5":
                        graph.printGraph();
                        break;

                    case "6":
                        CalculateInDegree(graph);
                        break;

                    case "7":
                        Console.WriteLine("Введите путь к файлу для сохранения:");
                        string filePath = Console.ReadLine();
                        graph.saveToFile(filePath);
                        break;

                    case "8":
                        FindPathThroughIntermediateVertex(graph);
                        break;

                    case "9":
                        RemoveEdgesToLeafVertices(graph);
                        break;
                    case "10":
                        Console.WriteLine(graph.CanRemoveVertexToFormTree());
                        break;

                    case "11":
                        Console.WriteLine("Введите вершину");
                        foreach (var entry in graph.ShortestPathsBFS(Console.ReadLine()))
                            Console.WriteLine($"Расстояние от {entry.Key} до заданной вершины - {(entry.Value == int.MaxValue ? "недостижимость" : entry.Value.ToString())}");
                        break;
                    case "13":
                        FindMinimumSpanningTree(graph);
                        break;

                    case "14":
                        Console.WriteLine("Введите N");
                        graph.minNWay(int.Parse(Console.ReadLine()));
                        break;
                    case "15":
                        Console.WriteLine("Введите вершину u");
                        var startVertex = Console.ReadLine();
                        Dictionary<string, string> predecessors;
                        var distances = graph.BellmanFord(startVertex, out predecessors);
                        foreach (var v in distances)
                        {
                            Console.WriteLine($"До вершины {v.Key} расстояние: {v.Value}");
                            var path = graph.GetPath(startVertex, v.Key, predecessors);
                            Console.WriteLine($"Путь: {string.Join(" -> ", path)}");
                        }
                        break;
                    case "16":
                        Console.WriteLine("Введите вершину u");
                        string start_Ver = Console.ReadLine();
                        Console.WriteLine("Введите значение N");
                        int N = int.Parse(Console.ReadLine());
                        var nPeripheria = graph.NPeripheria(start_Ver, N);
                        foreach (var vert in nPeripheria)
                        {
                            Console.Write(vert, " ");
                        }
                        Console.WriteLine();
                        break;
                    case "17":
                        Console.WriteLine("Введите вершину-источник:");
                        string source = Console.ReadLine();
                        Console.WriteLine("Введите вершину-сток:");
                        string sink = Console.ReadLine();

                        int maxFlow = graph.MaxFlowEdmondsKarp(source, sink);
                        Console.WriteLine($"Максимальный поток из {source} в {sink}: {maxFlow}");
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Некорректный выбор.");
                        break;
                }
            }
        }
    }
}
