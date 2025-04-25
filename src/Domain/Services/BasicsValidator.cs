using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Domain.Contracts;

namespace Domain.Services
{
    public class BasicsValidator : IBasicsValidator
    {
        public async ValueTask<bool> ValidateAge(DateOnly birthDate)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var age = today.Year - birthDate.Year;

            if (birthDate > today.AddYears(-age))
            {
                age--;
            }

            if (age < 18)
            {
                throw new ArgumentException("Idade menor que 18.");
            }

            return await ValueTask.FromResult(age >= 18);
        }
        public async ValueTask<bool> ValidateDocumentNumber(string documentNumber)
        {
            documentNumber = new string(documentNumber.Where(char.IsDigit).ToArray());

            if (documentNumber.Length != 11)
            {
                throw new ArgumentException("CPF inválido: Número de dígitos incorreto.");
            }

            bool allEquals = true;
            for (int i = 1; i < 11; i++)
            {
                if (documentNumber[i] != documentNumber[0])
                {
                    allEquals = false;
                    break;
                }
            }
            if (allEquals)
            {
                throw new ArgumentException("CPF inválido: Todos os dígitos são iguais.");
            }

            int soma = 0;
            for (int i = 0; i < 9; i++)
            {
                soma += int.Parse(documentNumber[i].ToString()) * (10 - i);
            }
            int leftover = soma % 11;
            int firstNumber = leftover < 2 ? 0 : 11 - leftover;

            if (int.Parse(documentNumber[9].ToString()) != firstNumber)
            {
                throw new ArgumentException("CPF inválido: Primeiro dígito verificador incorreto.");
            }

            soma = 0;
            for (int i = 0; i < 10; i++)
            {
                soma += int.Parse(documentNumber[i].ToString()) * (11 - i);
            }
            leftover = soma % 11;
            int secondNumber = leftover < 2 ? 0 : 11 - leftover;

            if (int.Parse(documentNumber[10].ToString()) != secondNumber)
            {
                throw new ArgumentException("CPF inválido: Segundo dígito verificador incorreto.");
            }

            return await ValueTask.FromResult(true);
        }
        public async ValueTask<bool> ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("E-mail inválido: E-mail vazio ou nulo.");
            }

            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            if (!Regex.IsMatch(email, pattern))
            {
                throw new ArgumentException("E-mail inválido: Formato incorreto.");
            }

            return await ValueTask.FromResult(Regex.IsMatch(email, pattern));
        }
        public async ValueTask<bool> ValidatePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                throw new ArgumentException("Número de telefone inválido: Número vazio ou nulo.");
            }

            string pattern = @"^\(?[1-9]{2}\)? ?(?:[2-8]|9[1-9])[0-9]{3}\-?[0-9]{4}$";

            if (!Regex.IsMatch(phoneNumber, pattern))
            {
                throw new ArgumentException("Número de telefone inválido: Formato incorreto.");
            }

            return await ValueTask.FromResult(Regex.IsMatch(phoneNumber, pattern));
        }
    }
}