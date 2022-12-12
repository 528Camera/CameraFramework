using NetMQ.Sockets;
using ProtoBuf;
using Google.Protobuf;
using Emgu.CV;
using System.Net.Sockets;
using System.Net;

namespace CameraFramework
{
    public class Programm
    {
        public static void Main()
        {
            var port = 5557;
            string localIp = GetLocalIPAddress();
            using (var sender_img = new PushSocket("tcp://" + localIp + ":" + port.ToString()))//создаем push сокет
            {
                Console.WriteLine("Programm started");
                Console.WriteLine("Local Ip:" + localIp);
                Console.WriteLine("Port:" + port.ToString());
                GetImgTest(false);
            }
        }

        public static string GetLocalIPAddress()
        {
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint? endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            return localIP;
        }

        public static FrameData GetImgTest(bool multy) //тест дома на ноуте
        {
            var count = 0;
            bool one = true;
            FrameData frame = new FrameData();
            using (Emgu.CV.VideoCapture capture = new Emgu.CV.VideoCapture()) //подключаемся к видеокамере
            {
                while (one)
                {
                    var captured_img = capture.QueryFrame();
                    var img = captured_img.ToImage<Emgu.CV.Structure.Bgr, byte>();
                    var imgData = img.ToJpegData();
                    frame.Version = 1;
                    using (var str = new MemoryStream(imgData))
                    {
                        frame.Image = ByteString.FromStream(str);
                    }
                    frame.Time = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow);
                    frame.FrameIndex = count;
                    one = multy;
                }
            }
            return frame;
        }

        public static void GetImg()//фунция для камеры в 528 
        {
            var count = 0;
            using (Emgu.CV.VideoCapture capture = new Emgu.CV.VideoCapture("http://guest:guest@192.168.9.37/video1.mjpg")) //подключаемся к видеокамере
            {
                while (true)
                {
                    FrameData frame = new FrameData();
                    var captured_img = capture.QueryFrame();
                    var img = captured_img.ToImage<Emgu.CV.Structure.Bgr, byte>();
                    var imgData = img.ToJpegData();
                    frame.Version = 1;
                    using (var str = new MemoryStream(imgData))
                    {
                        frame.Image = ByteString.FromStream(str);
                    }
                    frame.Time = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.Now); 
                    frame.FrameIndex = count;
                    count++;
                }
            }
        }

    }
}