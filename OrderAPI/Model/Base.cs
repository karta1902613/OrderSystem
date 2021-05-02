using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAPI.Model
{
    public class Login
    {
        public string userId { get; set; }
        public string userPassword { get; set; }
        public string sysUserId { get; set; }       
        public string userEmail { get; set; }       
    }
}
