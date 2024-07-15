using PaddleOCRSharp;
using System.Drawing;

namespace AutoDLL
{

    public enum Coordinate
    {
        RectCenter,
        RectLeft, 
        RectRight,
    }
    public class OcrJsonResult
    {
        /// <summary>
        /// 识别出的文本内容
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 文本识别的置信度分数,范围从0到1
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// 包围文本的矩形框的中心坐标
        /// </summary>
        public double X { get; set; }

        public double Y { get; set; }
    }

    public class PPOcrResult: TextBlock
    {
        public PPOcrResult(TextBlock block)
        {
            if(block is null)
                return;
            Text = block.Text;
            Score = block.Score;
            cls_label = block.cls_label;
            cls_score = block.cls_score;
            foreach (var item in block.BoxPoints)
            {
                BoxPoints.Add(new OCRPoint { X = item.X, Y = item.Y });
            }
        }
        public OCRPoint CenterPoint
        {
            get
            {
                if(BoxPoints.Count == 4)
                {
                    var centerX = (BoxPoints[1].X - BoxPoints[0].X) / 2 + BoxPoints[0].X;
                    var centerY = (BoxPoints[2].Y - BoxPoints[1].Y) / 2 + BoxPoints[1].Y;
                    return new OCRPoint(centerX, centerY);
                }
                return new();
            }
        }
        public OCRPoint LeftCenterPoint
        {
            get
            {
                if (BoxPoints.Count == 4)
                {
                    var centerX = BoxPoints[0].X;
                    var centerY = (BoxPoints[2].Y - BoxPoints[1].Y) / 2 + BoxPoints[1].Y;
                    return new OCRPoint(centerX, centerY);
                }
                return new();
            }
        }
        public OCRPoint RightCenterPoint
        {
            get
            {
                if (BoxPoints.Count == 4)
                {
                    var centerX = BoxPoints[1].X;
                    var centerY = (BoxPoints[2].Y - BoxPoints[1].Y) / 2 + BoxPoints[1].Y;
                    return new OCRPoint(centerX, centerY);
                }
                return new();
            }
        }
    }
}
