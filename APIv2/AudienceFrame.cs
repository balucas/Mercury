using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace API
{
    public class AudienceFrame
    {
        private readonly Stream _imgStream;
        public double AngerAvg { get; set; } = 0;
        public double ContemptAvg { get; set; } = 0;
        public double DisgustAvg { get; set; } = 0;
        public double FearAvg { get; set; } = 0;
        public double HappinessAvg { get; set; } = 0; 
        public double NeutralAvg { get; set; } = 0;
        public double SadnessAvg { get; set; } = 0;
        public double SurpriseAvg { get; set; } = 0;

        public AudienceFrame(byte[] imgBytes)
        {
            _imgStream = new MemoryStream(imgBytes);
        }

        public async Task Detect(IFaceClient client)
        {
            IList<DetectedFace> faceList = await client.Face.DetectWithStreamAsync(
                _imgStream,
                returnFaceAttributes: new List<FaceAttributeType>
                {
                    FaceAttributeType.Age,
                    FaceAttributeType.Emotion,
                    FaceAttributeType.HeadPose
                },
                recognitionModel: RecognitionModel.Recognition02
            );

            int faceCount = faceList.Count;

            foreach (var face in faceList)
            {
                AngerAvg += face.FaceAttributes.Emotion.Anger / faceCount;
                ContemptAvg += face.FaceAttributes.Emotion.Contempt / faceCount; 
                DisgustAvg += face.FaceAttributes.Emotion.Disgust / faceCount;
                FearAvg += face.FaceAttributes.Emotion.Fear / faceCount;
                HappinessAvg += face.FaceAttributes.Emotion.Happiness / faceCount;
                NeutralAvg += face.FaceAttributes.Emotion.Neutral / faceCount;
                SadnessAvg += face.FaceAttributes.Emotion.Sadness / faceCount;
                SurpriseAvg += face.FaceAttributes.Emotion.Surprise / faceCount;
            }
        }
    }
}
