using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.DataVisualization.Charting;

namespace GIS
{
    /// <summary>
    /// PieChartWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PieChartWindow : Window
    {
        public PieChartWindow()
        {
            InitializeComponent();
        }

        public void InitializeChart(List<KeyValuePair<string, long>> input)
        {
            ((PieSeries)mcChart.Series[0]).ItemsSource = input.ToArray();
        }

    }
}
