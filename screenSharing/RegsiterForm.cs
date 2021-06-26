using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
namespace screenSharing
{
    public partial class RegsiterForm : Form
    {
        HttpClient newClient;
        public RegsiterForm(HttpClient client)
        {
            InitializeComponent();
            newClient = client;
            textBox4.PasswordChar = '*';
            textBox5.PasswordChar = '*';
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            String ipAddress="", res;
            int _ScreenWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            int _ScreenHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            res = _ScreenWidth + "x" + _ScreenHeight;
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    Console.WriteLine(ni.Name);
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            if (ip.Address.ToString().Contains("25"))
                            {
                                ipAddress = (ip.Address.ToString()) + " ";
                            }

                        }
                    }
                }
            }
            if (textBox4.Text != textBox5.Text)
            {
                MessageBox.Show("Password doesnt match!","Error");
                return;
            }
            Users userDetails = new Users(textBox1.Text, textBox2.Text, textBox4.Text, ipAddress, res);
            await createUser(userDetails);
            Close();

        }
        async Task<Uri> createUser(Users usersDetails)
        {
            var content = new StringContent(JsonConvert.SerializeObject(usersDetails).ToString(), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await newClient.PostAsync("https://csharpbe.herokuapp.com/createUser", content);
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show(" Create successfully!", "Message");
            }
            else
            {
                MessageBox.Show("Account cant be created!", "Message");
            }
            return response.Headers.Location;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox4.PasswordChar = checkBox1.Checked ? '\0' : '*'; 
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            textBox5.PasswordChar = checkBox2.Checked ? '\0' : '*';
        }
    }
}
