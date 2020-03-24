using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentService.Application.Queries;
using PaymentService.Model;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediatr;

        public PaymentController(IMediator mediatr)
        {
            _mediatr = mediatr;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var command = new GetPaymentsQuery();
            var result = await _mediatr.Send(command);

            return result != null ? (IActionResult)Ok(result) : NotFound(new { message = "No notification found" });
        }
    }
}
