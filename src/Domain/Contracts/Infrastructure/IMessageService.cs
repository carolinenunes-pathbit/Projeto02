using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Contracts.Infrastructure
{
    public interface IMessageService
    {
        ValueTask SendMessageAsync(string queue, string message);
        Task GetMessageAsync( string queue, Action<string> callbackMethod);
    }
}