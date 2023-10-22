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

        #region Priperty : string[] - Имена файлов с графами
        private string[] _FileNames = new string[1];

        /// <summary>Имена файлов с графами</summary>
        public string[] FileNames
        {
            get => _FileNames;
            set => Set(ref _FileNames, value, "FileNames");
        }
        #endregion

        #region Priperty : string - Вывод на экран по нескольким функциям
        private string _Conclusion;

        /// <summary>Вывод на экран по нескольким функциям</summary>
        public string Conclusion
        {
            get => _Conclusion;
            set => Set(ref _Conclusion, value, "Conclusion");
        }
        #endregion 

        #region Priperty : string - Выбранный файл для загрузки
        private string _SelectFile;

        /// <summary>Выбранный файл для загрузки</summary>
        public string SelectFile
        {
            get => _SelectFile;
            set => Set(ref _SelectFile, value, "SelectFile");
        }
        #endregion 

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

        #region Priperty : string - Название файла
        private string _FileNameSave;

        /// <summary>Название файла</summary>
        public string FileNameSave
        {
            get => _FileNameSave;
            set => Set(ref _FileNameSave, value, "FileNameSave");
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

        #region Priperty : string - Первая вершина для проверки на смежность / показать вес ребра
        private string _VertexOneSm;

        /// <summary>Первая вершина для проверки на смежность / показать вес ребра</summary>
        public string VertexOneSm
        {
            get => _VertexOneSm;
            set => Set(ref _VertexOneSm, value, "VertexOneSm");
        }
        #endregion

        #region Priperty : string - Вторая вершина проверки на смежность / показать вес ребра
        private string _VertexTwoSm;

        /// <summary>Вторая вершина для проверки на смежность / показать вес ребра</summary>
        public string VertexTwoSm
        {
            get => _VertexTwoSm;
            set => Set(ref _VertexTwoSm, value, "VertexTwoSm");
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

        #region Command KolVertexCommand - Количество рершин в графе

        ///<summary>Количество рершин в графе</summary>
        private LambdaCommand? _KolVertexCommand;

        ///<summary>Количество рершин в графе</summary>
        public ICommand KolVertexCommand => _KolVertexCommand
                    ??= new(OnKolVertexCommandExecuted, CanKolVertexCommandExecute);

        ///<summary>Проверка возможности выполнения - Количество рершин в графе</summary>
        private bool CanKolVertexCommandExecute(object p) => true;

        ///<summary>Логика выполнения - Количество рершин в графе</summary>
        private void OnKolVertexCommandExecuted(object p)
        {
            Conclusion = CountGraph.ToString();
        }
        #endregion

        #region Command KolEdgeCommand - Количество ребер в графе

        ///<summary>Количество ребер в графе</summary>
        private LambdaCommand? _KolEdgeCommand;

        ///<summary>Количество ребер в графе</summary>
        public ICommand KolEdgeCommand => _KolEdgeCommand
                    ??= new(OnKolEdgeCommandExecuted, CanKolEdgeCommandExecute);

        ///<summary>Проверка возможности выполнения - Количество ребер в графе</summary>
        private bool CanKolEdgeCommandExecute(object p) => true;

        ///<summary>Логика выполнения - Количество ребер в графе</summary>
        private void OnKolEdgeCommandExecuted(object p)
        {
            Conclusion = Line.ToList().Count.ToString();
        }
        #endregion

        #region Command SmezCommand - Проверка на смежность 2х вершин

        ///<summary>Проверка на смежность 2х вершин</summary>
        private LambdaCommand? _SmezCommand;

        ///<summary>Проверка на смежность 2х вершин</summary>
        public ICommand SmezCommand => _SmezCommand
                    ??= new(OnSmezCommandExecuted, CanSmezCommandExecute);

        ///<summary>Проверка возможности выполнения - Проверка на смежность 2х вершин</summary>
        private bool CanSmezCommandExecute(object p)
        {
            try
            {
                var v1 = int.Parse(VertexOneSm);
                var v2 = int.Parse(VertexTwoSm);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        ///<summary>Логика выполнения - Проверка на смежность 2х вершин</summary>
        private void OnSmezCommandExecuted(object p)
        {
            try
            {
                var v1 =int.Parse(VertexOneSm);
                var v2 = int.Parse(VertexTwoSm);
                if (MatrixSm[v1, v2] > 0) Conclusion = "Trye";
                else Conclusion = "False";
            }
            catch (Exception)
            {
                Conclusion = "False";
            }
            
        }
        #endregion

        #region Command WaitEdgeCommand - Вес ребра заданного 2я вершинами

        ///<summary>Вес ребра заданного 2я вершинами</summary>
        private LambdaCommand? _WaitEdgeCommand;

        ///<summary>Вес ребра заданного 2я вершинами</summary>
        public ICommand WaitEdgeCommand => _WaitEdgeCommand
                    ??= new(OnWaitEdgeCommandExecuted, CanWaitEdgeCommandExecute);

        ///<summary>Проверка возможности выполнения - Вес ребра заданного 2я вершинами</summary>
        private bool CanWaitEdgeCommandExecute(object p)
        {
            try
            {
                var v1 = int.Parse(VertexOneSm);
                var v2 = int.Parse(VertexTwoSm);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        ///<summary>Логика выполнения - Вес ребра заданного 2я вершинами</summary>
        private void OnWaitEdgeCommandExecuted(object p)
        {
            try
            {
                var v1 = int.Parse(VertexOneSm);
                var v2 = int.Parse(VertexTwoSm);
                Conclusion = MatrixSm[v1, v2].ToString();
            }
            catch (Exception)
            {
                Conclusion = "0";
            }
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

        #region Command SaveToFileCommand - Сохранение в файл графа

        ///<summary>Сохранение в файл графа</summary>
        private LambdaCommand? _SaveToFileCommand;

        ///<summary>Сохранение в файл графа</summary>
        public ICommand SaveToFileCommand => _SaveToFileCommand
                    ??= new(OnSaveToFileCommandExecuted, CanSaveToFileCommandExecute);

        ///<summary>Проверка возможности выполнения - Сохранение в файл графа</summary>
        private bool CanSaveToFileCommandExecute(object p) => true;

        ///<summary>Логика выполнения - Сохранение в файл графа</summary>
        private void OnSaveToFileCommandExecuted(object p)
        {
            SaveToFile();
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
            LoadFileName();
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
                fs = new FileStream("Graph\\"+ SelectFile + ".txt", FileMode.Open);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Отсутствует файл " + "\"" + SelectFile + ".txt" + "\"");
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
                for (int i = 0; ; i++)
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

                    if (!sr.EndOfStream)
                        newLine = sr.ReadLine();
                    else break;
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
        private void LoadFileName()
        {
            var FileName = Directory.GetFiles("Graph");
            for (int i = 0; i < FileName.Length; i++)
            {
                FileName[i] = FileName[i].Split('\\')[1];
                FileName[i] = FileName[i].Split('.')[0];
            }
            FileNames = FileName;
        }
        private void SaveToFile()
        {
            var fileStream = File.Create("Graph\\" + FileNameSave + ".txt");
            fileStream.Close();
            using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine("Graph\\" + FileNameSave + ".txt")))
            {
                outputFile.WriteLine(CountGraph.ToString());
                if(!TipGraph)
                    for (int i = 0; i < CountGraph; i++)
                    {
                        for (int j = 0; j < CountGraph; j++)
                        {
                            outputFile.Write(MatrixSm[i,j].ToString());
                            if (j != CountGraph - 1) outputFile.Write(" ");
                        }
                        if(i != CountGraph - 1) outputFile.WriteLine("");
                    }
                else
                {
                    for (int i = 0; i < CountGraph; i++)
                    {
                        for (int j = i; j < CountGraph; j++)
                        {
                            if(MatrixSm[i, j] > 0)
                            {
                                outputFile.Write(i.ToString());
                                outputFile.Write(" ");
                                outputFile.Write(j.ToString());
                                outputFile.Write(" ");
                                outputFile.Write(MatrixSm[i, j].ToString());
                                outputFile.WriteLine("");
                            }
                        }
                    }
                }
            }
            LoadFileName();
        }
    }
}
