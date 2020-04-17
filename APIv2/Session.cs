using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Diagnostics;
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
        private List<AudienceFrame> _audienceFrames;

        private const string FileName = "savedsessions.json";

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
            _audienceFrames = new List<AudienceFrame>();
        }

        public async Task<AudienceFrame> CreateChartItem(byte[] imageBytes, string time)
        {
            AudienceFrame snapshot = new AudienceFrame(time);

            //Store frame data
            await snapshot.Detect(Client, imageBytes);
            _audienceFrames.Add(snapshot);

            return snapshot;
        }

        public static List<SavedSession> RetrieveSavedSessions(string path)
        {
            List<SavedSession> res;
            
            //Attempt to read save file
            try
            {
                //Read file and deserialize
                res = JsonSerializer.Deserialize<List<SavedSession>>(File.ReadAllText(path + "\\" + FileName));
            }
            catch
            {
                //Save date doesn't exist, create new
                res = new List<SavedSession>();
            }

            return res;
        }

        public void SaveSession(string path)
        {
            //no data, nothing to save
            if (_audienceFrames.Count == 0)
                return;

            List<SavedSession> savedata;

            //Attempt to read file if already exists
            try
            {
                //Read file and deserialize
                savedata = JsonSerializer.Deserialize<List<SavedSession>>(File.ReadAllText(path + "\\" + FileName));
            }
            catch
            {
                //Save data doesn't exist, create new
                savedata = new List<SavedSession>();
            }
            if(savedata.Count == 0)
            {
                Debug.WriteLine("ERROR SAVEDATA EMPTY ERROR");
            }
            //Add current session data and serialize
            savedata.Add(new SavedSession(_audienceFrames));
            string jsonString = JsonSerializer.Serialize(savedata);

            //save to file
            File.WriteAllText(path + "\\" + FileName, jsonString);
        }

    }

    //Class to attach timestamp to sessiondata for json serialization
    public struct SavedSession
    {
        public DateTime TimeStamp { get; set; }
        public List<AudienceFrame> SessionData { get; set; }

        public SavedSession(List<AudienceFrame> data)
        {
            TimeStamp = DateTime.Now;
            SessionData = data;
        }
    }
}
