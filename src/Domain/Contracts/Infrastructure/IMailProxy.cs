using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Contracts.Infrastructure
{
    public interface IMailProxy
    {
        ValueTask SendMailAsync(string email, string message);
    }
}