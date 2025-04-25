using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Requests
{
    public class CustomerPasswordRequest : CustomerBaseRequest
    {
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}