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

        public AudienceFrame(byte[] imgBytes)
        {
            _imgStream = new MemoryStream(imgBytes);
        }

        public async Task<FrameData> Detect(IFaceClient client)
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
            var res = new FrameData();

            foreach (var face in faceList)
            {
                res.Anger += face.FaceAttributes.Emotion.Anger / faceCount;
                res.Contempt += face.FaceAttributes.Emotion.Contempt / faceCount;
                res.Disgust += face.FaceAttributes.Emotion.Disgust / faceCount;
                res.Fear += face.FaceAttributes.Emotion.Fear / faceCount;
                res.Happiness += face.FaceAttributes.Emotion.Happiness / faceCount;
                res.Neutral += face.FaceAttributes.Emotion.Neutral / faceCount;
                res.Sadness += face.FaceAttributes.Emotion.Sadness / faceCount;
                res.Surprise += face.FaceAttributes.Emotion.Surprise / faceCount;
            }
            return res;
        }
    }
}
