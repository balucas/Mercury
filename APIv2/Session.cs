using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
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

        public void SaveSession(string path)
        {
            const string FileName = "savedsessions.json";
            List<SavedSession> savedata;

            //Attempt to read file if already exists
            try
            {
                //Read file and deserialize
                savedata = JsonSerializer.Deserialize<List<SavedSession>>(File.ReadAllText(path + "\\" + FileName));
            }
            catch (Exception ex)
            {
                //Save data doesnt exist, create new
                savedata = new List<SavedSession>();
            }

            //Add current session data and serialize
            savedata.Add(new SavedSession(_frameDatas));
            string jsonString = JsonSerializer.Serialize(savedata);

            //save to file
            File.WriteAllText(path + "\\" + FileName, jsonString);
        }

    }

    //Class to attach timestamp to sessiondata for json serialization
    class SavedSession
    {
        public DateTime TimeStamp { get; set; }
        public List<FrameData> SessionData { get; set; }

        public SavedSession(List<FrameData> data)
        {
            TimeStamp = DateTime.Now;
            SessionData = data;
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
