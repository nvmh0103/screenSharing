
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
using System.Threading;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.InteropServices;

namespace screenSharing
{
    public partial class Viewer : Form
    {
        private TcpClient client;
        private TcpListener server;
        private NetworkStream mainStream;

        private Thread Listen;
        private Thread getImage;
        public Viewer()
        {
            InitializeComponent();
            client = new TcpClient();
            Listen = new Thread(startListening);
            getImage = new Thread(receiveImage);
            this.pictureBox1.MouseWheel += pictureBox1_MouseWheel;
        }
        private void startListening()
        {
            while (!client.Connected)
            {
                server.Start();
                client = server.AcceptTcpClient();
            }
            getImage.Start();
        }
        private void stopListening()
        {
            server.Stop();
            client = null;
            if (Listen.IsAlive)
                Listen.Abort();
            if (getImage.IsAlive)
                getImage.Abort();

        }
        private Image ByteToImage(byte[] incoming)
        {
            Bitmap bmp;
            using (var ms = new MemoryStream(incoming))
            {
                bmp = new Bitmap(ms);
            }
            return bmp;
        }
        private void receiveImage()
        {
            connect();
            BinaryFormatter binFor = new BinaryFormatter();
            while (client.Connected)
            {
                mainStream = client.GetStream();
                byte[] receive = (byte[])binFor.Deserialize(mainStream);
                pictureBox1.Image = (Image)ByteToImage(receive);
            }
        }

        private void Viewer_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            server = new TcpListener(IPAddress.Any, 8080);
            Listen.Start();

        }

        private void Viewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            stopListening();
        }

        private void Viewer_MouseMove(object sender, MouseEventArgs e)
        {

        }


        TcpClient client1 = new TcpClient();
        private void connect()
        {
            /*26.249.38.179*/
            /*192.168.1.10*/
            client1.Connect("192.168.1.10", 8081);

        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (client1.Connected)
            {
                NetworkStream ns1 = client1.GetStream();
                BinaryFormatter binFor = new BinaryFormatter();
                Point cursor = new Point(e.X, e.Y);
                if (e.Button == MouseButtons.Left)
                {
                    sendBack inf1 = new sendBack(cursor, false, false, false, true,false,false,"");
                    binFor.Serialize(ns1, inf1);
                    return;
                }
                sendBack inf = new sendBack(cursor, false, false, false,false,false,false,"");
                binFor.Serialize(ns1, inf);
            }
        }
        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            NetworkStream ns1 = client1.GetStream();
            BinaryFormatter binFor = new BinaryFormatter();
            Point cursor = new Point(e.X, e.Y);
            if (e.Delta > 0)
            {
                sendBack inf = new sendBack(cursor, false, false, false, false, true, false,"");
                binFor.Serialize(ns1, inf);
            } else
            {
                sendBack inf = new sendBack(cursor, false, false, false, false, false, true,"");
                binFor.Serialize(ns1, inf);
            }
        }
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (client1.Connected)
            {
                NetworkStream ns1 = client1.GetStream();
                BinaryFormatter binFor = new BinaryFormatter();
                Point cursor = new Point(e.X, e.Y);
                if (e.Button == MouseButtons.Left)
                {
                    sendBack inf = new sendBack(cursor, true, false, false,false,false,false,"");
                    binFor.Serialize(ns1, inf);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    sendBack inf1 = new sendBack(cursor, false, false, true,false,false,false,"");
                    binFor.Serialize(ns1, inf1);
                }
            }
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (client1.Connected)
            {
                NetworkStream ns1 = client1.GetStream();
                BinaryFormatter binFor = new BinaryFormatter();
                Point cursor = new Point(e.X, e.Y);
                sendBack inf = new sendBack(cursor, false, true, false,false,false,false,"");
                binFor.Serialize(ns1, inf);
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            
        }

        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            pictureBox1.Focus();
        }
        

        private void pictureBox1_KeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            
            
                Point p = System.Windows.Forms.Control.MousePosition;
                NetworkStream ns1 = client1.GetStream();
                BinaryFormatter binFor = new BinaryFormatter();
                string keyPressed = KeyCodeToUnicode(e.KeyCode);
                sendBack inf = new sendBack(p, false, false, false, false, false, false, keyPressed);
                binFor.Serialize(ns1, inf);
            
            
        }
        public string KeyCodeToUnicode(Keys key)
        {
            byte[] keyboardState = new byte[255];
            bool keyboardStateStatus = GetKeyboardState(keyboardState);

            if (!keyboardStateStatus)
            {
                return "";
            }

            uint virtualKeyCode = (uint)key;
            uint scanCode = MapVirtualKey(virtualKeyCode, 0);
            IntPtr inputLocaleIdentifier = GetKeyboardLayout(0);

            StringBuilder result = new StringBuilder();
            ToUnicodeEx(virtualKeyCode, scanCode, keyboardState, result, (int)5, (uint)0, inputLocaleIdentifier);

            return result.ToString();
        }

        [DllImport("user32.dll")]
        static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll")]
        static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);

    }

}
