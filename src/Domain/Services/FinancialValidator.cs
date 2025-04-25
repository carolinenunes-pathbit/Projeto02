using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Contracts;

namespace Domain.Services
{
    public class FinancialValidator : IFinancialValidator
    {
        public async ValueTask<bool> ValidateRent(decimal rent, decimal financialAssets)
        {
            if (rent + financialAssets <= 1000)
            {
                throw new ArgumentException("A soma da renda com o patrimônio deve ser maior que 1000.");
            }

            return await ValueTask.FromResult(true);
        }
    }
}