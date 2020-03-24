using System;
using MediatR;
using PaymentService.Model;

namespace PaymentService.Application.Commands
{
    public class PostPaymentCommand : CommandDTO<PaymentCommand>, IRequest<BaseDTO>
    {
    }

    public class PaymentCommand
    {
        public string Payment_type { get; set; }
        public string Gross_amount { get; set; }
        public string Bank { get; set; }
        public int Order_id { get; set; }
    }
}
