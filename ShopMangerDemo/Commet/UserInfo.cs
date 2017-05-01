using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopMangerDemo
{
    class UserInfo
    {
        public  string username;
        public  string userpwd;

        public string UserName
        {
            get { return username; }
            set { username = value; }
        }
        public string UserPwd
        {
            get { return userpwd; }
            set { userpwd = value; }
        }

        public UserInfo(string names, string pass)
        {
            this.userpwd = pass;
            this.username = names;
        }

        public UserInfo()
        {
            
        }
    }
}
