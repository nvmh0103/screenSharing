using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace screenSharing
{
    public partial class ListOfUsers : Form
    {
        static HttpClient client = new HttpClient();
        String currentUser;
        String connectingUser;
        Users connectingTo;
        private TcpClient networkClient = new TcpClient();
        List<Users> users = new List<Users>();
        private TcpClient receiveClient= new TcpClient();
        private TcpListener server;
        private Thread Listen;
        public ListOfUsers(String email)
        {
            InitializeComponent();
            currentUser = email;
            label1.Text += email + "!";
            richTextBox1.ReadOnly = true;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            Listen = new Thread(startListening);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void clientReceiveMessage()
        {
            if (networkClient.Connected)
            {
                NetworkStream ns = networkClient.GetStream();
                byte[] recv = new byte[100];
                ns.Read(recv, 0, recv.Length);
                ns.Close();
                networkClient.Close();
                receiveClient.Close();
                server.Stop();
                string response = Encoding.UTF8.GetString(recv);
                if (response.TrimEnd('\n','\r','\0')=="ok")
                {
                    var myform = new Client(connectingUser);
                    myform.ShowDialog();
                }
            }
        }
        private void serverReceiveMessage()
        {
            if (receiveClient.Connected)
            {
                NetworkStream ns = receiveClient.GetStream();
                byte[] receive = new byte[100];
                ns.Read(receive, 0, receive.Length);
                string text = Encoding.UTF8.GetString(receive);
                if (MessageBox.Show(text,"Incoming connection",MessageBoxButtons.YesNo)== DialogResult.Yes)
                {
                    string response = "ok";
                    byte[] sendData = Encoding.UTF8.GetBytes(response);
                    ns.Write(sendData, 0, sendData.Length);
                    string[] stringSpilt = text.Split(new[] { 'm' },2);
                    var myform = new Server(currentUser,stringSpilt[1]);
                    myform.ShowDialog();
                }
            }
        }
        static async Task<List<Users>> GetAllUsers(string path)
        {
            string responseUser = "";
            List<Users> userList = new List<Users>();
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                responseUser = await response.Content.ReadAsStringAsync();
            }
            userList = JsonConvert.DeserializeObject<List<Users>>(responseUser);
            return userList;
        }
        private void ListOfUsers_Load(object sender, EventArgs e)
        {
            users = Task.Run(async () => await GetAllUsers("https://csharpbe.herokuapp.com/getAllUser")).Result;
            
            foreach (Users user in users)
            {
                if (user.getEmail().Equals(currentUser))
                {
                    continue;
                }
                richTextBox1.Text += user.getEmail() + '\n';
                comboBox1.Items.Add(user.getEmail());
                

            }
            server = new TcpListener(IPAddress.Any, 8080);

        }
        private void startListening()
        {
            while (!receiveClient.Connected)
            {
                server.Start();
                receiveClient = server.AcceptTcpClient();
            }
            Thread recvMess = new Thread(new ThreadStart(serverReceiveMessage));
            recvMess.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Listen.Suspend();

            foreach (Users user in users)
            {
                if (user.getEmail().Equals(comboBox1.Text))
                {
                    connectingUser = user.getEmail();
                    connectingTo = user;
                    break;
                }

            }
            networkClient.Connect(connectingTo.getIpAddress(), 8080);
            Thread clientListen = new Thread(new ThreadStart(clientReceiveMessage));
            clientListen.Start();
            if (networkClient.Connected)
            {
                NetworkStream ns = networkClient.GetStream();
                String connect = "Connect from " + currentUser;
                byte[] sendData = Encoding.UTF8.GetBytes(connect);
                ns.Write(sendData, 0, sendData.Length);
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Listen.Start();
        }
    }
}
