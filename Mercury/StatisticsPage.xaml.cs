using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using API;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Mercury
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class StatisticsPage : Page
	{
		public List<SavedSession> SelectedSessions;
		public ObservableCollection<AudienceFrame> FaceData { get; }

		public StatisticsPage()
		{
			this.InitializeComponent();
			FaceData = new ObservableCollection<AudienceFrame>();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			SelectedSessions = e.Parameter as List<SavedSession>;
			PopulateSessionList();
			PopulateGraphs();
		}

		private async void Page_Loaded(object sender, RoutedEventArgs e)
		{
			DataContext = this;
		}

		private void Button_Back(object sender, RoutedEventArgs e)
		{
			Frame.GoBack();
		}

		private void PopulateSessionList()
		{
			foreach (var session in SelectedSessions)
			{
				SessionListView.Items?.Add(session);
			}

			SessionListView.SelectedIndex = 0;
		}

		private void SessionListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			PopulateGraphs();
		}

		private void PopulateGraphs()
		{
			FaceData.Clear();
			var audienceFrames = (SessionListView.SelectedItem as SavedSession)?.SessionData;
			if (audienceFrames != null)
			{
				Debug.WriteLine(audienceFrames.Count);
				foreach (var frame in audienceFrames)
				{
					FaceData.Add(frame);
				}
			}
		}
	}
}
