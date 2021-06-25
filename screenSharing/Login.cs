using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace screenSharing
{
    class Login
    {
        public String email { get; set; }
        public String password { get; set; }
     
        public Login(String email,String password)
        {
            this.email = email;
            this.password = password;
        }
        public String getEmail()
        {
            return this.email;
        }
        public String getPassword()
        {
            return this.password;
        }
    }
}
