using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace visualisation_graph
{
    public partial class MainWindow : Window
    {
        private Graph graph;
        private Dictionary<string, Point> positions = new Dictionary<string, Point>();
        private List<string> userPath = new List<string>(); // Путь, выбранный пользователем
        private int score = 0;
        private Stopwatch stopwatch = new Stopwatch();

        public MainWindow()
        {
            InitializeComponent();
            graph = new Graph();
        }

        private void GenerateGraphButton_Click(object sender, RoutedEventArgs e)
        {
            GraphCanvas.Children.Clear();

            int verticesCount = int.TryParse(VerticesInput.Text, out verticesCount) ? verticesCount : 0;
            int edgesCount = int.TryParse(EdgesInput.Text, out edgesCount) ? edgesCount : 0;

            if (verticesCount <= 0 || edgesCount <= 0)
            {
                MessageBox.Show("Укажите корректное количество вершин и рёбер!");
                return;
            }

            graph = new Graph();
            graph.GenerateRandomGraph(verticesCount, edgesCount);

            var vertices = graph.GetVertices();
            var edges = graph.GetEdges();

            Random random = new();

            // Получаем текущие размеры Canvas
            double canvasWidth = GraphCanvas.ActualWidth;
            double canvasHeight = GraphCanvas.ActualHeight;

            positions = new Dictionary<string, Point>();

            // Располагаем вершины равномерно в пределах Canvas
            foreach (var vertex in vertices)
            {
                var position = new Point(
                    random.Next(50, (int)canvasWidth - 50),
                    random.Next(50, (int)canvasHeight - 50)
                );
                positions[vertex] = position;

                // Рисуем вершину
                Ellipse ellipse = new()
                {
                    Width = 30,
                    Height = 30,
                    Fill = Brushes.LightBlue,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                Canvas.SetLeft(ellipse, position.X - 15);
                Canvas.SetTop(ellipse, position.Y - 15);
                GraphCanvas.Children.Add(ellipse);

                // Привязываем событие клика к вершине
                ellipse.MouseLeftButtonDown += Vertex_Click;

                // Рисуем текст вершины
                TextBlock text = new()
                {
                    Text = vertex,
                    Foreground = Brushes.Black,
                    FontWeight = FontWeights.Bold,
                    FontSize = 16, // Увеличиваем размер шрифта
                    Margin = new Thickness(5) // Добавляем отступ
                };
                Canvas.SetLeft(text, position.X - 10);
                Canvas.SetTop(text, position.Y - 30); // Увеличиваем отступ сверху
                GraphCanvas.Children.Add(text);
            }

            // Рисуем рёбра
            foreach (var (from, to, weight) in edges)
            {
                if (positions.ContainsKey(from) && positions.ContainsKey(to))
                {
                    var fromPos = positions[from];
                    var toPos = positions[to];

                    // Линия для ребра
                    Line line = new()
                    {
                        X1 = fromPos.X,
                        Y1 = fromPos.Y,
                        X2 = toPos.X,
                        Y2 = toPos.Y,
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
                    };
                    GraphCanvas.Children.Add(line);

                    // Рисуем вес ребра
                    var midX = (fromPos.X + toPos.X) / 2;
                    var midY = (fromPos.Y + toPos.Y) / 2;
                    TextBlock weightText = new()
                    {
                        Text = weight.ToString(),
                        Foreground = Brushes.Red,
                        FontSize = 14 // Увеличиваем размер шрифта
                    };
                    Canvas.SetLeft(weightText, midX);
                    Canvas.SetTop(weightText, midY);
                    GraphCanvas.Children.Add(weightText);
                }
            }
        }

        private void StartCompetition_Click(object sender, RoutedEventArgs e)
        {
            string startVertex = StartVertexInput.Text;
            string endVertex = ParameterInput.Text;

            try
            {
                if (string.IsNullOrEmpty(startVertex) || string.IsNullOrEmpty(endVertex))
                {
                    MessageBox.Show("Укажите начальную и конечную вершины!");
                    return;
                }

                // Очистить пользовательский путь
                userPath.Clear();
                MessageBox.Show("Выберите путь вручную, кликая на вершины!");

                // Запустить таймер
                stopwatch.Restart();

                // Обновить текущий путь и время
                UpdateCurrentPathAndTime();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void EndUserPathSelection_Click(object sender, RoutedEventArgs e)
        {
            string startVertex = StartVertexInput.Text;
            string endVertex = ParameterInput.Text;

            try
            {
                if (string.IsNullOrEmpty(startVertex) || string.IsNullOrEmpty(endVertex))
                {
                    MessageBox.Show("Укажите начальную и конечную вершины!");
                    return;
                }

                // Остановить таймер
                stopwatch.Stop();

                // Компьютер находит кратчайший путь
                var algorithm = AlgorithmSelector.SelectedItem?.ToString() ?? "Dijkstra"; // Здесь можно менять алгоритм
                var computerPath = graph.FindShortestPath(startVertex, endVertex, algorithm);

                // Подсветить кратчайший путь компьютера
                HighlightPath(computerPath);

                // Сравнить пользовательский путь с оптимальным
                int pathScore = CalculateScore(userPath, computerPath, stopwatch.ElapsedMilliseconds);
                score += pathScore;

                MessageBox.Show($"Ваш путь завершён! Очки за попытку: {pathScore}. Общий счёт: {score}");
                ScoreText.Text = $"Очки: {score}";

                // Обновить текущий путь и время
                UpdateCurrentPathAndTime();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void ResetGraph_Click(object sender, RoutedEventArgs e)
        {
            // Сбросить пользовательский путь
            userPath.Clear();

            // Сбросить таймер
            stopwatch.Reset();

            // Очистить выделения
            foreach (var child in GraphCanvas.Children)
            {
                if (child is Ellipse ellipse)
                {
                    ellipse.Fill = Brushes.LightBlue; // Сбросить цвет вершин
                }
                else if (child is Line line)
                {
                    line.Stroke = Brushes.Black; // Сбросить цвет рёбер
                    line.StrokeThickness = 2;
                }
            }

            // Обновить текущий путь и время
            UpdateCurrentPathAndTime();

            // Обновить очки
            ScoreText.Text = $"Очки: 0";
            score = 0;

            // Обновить текстовые поля
            StartVertexInput.Text = string.Empty;
            ParameterInput.Text = string.Empty;
        }

        private void Vertex_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Ellipse ellipse)
            {
                // Найти вершину, соответствующую клику
                foreach (var pair in positions)
                {
                    if (IsEllipseAtPosition(ellipse, pair.Value))
                    {
                        string vertex = pair.Key;
                        if (userPath.Count == 0 || graph.IsNeighbor(userPath[userPath.Count - 1], vertex))
                        {
                            userPath.Add(vertex);
                            ellipse.Fill = Brushes.Green; // Подсветить выбранную вершину
                            UpdateCurrentPathAndTime();
                        }
                        else
                        {
                            MessageBox.Show("Выбранная вершина не является соседней!");
                        }

                        break;
                    }
                }
            }
        }

        private int CalculateScore(List<string> userPath, List<string> optimalPath, long elapsedTime)
        {
            int baseScore = 0;
            int timeBonus = 0;

            // Оценка правильности выбора пути
            if (userPath.Count == optimalPath.Count)
            {
                int matches = 0;
                for (int i = 0; i < userPath.Count; i++)
                {
                    if (userPath[i] == optimalPath[i])
                        matches++;
                }

                baseScore = matches * 10; // За каждую правильно угаданную вершину — 10 очков
            }

            // Бонус за время (чем быстрее, тем больше очков)
            timeBonus = (int)(10000 / elapsedTime);

            return baseScore + timeBonus;
        }

        private void HighlightPath(List<string> path)
        {
            foreach (var child in GraphCanvas.Children)
            {
                if (child is Line line)
                {
                    string from = GetVertexAtPosition(new Point(line.X1, line.Y1));
                    string to = GetVertexAtPosition(new Point(line.X2, line.Y2));

                    if (from != null && to != null && path.Contains(from) && path.Contains(to))
                    {
                        line.Stroke = Brushes.Red; // Подсветить рёбра
                        line.StrokeThickness = 3;
                    }
                }
            }
        }

        private string GetVertexAtPosition(Point position)
        {
            foreach (var pair in positions)
            {
                if (Math.Abs(pair.Value.X - position.X) < 5 && Math.Abs(pair.Value.Y - position.Y) < 5)
                    return pair.Key;
            }
            return null;
        }

        private bool IsEllipseAtPosition(Ellipse ellipse, Point position)
        {
            double left = Canvas.GetLeft(ellipse) + ellipse.Width / 2;
            double top = Canvas.GetTop(ellipse) + ellipse.Height / 2;

            return Math.Abs(left - position.X) < 5 && Math.Abs(top - position.Y) < 5;
        }

        private void UpdateCurrentPathAndTime()
        {
            // Обновить текущий путь
            CurrentPathText.Text = string.Join(" -> ", userPath);

            // Обновить текущее время
            CurrentTimeText.Text = $"Время: {stopwatch.ElapsedMilliseconds / 1000.0:F2} сек";
        }
    }
}
