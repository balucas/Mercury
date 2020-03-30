using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Mercury
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{

		public ObservableCollection<FaceDataItem> FaceData { get; }
		public ObservableCollection<RealTimeDataItem> RealTimeData { get; }
		private DispatcherTimer _timer;
		private bool _activeRecording;

		public MainPage()
		{
			this.InitializeComponent();

			FaceData = new ObservableCollection<FaceDataItem>();
			RealTimeData = new ObservableCollection<RealTimeDataItem>();
			_timer = new DispatcherTimer();
			_activeRecording = false;

			Application.Current.Resuming += Application_Resuming;
		}

		private MediaCapture _mediaCapture;
		public ObservableCollection<String> ConceptCharts;
		public ObservableCollection<RealTImeFacialFeatureData> RealTimeFaceData;

		private async void Application_Resuming(object sender, object o)
		{
			await InitializeCameraAsync();
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			await InitializeCameraAsync();
		}

		private async Task InitializeCameraAsync()
		{
			if (_mediaCapture == null)
			{
				// Get the camera devices
				var cameraDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
				_mediaCapture = new MediaCapture();

				await _mediaCapture.InitializeAsync(new MediaCaptureInitializationSettings
				{
					VideoDeviceId = cameraDevices.FirstOrDefault().Id
				});

				PreviewControl.Source = _mediaCapture;
			}
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			FillVisitsData();
			FillInitialRealTimeData();

			this.DataContext = this;

			_timer.Interval = TimeSpan.FromSeconds(1);
			_timer.Tick += UpdateRealTimeData;
			_timer.Start();
		}

		private void FillVisitsData()
		{
			var startDate = DateTime.Today.AddMonths(-1);
			var visits = 200;
			var newVisits = 50;
			var pageviews = 390;

			var rnd = new Random();

			for (var date = startDate; date < DateTime.Today; date = date.AddDays(1))
			{
				FaceData.Add(new FaceDataItem()
				{
					Date = date.ToString("MMM-dd"),
					Var1 = visits,
					Var2 = newVisits,
					Var3 = pageviews
				});

				visits += (int)(50 * (rnd.NextDouble() - 0.3));
				newVisits += (int)(30 * (rnd.NextDouble() - 0.3));
				pageviews += (int)(80 * (rnd.NextDouble() - 0.3));
			}
		}

		private void FillInitialRealTimeData()
		{
			var startDate = DateTime.Now.AddSeconds(-15);
			var visits = 10;

			var rnd = new Random();

			for (var date = startDate; date < DateTime.Now; date = date.AddSeconds(1))
			{
				RealTimeData.Add(new RealTimeDataItem()
				{
					Seconds = date.Second,
					Visits = visits
				});

				visits += (int)(5 * (rnd.NextDouble() - 0.5));
				visits = visits < 0 ? 0 : visits;
			}
		}
		private void UpdateRealTimeData(object sender, object e)
		{
			RealTimeData.RemoveAt(0);

			var rnd = new Random();
			var visits = RealTimeData[RealTimeData.Count - 1].Visits;
			visits += (int)(5 * (rnd.NextDouble() - 0.5));
			visits = visits < 0 ? 0 : visits;

			RealTimeData.Add(new RealTimeDataItem()
			{
				Seconds = DateTime.Now.Second,
				Visits = visits
			});
		}

		private async void _takePhoto()
		{

			ImageEncodingProperties imgFormat = ImageEncodingProperties.CreateJpeg();

			// create storage file in local app storage
			StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
				"TestPhoto.jpg",
				CreationCollisionOption.GenerateUniqueName);

			// take photo
			await _mediaCapture.CapturePhotoToStorageFileAsync(imgFormat, file);

			// Get photo as a BitmapImage
			BitmapImage bmpImage = new BitmapImage(new Uri(file.Path));

			// imagePreview is a <Image> object defined in XAML
			imageControl.Source = bmpImage;

		}


		private void Add_Chart_Data(object sender, RoutedEventArgs e)
		{

		}

		private void Button_Toggle_Pane(object sender, RoutedEventArgs e)
		{
			MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
		}

		private void Button_Capture_Photo(object sender, RoutedEventArgs e)
		{
			try
			{
				_takePhoto();
			}
			catch
			{
				// exception here...
			}

		}

		private async void Button_Toggle_Recording(object sender, RoutedEventArgs e)
		{
			if (_activeRecording == false)
			{
				await _mediaCapture.StartPreviewAsync();
				RecordingButton.Content = "ON";
				_activeRecording = true;
			}
			else
			{
				await _mediaCapture.StopPreviewAsync();
				RecordingButton.Content = "OFF";
				_activeRecording = false;
			}
		}
	}

	public class FaceDataItem
	{
		public string Date { get; set; }
		public int Var1 { get; set; }
		public int Var2 { get; set; }
		public int Var3 { get; set; }
	}

	public class RealTimeDataItem
	{
		public int Seconds { get; set; }
		public int Visits { get; set; }
	}
	//***********************************************************


	public class RealTImeFacialFeatureData
	{
		public string Time { get; set; }
		public int Var1 { get; set; }
		public int Var2 { get; set; }
		public int Var3 { get; set; }
	}


}
/*
 
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>


	public sealed partial class MainPage : Page
{
	// Provides functionality to capture the output from the camera
	private MediaCapture _mediaCapture;
	public ObservableCollection<String> ConceptCharts;
	public ObservableCollection<RealTImeFacialFeatureData> RealTimeFaceData;

	public MainPage()
	{
		InitializeComponent();

		ConceptCharts = new ObservableCollection<string>();

		PopulateSelectionBoxes(ChartOneSelection);
		PopulateSelectionBoxes(ChartTwoSelection);
		PopulateSelectionBoxes(ChartThreeSelection);

		RealTimeFaceData = new ObservableCollection<RealTImeFacialFeatureData>();
		FillChartData();

		Application.Current.Resuming += Application_Resuming;
	}

	//private void Page_Loaded(object sender, RoutedEventArgs e)
	//{


	//	this.DataContext = this;
	//}


	private void PopulateSelectionBoxes(ComboBox box)
	{
		box.Items.Add("Var1");
		box.Items.Add("Var2");
		box.Items.Add("Var3");
	}


	private void FillChartData()
	{
		var chartData = 1;

		for (var time = 1; time < 10; time++)
		{
			RealTimeFaceData.Add(new RealTImeFacialFeatureData()
			{
				Time = time.ToString(),
				Var1 = chartData,
				Var2 = chartData + 100,
				Var3 = chartData + 1000
			});

			chartData += 10;

		}
	}

	private void Add_Chart_Data(object sender, RoutedEventArgs e)
	{
		Random random = new Random();
		RealTimeFaceData.Add(new RealTImeFacialFeatureData()
		{
			Time = random.Next(1, 100).ToString(),
			Var1 = random.Next(1, 100),
			Var2 = random.Next(1, 100),
			Var3 = random.Next(1, 100),

		});
	}

	private void Selection_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		try
		{
			GraphOne.ValueMemberPath = ChartOneSelection.SelectedItem.ToString();
		}
		catch
		{

		}
		try
		{
			GraphTwo.ValueMemberPath = ChartTwoSelection.SelectedItem.ToString();
		}
		catch
		{

		}

		try
		{
			GraphThree.ValueMemberPath = ChartThreeSelection.SelectedItem.ToString();
		}
		catch
		{

		}
	}
}



public class RealTImeFacialFeatureData
{
	public string Time { get; set; }
	public int Var1 { get; set; }
	public int Var2 { get; set; }
	public int Var3 { get; set; }
}

}

 */
