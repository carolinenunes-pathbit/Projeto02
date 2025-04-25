using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Application.Requests;
using Domain.Models;

namespace Application.Profiles
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<CustomerBasicRequest, Customer>();
            CreateMap<CustomerFinancialRequest, Customer>();
            CreateMap<CustomerAddressRequest, Customer>();
            CreateMap<CustomerPasswordRequest, Customer>();
        }
    }
}