using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Application.Contracts;
using Application.Requests;

namespace Api.Controllers
{
    [Route("customer/financial")]
    [ApiController]
    public class CustomerFinancialController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerFinancialController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateFinancial([FromBody] CustomerFinancialRequest customerRequest)
        {
            try {
                await _customerService.SaveFinancialAsync(customerRequest);
                return Ok(new { message = "Cliente atualizado com sucesso." });
            } 
            catch {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}