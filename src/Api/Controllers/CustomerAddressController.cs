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
    [Route("customer/address")]
    [ApiController]
    public class CustomerAddressController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerAddressController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAddress([FromBody] CustomerAddressRequest customerRequest)
        {
            try {
                await _customerService.SaveAddressAsync(customerRequest);
                return Ok(new { message = "Cliente atualizado com sucesso." });
            } 
            catch {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}