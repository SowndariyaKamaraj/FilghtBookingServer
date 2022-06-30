using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using LoginApiServices.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime;
using System.Threading.Tasks;

namespace LoginApiServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public User user = new User();

        private readonly IConfiguration _config;

        private readonly IUserRepository _userrepository;

        public AuthController(IConfiguration configuration, IUserRepository userrepository)
        {
            _config = configuration;
            _userrepository = userrepository;

        }


        [HttpPost("Login")]
        public ActionResult Login(Userdetails request)
        {
            try
            {
                var name = _userrepository.LoginDetails(request.UserName);
                user = name;
                

                if (!verifyPasswordHash(request.Password, name.PasswordHash, name.PasswordSalt))
                {
                    return BadRequest();
                }

                string token = CreateToken(name);
                var refreshtoken = GenerateRefreshToken();
                SetFefreshToken(refreshtoken);
                return Ok(new { Token = token, role = user.Role,user=user.UserId,user.UserName });

            }
            catch(Exception ex)
            {
                throw ex;
            }
         
        }


        [HttpPost("Register")]

        public ActionResult Register(Userdetails request)
        {
            //var register = _registerrepository.Register(new User() { UserName = request.UserName, Password = request.Password, EmailID = request.EmailID, Role= request.Role});

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.UserName = request.UserName;
            user.EmailID = request.EmailID;
            user.Role = request.Role;
            user.Gender = request.Gender;
            user.Age = request.Age;
            user.MobileNumber = request.MobileNumber;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            _userrepository.CreateUser(user);
            return Ok(user);

        }
        private void CreatePasswordHash(string password, out byte[] passwordhash, out byte[] passwordsalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordsalt = hmac.Key;
                passwordhash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

        }


        private static RefreshToken GenerateRefreshToken()
        {
            var byte_ = BitConverter.GetBytes(64);
            var refreshToken = new RefreshToken
            {
                //Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Token = Convert.ToBase64String(byte_),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            return refreshToken;
        }
        private void SetFefreshToken(RefreshToken tkn)
        {
            var cache = new CookieOptions
            {
                HttpOnly = true,
                Expires = tkn.Expires
            };
            Response.Cookies.Append("refreshtoken", tkn.Token, cache);
            user.RefreshToken = tkn.Token;
            user.Expires = tkn.Expires;
            user.Created = tkn.Created;

        }


        [HttpPost("refresh-token")]
        public  ActionResult<string> RefreshToken()
        {
            var refreshtoken = Request.Cookies["refreshToken"];
            if (!user.RefreshToken.Equals(refreshtoken))
            {
                return Unauthorized("Invalid Refresh Token");
            }
            else if(user.Expires < DateTime.Now)
                    {
                return Unauthorized("Token Expired");
            }
            string tkn = CreateToken(user);
            var reftkn = GenerateRefreshToken();
            SetFefreshToken(reftkn);
            return Ok(tkn);


        }


        private bool verifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
           
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,user.UserName),
                //new Claim(ClaimTypes.Role,"Admin"),
                new Claim(ClaimTypes.Role,user.Role)
             };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddDays(2), signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }



    }
}
