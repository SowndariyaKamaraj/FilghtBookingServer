using LoginApiServices;
using LoginApiServices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginApiServices.Services
{
    public class SqlRepository : IUserRepository
    {
        private readonly AppDbContext _appdbcontext;
        public SqlRepository(AppDbContext appdbcontext)
        {
            _appdbcontext = appdbcontext;
        }

        public string CreateUser(User Userdetails)
        {
            try
            {
                _appdbcontext.UserDet.Add(Userdetails);
                _appdbcontext.SaveChanges();
                return "Register Completed Successfully";
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public User LoginDetails(string login)
        {
            var loguser = _appdbcontext.UserDet.FirstOrDefault(x => x.UserName == login
           );
            return loguser;

        }

        //public User Login(User Userdetails)
        //{
        //    var loguser = _appdbcontext.UserDetails.FirstOrDefault(x => x.UserName == Userdetails.UserName
        //   && x.Password == Userdetails.Password);
        //    return loguser;
        //}

        // public User Register(User Userdetails)
        // {
        //     var registeruser = _appdbcontext.UserDetails.FirstOrDefault(x => x.UserName == Userdetails.UserName
        //&& x.Password == Userdetails.Password && x.EmailID == Userdetails.EmailID && x.Role == Userdetails.Role);
        //     return registeruser;
        // }

    }



}
