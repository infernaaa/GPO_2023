using ScottPlot;
using ScottPlot.Plottable;
using ScottPlot.WPF;
using System;
using System.Collections.Generic;
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
            TextBox_Number_Detector.Text = pointX.ToString();
            TextBox_Number_Str.Text = pointY.ToString();
            //Console.WriteLine($"Point index {pointIndex} at ({pointX:N2}, {pointY:N2})");
        }

        public MainWindow()
        {
            InitializeComponent();


            // create a scatter plot from some random data and save it
            double[] dataX = new double[] { 1, 2, 3, 4, 5 };
            double[] dataY = new double[] { 1, 4, 9, 16, 25 };
            MyScatterPlot = wpf_plot.Plot.AddScatter(dataX, dataY);



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
