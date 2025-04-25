using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IFinancialValidator
    {
        ValueTask<bool> ValidateRent(decimal rent, decimal financialAssets);
    }
}