using System;
using MediatR;
using PaymentService.Model;

namespace PaymentService.Application.Commands
{
    public class DeletePaymentCommand : IRequest<BaseDTO>
    {
        public int Id { get; set; }

        public DeletePaymentCommand(int id)
        {
            Id = id;
        }
    }
}
