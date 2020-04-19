using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;

namespace Mercury.Classes
{
	public class MediaControls
	{
		private MediaCapture _video;

		public async Task<MediaCapture> InitializeCamera()
		{
			if (_video == null)
			{
				// Get the camera devices
				var cameraDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
				_video = new MediaCapture();

				var videoDeviceId = cameraDevices.FirstOrDefault()?.Id;
				if (videoDeviceId != null)
					await _video.InitializeAsync(new MediaCaptureInitializationSettings
					{
						VideoDeviceId = videoDeviceId
					});
			}
			return _video;
		}

		// Convert frame from mediacapture into byte array
		public async Task<byte[]> CaptureImageToByteArray()
		{
			var stream = new InMemoryRandomAccessStream();
			await _video.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);

			stream.Seek(0);
			var readStream = stream.AsStreamForRead();
			var byteArray = new byte[readStream.Length];
			await readStream.ReadAsync(byteArray, 0, byteArray.Length);
			return byteArray;
		}

		// Convert frame from mediacapture into bitmap
		//public async Task<BitmapImage> CaptureImageToBitMap()
		//{
		//	var stream = new InMemoryRandomAccessStream();
		//	await Video.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);

		//	var bitMap = new BitmapImage();
		//	stream.Seek(0);
		//	await bitMap.SetSourceAsync(stream);
		//	return bitMap;
		//}

		// Start the mediacapture preview
		public async void StartRecording()
		{
			await _video.StartPreviewAsync();
		}

		// Stop the mediacapture preview
		public async void StopRecording()
		{
			try
			{
				await _video.StopPreviewAsync();
			}
			catch
			{
				Debug.WriteLine("Attempted to stop an unstarted MediaCapture");
			}
		}
	}

	
}
