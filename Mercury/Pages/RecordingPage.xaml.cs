using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using API;
using Mercury.Classes;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Mercury.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>

	// 
	internal enum RecordingStatus
	{
		NotStarted,
		Started,
		Finished
	}

	// Enum used for Creating ComboBox Items
	

	public sealed partial class RecordingPage : Page
	{
		private readonly DispatcherTimer _timer;
		private readonly DispatcherTimer _clock;
		private readonly Session _session;
		private readonly MediaControls _mediaControls;

		public ObservableCollection<AudienceFrame> FaceData { get; }

		private DateTime _startDate;
		private string _time;
		private RecordingStatus _status;

		public RecordingPage()
		{
			this.InitializeComponent();

			_timer = new DispatcherTimer();
			_clock = new DispatcherTimer();
			_timer.Interval = new TimeSpan(0, 0, 5);
			_timer.Tick += UpdateRealTimeDataAsync;
			_clock.Tick += UpdateClock;
			_mediaControls = new MediaControls();
			_status = RecordingStatus.NotStarted;
			_session = new Session();
			FaceData = new ObservableCollection<AudienceFrame>();
			InitializeGraphs();
		}

		private void InitializeGraphs()
		{
			GraphingTools.PopulateComboBox(GraphOneSelection);
			GraphingTools.PopulateComboBox(GraphTwoSelection);
			GraphingTools.PopulateComboBox(GraphThreeSelection);
			GraphingTools.PopulateComboBox(GraphFourSelection);

			GraphOneSelection.SelectedIndex = 0;
			GraphTwoSelection.SelectedIndex = 1;
			GraphThreeSelection.SelectedIndex = 2;
			GraphFourSelection.SelectedIndex = 3;
		}

		// ensures the observable collections are being referenced and begins video preview
		private async void Page_Loaded(object sender, RoutedEventArgs e)
		{
			DataContext = this;
			await StartVideoAsync();
		}

		// calls the api to get facial data and adds it to the data being graphed
		private async void UpdateRealTimeDataAsync(object sender, object e)
		{
			FaceData.Add(await _session.CreateChartItem(await TakePhoto(), _time));
		}

		// updates the session time
		private void UpdateClock(object sender, object e)
		{
			var duration = DateTime.Now - _startDate;
			_time = $"{duration.Hours}:{duration.Minutes}:{duration.Seconds}";
			RecordingDuration.Text = $"Duration: {_time}";
		}

		// toggles whether the graph panel is shown
		private void Button_Toggle_Pane(object sender, RoutedEventArgs e)
		{
			MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
			TogglePaneButton.Content = MainSplitView.IsPaneOpen ? "Hide Graphs" : "Show Graphs";
		}

		// starts/stops recording
		private void Button_Toggle_Recording(object sender, RoutedEventArgs e)
		{
			ToggleRecording();
		}

		// Start recording if new session, or stop if currently recording
		// also called when exiting the page before stopping to ensure the session is saved
		// a new session must be started to start a new recording
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
					RecordingDuration.Text += " - Stopped";
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
			_session.SaveSession(testfolder.Path);
		}

		// Return to main menu
		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			
			Frame.Navigate(typeof(MainMenu));
		}

		// Ensures the video control stops correctly
		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			if (_status == RecordingStatus.Started)
				ToggleRecording();
			else
			{
				_mediaControls.StopRecording();
				_timer.Stop();
				_clock.Stop();
			}
		}

		// Initializes and begins the video capture
		private async Task StartVideoAsync()
		{
			PreviewControl.Source = await _mediaControls.InitializeCamera();
			_mediaControls.StartRecording();
		}

		// Captures a frame from video and converts it into a byte array
		private async Task<byte[]> TakePhoto()
		{
			//ImagePreview.Source = await MediaControls.CaptureImageToBitMap();
			return await _mediaControls.CaptureImageToByteArray();
		}

		// Adjusts the graphs in the page to the data selected in the combo boxes
		private void UpdateGraphOneSelection(object sender, SelectionChangedEventArgs e)
		{
			ComboBox comboBox = sender as ComboBox;
			GraphingTools.UpdateGraph(comboBox, Chart1);
		}
		private void UpdateGraphTwoSelection(object sender, SelectionChangedEventArgs e)
		{
			ComboBox comboBox = sender as ComboBox;
			GraphingTools.UpdateGraph(comboBox, Chart2);
		}
		private void UpdateGraphThreeSelection(object sender, SelectionChangedEventArgs e)
		{
			ComboBox comboBox = sender as ComboBox;
			GraphingTools.UpdateGraph(comboBox, Chart3);
		}
		private void UpdateGraphFourSelection(object sender, SelectionChangedEventArgs e)
		{
			ComboBox comboBox = sender as ComboBox;
			GraphingTools.UpdateGraph(comboBox, Chart4);
		}
	}
}