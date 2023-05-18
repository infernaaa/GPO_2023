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

using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Model_data
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly ScottPlot.Plottable.MarkerPlot HighlightedPoint;
        private int LastHighlightedIndex = -1;
        
        private readonly ScottPlot.Plottable.ScatterPlot MyScatterPlot;

        

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

        public MainWindow()
        {
            
            InitializeComponent();
            
            TextBox_File_Name.Text = "X_Data";
            TextBox_Number_Detector.Text = "detector_0";
            TextBox_Number_Str.Text = "0";
            //без обработки
            double[] dataY_old = new double[] { 37.094, -55.426, 61.188, -59.599, 55.714, -43.451, 43.818, 47.329, -54.553, 51.536, -39.598, 36.661, -35.327, 33.466, 42.332, -48.332, 42.708, -37.523, 33.466, -32.0, 29.394, 30.463, 36.222, -41.569, 34.871, -32.985, 29.394, -35.777, 28.844, -29.394 };

        double[] dataX_old = new double[] { 26.0, 26.1, 26.3, 26.4, 26.6, 26.7, 29.1, 29.2, 29.3, 29.5, 29.7, 30.5, 30.6, 32.2, 32.3, 32.5, 32.6, 32.7, 33.5, 33.7, 34.8, 35.2, 35.3, 35.5, 35.7, 38.3, 38.4, 38.6, 38.7, 41.5 };

            MyScatterPlot = wpf_plot.Plot.AddScatter(dataX_old, dataY_old,label: "необработанные");// create a scatter plot from some random data and save it
            double[] dataY = new double[] {  37.947, -54.259, 61.709, -59.867, 55.136, -46.648, 39.598, 47.666, -54.845, 52.46,
           -38.781, 31.496, 27.129, -35.327, 34.409, 42.332, -48.99, 44.181, -30.463, -33.466,
           32.0, 30.984, -29.394, 36.222, -41.183, 34.871, -27.713, 29.933, -34.871, -26.533,
           -28.284};
            double[] dataX = new double[] {25.8, 26.1, 26.2, 26.3, 26.5, 26.6, 27.1, 29.1, 29.3, 29.4, 29.5, 29.7, 30.4, 30.5, 30.7, 32.2, 32.3, 32.5, 32.6, 33.5, 33.7, 34.8, 34.9, 35.2, 35.3, 35.5, 36.1, 38.3, 38.4, 39.1, 41.5};
            MyScatterPlot = wpf_plot.Plot.AddScatter(dataX, dataY,label:"обработанные");
            wpf_plot.Plot.Legend();



            // Add a red circle we can move around later as a highlighted point indicator
            HighlightedPoint = wpf_plot.Plot.AddPoint(0, 0);
            HighlightedPoint.Color = System.Drawing.Color.Red;
            HighlightedPoint.MarkerSize = 10;
            HighlightedPoint.MarkerShape = ScottPlot.MarkerShape.openCircle;
            HighlightedPoint.IsVisible = false;
            wpf_plot.MouseMove += formsPlot1_MouseMove;
            wpf_plot.RightClicked += DeployCustomMenu;
            wpf_plot.Refresh();




        }
    }
}
