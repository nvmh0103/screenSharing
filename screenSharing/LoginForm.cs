using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace screenSharing
{
    public partial class LoginForm : Form
    {
        static HttpClient client = new HttpClient();
        bool isSuccess = false;
        public LoginForm()
        {
            InitializeComponent();
            textBox2.PasswordChar = '*';
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var myForm = new RegsiterForm(client);
            myForm.Show();  
        }
        // get list of users
        /* List<Users> users = await GetProductAsync("https://csharpbe.herokuapp.com/getAllUsers");
            foreach (Users user in users)
            {
                MessageBox.Show(user.getEmail());
            }*/
        private async void button1_Click(object sender, EventArgs e)
        {
            Login loginCredentials = new Login(textBox1.Text, textBox2.Text);
            String email = textBox1.Text;
            await Login(loginCredentials);
            if (isSuccess)
            {
                Hide();
                var myForm = new ListOfUsers(email);
                myForm.Show();
            }
            
        }
        
        async Task<Uri> Login(Login loginCredentials)
        {

            HttpResponseMessage response = await client.PostAsJsonAsync("https://csharpbe.herokuapp.com/login", loginCredentials);
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Login success !", "Message");
                isSuccess = true;
            }
            else
            {
                MessageBox.Show("Email or password is incorrect!", "Message");
                
            }
            return response.Headers.Location;

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.PasswordChar = checkBox1.Checked ? '\0' : '*';
        }
    }
}
