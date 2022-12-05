using NetMQ.Sockets;
using ProtoBuf;
using Google.Protobuf;

namespace CameraFramework
{
    public class Programm
    {
        public static void Main()
        {
            Console.WriteLine("Programm started");
            var sender_img = new PushSocket("tcp://localhost:5557");//создаем push сокет
                                                                    //new Emgu.CV.VideoCapture("http://guest:guest@192.168.9.37/video1.mjpg") для 528 аудитории
            CapturedImage data = new CapturedImage();
            using (Emgu.CV.VideoCapture capture = new Emgu.CV.VideoCapture()) //создаем захват видео 
            {
                var capture_img = capture.QueryFrame();
                var image = capture_img.ToImage<Emgu.CV.Structure.Bgr, byte>();
                var imageData = image.ToJpegData(); //take a picture
                data.Id = 1;
                data.Time = DateTime.Now;
                using (var str = new MemoryStream(imageData))
                {
                    data.Image = ByteString.FromStream(str);
                }
                //send объекта через zeromq
            }
        }

    }


    [ProtoContract]
    public class CapturedImage{
        [ProtoMember(1)]
        public int Id { get; set; }
        [ProtoMember(2)]
        public ByteString? Image { get; set; }
        [ProtoMember(3)]
        public DateTime Time { get; set; }
    }
}