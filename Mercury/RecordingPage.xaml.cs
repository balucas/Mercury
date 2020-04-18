using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using API;
using System.Diagnostics;
using Windows.Devices.PointOfService;
using Windows.UI.WindowManagement;
using Ailon.QuickCharts;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Mercury
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>

	enum RecordingStatus
	{
		NotStarted,
		Started2,
		Finished
	}

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

	public sealed partial class RecordingPage : Page
	{

		private DispatcherTimer _timer;
		private DispatcherTimer _clock;
		private DateTime _startDate;
		private string _time;
		private RecordingStatus _status;
		public Session Session;
		public MediaControls MediaControls;
		public List<SavedSession> SessionList;
		public ObservableCollection<AudienceFrame> FaceData { get; }

		public RecordingPage()
		{
			this.InitializeComponent();

			_timer = new DispatcherTimer();
			_clock = new DispatcherTimer();
			MediaControls = new MediaControls();
			_status = RecordingStatus.NotStarted;
			Session = new Session();
			
			FaceData = new ObservableCollection<AudienceFrame>();

			InitializeGraphs();

			Application.Current.Resuming += Application_Resuming;
		}

		private void InitializeGraphs()
		{
			PopulateComboBox(GraphOneSelection);
			PopulateComboBox(GraphTwoSelection);
			PopulateComboBox(GraphThreeSelection);
			PopulateComboBox(GraphFourSelection);

			GraphOneSelection.SelectedIndex = 0;
			GraphTwoSelection.SelectedIndex = 1;
			GraphThreeSelection.SelectedIndex = 2;
			GraphFourSelection.SelectedIndex = 3;
		}

		private async void Page_Loaded(object sender, RoutedEventArgs e)
		{
			DataContext = this;
			_timer.Interval = new TimeSpan(0, 0, 5);
			_timer.Tick += UpdateRealTimeDataAsync;
			_clock.Tick += UpdateClock;
			await StartVideoAsync();
		}

		private async void UpdateRealTimeDataAsync(object sender, object e)
		{
			FaceData.Add(await Session.CreateChartItem(await TakePhoto(), _time));
		}

		private void UpdateClock(object sender, object e)
		{
			var duration = DateTime.Now - _startDate;
			_time = $"{duration.Hours}:{duration.Minutes}:{duration.Seconds}";
			RecordingDuration.Text = $"Duration: {_time}";
		}

		private void Button_Toggle_Pane(object sender, RoutedEventArgs e)
		{
			TogglePaneButton.Content = MainSplitView.IsPaneOpen ? "Hide Graphs" : "Show Graphs";
			MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
		}

		private void Button_Toggle_Recording(object sender, RoutedEventArgs e)
		{
			switch (_status)
			{
				case RecordingStatus.NotStarted:
					RecordingButton.Content = "Stop Recording";
					_status = RecordingStatus.Started2;
					_timer.Start();
					_clock.Start();
					_startDate = DateTime.Now;
					break;
				case RecordingStatus.Started2:
					RecordingButton.Content = "Recording Over";
					_status = RecordingStatus.Finished;
					_timer.Stop();
					_clock.Stop();
					SaveSession();
					break;
			}
		}

		//Save current session
		private void SaveSession()
		{
			//Session Storage
			StorageFolder testfolder = ApplicationData.Current.LocalFolder;
			Debug.WriteLine("Local storage: " + testfolder.Path);
			Session.SaveSession(testfolder.Path);
		}
		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(MainMenu), SessionList);
		}

		private async void Application_Resuming(object sender, object o)
		{
			await StartVideoAsync();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			SessionList = e.Parameter as List<SavedSession>;
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			StopVideo();
			_timer.Stop();
			_clock.Stop();
		}

		private async Task StartVideoAsync()
		{
			PreviewControl.Source = await MediaControls.InitializeCamera();
			MediaControls.StartRecording();
		}

		private void StopVideo()
		{
			MediaControls.StopRecording();
		}

		private async Task<byte[]> TakePhoto()
		{
			//ImagePreview.Source = await MediaControls.CaptureImageToBitMap();
			return await MediaControls.CaptureImageToByteArray();
		}

		private void PopulateComboBox(ComboBox comboBox)
		{
			foreach (var value in Enum.GetValues(typeof(GraphData)))
			{
				comboBox.Items?.Add(value);
			}
		}

		private void UpdateGraphOneSelection(object sender, SelectionChangedEventArgs e)
		{
			var comboBox = sender as ComboBox;

			G1_Anger.Visibility = Visibility.Collapsed;
			G1_Contempt.Visibility = Visibility.Collapsed;
			G1_Disgust.Visibility = Visibility.Collapsed;
			G1_Fear.Visibility = Visibility.Collapsed;
			G1_Happiness.Visibility = Visibility.Collapsed;
			G1_Neutral.Visibility = Visibility.Collapsed;
			G1_Sadness.Visibility = Visibility.Collapsed;
			G1_Surprise.Visibility = Visibility.Collapsed;


			switch (comboBox.SelectedItem)
			{
				case GraphData.Anger:
					G1_Anger.Visibility = Visibility.Visible;
					break;
				case GraphData.Contempt:
					G1_Contempt.Visibility = Visibility.Visible;
					break;
				case GraphData.Disgust:
					G1_Disgust.Visibility = Visibility.Visible;
					break;
				case GraphData.Fear:
					G1_Fear.Visibility = Visibility.Visible;
					break;
				case GraphData.Happiness:
					G1_Happiness.Visibility = Visibility.Visible;
					break;
				case GraphData.Neutral:
					G1_Neutral.Visibility = Visibility.Visible;
					break;
				case GraphData.Sadness:
					G1_Sadness.Visibility = Visibility.Visible;
					break;
				case GraphData.Surprise:
					G1_Surprise.Visibility = Visibility.Visible;
					break;
				default:
					G1_Anger.Visibility = Visibility.Visible;
					break;
			}
		}

		private void UpdateGraphTwoSelection(object sender, SelectionChangedEventArgs e)
		{
			var comboBox = sender as ComboBox;

			G2_Anger.Visibility = Visibility.Collapsed;
			G2_Contempt.Visibility = Visibility.Collapsed;
			G2_Disgust.Visibility = Visibility.Collapsed;
			G2_Fear.Visibility = Visibility.Collapsed;
			G2_Happiness.Visibility = Visibility.Collapsed;
			G2_Neutral.Visibility = Visibility.Collapsed;
			G2_Sadness.Visibility = Visibility.Collapsed;
			G2_Surprise.Visibility = Visibility.Collapsed;


			switch (comboBox.SelectedItem)
			{
				case GraphData.Anger:
					G2_Anger.Visibility = Visibility.Visible;
					break;
				case GraphData.Contempt:
					G2_Contempt.Visibility = Visibility.Visible;
					break;
				case GraphData.Disgust:
					G2_Disgust.Visibility = Visibility.Visible;
					break;
				case GraphData.Fear:
					G2_Fear.Visibility = Visibility.Visible;
					break;
				case GraphData.Happiness:
					G2_Happiness.Visibility = Visibility.Visible;
					break;
				case GraphData.Neutral:
					G2_Neutral.Visibility = Visibility.Visible;
					break;
				case GraphData.Sadness:
					G2_Sadness.Visibility = Visibility.Visible;
					break;
				case GraphData.Surprise:
					G2_Surprise.Visibility = Visibility.Visible;
					break;
				default:
					G2_Anger.Visibility = Visibility.Visible;
					break;
			}
		}

		private void UpdateGraphThreeSelection(object sender, SelectionChangedEventArgs e)
		{
			var comboBox = sender as ComboBox;

			G3_Anger.Visibility = Visibility.Collapsed;
			G3_Contempt.Visibility = Visibility.Collapsed;
			G3_Disgust.Visibility = Visibility.Collapsed;
			G3_Fear.Visibility = Visibility.Collapsed;
			G3_Happiness.Visibility = Visibility.Collapsed;
			G3_Neutral.Visibility = Visibility.Collapsed;
			G3_Sadness.Visibility = Visibility.Collapsed;
			G3_Surprise.Visibility = Visibility.Collapsed;


			switch (comboBox.SelectedItem)
			{
				case GraphData.Anger:
					G3_Anger.Visibility = Visibility.Visible;
					break;
				case GraphData.Contempt:
					G3_Contempt.Visibility = Visibility.Visible;
					break;
				case GraphData.Disgust:
					G3_Disgust.Visibility = Visibility.Visible;
					break;
				case GraphData.Fear:
					G3_Fear.Visibility = Visibility.Visible;
					break;
				case GraphData.Happiness:
					G3_Happiness.Visibility = Visibility.Visible;
					break;
				case GraphData.Neutral:
					G3_Neutral.Visibility = Visibility.Visible;
					break;
				case GraphData.Sadness:
					G3_Sadness.Visibility = Visibility.Visible;
					break;
				case GraphData.Surprise:
					G3_Surprise.Visibility = Visibility.Visible;
					break;
				default:
					G3_Anger.Visibility = Visibility.Visible;
					break;
			}
		}

		private void UpdateGraphFourSelection(object sender, SelectionChangedEventArgs e)
		{
			var comboBox = sender as ComboBox;

			G4_Anger.Visibility = Visibility.Collapsed;
			G4_Contempt.Visibility = Visibility.Collapsed;
			G4_Disgust.Visibility = Visibility.Collapsed;
			G4_Fear.Visibility = Visibility.Collapsed;
			G4_Happiness.Visibility = Visibility.Collapsed;
			G4_Neutral.Visibility = Visibility.Collapsed;
			G4_Sadness.Visibility = Visibility.Collapsed;
			G4_Surprise.Visibility = Visibility.Collapsed;


			switch (comboBox.SelectedItem)
			{
				case GraphData.Anger:
					G4_Anger.Visibility = Visibility.Visible;
					break;
				case GraphData.Contempt:
					G4_Contempt.Visibility = Visibility.Visible;
					break;
				case GraphData.Disgust:
					G4_Disgust.Visibility = Visibility.Visible;
					break;
				case GraphData.Fear:
					G4_Fear.Visibility = Visibility.Visible;
					break;
				case GraphData.Happiness:
					G4_Happiness.Visibility = Visibility.Visible;
					break;
				case GraphData.Neutral:
					G4_Neutral.Visibility = Visibility.Visible;
					break;
				case GraphData.Sadness:
					G4_Sadness.Visibility = Visibility.Visible;
					break;
				case GraphData.Surprise:
					G4_Surprise.Visibility = Visibility.Visible;
					break;
				default:
					G3_Anger.Visibility = Visibility.Visible;
					break;
			}
		}



		//public void AdjustGraphOne(string title)
		//{
		//	GraphOne = new LineGraph
		//	{
		//		Title = title,
		//		ValueMemberPath = title
		//	};
		//}
		//public void AdjustGraphTwo(string title)
		//{
		//	GraphTwo = new LineGraph
		//	{
		//		Title = title,
		//		ValueMemberPath = title
		//	};
		//}
		//public void AdjustGraphThree(string title)
		//{
		//	GraphThree = new LineGraph
		//	{
		//		Title = title,
		//		ValueMemberPath = title
		//	};
		//}
		//public void AdjustGraphFour(string title)
		//{
		//	GraphFour = new LineGraph
		//	{
		//		Title = title,
		//		ValueMemberPath = title
		//	};
		//}
	}
}