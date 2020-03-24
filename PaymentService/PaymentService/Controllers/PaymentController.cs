using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentService.Application.Commands;
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

            return result != null ? (IActionResult)Ok(result) : NotFound(new { message = "Data not found" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetId(int id)
        {
            var command = new GetPaymentByIdQuery(id);
            var result = await _mediatr.Send(command);
            return result != null ? (IActionResult)Ok(result) : NotFound(new { Message = "Notification not found" });
        }

        [HttpPost]
        public async Task<IActionResult> Post(PostPaymentCommand data)
        {
            var result = await _mediatr.Send(data);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeletePaymentCommand(id);
            var result = await _mediatr.Send(command);
            return result != null ? (IActionResult)Ok(new { Message = "success" }) : NotFound(new { Message = "Data not found" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, PutPaymentCommand data)
        {
            data.Data.Attributes.Id = id;
            var result = await _mediatr.Send(data);
            return Ok(result);
        }
    }
}
