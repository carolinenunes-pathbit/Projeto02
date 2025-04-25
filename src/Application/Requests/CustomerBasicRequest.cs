using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Requests
{
    public class CustomerBasicRequest
    {
        public string Name { get; set; }
        public DateOnly BirthDate { get; set; }
        public string DocumentNumber { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}