using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Requests;

namespace Application.Contracts
{
    public interface ICustomerService
    {
        Task<string> SaveBasicAsync(CustomerBasicRequest customerRequest);
        Task SaveFinancialAsync(CustomerFinancialRequest customerRequest);
        Task SaveAddressAsync(CustomerAddressRequest customerRequest);
        Task SavePasswordAsync(CustomerPasswordRequest customerRequest);
    }
}