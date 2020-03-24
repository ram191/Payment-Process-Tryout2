using System;
using PaymentService.Model;

namespace PaymentService.Application.Queries
{
    public class GetPaymentByIdDTO : BaseDTO
    {
        public PaymentData Data { get; set; }
    }
}
