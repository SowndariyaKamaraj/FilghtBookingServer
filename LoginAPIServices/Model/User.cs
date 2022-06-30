using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LoginApiServices
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        [NotMapped]
        public string RefreshToken { get; set; } = string.Empty;
        [NotMapped]
        public DateTime Created { get; set; }
        [NotMapped]
        public DateTime Expires { get; set; }

        //public string Password { get; set; }
        public string Role { get; set; }
        //[ForeignKey("")]
        public string EmailID { get; set; }
        public string Gender { get; set; }
        public string Age { get; set; }
        public long MobileNumber { get; set; }




    }
}
