using System;
using System.Collections.Generic;
using PaymentService.Model;

namespace PaymentService.Application.Queries
{
    public class GetPaymentsDTO : BaseDTO
    {
        public List<PaymentData> Data { get; set; }

    }
}
