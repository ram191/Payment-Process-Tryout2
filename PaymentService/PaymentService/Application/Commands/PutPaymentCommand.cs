using System;
using MediatR;
using PaymentService.Model;

namespace PaymentService.Application.Commands
{
    public class PutPaymentCommand : CommandDTO<Payment> , IRequest<PaymentData>
    {
    }
}
