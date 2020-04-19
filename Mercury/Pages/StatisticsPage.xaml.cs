using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using API;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Mercury.Pages
{
	/// <summary>
	/// Shows data from different sessions, each in their own graph
	/// </summary>
	public sealed partial class StatisticsPage : Page
	{
		private List<SavedSession> _selectedSessions;
		public ObservableCollection<AudienceFrame> FaceData { get; }

		public StatisticsPage()
		{
			this.InitializeComponent();
			FaceData = new ObservableCollection<AudienceFrame>();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			_selectedSessions = e.Parameter as List<SavedSession>;
			PopulateSessionList();
			PopulateGraphs();
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			DataContext = this;
		}

		private void Button_Back(object sender, RoutedEventArgs e)
		{
			Frame.GoBack();
		}

		private void PopulateSessionList()
		{
			foreach (var session in _selectedSessions)
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
