using LoginApiServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginApiServices.Services
{
    public interface IUserRepository
    {
        //  User Login(User Userdetails);
        String CreateUser(User Userdetails);
        User LoginDetails(string login);


    }
}
