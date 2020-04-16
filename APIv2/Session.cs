using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace API
{
    public class Session
    {
        private string SUBSCRIPTION_KEY;
        private string ENDPOINT;

        private IFaceClient Client;
        private List<FrameData> _frameDatas;

        public Session()
        {
            //Note: Set environment variables through cmd, e.g. "setx FACE_SUBSCRIPTION_KEY [api key]"
            SUBSCRIPTION_KEY = Environment.GetEnvironmentVariable("FACE_SUBSCRIPTION_KEY");
            ENDPOINT = Environment.GetEnvironmentVariable("FACE_ENDPOINT");

            AuthenticateSession(ENDPOINT, SUBSCRIPTION_KEY);
        }

        //Authenticate Client
        private void AuthenticateSession(string endpoint, string key)
        {
            Client = new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
            _frameDatas = new List<FrameData>();
        }

        public async Task<FrameData> CreateChartItem(byte[] imageBytes, DateTime time)
        {
            AudienceFrame snapshot = new AudienceFrame(imageBytes);

            //Store frame data
            var frame = await snapshot.Detect(Client);
            _frameDatas.Add(frame);

            return frame;
        }

        public void SaveSession()
        {
            var csv = new StringBuilder();

            foreach(var data in _frameDatas)
            {
                csv.AppendLine($"{data.Time},{data.Anger},{data.Contempt},{data.Disgust},{data.Fear},{data.Happiness},{data.Neutral},{data.Sadness},{data.Surprise}");
            }

            //File.WriteAllText("./test.csv", csv.ToString());
        }

    }

    //PLACEHOLDER CHART ITEM
    public class FrameData
    {
        public double Time { get; set; }
        public double Anger { get; set; } = 0;
        public double Contempt { get; set; } = 0;
        public double Disgust { get; set; } = 0;
        public double Fear { get; set; } = 0;
        public double Happiness { get; set; } = 0;
        public double Neutral { get; set; } = 0;
        public double Sadness { get; set; } = 0;
        public double Surprise { get; set; } = 0;
    }
}
