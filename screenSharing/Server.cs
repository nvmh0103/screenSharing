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
using System.Collections.Specialized;
using System.Net.Http;
using Newtonsoft.Json;

namespace screenSharing
{
    public partial class Server : Form
    {
        private TcpClient client = new TcpClient();
        private TcpClient client1 = new TcpClient();
        private NetworkStream ns;
        static HttpClient httpClient = new HttpClient();
        bool isClick = true;
        string userClientGlobal;
        string userServerGlobal;

        Users serverUser;


        private StringCollection filenames = new StringCollection();
        public Server(String clientUser,String serverUser)
        {
            InitializeComponent();
            userServerGlobal = serverUser.Trim('\n','\r','\0',' ');


        }

        static async Task<List<Users>> GetAllUsers(string path)
        {
            string responseUser = "";
            List<Users> userList = new List<Users>();
            HttpResponseMessage response = await httpClient.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                responseUser = await response.Content.ReadAsStringAsync();
            }
            userList = JsonConvert.DeserializeObject<List<Users>>(responseUser);
            return userList;
        }

        // Capture current screen and save as bitmap
        private static Bitmap screenCapture()
        {
            Rectangle bound = Screen.PrimaryScreen.Bounds;
            Bitmap screenShot = new Bitmap(bound.Width, bound.Height, PixelFormat.Format32bppArgb);
            Graphics graphic = Graphics.FromImage(screenShot);
            graphic.CopyFromScreen(bound.X, bound.Y, 0, 0, bound.Size, CopyPixelOperation.SourceCopy);
            return screenShot;
        }
        // Compress bitmap with jpeg encode for lossing to improve quaility
        private byte[] GetCompressedBitmap(Bitmap bmp, long quality)
        {
            using (var mss = new MemoryStream())
            {
                EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                ImageCodecInfo imageCodec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(o => o.FormatID == ImageFormat.Jpeg.Guid);
                EncoderParameters parameters = new EncoderParameters(1);
                parameters.Param[0] = qualityParam;
                bmp.Save(mss, imageCodec, parameters);
                // return as an array
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
                // get hashcode for current screenshot
                currHash = curr.GetHashCode();
                // if the screenshot is the same, we wont send it to improve speed
                if (currHash != prevHash)
                {
                    // sending screenshot with quality of 60.
                    binFor.Serialize(ns, GetCompressedBitmap(curr, 60L));
                }
                prevHash = currHash;
            }

        }

        private void Client_Load(object sender, EventArgs e)
        {
            Thread server = new Thread(new ThreadStart(serverThread));
            server.Start();

            
            // get profile of client and server 
            List<Users> users = Task.Run(async () => await GetAllUsers("https://csharpbe.herokuapp.com/getAllUser")).Result;

            foreach (Users user in users)
            {
                if (user.getEmail().Equals(userServerGlobal))
                {
                    serverUser = user;
                }
            }

            label1.Text = "Accepted connection from " + serverUser.getEmail();

            /*192.168.1.9*/
            client.Connect(serverUser.getIpAddress(), 8080);

            Thread startSending = new Thread(new ThreadStart(send));
            startSending.Start();


            // Bắt đầu nhận Data Connection
            Thread RecvData = new Thread(new ThreadStart(RecvFileConnection));
            RecvData.Start();
        }

        private void RecvFileConnection()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 8082);

            listener.Start();
            while (true)
            {
                TcpClient RecvClient = listener.AcceptTcpClient();

                Thread RecvThread = new Thread(StartTransfering);
                RecvThread.SetApartmentState(ApartmentState.STA);
                RecvThread.Start(RecvClient);
            }
        }

        private void serverThread()
        {
            TcpListener server = new TcpListener(IPAddress.Any, 8081);
            while (!client1.Connected)
            {
                server.Start();
                client1 = server.AcceptTcpClient();
            }
            Thread mouse = new Thread(new ThreadStart(mouseMovement));
            mouse.Start();
        }
        private void mouseMovement()
        {
            NetworkStream ns = client1.GetStream();
            BinaryFormatter binFor = new BinaryFormatter();
            while (true)
            {
                sendBack inf = (sendBack)binFor.Deserialize(ns);
                Point current = inf.getMouse();
                // do calculate to make mouse move correctly.
                int clientX, clientY, serverX, serverY;
                string[] stringSplitServer = inf.getRes().Split('x');
                clientX = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                clientY = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
                serverX = Int32.Parse(stringSplitServer[0]);
                serverY = Int32.Parse(stringSplitServer[1]);

                // change mouse coordinate base on client and server resolution
                
                 current.X = current.X * clientX / serverX-20;
                 current.Y = current.Y * clientY / serverY-20;
               

                
                Cursor.Position = current;
                if (inf.getLeftClick())
                {
                    Click c = new Click();
                    c.leftClick(current);
                }
                if (inf.getRightClick())
                {
                    Click c = new Click();
                    c.rightClick(current);
                }
                if (inf.getDoubleClick())
                {
                    Click c = new Click();
                    c.dobuleClick(current);
                }
                if (inf.getHoldClick())
                {
                    Click c = new Click();
                    c.holdClick(current);
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
                    string keysReceive = "";
                    if (inf.getCtrl())
                    {
                        keysReceive = "{CTRL}";
                    } else if (inf.getKeyBoard() == " ")
                    {
                        keysReceive = " ";
                    }
                    else if (inf.getKeyBoard() == "\t")
                    {
                        keysReceive = "{tab}";
                    }
                    else
                    {
                        keysReceive += "{" + inf.getKeyBoard() + "}";
                    }
                    SendKeys.SendWait(keysReceive);
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {

            /*26.242.248.193*/
            /*192.168.1.11*/
            


        }
        private void button2_Click(object sender, EventArgs e)
        {

            

        }

        private void StartTransfering(object obj)
        {
            TcpClient TransferConnection = (TcpClient)obj;

            NetworkStream TransferStream = TransferConnection.GetStream();
            BinaryFormatter BinFor = new BinaryFormatter();

            Data RecvData = (Data)BinFor.Deserialize(TransferStream);

            if (RecvData.GetDataType() == 0)
            {
                string ClipboardText = (string)(RecvData.GetData());
                Clipboard.SetText(ClipboardText, TextDataFormat.UnicodeText);

                // Gửi tín hiệu xử lý xong
                BinFor.Serialize(TransferStream, 0);

                TransferStream.Close();
                TransferConnection.Close();
            }

            if (RecvData.GetDataType() == 1)
            {
                Bitmap ClipboardImage = (Bitmap)(RecvData.GetData());
                Clipboard.SetImage(ClipboardImage);

                // Gửi tín hiệu xử lý xong
                BinFor.Serialize(TransferStream, 0);

                TransferStream.Close();
                TransferConnection.Close();
            }

            if (RecvData.GetDataType() == 2)
            {
                /* Lưu file vào temp folder trước, rồi thực hiện cut */
                string filename = (string)RecvData.GetData();
                string tmpDir = Path.GetTempPath();

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
            }

            if (RecvData.GetDataType() == 3)
            {
                /* Set clipboard operation = cut */
                /*----------------------------------------------------------------------*/
                byte[] moveEffect = { 2, 0, 0, 0 };

                MemoryStream dropEffect = new MemoryStream();
                dropEffect.Write(moveEffect, 0, moveEffect.Length);

                DataObject data = new DataObject("Preferred DropEffect", dropEffect);
                data.SetFileDropList(filenames);
                /*----------------------------------------------------------------------*/

                Clipboard.SetDataObject(data, true);
                filenames.Clear();

                BinFor.Serialize(TransferStream, 0);

                TransferStream.Close();
                TransferConnection.Close();
            }

            // Truyền dữ liệu từ client sang server
            if (RecvData.GetDataType() == 4)
            {
                IDataObject DataObject = Clipboard.GetDataObject();

                if (DataObject.GetDataPresent(DataFormats.UnicodeText))
                {
                    string text = (string)DataObject.GetData(DataFormats.UnicodeText);
                    Data SendData = new Data(0, text);

                    BinFor.Serialize(TransferStream, SendData);

                    TransferStream.Close();
                    TransferConnection.Close();
                }

                if (DataObject.GetDataPresent(DataFormats.Bitmap))
                {
                    Bitmap image = (Bitmap)DataObject.GetData(DataFormats.Bitmap);
                    Data SendData = new Data(1, image);

                    BinFor.Serialize(TransferStream, SendData);

                    TransferStream.Close();
                    TransferConnection.Close();
                }

                if (DataObject.GetDataPresent(DataFormats.FileDrop))
                {
                    Data FileHeader = null;

                    if (filenames.Count == 0)
                        filenames = Clipboard.GetFileDropList();

                    string filename = Path.GetFileName(filenames[0]);

                    if (filenames.Count > 1)
                        FileHeader = new Data(2, 1, filename);
                    else
                        FileHeader = new Data(2, 0, filename);
                    
                    BinFor.Serialize(TransferStream, FileHeader);

                    TransferConnection.Client.SendFile(filenames[0]);

                    TransferStream.Close();
                    TransferConnection.Close();

                    filenames.RemoveAt(0);
                }
            }
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

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}