using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Contracts.Infrastructure
{
    public interface ICustomerRepository
    {
        Task<Customer> GetCustomerIdAsync(string id);
        ValueTask<string> GetCustomerIdByEmailAsync(string email);
        Task SaveCustomerAsync(Customer customer);
        Task UpdateCustomerAsync(Customer customer);
        ValueTask<bool> ExistingEmailAsync(string email);
        ValueTask<bool> ExistingDocumentNumberAsync(string documentNumber);
    }
}