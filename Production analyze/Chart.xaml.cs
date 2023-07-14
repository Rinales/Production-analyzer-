using System.Windows;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using OxyPlot.Wpf;
using System.Collections.Generic;
using System.Linq;
using static OfficeOpenXml.ExcelErrorValue;
using System.Windows.Markup;

namespace Production_analyze
{
    /// <summary>
    /// Interakční logika pro Menu.xaml
    /// </summary>
    public partial class Chart : Window
    {
        PlotView LineChart;
        PlotView BarChart; 
        private CategoryAxis categoryAxis;
        private BarSeries barSeries;


        public Chart(PlotView lineChart, PlotView barChart)
        {
            LineChart = lineChart;
            BarChart = barChart;
        }

        public void setUpChart(List<DataPoint> dataPoinsList)
        {
            var plotModel = new PlotModel();
            var lineSeries = new LineSeries();
            var scatterSeries = new ScatterSeries();

            if (dataPoinsList != null)
            {
                var dataPoints = dataPoinsList;
            }
            /* lineSeries.Points.Add(new DataPoint(0, 0));
             lineSeries.Points.Add(new DataPoint(1, 1));
             lineSeries.Points.Add(new DataPoint(2, 2));
             plotModel.Series.Add(lineSeries);*/


            lineSeries.Points.Add(new DataPoint(0, 0));
            lineSeries.Points.Add(new DataPoint(1, 1));
            lineSeries.Points.Add(new DataPoint(2, 2));

            scatterSeries.Points.Add(new ScatterPoint(1, 10));
            scatterSeries.Points.Add(new ScatterPoint(2, 20));
            scatterSeries.Points.Add(new ScatterPoint(3, 15));

            plotModel.Series.Add(lineSeries);
            plotModel.Series.Add(scatterSeries);

            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left });
            plotModel.Title = "Výroba";

            LineChart.Model = plotModel;
        }

        public void setUpBarChart()
        {
            var plotModel = new PlotModel();
            barSeries = new BarSeries();

            categoryAxis = new CategoryAxis();
            categoryAxis.Position = AxisPosition.Left;

            plotModel.Series.Add(barSeries);
            plotModel.Axes.Add(categoryAxis);

            BarChart.Model = plotModel;
        }

        public void barChartDataLoad(List<string> Reasons, List<double> Values)
        {
            for (int i = 0; i < Reasons.Count; i++)
            {
                categoryAxis.Labels.Add(Reasons[i]);
                barSeries.Items.Add(new BarItem { Value = Values[i] });
                System.Console.WriteLine("" + i);
            }
            BarChart.InvalidatePlot(true);
        }

        public void lineChartDataLoad(List<string> Causes, List<double> Values, string stroj)
        {
            var plotModel = LineChart.Model;
            plotModel.Series.Clear();
            var lineSeries = new LineSeries();
            plotModel.Title = stroj;
            for (int i = 0; i < Causes.Count; i++)
            {
              //  lineSeries.Points.Add(new DataPoint(Causes[i], Values[i]));
            }


            for (int i = 0; i < Causes.Count; i++)
            {
                categoryAxis.Labels.Add(Causes[i]);
                barSeries.Items.Add(new BarItem { Value = Values[i] });
                System.Console.WriteLine("" + i);
            }
            LineChart.InvalidatePlot(true);

        }



    }
}
