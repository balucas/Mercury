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
        //private readonly Stream _imgStream;
        public string Time { get; set; }
        public double Anger { get; set; } = 0;
        public double Contempt { get; set; } = 0;
        public double Disgust { get; set; } = 0;
        public double Fear { get; set; } = 0;
        public double Happiness { get; set; } = 0;
        public double Neutral { get; set; } = 0;
        public double Sadness { get; set; } = 0;
        public double Surprise { get; set; } = 0;

        public async Task Detect(IFaceClient client, byte[] imgBytes, string time)
        {
            Time = time;
            IList<DetectedFace> faceList = await client.Face.DetectWithStreamAsync(
                new MemoryStream(imgBytes),
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
                Anger += face.FaceAttributes.Emotion.Anger / faceCount;
                Contempt += face.FaceAttributes.Emotion.Contempt / faceCount;
                Disgust += face.FaceAttributes.Emotion.Disgust / faceCount;
                Fear += face.FaceAttributes.Emotion.Fear / faceCount;
                Happiness += face.FaceAttributes.Emotion.Happiness / faceCount;
                Neutral += face.FaceAttributes.Emotion.Neutral / faceCount;
                Sadness += face.FaceAttributes.Emotion.Sadness / faceCount;
                Surprise += face.FaceAttributes.Emotion.Surprise / faceCount;
            }
        }
    }
}
