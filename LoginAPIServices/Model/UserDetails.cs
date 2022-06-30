using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginApiServices
{
    public class Userdetails
    {
        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; }
        public string Role { get; set; }

        public string EmailID { get; set; }
        public string Gender { get; set; }
        public string Age { get; set; }
        public long MobileNumber { get; set; }


    }
}
