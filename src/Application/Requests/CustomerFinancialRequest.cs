using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Requests
{
    public class CustomerFinancialRequest : CustomerBaseRequest
    {
        public decimal Rent { get; set; }
        public decimal FinancialAssets { get; set; }
    }
}