using PaddleOCRSharp;
using System.Drawing;

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
                                     bool isReverse = false,
                                     int index = 1,
                                     Coordinate coordinate = Coordinate.RectCenter)
        {
            var results = GetOcrResults(imagePath);
            results = results
                .Where(x => x.Text.Contains(text)).Select(x => x).ToList();
            if (!isReverse)
            {
                results.Reverse();
            }

            if (results.Count >= index)
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

        public List<ChatMessage> GetChatMessages(string imagePath, string customerColorStr, string assistantColorStr, int colorTolerance = 55, int yTolerance = 20)
        {
            var ocrResults = GetOcrResults(imagePath);
            var chatMessages = new List<ChatMessage>();

            Color customerColor = ColorTranslator.FromHtml(customerColorStr);
            Color assistantColor = ColorTranslator.FromHtml(assistantColorStr);

            using (var bitmap = new Bitmap(imagePath))
            {
                var mergedResults = MergeAdjacentBlocks(ocrResults, bitmap, customerColor, assistantColor, colorTolerance, yTolerance);

                foreach (var result in mergedResults)
                {
                    var bubbleColor = GetDominantColor(bitmap, result.BoxPoints);
                    bool isCustomer = ColorDistance(bubbleColor, customerColor) < ColorDistance(bubbleColor, assistantColor);

                    chatMessages.Add(new ChatMessage
                    {
                        IsCustomer = isCustomer,
                        Text = result.Text
                    });
                }
            }

            return chatMessages;
        }

        public List<ChatMessage> GetChatMessages(int leftX, int rightX, string imagePath, string customerColorStr, string assistantColorStr, int colorTolerance = 55, int yTolerance = 20)
        {
            var ocrResults = GetOcrResults(imagePath);
            var chatMessages = new List<ChatMessage>();

            Color customerColor = ColorTranslator.FromHtml(customerColorStr);
            Color assistantColor = ColorTranslator.FromHtml(assistantColorStr);

            using (var bitmap = new Bitmap(imagePath))
            {
                var mergedResults = MergeAdjacentBlocks(ocrResults, bitmap, customerColor, assistantColor, colorTolerance, yTolerance);

                foreach (var result in mergedResults)
                {
                    if(result.BoxPoints[0].X >= leftX && result.BoxPoints[1].X <= rightX)
                    {
                        var bubbleColor = GetDominantColor(bitmap, result.BoxPoints);
                        bool isCustomer = ColorDistance(bubbleColor, customerColor) < ColorDistance(bubbleColor, assistantColor);

                        chatMessages.Add(new ChatMessage
                        {
                            IsCustomer = isCustomer,
                            Text = result.Text
                        });
                    }

                }
            }

            return chatMessages;
        }

        private List<PPOcrResult> MergeAdjacentBlocks(List<PPOcrResult> results, Bitmap bitmap, Color customerColor, Color assistantColor, int colorTolerance, int yTolerance)
        {
            var mergedResults = new List<PPOcrResult>();
            PPOcrResult currentBlock = null;

            foreach (var result in results.OrderBy(r => r.BoxPoints[0].Y))
            {
                var blockColor = GetDominantColor(bitmap, result.BoxPoints);

                // 检查颜色是否接近客户或助手的颜色
                bool isValidColor = ColorDistance(blockColor, customerColor) <= colorTolerance ||
                                    ColorDistance(blockColor, assistantColor) <= colorTolerance;

                if (!isValidColor)
                {
                    // 如果颜色既不接近客户色也不接近助手色，则跳过这个文本块
                    continue;
                }

                if (currentBlock == null)
                {
                    currentBlock = result;
                    continue;
                }

                var currentColor = GetDominantColor(bitmap, currentBlock.BoxPoints);

                if (ColorDistance(currentColor, blockColor) <= colorTolerance &&
                    Math.Abs(currentBlock.BoxPoints[2].Y - result.BoxPoints[0].Y) <= yTolerance)
                {
                    // Merge blocks
                    currentBlock.Text += " " + result.Text;
                    currentBlock.BoxPoints[1] = new OCRPoint(Math.Max(currentBlock.BoxPoints[1].X, result.BoxPoints[1].X), currentBlock.BoxPoints[1].Y);
                    currentBlock.BoxPoints[2] = new OCRPoint(Math.Max(currentBlock.BoxPoints[2].X, result.BoxPoints[2].X), result.BoxPoints[2].Y);
                    currentBlock.BoxPoints[3] = new OCRPoint(currentBlock.BoxPoints[3].X, result.BoxPoints[3].Y);
                }
                else
                {
                    mergedResults.Add(currentBlock);
                    currentBlock = result;
                }
            }

            if (currentBlock != null)
            {
                mergedResults.Add(currentBlock);
            }

            return mergedResults;
        }

        private Color GetDominantColor(Bitmap bitmap, List<OCRPoint> points)
        {
            int minX = (int)points.Min(p => p.X);
            int maxX = (int)points.Max(p => p.X);
            int minY = (int)points.Min(p => p.Y);
            int maxY = (int)points.Max(p => p.Y);

            int r = 0, g = 0, b = 0;
            int total = 0;

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    r += pixel.R;
                    g += pixel.G;
                    b += pixel.B;
                    total++;
                }
            }

            if (total == 0) return Color.White;

            return Color.FromArgb(r / total, g / total, b / total);
        }

        private bool IsCloserToColor(Color color, Color customerColor, Color assistantColor)
        {
            int customerDistance = ColorDistance(color, customerColor);
            int assistantDistance = ColorDistance(color, assistantColor);
            return customerDistance < assistantDistance;
        }

        private int ColorDistance(Color c1, Color c2)
        {
            return (int)Math.Sqrt(
                Math.Pow(c1.R - c2.R, 2) +
                Math.Pow(c1.G - c2.G, 2) +
                Math.Pow(c1.B - c2.B, 2)
            );
        }
    }
}
