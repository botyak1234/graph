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
  


        public static void Main(string[] args)
        {
            // Ввод пользователя: выбор создания графа с нуля или считывания из файла
            Console.WriteLine("Вы хотите создать новый граф (1) или считать граф из файла (2)? Введите 1 или 2: ");
            string choice = Console.ReadLine();

            Graph graph = null;

            if (choice == "1")
            {
                // Создание графа с нуля

                // Ввод пользователя: выбор ориентированного или неориентированного графа
                Console.WriteLine("Вы хотите работать с ориентированным графом? (y/n): ");
                string directedChoice = Console.ReadLine();
                bool isDirected = directedChoice.ToLower() == "y";

                // Ввод пользователя: выбор взвешенного или невзвешенного графа
                Console.WriteLine("Вы хотите работать с взвешенным графом? (y/n): ");
                string weightedChoice = Console.ReadLine();
                bool isWeighted = weightedChoice.ToLower() == "y";

                // Создание пустого графа с заданными параметрами
                graph = new Graph(isDirected, isWeighted);

                // Добавляем вершины и ребра вручную (пример: добавим несколько ребер)
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

                    int startVertex = int.Parse(edgeParts[0]);
                    int endVertex = int.Parse(edgeParts[1]);
                    int weight = isWeighted ? int.Parse(edgeParts[2]) : 1;

                    graph.addEdge_const(startVertex, endVertex, weight);
                }
            }
            else if (choice == "2")
            {
                // Считывание графа из файла
                Console.WriteLine("Введите имя файла для загрузки (например, graph_data.txt): ");
                string fileName = Console.ReadLine();
                string fullPath = Path.Combine("path_to_your_directory", fileName);

                // Создание графа из файла
                graph = new Graph(fullPath);
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
                Console.WriteLine("6. Сохранить в файл");
                Console.WriteLine("0. Выйти");

                string cases = Console.ReadLine();

                switch (cases)
                {
                    case "1":
                        Console.WriteLine("Введите номер вершины:");
                        int vertex = int.Parse(Console.ReadLine());
                        graph.addVertex(vertex);
                        break;

                    case "2":
                        Console.WriteLine("Введите начальную вершину, конечную вершину и вес через пробел:");
                        var edgeParts = Console.ReadLine().Split(' ');
                        int start = int.Parse(edgeParts[0]);
                        int end = int.Parse(edgeParts[1]);
                        int weight = int.Parse(edgeParts[2]);
                        graph.addEdge(start, end, weight);
                        break;

                    case "3":
                        Console.WriteLine("Введите номер вершины:");
                        int removeVertex = int.Parse(Console.ReadLine());
                        graph.removeVertex(removeVertex);
                        break;

                    case "4":
                        Console.WriteLine("Введите начальную вершину, конечную вершину и вес через пробел:");
                        var removeEdgeParts = Console.ReadLine().Split(' ');
                        int rStart = int.Parse(removeEdgeParts[0]);
                        int rEnd = int.Parse(removeEdgeParts[1]);
                        int rWeight = int.Parse(removeEdgeParts[2]);
                        graph.removeEdge(rStart, rEnd, rWeight);
                        break;

                    case "5":
                        graph.printGraph();
                        break;

                    case "6":
                        Console.WriteLine("Введите путь к файлу для сохранения:");
                        string filePath = Console.ReadLine();
                        graph.saveToFile(filePath);
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
