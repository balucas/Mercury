using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public List<AudienceFrame> AFrameList;
        public ObservableCollection<TestChartItem> Chart;

        public Session()
        {
            SUBSCRIPTION_KEY = Environment.GetEnvironmentVariable("FACE_SUBSCRIPTION_KEY");
            ENDPOINT = Environment.GetEnvironmentVariable("FACE_ENDPOINT");

            AuthenticateSession(ENDPOINT, SUBSCRIPTION_KEY);
        }
        
        //Authenticate Client
        private void AuthenticateSession(string endpoint, string key)
        {
            Client = new FaceClient(new ApiKeyServiceClientCredentials(key)){ Endpoint = endpoint };
        }

        public async Task CreateChartItem(byte[] imageBytes, DateTime time)
        {
            AudienceFrame snapshot = new AudienceFrame(imageBytes);

            await snapshot.Detect(Client);
            
            Chart.Add(new TestChartItem()
            {
                Time = time.ToString("hh:mm:ss"),
                Var1 = snapshot.AngerAvg,
                Var2 = snapshot.ContemptAvg
            });
        }

    }

    //PLACEHOLDER CHART ITEM
    public class TestChartItem
    {
        public string Time;
        public double Var1;
        public double Var2;
    }
}   
