using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Ailon.QuickCharts;

namespace Mercury.Classes
{
	enum GraphData
	{
		Anger,
		Contempt,
		Disgust,
		Fear,
		Happiness,
		Neutral,
		Sadness,
		Surprise
	}

	static class GraphingTools
	{
		// Fills a combobox with items
		public static void PopulateComboBox(ComboBox comboBox)
		{
			foreach (var value in Enum.GetValues(typeof(GraphData)))
			{
				comboBox.Items?.Add(value);
			}
		}

		// Make all graphs invisible, then only show the chosen data
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
