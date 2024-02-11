using Emgu.CV;
using Emgu.CV.Structure;
using System.Text;

namespace BadApple
{
    internal class Program
    {
        public int height = 360;
        public int width = 480;

        static void Main(string[] args)
        {
            var p = new Program();
            Console.WriteLine("Path for video: ");
            var pathVideo = Console.ReadLine();
            var capture = new VideoCapture(pathVideo);
            p.height = capture.Height;
            p.width = capture.Width;

            p.GetFramesAndSave(capture);
            p.SavedFramesInFile();
            p.PlayInConsole();
        }

        public void GetFramesAndSave(VideoCapture capture)
        {
            Console.WriteLine("Path for frames: ");
            var pathFrame = Console.ReadLine();

            for (int framecount = 0; ; framecount++)
            {
                Image<Gray, byte>? frame = capture.QueryFrame()?.ToImage<Gray, byte>();

                if (frame == null) break;

                frame.Save((pathFrame + GetPathToFrame(framecount)));
                Console.Write($"Frame: {framecount}  saved");
            }
        }

        public void SavedFramesInFile()
        {
            Console.WriteLine("Path folder with frames: ");
            var pathFrame = Console.ReadLine();

            using (BinaryWriter writer = new BinaryWriter(File.Open(pathFrame + "/data.txt", FileMode.Create), System.Text.Encoding.UTF8))
            {
                for (int i = 0; ; i++)
                {
                    string path = pathFrame + GetPathToFrame(i);

                    if (!File.Exists(path)) break;

                    Image<Gray, byte> frame = new(pathFrame + GetPathToFrame(i));

                    for (int y = 0; y < frame.Height; y++)
                    {
                        for (int x = 0; x < frame.Width; x++)
                        {
                            Gray pixel = frame[y, x];

                            bool isBlack = pixel.Intensity >= 150;

                            writer.Write(isBlack ? '0' : '1');
                        }
                    }


                    Console.WriteLine($"Frame: {i}  write in file");
                }
            }
        }

        public void PlayInConsole()
        {
            Console.WriteLine("Path data: ");
            var pathData = Console.ReadLine();
            StringBuilder frame = new StringBuilder();
            frame.Capacity = height * width + 1;

            using (BinaryReader reader = new BinaryReader(File.Open(pathData, FileMode.Open)))
            {
                while (true)
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            bool isBlack = reader.ReadBoolean();

                            if (isBlack)
                            {
                                frame.Append('&');
                            }
                            else
                            {
                                frame.Append(' ');
                            }
                        }
                        frame.Append('\n');
                    }
                    Console.WriteLine(frame);
                    frame.Remove(0, frame.Length);
                }
            }
        }

        public string GetPathToFrame(int frameNumber)
        {
            return ("/frame" + frameNumber.ToString("00000") + ".jpg");
        }

    }
}
