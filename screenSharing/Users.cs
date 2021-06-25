using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace screenSharing
{
    [Serializable]
    class Users
    {
        public String name { get; set; }
        public String email { get; set; }
        public String password { get; set; }
        public String ipAddress { get; set; }
        public String res { get; set; }
        public Users(String name,String email,String password,String ipAddress,String res)
        {
            this.name = name;
            this.email = email;
            this.password = password;
            this.ipAddress = ipAddress;
            this.res = res;
        }
        public String getName()
        {
            return this.name;
        }
        public String getEmail()
        {
            return this.email;
        }
        public String getIpAddress()
        {
            return this.ipAddress;
        }
        public String getRes()
        {
            return this.res;
        }
        
    }
}
