using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using API;


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
		public Session Session;

		public MainPage()
		{
			this.InitializeComponent();

			FaceData = new ObservableCollection<FaceDataItem>();
			RealTimeData = new ObservableCollection<RealTimeDataItem>();
			_timer = new DispatcherTimer();
			_activeRecording = false;
			Session = new Session();

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
			//FillVisitsData();
			//FillInitialRealTimeData();

			this.DataContext = this;

			_timer.Interval = new TimeSpan(0,0,5);
			_timer.Tick += UpdateRealTimeData;
			_timer.Start();
		}

		//private void FillVisitsData()
		//{
		//	var startDate = DateTime.Today.AddMonths(-1);
		//	var visits = 200;
		//	var newVisits = 50;
		//	var pageviews = 390;

		//	var rnd = new Random();

		//	for (var date = startDate; date < DateTime.Today; date = date.AddDays(1))
		//	{
		//		FaceData.Add(new FaceDataItem()
		//		{
		//			Date = date.ToString("MMM-dd"),
		//			Var1 = visits,
		//			Var2 = newVisits,
		//			Var3 = pageviews
		//		});

		//		visits += (int)(50 * (rnd.NextDouble() - 0.3));
		//		newVisits += (int)(30 * (rnd.NextDouble() - 0.3));
		//		pageviews += (int)(80 * (rnd.NextDouble() - 0.3));
		//	}
		//}

		//private void FillInitialRealTimeData()
		//{
		//	var startDate = DateTime.Now.AddSeconds(-15);
		//	var visits = 10;

		//	var rnd = new Random();

		//	for (var date = startDate; date < DateTime.Now; date = date.AddSeconds(1))
		//	{
		//		RealTimeData.Add(new RealTimeDataItem()
		//		{
		//			Seconds = date.Second,
		//			Visits = visits
		//		});

		//		visits += (int)(5 * (rnd.NextDouble() - 0.5));
		//		visits = visits < 0 ? 0 : visits;
		//	}
		//}
		private async void UpdateRealTimeData(object sender, object e)
		{
			//RealTimeData.RemoveAt(0);

			//var rnd = new Random();
			//var visits = RealTimeData[RealTimeData.Count - 1].Visits;
			//visits += (int)(5 * (rnd.NextDouble() - 0.5));
			//visits = visits < 0 ? 0 : visits;

			//RealTimeData.Add(new RealTimeDataItem()
			//{
			//	Seconds = DateTime.Now.Second,
			//	Visits = visits
			//});

			await Session.CreateChartItem(await _takePhoto(), DateTime.Now);
		}

		private async Task<byte[]> _takePhoto()
		{

			/*
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
			*/


			var stream = new InMemoryRandomAccessStream();
			var imgFormat = ImageEncodingProperties.CreateBmp();
			await _mediaCapture.CapturePhotoToStreamAsync(imgFormat, stream);


			var readStream = stream.AsStreamForRead();
			var byteArray = new byte[readStream.Length];
			await readStream.ReadAsync(byteArray, 0, byteArray.Length);
			return byteArray;
		}


		private void Button_Toggle_Pane(object sender, RoutedEventArgs e)
		{
			MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
		}

		private void Button_Capture_Photo(object sender, RoutedEventArgs e)
		{
			//try
			//{
			//	_takePhoto();
			//}
			//catch
			//{
			//	// exception here...
			//}

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