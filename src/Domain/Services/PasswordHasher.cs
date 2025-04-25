using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;

namespace Domain.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public async ValueTask<string> HashPassword(string password, string confirmPassword)
        {
            if(string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                throw new ArgumentException("O campo não pode estar nulo.");
            }

            if (password != confirmPassword)
            {
                throw new ArgumentException("Senha incorreta.");
            }

            using var sha256 = SHA256.Create();
            var hashedPassword = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
            return await ValueTask.FromResult(hashedPassword);
        }
    }
}