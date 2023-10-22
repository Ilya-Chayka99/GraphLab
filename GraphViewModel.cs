using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.IO;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Input;
using gr6;
using System.Data;

namespace gr6
{
    internal class GraphViewModel : ViewModel
    {
        public double Width { get; set; } = 1300;
        public double Heigth { get; set; } = 670;
        private int radius = 30;

        #region Priperty : int - Количество вершин графа
        private int _CountGraph = 0;

        /// <summary>Количество вершин графа</summary>
        public int CountGraph
        {
            get => _CountGraph;
            set => Set(ref _CountGraph, value, "CountGraph");
        }
        #endregion

        #region Priperty : double - Матрица смежности
        private double[,] _MatrixSm = new double[1, 1];

        /// <summary>Матрица смежности</summary>
        public double[,] MatrixSm
        {
            get => _MatrixSm;
            set
            {
                Set(ref _MatrixSm, value, "MatrixSm");
                GenerateMatrixView();
            }
        }
        #endregion

        #region Priperty : bool - Как задается граф: false - матрица смежности
        private bool _TipGraph = false;

        /// <summary>Как задается граф: false - матрица смежности</summary>
        public bool TipGraph
        {
            get => _TipGraph;
            set => Set(ref _TipGraph, value, "TipGraph");
        }
        #endregion 

        #region Priperty : DataView  - Матрица смежности для отображения
        private DataView _MatrixSmView = new DataView() ;

        /// <summary>Матрица смежности для отображения</summary>
        public DataView MatrixSmView
        {
            get => _MatrixSmView;
            set => Set(ref _MatrixSmView, value, "MatrixSmView");
        }
        #endregion

        #region Priperty : Point[] - Массив вершин графа (Координаты)
        private Point[] _PointGraph = new Point[1];

        /// <summary>Массив вершин графа (Координаты)</summary>
        public Point[] PointGraph
        {
            get => _PointGraph;
            set => Set(ref _PointGraph, value, "PointGraph");
        }
        #endregion

        #region Priperty : ObservableCollection<Ellipses> - Вершины графа для отображения
        private ObservableCollection<Ellipses> _EllipseP = new ObservableCollection<Ellipses>();

        /// <summary>Вершины графа для отображения</summary>
        public ObservableCollection<Ellipses> EllipseP
        {
            get => _EllipseP;
            set => Set(ref _EllipseP, value, "EllipseP");
        }
        #endregion

        #region Priperty : string - Первая вершина для добавления / удаления ребра
        private string _VertexOne;

        /// <summary>Первая вершина для добавления / удаления ребра</summary>
        public string VertexOne
        {
            get => _VertexOne;
            set => Set(ref _VertexOne, value, "VertexOne");
        }
        #endregion

        #region Priperty : string - Вторая вершина для добавления / удаления ребра
        private string _VertexTwo;

        /// <summary>Вторая вершина для добавления / удаления ребра</summary>
        public string VertexTwo
        {
            get => _VertexTwo;
            set => Set(ref _VertexTwo, value, "VertexTwo");
        }
        #endregion

        #region Priperty : string - Вершина для удаления 
        private string _VertexDel;

        /// <summary>Вторая вершина для добавления / удаления ребра</summary>
        public string VertexDel
        {
            get => _VertexDel;
            set => Set(ref _VertexDel, value, "VertexDel");
        }
        #endregion

        #region Priperty : string - Вес ребра добавление / удаление 
        private string _EdgeWeight;

        /// <summary>Вес ребра добавление / удаление</summary>
        public string EdgeWeight
        {
            get => _EdgeWeight;
            set => Set(ref _EdgeWeight, value, "EdgeWeight");
        }
        #endregion

        #region Command AddVertexCommand - Добавление вершины в граф

        ///<summary>Добавление вершины в граф</summary>
        private LambdaCommand? _AddVertexCommand;

        ///<summary>Добавление вершины в граф</summary>
        public ICommand AddVertexCommand => _AddVertexCommand
                    ??= new(OnAddVertexCommandExecuted, CanAddVertexCommandExecute);

        ///<summary>Проверка возможности выполнения - Добавление вершины в граф</summary>
        private bool CanAddVertexCommandExecute(object p) => true;

        ///<summary>Логика выполнения - Добавление вершины в граф</summary>
        private void OnAddVertexCommandExecuted(object p)
        {
            AddVertex();
        }
        #endregion

        #region Command DelVertexCommand - Удаление вершины из графа

        ///<summary>Удаление вершины из графа</summary>
        private LambdaCommand? _DelVertexCommand;

        ///<summary>Удаление вершины из графа</summary>
        public ICommand DelVertexCommand => _DelVertexCommand
                    ??= new(OnDelVertexCommandExecuted, CanDelVertexCommandExecute);

        ///<summary>Проверка возможности выполнения - Удаление вершины из графа</summary>
        private bool CanDelVertexCommandExecute(object p)
        {
            try
            {
                var ver = int.Parse(VertexDel);
                return true;
            }
            catch 
            {
                return false;
            }
        }

        ///<summary>Логика выполнения - Удаление вершины из графа</summary>
        private void OnDelVertexCommandExecuted(object p)
        {
            DeleteVertex();
        }
        #endregion

        #region Command LoadFileCommand - Загрузка из файла графа

        ///<summary>Загрузка из файла графа</summary>
        private LambdaCommand? _LoadFileCommand;

        ///<summary>Загрузка из файла графа</summary>
        public ICommand LoadFileCommand => _LoadFileCommand
                    ??= new(OnLoadFileCommandExecuted, CanLoadFileCommandExecute);

        ///<summary>Проверка возможности выполнения - Загрузка из файла графа</summary>
        private bool CanLoadFileCommandExecute(object p) => true;

        ///<summary>Логика выполнения - Загрузка из файла графа</summary>
        private void OnLoadFileCommandExecuted(object p)
        {
            GetFileContains();
            CalculatePoint();
            CreateEllipse();
            CalculateLine();
            GenerateMatrixView();
        }
        #endregion


        #region Command CreateEdgeCommand - Добавление ребра
#nullable enable
        ///<summary>Добавление ребра</summary>
        private LambdaCommand? _CreateEdgeCommand;

        ///<summary>Добавление ребра</summary>
        public ICommand CreateEdgeCommand => _CreateEdgeCommand
                    ??= new(OnCreateEdgeCommandExecuted, CanCreateEdgeCommandExecute);

        ///<summary>Проверка возможности выполнения - Добавление ребра</summary>
        private bool CanCreateEdgeCommandExecute(object p)
        {
            try
            {
                if(EdgeWeight != null && EdgeWeight != "")
                {
                    var d = double.Parse(EdgeWeight);
                    if (VertexTwo != null && VertexTwo != "")
                    {
                        var d1 = int.Parse(VertexTwo);
                        if (VertexOne != null && VertexOne != "")
                        {
                            var d2 = int.Parse(VertexOne);
                            if(d2<CountGraph && d1<CountGraph)
                                return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        ///<summary>Логика выполнения - Добавление ребра</summary>
        private void OnCreateEdgeCommandExecuted(object p)
        {
            var m = MatrixSm;
            m[int.Parse(VertexOne), int.Parse(VertexTwo)] = double.Parse(EdgeWeight);
            m[ int.Parse(VertexTwo),int.Parse(VertexOne)] = double.Parse(EdgeWeight);
            MatrixSm = m;
            CalculateLine();
        }
        #endregion

        #region Command DeleteEdgeCommand - Удаление ребра
#nullable enable
        ///<summary>Удаление ребра</summary>
        private LambdaCommand? _DeleteEdgeCommand;

        ///<summary>Удаление ребра</summary>
        public ICommand DeleteEdgeCommand => _DeleteEdgeCommand
                    ??= new(OnDeleteEdgeCommandExecuted, CanDeleteEdgeCommandExecute);

        ///<summary>Проверка возможности выполнения - Удаление ребра</summary>
        private bool CanDeleteEdgeCommandExecute(object p)
        {
            try
            {
                if (VertexTwo != null && VertexTwo != "")
                {
                   var d1 = int.Parse(VertexTwo);
                   if (VertexOne != null && VertexOne != "")
                   {
                      var d2 = int.Parse(VertexOne);
                      if (d2 < CountGraph && d1 < CountGraph && 
                            MatrixSm[int.Parse(VertexOne), int.Parse(VertexTwo)] > 0 &&
                              MatrixSm[int.Parse(VertexTwo), int.Parse(VertexOne)] > 0 )
                      return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        ///<summary>Логика выполнения - Удаление ребра</summary>
        private void OnDeleteEdgeCommandExecuted(object p)
        {
            var m = MatrixSm;
            m[int.Parse(VertexOne), int.Parse(VertexTwo)] = 0;
            m[int.Parse(VertexTwo), int.Parse(VertexOne)] = 0;
            MatrixSm = m;
            CalculateLine();
        }
        #endregion

        #region Priperty : ObservableCollection<Li> - Колекция ребер графа
        private ObservableCollection<Li> _Line = new ObservableCollection<Li>();

        /// <summary>Колекция ребер графа</summary>
        public ObservableCollection<Li> Line
        {
			get => _Line;
			set => Set(ref _Line, value, "Line");
		}
 
        #endregion
		public GraphViewModel()
        {
        }
     
        private void CreateEllipse()
        {
            EllipseP.Clear();
            int j = 0;
            foreach (var p in PointGraph)
            {
                EllipseP.Add(new Ellipses
                {
                    Width = 25,
                    Height = 25,
                    Margin = new Thickness(Width / 2 + p.X, Heigth / 2 + p.Y, 0, 0),
                    Fill = System.Windows.Media.Brushes.Black,
                    Content = j.ToString(),
                    Top = Heigth / 2 + p.Y + 25 / 2 - 10,
                    Left = Width / 2 + p.X + 25 / 2 - 4
                }) ;
                j++;
            }
        }
        private void CalculateLine()
        {
            Line.Clear();
            Point[] point = PointGraph;
            double[,] ma = MatrixSm;
            for (int i = 0; i < point.Length; i++)
            {
                for (int j = i; j < CountGraph; j++)
                {
                    if(i != j && ma[i,j]>0 && ma[i, j] == ma[j,i])
                    {
                        Line.Add(new Li
                        {
                            X1 = point[i].X + (double)Width / 2 + 25 / 2,
                            X2 = point[j].X + (double)Width / 2 + 25 / 2,
                            Y1 = point[i].Y + (double)Heigth / 2 + 25 / 2,
                            Y2 = point[j].Y + (double)Heigth / 2 + 25 / 2,
                            Stroke = System.Windows.Media.Brushes.Black,
                            StrokeThickness = 2
                        });
                    }
                }
            }
        }
        private void CalculatePoint()
        {
            Point[] point = new Point[CountGraph];
            int j = 0;
            bool fl = false;
            if (CountGraph is 0) 
            { 
                PointGraph = point;
                return;
            }
            while (!fl)
            {
                for (double i = 0; i < 360 - 1E-05; i += (double)360 / CountGraph)
                {
                    double rad = (double)i / 180 * 3.1415926535;
                    double x = radius * Math.Cos(rad);
                    double y = radius * Math.Sin(rad);
                    point[j] = new Point(x, y);
                    j++;
                    if(j is 2)
                    {
                        var distance = Math.Sqrt(Math.Pow(point[1].X - point[0].X, 2) + Math.Pow(point[1].Y - point[0].Y, 2));
                        if (distance < 10 + 50)
                        {
                            radius+=2;
                            j = 0;
                            point = new Point[CountGraph];
                            break;
                        }
                    }
                    else if (j > 2 || CountGraph < 2) fl = true;
                }
                if(j == CountGraph) fl = true;
            }
            PointGraph = point;
        }
        private void GetFileContains()
        {
            FileStream fs;
            try
            {
                fs = new FileStream("Graph\\Graph1.txt", FileMode.Open);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Отсутствует конфигурационный файл " + "\"" + "Graph1.txt" + "\"");
                return;
            }

            StreamReader sr = new StreamReader(fs);
            bool fl1 = false;
            while (!sr.EndOfStream)
            {
                string newLine = sr.ReadLine();
                if (!fl1) CountGraph = int.Parse(newLine);
                if (CountGraph != 0 && fl1 == false)
                {
                    fl1 = true;
                    continue;
                }
                double[,] matrix = new double[CountGraph, CountGraph];
                for (int i = 0; i < CountGraph && !sr.EndOfStream; i++)
                {
                    newLine = Regex.Replace(newLine, @"\s+", " ");
                    var cur = newLine.Replace("\t", "]")
                        .Replace(" ", "]")
                        .Replace('.', ',')
                        .Split(']');
                    if (!TipGraph)
                    {
                        double[] curPars = new double[CountGraph];
                        for (int j = 0; j < cur.Length; j++)
                        {
                            curPars[j] = double.Parse(cur[j]);
                        }
                        for (int v = 0; v < CountGraph; v++)
                        {
                            matrix[i, v] = curPars[v];
                        }
                    }
                    else
                    {
                        int v = int.Parse(cur[0])-1;
                        int j = int.Parse(cur[1])-1;
                        double weight = double.Parse(cur[2]);
                        matrix[v,j] = weight;
                        matrix[j, v] = weight;
                    }
                    

                    newLine = sr.ReadLine();
                }
                MatrixSm = matrix;

            }

            sr.Close();
            fs.Close();
        }
        private void GenerateMatrixView()
        {
            var array = MatrixSm;
            var rows = array.GetLength(0);
            var columns = array.GetLength(1);
            var t = new DataTable();
            for (var c = 0; c < columns; c++)
            {
                t.Columns.Add(new DataColumn(c.ToString()));
            }
            for (var r = 0; r < rows; r++)
            {
                var newRow = t.NewRow();
                for (var c = 0; c < columns; c++)
                {
                    newRow[c] = array[r, c];
                }
                t.Rows.Add(newRow);
            }
            MatrixSmView = t.DefaultView;
        }
        private void AddVertex()
        {
            var matrix = MatrixSm;
            CountGraph++;
            var newMatrix = new double[CountGraph, CountGraph];
            for (int i = 0; i < CountGraph-1; i++)
            {
                for (int j = 0; j < CountGraph-1; j++)
                {
                    newMatrix[i, j] = matrix[i, j];
                }
            }
            MatrixSm = newMatrix;
            CalculatePoint();
            CreateEllipse();
            CalculateLine();
        }
        private void DeleteVertex()
        {
            CountGraph--;
            var matrix = MatrixSm;
            var ver = int.Parse(VertexDel);
            var newMatrix = new double[CountGraph, CountGraph];
            int k = 0, c = 0;
            for (int i = 0; i < CountGraph+1; i++)
            {
                for (int j = 0; j < CountGraph+1; j++)
                {
                    if (j != ver && i != ver)
                    {
                        newMatrix[k, c] = matrix[i, j];
                        c++;
                    }
                }

                if(ver != i)  k++;
                c = 0;
                if (i+1 == ver) i++;
            }
            MatrixSm = newMatrix;
            radius = 30;
            CalculatePoint();
            CreateEllipse();
            CalculateLine();
        }

    }
}
