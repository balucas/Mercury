using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Ailon.QuickCharts;

namespace Mercury
{
	static class GraphingTools
	{
		public static void UpdateGraph(ComboBox comboBox, SerialChart chart)
		{
			foreach (var graph in chart.Graphs)
			{
				graph.Visibility = Visibility.Collapsed;
			}

			chart.Graphs[(int)comboBox.SelectedIndex].Visibility = Visibility.Visible;
		}
	}
}
