using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IBasicsValidator
    {
        ValueTask<bool> ValidateAge(DateOnly birthDate);
        ValueTask<bool> ValidateDocumentNumber(string documentNumber);
        ValueTask<bool> ValidateEmail(string email);
        ValueTask<bool> ValidatePhoneNumber(string phoneNumber);
    }
}