using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public List<AudienceFrame> AFrameList;
        public ObservableCollection<TestChartItem> Chart;

        public Session()
        {
            //Note: Set environment variables through cmd, e.g. "setx FACE_SUBSCRIPTION_KEY [api key]"
            SUBSCRIPTION_KEY = Environment.GetEnvironmentVariable("FACE_SUBSCRIPTION_KEY");
            ENDPOINT = Environment.GetEnvironmentVariable("FACE_ENDPOINT");

            AuthenticateSession(ENDPOINT, SUBSCRIPTION_KEY);

            AFrameList = new List<AudienceFrame>();
            Chart = new ObservableCollection<TestChartItem>();
        }
        
        //Authenticate Client
        private void AuthenticateSession(string endpoint, string key)
        {
            Client = new FaceClient(new ApiKeyServiceClientCredentials(key)){ Endpoint = endpoint };
        }

        public async Task<TestChartItem> CreateChartItem(byte[] imageBytes, DateTime time)
        {
            AudienceFrame snapshot = new AudienceFrame(imageBytes);

            await snapshot.Detect(Client);
            
            var newChartData = new TestChartItem()
            {
                Time = time.Second,
                Var1 = snapshot.AngerAvg,
                Var2 = snapshot.ContemptAvg
            };

            return newChartData;
        }

    }

    //PLACEHOLDER CHART ITEM
    public class TestChartItem
    {
	    public double Time;
	    public double Var1;
	    public double Var2;
    }
}   
