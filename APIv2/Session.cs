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
            AudienceFrame snapshot = new AudienceFrame();

            //Store frame data
            await snapshot.Detect(Client, imageBytes, time);
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

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

            List<SavedSession> savedata = Session.RetrieveSavedSessions(path);

            //Add current session data and serialize
            SavedSession s = new SavedSession();
            s.SessionData = _audienceFrames;
            s.TimeStamp = DateTime.Now;
            savedata.Add(s);

            string jsonString = JsonSerializer.Serialize(savedata);

            //save to file
            File.WriteAllText(path + "\\" + FileName, jsonString);
        }

    }

    //class to attach timestamp to session data for json serialization
    public class SavedSession
    {
        public DateTime TimeStamp { get; set; }
        public List<AudienceFrame> SessionData { get; set; }
    }
}
