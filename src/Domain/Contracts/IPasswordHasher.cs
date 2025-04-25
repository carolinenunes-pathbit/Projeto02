using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IPasswordHasher
    {
        ValueTask<string> HashPassword(string password, string confirmPassword);
    }
}