using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MyFace.Models.Database;
using MyFace.Models.Request;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace MyFace.Services
{
    public interface IAuthService
    {
        bool Authorize(string authorizationHeader);
    }
    public class AuthService : IAuthService
    {
        private readonly MyFaceDbContext _context;

        public AuthService(MyFaceDbContext context)
        {
            _context = context;
        }

        public bool Authorize(string authorizationHeader)
        {
            if (authorizationHeader != null && authorizationHeader.StartsWith("Basic")) 
            {
                string encodedUsernamePassword = authorizationHeader.Substring("Basic ".Length).Trim();
                string decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                
                string[] usernamePasswordArray = decodedUsernamePassword.Split(':');
                string username = usernamePasswordArray[0];
                string password = usernamePasswordArray[1];          

                try
                {
                    var user = _context.Users.Single(user => user.Username == username);

                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: user.Salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));
                    
                    return(user.HashedPassword == hashed);
                }
                catch(InvalidOperationException)
                {
                    return false;
                }
            }
            else
                return false;
        }
        
    }
}