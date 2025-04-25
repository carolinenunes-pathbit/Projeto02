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
    [Route("customer/basic")]
    [ApiController]
    public class CustomerBasicController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerBasicController> _logger;

        public CustomerBasicController(ICustomerService customerService, ILogger<CustomerBasicController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerBasicRequest customerRequest)
        {
            try {
                var id = await _customerService.SaveBasicAsync(customerRequest);
                return CreatedAtAction(nameof(Create), new { message = "Cliente criado com sucesso", id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao criar cliente");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}