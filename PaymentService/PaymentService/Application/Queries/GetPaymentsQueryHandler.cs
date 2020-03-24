using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentService.Model;

namespace PaymentService.Application.Queries
{
    public class GetPaymentsQueryHandler : IRequestHandler<GetPaymentsQuery, GetPaymentsDTO>
    {
        PaymentContext _context;
        public GetPaymentsQueryHandler(PaymentContext context)
        {
            _context = context;
        }

        public async Task<GetPaymentsDTO> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
        {
            var paymentsData = await _context.Payments.ToListAsync();
            var result = new List<PaymentData>();

            foreach(var x in paymentsData)
            {
                result.Add(new PaymentData()
                {
                    Order_id = x.Order_id,
                    Transaction_id = x.Transaction_id,
                    Payment_type = x.Payment_type,
                    Gross_amount = x.Gross_amount,
                    Transaction_time = x.Transaction_time,
                    Transaction_status = x.Transaction_status
                });
            }

            return new GetPaymentsDTO
            {
                Message = "Payment successfully retrieved",
                Success = true,
                Data = result
            };
        }
    }
}
