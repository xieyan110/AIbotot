using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.ComponentModel;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;


namespace WindowsAPI
{
    public class WindowCapture
    {
        // 导入必要的Windows API函数
        [DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, uint nFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("kernel32.dll")]
        private static extern uint GetLastError();


        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }



        public static Bitmap CaptureWindow(IntPtr hwnd)
        {
            RECT rect;
            GetWindowRect(hwnd, out rect);
            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;

            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            IntPtr hwndDC = GetWindowDC(hwnd);

            try
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    IntPtr hdc = graphics.GetHdc();

                    bool result = PrintWindow(hwnd, hdc, 3);
                    graphics.ReleaseHdc(hdc);

                    if (!result)
                    {
                        int errorCode = Marshal.GetLastWin32Error();
                        Console.WriteLine($"常规 PrintWindow 失败。错误代码: {errorCode}。尝试 Direct3D 捕获...");

                        // 尝试使用 Direct3D 捕获
                        return CaptureWindowDirect3D(hwnd, width, height);
                    }
                }

                return bitmap;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"捕获窗口失败: {ex.Message}");
                bitmap.Dispose();
                return null;
            }
            finally
            {
                ReleaseDC(hwnd, hwndDC);
            }

        }

        private static Bitmap CaptureWindowDirect3D(IntPtr hwnd, int width, int height)
        {
            try
            {
                using (var factory = new Factory1())
                using (var adapter = factory.GetAdapter1(0))
                using (var device = new SharpDX.Direct3D11.Device(adapter))
                {
                    var output = adapter.GetOutput(0);
                    var output1 = output.QueryInterface<Output1>();

                    using (var duplicatedOutput = output1.DuplicateOutput(device))
                    {
                        SharpDX.DXGI.Resource screenResource;
                        OutputDuplicateFrameInformation duplicateFrameInformation;

                        // 尝试获取下一帧
                        duplicatedOutput.AcquireNextFrame(1000, out duplicateFrameInformation, out screenResource);

                        // 创建纹理描述
                        var textureDesc = new Texture2DDescription
                        {
                            CpuAccessFlags = CpuAccessFlags.Read,
                            BindFlags = BindFlags.None,
                            Format = Format.B8G8R8A8_UNorm,
                            Width = width,
                            Height = height,
                            OptionFlags = ResourceOptionFlags.None,
                            MipLevels = 1,
                            ArraySize = 1,
                            SampleDescription = { Count = 1, Quality = 0 },
                            Usage = ResourceUsage.Staging
                        };

                        // 创建纹理
                        using (var screenTexture2D = new Texture2D(device, textureDesc))
                        {
                            // 复制资源到纹理
                            using (var screenTexture = screenResource.QueryInterface<Texture2D>())
                            {
                                device.ImmediateContext.CopyResource(screenTexture, screenTexture2D);
                            }

                            // 获取纹理数据
                            var mapSource = device.ImmediateContext.MapSubresource(screenTexture2D, 0, MapMode.Read, SharpDX.Direct3D11.MapFlags.None);

                            // 创建位图
                            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                            var boundsRect = new System.Drawing.Rectangle(0, 0, width, height);

                            // 锁定位图位
                            var bmpData = bitmap.LockBits(boundsRect, ImageLockMode.WriteOnly, bitmap.PixelFormat);

                            // 复制数据
                            IntPtr sourcePtr = mapSource.DataPointer;
                            IntPtr destPtr = bmpData.Scan0;
                            for (int y = 0; y < height; y++)
                            {
                                SharpDX.Utilities.CopyMemory(destPtr, sourcePtr, width * 4);
                                sourcePtr = IntPtr.Add(sourcePtr, mapSource.RowPitch);
                                destPtr = IntPtr.Add(destPtr, bmpData.Stride);
                            }

                            // 解锁位图位
                            bitmap.UnlockBits(bmpData);

                            // 取消纹理映射
                            device.ImmediateContext.UnmapSubresource(screenTexture2D, 0);

                            // 释放帧
                            duplicatedOutput.ReleaseFrame();

                            return bitmap;
                        }
                    }
                }
            }
            catch (SharpDXException ex)
            {
                Console.WriteLine($"Direct3D 捕获失败 (SharpDXException): {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Direct3D 捕获失败: {ex.Message}");
                return null;
            }
        }
    }
}
