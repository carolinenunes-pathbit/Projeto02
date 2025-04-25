using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json;
using Application.Requests;
using Application.Contracts;
using Domain.Models;
using Domain.Contracts;
using Domain.Contracts.Infrastructure;

namespace Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _customerRepository;
        private readonly IBasicsValidator _basicsValidator;
        private readonly IFinancialValidator _financialValidator;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMessageService _messageService;

        public CustomerService(
            IMapper mapper,
            ICustomerRepository customerRepository,
            IBasicsValidator basicsValidator,
            IFinancialValidator financialValidator,
            IPasswordHasher passwordHasher, 
            IMessageService messageService
        )

        {
            _mapper = mapper;
            _customerRepository = customerRepository;
            _basicsValidator = basicsValidator;
            _financialValidator = financialValidator;
            _passwordHasher = passwordHasher;
            _messageService = messageService;
        }
        public async Task<string> SaveBasicAsync(CustomerBasicRequest customerRequest)
        {
            await _basicsValidator.ValidateAge(customerRequest.BirthDate);

            var documentNumberExists = await _customerRepository.ExistingDocumentNumberAsync(customerRequest.DocumentNumber);
            if(documentNumberExists){
                throw new ArgumentException("Este CPF já está cadastrado.");
            }

            await _basicsValidator.ValidateDocumentNumber(customerRequest.DocumentNumber);

            var emailExists = await _customerRepository.ExistingEmailAsync(customerRequest.Email);
            if(emailExists){
                throw new ArgumentException("Este email já está cadastrado.");
            }

            await _basicsValidator.ValidateEmail(customerRequest.Email);
            await _basicsValidator.ValidatePhoneNumber(customerRequest.PhoneNumber);

            var customer = _mapper.Map<Customer>(customerRequest);

            await _customerRepository.SaveCustomerAsync(customer);

            return await _customerRepository.GetCustomerIdByEmailAsync(customer.Email);
        }

        public async Task SaveFinancialAsync(CustomerFinancialRequest customerRequest)
        {
            await _financialValidator.ValidateRent(customerRequest.Rent, customerRequest.FinancialAssets);

            var customer = await _customerRepository.GetCustomerIdAsync(customerRequest.Id);

            _mapper.Map(customerRequest, customer);

            await _customerRepository.UpdateCustomerAsync(customer);
        }

        public async Task SaveAddressAsync(CustomerAddressRequest customerRequest)
        {
            var customer = await _customerRepository.GetCustomerIdAsync(customerRequest.Id);

            _mapper.Map(customerRequest, customer);

            await _customerRepository.UpdateCustomerAsync(customer);
        }

        public async Task SavePasswordAsync(CustomerPasswordRequest customerRequest)
        {
            var customer = await _customerRepository.GetCustomerIdAsync(customerRequest.Id);

            _mapper.Map(customerRequest, customer);

            var hashedPassword = await _passwordHasher.HashPassword(customerRequest.Password, customerRequest.ConfirmPassword);
            customer.Password = hashedPassword;
            customer.ConfirmPassword = hashedPassword;

            await _customerRepository.UpdateCustomerAsync(customer);

            var message = JsonConvert.SerializeObject(customer);
            await _messageService.SendMessageAsync("cadastro.em.analise.email", message);
        }
    }
}