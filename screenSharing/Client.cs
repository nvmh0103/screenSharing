
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
using System.Collections.Specialized;
using System.Net.Http;
using Newtonsoft.Json;

namespace screenSharing
{
    public partial class Client : Form
    {
        static HttpClient httpclient = new HttpClient();
        private TcpClient client;
        private TcpListener server;
        private NetworkStream mainStream;

        private Thread Listen;
        private Thread getImage;

        private StringCollection filenames = new StringCollection();
        string currentUser;
        Users activeUser;
        public Client(String users)
        {
            InitializeComponent();
            // Thread for client
            client = new TcpClient();
            //Thread for server
            Listen = new Thread(startListening);
            // Start sending image
            getImage = new Thread(receiveImage);
            this.pictureBox1.MouseWheel += pictureBox1_MouseWheel;
            currentUser = users;
        }
        // Call api for getting list of all users
        static async Task<List<Users>> GetAllUsers(string path)
        {
            string responseUser = "";
            List<Users> userList = new List<Users>();
            HttpResponseMessage response = await httpclient.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                responseUser = await response.Content.ReadAsStringAsync();
            }
            userList = JsonConvert.DeserializeObject<List<Users>>(responseUser);
            return userList;
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
                //Receive image
                byte[] receive = (byte[])binFor.Deserialize(mainStream);
                // call ByteToImage to convert array of byte into image
                pictureBox1.Image = (Image)ByteToImage(receive);
            }
        }

        private void Viewer_Load(object sender, EventArgs e)
        {
            // get the profile of connecting client.
            List<Users> users = Task.Run(async () => await GetAllUsers("https://csharpbe.herokuapp.com/getAllUser")).Result;
            foreach (Users user in users)
            {
                if (user.getEmail() == currentUser)
                {
                    activeUser = user;
                    break;
                }
            }
            // maximize windows
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

        // init a new connect 
        TcpClient receiveMouseAndKeyboard = new TcpClient();
        private void connect()
        {
            /*26.249.38.179*/
            /*192.168.1.10*/
            //192.168.70.128
            //use this connect to get mouse and keyboard control
            receiveMouseAndKeyboard.Connect(activeUser.getIpAddress(), 8081);

        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        // capture mouse movement
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (receiveMouseAndKeyboard.Connected)
            {
                NetworkStream sendBackStream = receiveMouseAndKeyboard.GetStream();
                BinaryFormatter binFor = new BinaryFormatter();
                Point cursor = new Point(e.X, e.Y);
                // trigger a drag and drop mouse event
                if (e.Button == MouseButtons.Left)
                {
                    sendBack inf1 = new sendBack(cursor, false, false, false, true,false,false,"");
                    binFor.Serialize(sendBackStream, inf1);
                    return;
                }
                // send mouse coordinate 
                sendBack inf = new sendBack(cursor, false, false, false,false,false,false,"");
                binFor.Serialize(sendBackStream, inf);
            }
        }
        //Trigger scroll event
        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            NetworkStream sendBackStream = receiveMouseAndKeyboard.GetStream();
            BinaryFormatter binFor = new BinaryFormatter();
            Point cursor = new Point(e.X, e.Y);
            // scroll forward
            if (e.Delta > 0)
            {
                sendBack inf = new sendBack(cursor, false, false, false, false, true, false,"");
                binFor.Serialize(sendBackStream, inf);
            } else //scroll backward
            {
                sendBack inf = new sendBack(cursor, false, false, false, false, false, true,"");
                binFor.Serialize(sendBackStream, inf);
            }
        }
        // Trigger mouse click
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (receiveMouseAndKeyboard.Connected)
            {
                NetworkStream sendBackStream = receiveMouseAndKeyboard.GetStream();
                BinaryFormatter binFor = new BinaryFormatter();
                Point cursor = new Point(e.X, e.Y);
                // Left click
                if (e.Button == MouseButtons.Left)
                {
                    sendBack inf = new sendBack(cursor, true, false, false,false,false,false,"");
                    binFor.Serialize(sendBackStream, inf);
                }
                // right click
                else if (e.Button == MouseButtons.Right)
                {
                    sendBack inf1 = new sendBack(cursor, false, false, true,false,false,false,"");
                    binFor.Serialize(sendBackStream, inf1);
                }
            }
        }
        // Trigger mouse double click
        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (receiveMouseAndKeyboard.Connected)
            {
                NetworkStream sendBackStream = receiveMouseAndKeyboard.GetStream();
                BinaryFormatter binFor = new BinaryFormatter();
                Point cursor = new Point(e.X, e.Y);
                sendBack inf = new sendBack(cursor, false, true, false,false,false,false,"");
                binFor.Serialize(sendBackStream, inf);
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            
        }

        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            pictureBox1.Focus();
        }
        
        // Gửi input
        private void pictureBox1_KeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.V && ModifierKeys.HasFlag(Keys.Control))
            {
                bool FileTransfer = false;
                IDataObject DataObject = Clipboard.GetDataObject();

                if (DataObject.GetDataPresent(DataFormats.UnicodeText))
                {
                    TcpClient TransferConnection = new TcpClient();
                    TransferConnection.Connect(activeUser.getIpAddress(), 8082);

                    NetworkStream TransferStream = TransferConnection.GetStream();
                    BinaryFormatter BinFor = new BinaryFormatter();

                    string text = (string)DataObject.GetData(DataFormats.UnicodeText);
                    Data SendData = new Data(0, text);

                    BinFor.Serialize(TransferStream, SendData);

                    // Tín hiệu bên client đã xử lý xong
                    object signal = BinFor.Deserialize(TransferStream);

                    TransferStream.Close();
                    TransferConnection.Close();
                }

                if (DataObject.GetDataPresent(DataFormats.Bitmap))
                {
                    TcpClient TransferConnection = new TcpClient();
                    TransferConnection.Connect(activeUser.getIpAddress(), 8082);

                    NetworkStream TransferStream = TransferConnection.GetStream();
                    BinaryFormatter BinFor = new BinaryFormatter();

                    Bitmap image = (Bitmap)DataObject.GetData(DataFormats.Bitmap);
                    Data SendData = new Data(1, image);

                    BinFor.Serialize(TransferStream, SendData);

                    // Tín hiệu bên client đã xử lý xong
                    object signal = BinFor.Deserialize(TransferStream);

                    TransferStream.Close();
                    TransferConnection.Close();
                }

                if (DataObject.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] paths = (string[])DataObject.GetData(DataFormats.FileDrop);
                    int NumOfFiles = paths.Length;

                    string[] files = new string[NumOfFiles];

                    for (int i = 0; i < files.Length; i++)
                        files[i] = Path.GetFileName(paths[i]);

                    for (int i = 0; i < NumOfFiles; i++)
                    {
                        Data FileHeader = new Data(2, files[i]);

                        TcpClient client = new TcpClient();
                        client.Connect(activeUser.getIpAddress(), 8082);

                        NetworkStream ns = client.GetStream();
                        BinaryFormatter BinFor = new BinaryFormatter();

                        BinFor.Serialize(ns, FileHeader);

                        client.Client.SendFile(paths[i]);

                        ns.Close();
                        client.Close();
                    }

                    FileTransfer = true;
                }

                if (FileTransfer == true)
                {
                    TcpClient SignalClient = new TcpClient();
                    SignalClient.Connect(activeUser.getIpAddress(), 8082);
                    NetworkStream SignalStream = SignalClient.GetStream();
                    BinaryFormatter bf = new BinaryFormatter();

                    Data signal = new Data(3, string.Empty);

                    bf.Serialize(SignalStream, signal);

                    bf.Deserialize(SignalStream);

                    SendKeys(e);

                    MessageBox.Show("Files successfully transferred.");
                }

                else
                    SendKeys(e);

            }

            // Lấy dữ liệu từ client
            else if ((e.KeyCode == Keys.C || e.KeyCode == Keys.X) && ModifierKeys.HasFlag(Keys.Control))
            {
                SendKeys(e);
                
                TcpClient TransferConnection = new TcpClient();
                TransferConnection.Connect(activeUser.getIpAddress(), 8082);

                NetworkStream TransferStream = TransferConnection.GetStream();
                BinaryFormatter BinFor = new BinaryFormatter();

                Data signal = new Data(4, string.Empty);

                BinFor.Serialize(TransferStream, signal);

                Data RecvData = (Data)BinFor.Deserialize(TransferStream);

                if (RecvData.GetDataType() == 0)
                {
                    string ClipboardText = (string)RecvData.GetData();
                    Clipboard.SetText(ClipboardText, TextDataFormat.UnicodeText);

                    TransferStream.Close();
                    TransferConnection.Close();
                }

                if (RecvData.GetDataType() == 1)
                {
                    Bitmap ClipboardImage = (Bitmap)RecvData.GetData();
                    Clipboard.SetImage(ClipboardImage);

                    TransferStream.Close();
                    TransferConnection.Close();
                }

                if (RecvData.GetDataType() == 2)
                {
                    string filename = (string)RecvData.GetData();
                    string tmpDir = Path.GetTempPath();
                    int status = RecvData.GetStatus();

                    filenames.Add(tmpDir + filename);

                    using (var output = File.Create(tmpDir + filename))
                    {
                        var buffer = new byte[1024];
                        int bytesRead;
                        while ((bytesRead = TransferStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            output.Write(buffer, 0, bytesRead);
                        }
                    }

                    TransferStream.Close();
                    TransferConnection.Close();

                    do
                    {
                        TcpClient client = new TcpClient();
                        client.Connect(activeUser.getIpAddress(), 8082);

                        NetworkStream ns = client.GetStream();
                        BinaryFormatter bf = new BinaryFormatter();

                        Data NextData = new Data(4, string.Empty);

                        bf.Serialize(ns, NextData);

                        NextData = (Data)bf.Deserialize(ns);

                        filename = (string)NextData.GetData();
                        tmpDir = Path.GetTempPath();
                        status = NextData.GetStatus();

                        filenames.Add(tmpDir + filename);

                        using (var output = File.Create(tmpDir + filename))
                        {
                            var buffer = new byte[1024];
                            int bytesRead;
                            while ((bytesRead = ns.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                output.Write(buffer, 0, bytesRead);
                            }
                        }

                        ns.Close();
                        client.Close();
                    }
                    while (status != 0);

                    /* Set clipboard cut operation */
                    /*-----------------------------------------------------------------------*/
                    byte[] moveEffect = { 2, 0, 0, 0 };
                    MemoryStream dropEffect = new MemoryStream();
                    dropEffect.Write(moveEffect, 0, moveEffect.Length);

                    DataObject data = new DataObject("Preferred DropEffect", dropEffect);
                    data.SetFileDropList(filenames);
                    /*-----------------------------------------------------------------------*/

                    Clipboard.SetDataObject(data, true);

                    MessageBox.Show("Files successfully copied.");

                    filenames.Clear();
                } 
            }
            else
                SendKeys(e);
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

        void SendKeys(PreviewKeyDownEventArgs e)
        {
            Point p = System.Windows.Forms.Control.MousePosition;
            NetworkStream sendBackStream = receiveMouseAndKeyboard.GetStream();
            BinaryFormatter binFor = new BinaryFormatter();
            if (e.KeyCode == Keys.Enter)
            {
                sendBack inf1 = new sendBack(p, false, false, false, false, false, false, "enter");
                binFor.Serialize(sendBackStream, inf1);
                return;
            }
            string keyPressed = KeyCodeToUnicode(e.KeyCode);
            sendBack inf = new sendBack(p, false, false, false, false, false, false, keyPressed);
            binFor.Serialize(sendBackStream, inf);
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
