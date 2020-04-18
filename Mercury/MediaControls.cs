using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Mercury
{
	public class MediaControls
	{
		public MediaCapture Video;

		//public MediaControls()
		//{
		//}

		public async Task<MediaCapture> InitializeCamera()
		{
			if (Video == null)
			{
				// Get the camera devices
				var cameraDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
				Video = new MediaCapture();

				await Video.InitializeAsync(new MediaCaptureInitializationSettings
				{
					VideoDeviceId = cameraDevices.FirstOrDefault().Id
				});
			}
			return Video;
		}

		public async Task<byte[]> CaptureImageToByteArray()
		{
			var stream = new InMemoryRandomAccessStream();
			await Video.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);

			stream.Seek(0);
			var readStream = stream.AsStreamForRead();
			var byteArray = new byte[readStream.Length];
			await readStream.ReadAsync(byteArray, 0, byteArray.Length);
			return byteArray;
		}

		//public async Task<BitmapImage> CaptureImageToBitMap()
		//{
		//	var stream = new InMemoryRandomAccessStream();
		//	await Video.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);

		//	var bitMap = new BitmapImage();
		//	stream.Seek(0);
		//	await bitMap.SetSourceAsync(stream);
		//	return bitMap;
		//}

		public async void StartRecording()
		{
			await Video.StartPreviewAsync();
		}

		public async void StopRecording()
		{
			await Video.StopPreviewAsync();
		}
	}

	
}
