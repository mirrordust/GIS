using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.DataVisualization.Charting;

namespace GIS
{
    /// <summary>
    /// ChartWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BarChartWindow : Window
    {
        public BarChartWindow()
        {
            InitializeComponent();
        }

        public void InitializeChart(List<KeyValuePair<string, long>> input)
        {
            ((ColumnSeries)mcChart.Series[0]).ItemsSource = input.ToArray();
        }
    }
}
