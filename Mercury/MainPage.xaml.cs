using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
using System.Diagnostics;


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
		public ObservableCollection<MoreTestChartData> TestChart { get; }

		public MainPage()
		{
			this.InitializeComponent();

			FaceData = new ObservableCollection<FaceDataItem>();
			RealTimeData = new ObservableCollection<RealTimeDataItem>();
			_timer = new DispatcherTimer();
			_activeRecording = false;
			Session = new Session();
			TestChart = new ObservableCollection<MoreTestChartData>();

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
			//FillInitialData();

			this.DataContext = this;

			_timer.Interval = new TimeSpan(0,0,5);
			_timer.Tick += UpdateRealTimeDataAsync;
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

		private void FillInitialData()
		{
			var startDate = DateTime.Now.AddSeconds(-15);

			var rnd = new Random();

			for (var date = startDate; date < DateTime.Now; date = date.AddSeconds(1))
			{
				TestChart.Add(new MoreTestChartData()
				{
					Time = date.Second,
					Var1 = (double) rnd.Next(1, 100) / 100,
					Var2 = (double)rnd.Next(1, 100) / 100
				});
			}
		}
		private async void UpdateRealTimeDataAsync(object sender, object e)
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


			var tempChart = await Session.CreateChartItem(await TakePhoto(), DateTime.Now);
			TestChart.Add(new MoreTestChartData()
			{
				Time = tempChart.Time,
				Var1 = tempChart.Anger,
				Var2 = tempChart.Contempt
			});

			//await TakePhoto();
		}

		private async Task<byte[]> TakePhoto()
		{

			var stream = new InMemoryRandomAccessStream();
			await _mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);

			var bitMap = new BitmapImage();
			stream.Seek(0);
			await bitMap.SetSourceAsync(stream);
			imageControl.Source = bitMap;

			stream.Seek(0);
			var readStream = stream.AsStreamForRead();
			var byteArray = new byte[readStream.Length];
			await readStream.ReadAsync(byteArray, 0, byteArray.Length);
			return byteArray;


			//ImageEncodingProperties imgFormat = ImageEncodingProperties.CreateJpeg();

			//// create storage file in local app storage
			//StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
			//	"TestPhoto.jpg",
			//	CreationCollisionOption.GenerateUniqueName);

			//// take photo
			//await _mediaCapture.CapturePhotoToStorageFileAsync(imgFormat, file);

			//// Get photo as a BitmapImage
			//BitmapImage bmpImage = new BitmapImage(new Uri(file.Path));

			//// imagePreview is a <Image> object defined in XAML
			//imageControl.Source = bmpImage;




			//return await ImageToBytes(bmpImage);

		}

		//public static async Task<byte[]> ImageToBytes(BitmapImage image)
		//{
		//	RandomAccessStreamReference streamRef = RandomAccessStreamReference.CreateFromUri(image.UriSource);
		//	IRandomAccessStreamWithContentType streamWithContent = await streamRef.OpenReadAsync();
		//	byte[] buffer = new byte[streamWithContent.Size];
		//	await streamWithContent.ReadAsync(buffer.AsBuffer(), (uint)streamWithContent.Size, InputStreamOptions.None);
		//	return buffer;
		//}

		//public byte[] ImageToByteArray(Image imageIn)
		//{
		//	MemoryStream ms = new MemoryStream();
		//	imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
		//	return ms.ToArray();
		//}


		private void Button_Toggle_Pane(object sender, RoutedEventArgs e)
		{
			MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
		}

		private async void Button_Capture_Photo(object sender, RoutedEventArgs e)
		{
			//try
			//{
			//	_takePhoto();
			//}
			//catch
			//{
			//	// exception here...
			//}
			TakePhoto();
		}

		private async void Button_Toggle_Recording(object sender, RoutedEventArgs e)
		{
			if (_activeRecording == false)
			{
				await _mediaCapture.StartPreviewAsync();
				RecordingButton.Content = "ON";
				_activeRecording = true;
				_timer.Start();
			}
			else
			{
				await _mediaCapture.StopPreviewAsync();
				RecordingButton.Content = "OFF";
				_activeRecording = false;
				_timer.Stop();

				//Session Storage
				Windows.Storage.StorageFolder testfolder = Windows.Storage.ApplicationData.Current.LocalFolder;
				Debug.WriteLine("Local storage: " + testfolder.Path);
				Session.SaveSession(testfolder.Path);
			}
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(MainMenu));
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

	public class MoreTestChartData
	{
		public double Time { get; set; }
		public double Var1 { get; set; }
		public double Var2 { get; set; }
	}


}