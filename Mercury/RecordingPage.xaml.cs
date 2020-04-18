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

	// 
	enum RecordingStatus
	{
		NotStarted,
		Started,
		Finished
	}

	// Enum used for Creating ComboBox Items
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
		//public List<SavedSession> SavedSessions;
		
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
			ToggleRecording();
		}

		private void ToggleRecording()
		{
			switch (_status)
			{
				case RecordingStatus.NotStarted:
					RecordingButton.Content = "Stop Recording";
					_status = RecordingStatus.Started;
					_timer.Start();
					_clock.Start();
					_startDate = DateTime.Now;
					break;
				case RecordingStatus.Started:
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

		// Return to main menu
		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			if(_status == RecordingStatus.Started)
				ToggleRecording();
			Frame.Navigate(typeof(MainMenu));
		}

		// Ensure Video control updates properly if lost focus
		private async void Application_Resuming(object sender, object o)
		{
			await StartVideoAsync();
		}

		// Ensures the video control stops correctly
		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			MediaControls.StopRecording();
			_timer.Stop();
			_clock.Stop();
		}

		// Initializes and begins the video capture
		private async Task StartVideoAsync()
		{
			PreviewControl.Source = await MediaControls.InitializeCamera();
			MediaControls.StartRecording();
		}

		// Captures a frame from video and converts it into a byte array
		private async Task<byte[]> TakePhoto()
		{
			//ImagePreview.Source = await MediaControls.CaptureImageToBitMap();
			return await MediaControls.CaptureImageToByteArray();
		}

		// Fills a combobox with items
		private void PopulateComboBox(ComboBox comboBox)
		{
			foreach (var value in Enum.GetValues(typeof(GraphData)))
			{
				comboBox.Items?.Add(value);
			}
		}

		// Adjusts the graphs in the page to the data selected in the combo boxes
		private void UpdateGraphOneSelection(object sender, SelectionChangedEventArgs e)
		{
			ComboBox comboBox = sender as ComboBox;
			UpdateGraphSelection(comboBox, Chart1);
		}
		private void UpdateGraphTwoSelection(object sender, SelectionChangedEventArgs e)
		{
			ComboBox comboBox = sender as ComboBox;
			UpdateGraphSelection(comboBox, Chart2);
		}
		private void UpdateGraphThreeSelection(object sender, SelectionChangedEventArgs e)
		{
			ComboBox comboBox = sender as ComboBox;
			UpdateGraphSelection(comboBox, Chart3);
		}
		private void UpdateGraphFourSelection(object sender, SelectionChangedEventArgs e)
		{
			ComboBox comboBox = sender as ComboBox;
			UpdateGraphSelection(comboBox, Chart4);
		}

		private void UpdateGraphSelection(ComboBox comboBox, SerialChart chart)
		{
			GraphingTools.UpdateGraph(comboBox, chart);
		}
	}
}