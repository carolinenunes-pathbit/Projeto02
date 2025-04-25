using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Requests
{
    public class CustomerAddressRequest : CustomerBaseRequest
    {
        public string ZipCode { get; set; }
        public int Number { get; set; }
        public string Street { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string State { get; set; }
    }
}