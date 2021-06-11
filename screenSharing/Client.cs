using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Drawing.Imaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Net;
using System.IO;

namespace screenSharing
{
    public partial class Client : Form
    {
        private TcpClient client = new TcpClient();
        private TcpClient client1 = new TcpClient();
        private NetworkStream ns;
        bool isClick = true;
        public Client()
        {
            InitializeComponent();
        }

        private static Bitmap screenCapture()
        {
            Rectangle bound = Screen.PrimaryScreen.Bounds;
            Bitmap screenShot = new Bitmap(bound.Width, bound.Height, PixelFormat.Format32bppArgb);
            Graphics graphic = Graphics.FromImage(screenShot);
            graphic.CopyFromScreen(bound.X, bound.Y, 0, 0, bound.Size, CopyPixelOperation.SourceCopy);
            return screenShot;
        }
        private byte[] imageCompress(Image img)
        {
            using (var ms = new MemoryStream())
            {
                Bitmap bmp = new Bitmap(img);
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                return ms.ToArray();
            }
        }
        private byte[] GetCompressedBitmap(Bitmap bmp, long quality)
        {
            using (var mss = new MemoryStream())
            {
                EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                ImageCodecInfo imageCodec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(o => o.FormatID == ImageFormat.Jpeg.Guid);
                EncoderParameters parameters = new EncoderParameters(1);
                parameters.Param[0] = qualityParam;
                bmp.Save(mss, imageCodec, parameters);
                return mss.ToArray();
            }
        }


        private void send()
        {

            BinaryFormatter binFor = new BinaryFormatter();
            ns = client.GetStream();
            Bitmap curr;
            int prevHash = 0, currHash = 0;
            while (isClick)
            {
                curr = screenCapture();

                currHash = curr.GetHashCode();
                if (currHash != prevHash)
                {
                    binFor.Serialize(ns, GetCompressedBitmap(curr, 60L));
                }
                prevHash = currHash;
            }

        }

        private void Client_Load(object sender, EventArgs e)
        {
            Thread server = new Thread(new ThreadStart(serverThread));
            server.Start();
        }

        private void serverThread()
        {
            TcpListener server = new TcpListener(IPAddress.Any, 8081);
            while (!client1.Connected)
            {
                server.Start();
                client1 = server.AcceptTcpClient();
            }
        }
        private void mouseMovement()
        {
            NetworkStream ns = client1.GetStream();
            BinaryFormatter binFor = new BinaryFormatter();
            while (true)
            {
                sendBack inf = (sendBack)binFor.Deserialize(ns);
                Point test = inf.getMouse();
                textBox1.Text = "X: " + test.X + " Y: " + test.Y;
                test.X = test.X * 1366 / 1856;
                test.Y = test.Y * 768 / 1054;
                Cursor.Position = test;
                if (inf.getLeftClick())
                {
                    Click c = new Click();
                    c.leftClick(test);
                }
                if (inf.getRightClick())
                {
                    Click c = new Click();
                    c.rightClick(test);
                }
                if (inf.getDoubleClick())
                {
                    Click c = new Click();
                    c.dobuleClick(test);
                }
                if (inf.getHoldClick())
                {
                    Click c = new Click();
                    c.holdClick(test);
                }
                if (inf.getWheelUp())
                {
                    Click c = new Click();
                    c.wheelUp();
                }
                if (inf.getWheelDown())
                {
                    Click c = new Click();
                    c.wheelDown();
                }
                if (inf.getKeyBoard() != "")
                {
                    string keysReceive = "{" + inf.getKeyBoard() + "}";
                    SendKeys.SendWait(keysReceive);
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {

            /*26.242.248.193*/
            /*192.168.1.11*/
            client.Connect("192.168.1.11", 8080);


        }
        private void button2_Click(object sender, EventArgs e)
        {

            Thread startSending = new Thread(new ThreadStart(send));
            startSending.Start();
            Thread mouse = new Thread(new ThreadStart(mouseMovement));
            mouse.Start();

        }

        private void Client_FormClosing(object sender, FormClosingEventArgs e)
        {
            isClick = false;
            ns.Close();
            client.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

