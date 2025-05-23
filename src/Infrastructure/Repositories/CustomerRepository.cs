using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Domain.Contracts.Infrastructure;
using Domain.Models;

namespace Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IMongoCollection<Customer> _collection;

        public CustomerRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<Customer>("customers");
        }

        public async Task<Customer> GetCustomerIdAsync(string id)
        {
            var filter = Builders<Customer>.Filter.Eq(c => c.Id, id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async ValueTask<string?> GetCustomerIdByEmailAsync(string email)
        {
            var filter = Builders<Customer>.Filter.Eq(c => c.Email, email);
            var customer = await _collection.Find(filter).FirstOrDefaultAsync();
            if (customer is null)
            {
                return null;
            }
            return customer.Id;
        }

        public async Task SaveCustomerAsync(Customer customer)
        {
            await _collection.InsertOneAsync(customer);
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            var filter = Builders<Customer>.Filter.Eq(c => c.Id, customer.Id);
            await _collection.ReplaceOneAsync(filter, customer);
        }

        public async ValueTask<bool> ExistingEmailAsync(string email)
        {
            var filter = Builders<Customer>.Filter.Eq(c => c.Email, email);
            var customer = await _collection.Find(filter).FirstOrDefaultAsync();
            return customer != null;
        }

        public async ValueTask<bool> ExistingDocumentNumberAsync(string documentNumber)
        {
            var filter = Builders<Customer>.Filter.Eq(c => c.DocumentNumber, documentNumber);
            var customer = await _collection.Find(filter).FirstOrDefaultAsync();
            return customer != null;
        }
    }
}