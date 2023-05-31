using ScottPlot;
using ScottPlot.Plottable;
using ScottPlot.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;

using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace Model_data
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private  ScottPlot.Plottable.MarkerPlot HighlightedPoint;
        private int LastHighlightedIndex = -1;
        private string temp = "123";
        private  ScottPlot.Plottable.ScatterPlot MyScatterPlot;



        private void ClearPlot(object sender, RoutedEventArgs e)
        {
            wpf_plot.Plot.Clear();
            wpf_plot.Plot.AxisAuto();
        }

        private void DeployCustomMenu(object sender, EventArgs e)
        {


            MenuItem clearPlotMenuItem = new MenuItem() { Header = "Clear Plot" };
            clearPlotMenuItem.Click += ClearPlot;

            ContextMenu rightClickMenu = new ContextMenu();

            rightClickMenu.Items.Add(clearPlotMenuItem);

            rightClickMenu.IsOpen = true;
        }
        private void formsPlot1_MouseMove(object sender, MouseEventArgs e)
        {

            // determine point nearest the cursor
            (double mouseCoordX, double mouseCoordY) = wpf_plot.GetMouseCoordinates();
            double xyRatio = wpf_plot.Plot.XAxis.Dims.PxPerUnit / wpf_plot.Plot.YAxis.Dims.PxPerUnit;
            (double pointX, double pointY, int pointIndex) = MyScatterPlot.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);

            // place the highlight over the point of interest
            HighlightedPoint.X = pointX;
            HighlightedPoint.Y = pointY;
            HighlightedPoint.IsVisible = true;

            // render if the highlighted point chnaged
            if (LastHighlightedIndex != pointIndex)
            {
                LastHighlightedIndex = pointIndex;
                wpf_plot.Render();
            }

            // update the GUI to describe the highlighted point
            //TextBox_File_Name.Text =pointIndex.ToString();
            Title = $"X:{pointX.ToString()} Y:{pointY.ToString()}";
            //Console.WriteLine($"Point index {pointIndex} at ({pointX:N2}, {pointY:N2})");
        }
        private string _myVariable;

        public string MyVariable
        {
            get { return _myVariable; }
            set
            {
                _myVariable = value;
                OnPropertyChanged(nameof(MyVariable));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void Draw( string TextBox_Number_Detector, string TextBox_Number_Str, string TextBox_File_Name)
        {
            String scriptFileName = "preprocess_data.py";

            // Проверка, сохранился ли файл
            if (!File.Exists(scriptFileName))
            {
                MessageBox.Show("Отсутствует файл скрипта обработки данных");
                return;
            }

            MessageBox.Show("Запуск алгоритма обработки");

            // Запуск алгоритма обработки в python
            try
            {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "CMD.exe";
                startInfo.Arguments = "/C python " + scriptFileName +
                                                    " " + TextBox_File_Name +
                                                    " " + TextBox_Number_Detector +
                                                    " " + TextBox_Number_Str;
                process.StartInfo = startInfo;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                
                process.WaitForExit();
                MessageBox.Show(process.StandardOutput.ReadToEnd());

            }
            catch (Exception e_i)
            {
                MessageBox.Show(e_i.Message);
            }
            
            /*Console.WriteLine("дошли до конца функции");
            double[] dataX = new double[32];
            double[] dataY = new double[32];
            string input = temp;
            // Удаление лишних символов
            input = input.Replace("[", "").Replace("]", "").Replace(" ", "");

            // Разделение строки на массив строк
            string[] numberStrings = input.Split(',');

            // Создание массива для хранения чисел с плавающей точкой
            double[] numbers = new double[numberStrings.Length];

            // Конвертация строковых значений в числа с плавающей точкой
            for (int i = 0; i < numberStrings.Length; i++)
            {
                numberStrings[i] = numberStrings[i].Replace(".", ",");
                numbers[i] = double.Parse(numberStrings[i]);
            }
            for (int i = 0; i < 32; i++)
            {
                dataX[i] = numbers[i];
                dataY[i] = numbers[i + 32];
            }
            // костыль который чинит рисовку последнего элемента графика
            (dataX[0], dataX[31]) = (dataX[31], dataX[0]);
            (dataY[0], dataY[31]) = (dataY[31], dataY[0]);
            MyScatterPlot = wpf_plot.Plot.AddScatter(dataX, dataY, label: "обработанные");
            wpf_plot.Plot.Legend();



            // Add a red circle we can move around later as a highlighted point indicator
            HighlightedPoint = wpf_plot.Plot.AddPoint(0, 0);
            HighlightedPoint.Color = System.Drawing.Color.Red;
            HighlightedPoint.MarkerSize = 10;
            HighlightedPoint.MarkerShape = ScottPlot.MarkerShape.openCircle;
            HighlightedPoint.IsVisible = false;
            wpf_plot.MouseMove += formsPlot1_MouseMove;
            wpf_plot.RightClicked += DeployCustomMenu;
            wpf_plot.Refresh();*/
            return;
        }
        
        private void Button_Click(object sender, RoutedEventArgs e) {
            Draw(TextBox_File_Name.Text, TextBox_Number_Detector.Text, TextBox_Number_Str.Text);
            
        }
        
        public void Risovaka(string st)
        {
            
        }
        
        public MainWindow()
        {

            InitializeComponent();
            /*
            
            */


        }
    }
}
