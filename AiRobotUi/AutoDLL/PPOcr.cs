using PaddleOCRSharp;

namespace AutoDLL
{

    public class PPOcr
    {
        public static PaddleOCREngine Engine { get; set; }
        static PPOcr()
        {
            Engine = new PaddleOCREngine(null, new OCRParameter());
        }

        public List<PPOcrResult> GetOcrResults(string imagePath)
        {
            var results = new List<PPOcrResult>();
            var ocrResult = Engine.DetectText(imagePath);
            {

                foreach (var region in ocrResult.TextBlocks)
                {
                    var resu = new PPOcrResult(region);
                    results.Add(resu);
                }
            }
            return results;
        }

        public List<OcrJsonResult> GetOcrJson(string imagePath)
        {
            var results = GetOcrResults(imagePath).Where(x => !string.IsNullOrEmpty(x.Text)).ToList();
            var jsonResult = new List<OcrJsonResult>();
            foreach (var ocrResult in results)
            {
                jsonResult.Add(new OcrJsonResult
                {
                    Text = ocrResult.Text,
                    Score = ocrResult.Score,
                    X = ocrResult.CenterPoint.X,
                    Y = ocrResult.CenterPoint.Y,
                });
            }
            return jsonResult;
        }

        public OCRPoint? GetTextPoint(string imagePath,
                                     string text,
                                     bool isReverse=false,
                                     int index = 1,
                                     Coordinate coordinate=Coordinate.RectCenter)
        {
            var results = GetOcrResults(imagePath);
            results = results
                .Where(x => x.Text.Contains(text)).Select(x => x).ToList();
            if (!isReverse)
            {
                results.Reverse();
            }

            if(results.Count >= index)
            {
                var result = results[index - 1];
                switch (coordinate)
                {
                    case Coordinate.RectRight:
                        return result.RightCenterPoint;
                    case Coordinate.RectLeft:
                        return result.LeftCenterPoint;
                    case Coordinate.RectCenter:
                    default:
                        return result.CenterPoint;
                }
            }
            else
            {
                return null;
            }
        }


    }
}
