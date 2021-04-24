using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;
using AForge.Video.DirectShow;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Producer
{
    class Program
    {
        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        [DllImport("kernel32.dll")]

        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]

        static extern bool ShowWindow(IntPtr intPtr, int cmdShow);
            





        private static IPEndPoint iPEndPoint;
        private static UdpClient udpClient = new UdpClient();
        
        static void Main(string[] args)
        {
            var consumerIp = ConfigurationManager.AppSettings.Get("consumerIp");

            var consumerPort = int.Parse(ConfigurationManager.AppSettings.Get("consumerPort"));

            iPEndPoint = new IPEndPoint(IPAddress.Parse(consumerIp), consumerPort);

            Console.WriteLine($"consumer : {iPEndPoint}");

            FilterInfoCollection videoDevisez = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            VideoCaptureDevice captureDevice = new VideoCaptureDevice(videoDevisez[0].MonikerString);

            captureDevice.NewFrame += CaptureDevice_NewFrame;

            captureDevice.Start();

            Console.WriteLine("Нажмите на кнопку Enter для старта веб-камеры/////////");

            Console.ReadLine();

            ShowWindow(GetConsoleWindow(),SW_HIDE);

            
        }

        private static void CaptureDevice_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            var bmp = new Bitmap(eventArgs.Frame, 800, 600);

            try
            {
                using(var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Jpeg);
                    var bytes = ms.ToArray();

                    udpClient.Send(bytes, bytes.Length, iPEndPoint);

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    internal class DilImportAttribute : Attribute
    {
    }
}
