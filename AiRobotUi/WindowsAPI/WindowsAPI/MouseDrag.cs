using System;
using System.Drawing;

namespace WindowsAPI
{

    /// <summary>
    /// Mouse 类的助手。允许鼠标滑动。
    /// </summary>
    public static class MouseDrag
    {

        /// <summary>
        /// 按住鼠标左键并将其从一个点线性拖动到另一个点。
        /// </summary>
        /// <param name="point1">The initial point.</param>
        /// <param name="point2">The destination.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="lag">The delay between intervals in milliseconds.</param>
        public static void LeftDrag(Point point1, Point point2, int interval, int lag)
        {
            double slope = ((double)point2.Y - (double)point1.Y) / ((double)point2.X - (double)point1.X);
            double x = point1.X;
            double y = (slope * x) - (slope * point2.X) + point2.Y;

            if (point1 == point2) throw new Exception("Points cannot be equal.");
            if (interval > 100 || interval < 0) throw new Exception("Interval is a percentage and therefore must be between 0 and 100.");

            // This is so its not stuck in a permanent loop
            int exit = 0;

            Mouse.LeftDown(point1.X, point1.Y);

            if (point1.X < point2.X) // If dragging left to right.
            {
                for (int i = 0; i < point2.X - point1.X; i += interval)
                {
                    exit++;
                    x = point1.X + i;
                    y = (slope * x) - (slope * point2.X) + point2.Y;
                    Mouse.Move((int)x, (int)y);
                    System.Threading.Thread.Sleep(lag);
                    if (exit > 10000) break;
                }
            }
            else if (point1.X > point2.X) // If dragging right to left.
            {
                for (int i = 0; i < Math.Abs(point2.X - point1.X); i += interval)
                {
                    exit++;
                    x = point1.X - i;
                    y = (slope * x) - (slope * point2.X) + point2.Y;
                    Mouse.Move((int)x, (int)y);
                    System.Threading.Thread.Sleep(lag);
                    if (exit > 10000) break;
                }
            }
            else if (point1.X == point2.X && point1.Y < point2.Y) // If dragging vertically upwards.
            {
                for (int i = 0; i < Math.Abs(point2.Y - point1.Y); i += interval)
                {
                    exit++;
                    y = point1.Y + i;
                    Mouse.Move((int)x, (int)y);
                    System.Threading.Thread.Sleep(lag);
                    if (exit > 10000) break;
                }
            }
            else if (point1.X == point2.X && point1.Y > point2.Y)
            {
                for (int i = 0; i < Math.Abs(point2.Y - point1.Y); i += interval)
                {
                    exit++;
                    y = point1.Y - i;
                    Mouse.Move((int)x, (int)y);
                    System.Threading.Thread.Sleep(lag);
                    if (exit > 10000) break;
                }
            }
            Mouse.LeftUp();
        }
    }
}
