using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaymentService.Model;

namespace PaymentService.Application.Queries
{
    public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, GetPaymentByIdDTO>
    {
        private readonly PaymentContext _context;

        public GetPaymentByIdQueryHandler(PaymentContext context)
        {
            _context = context;
        }

        public async Task<GetPaymentByIdDTO> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
        {
            var payment = await _context.Payments.FindAsync(request.Id);

            if(payment != null)
            {
                var data = new PaymentData
                {
                    Order_id = payment.Order_id,
                    Transaction_id = payment.Transaction_id,
                    Payment_type = payment.Payment_type,
                    Gross_amount = payment.Gross_amount,
                    Transaction_time = payment.Transaction_time,
                    Transaction_status = payment.Transaction_status
                };

                return new GetPaymentByIdDTO
                {
                    Message = "Payment successfully retrieved",
                    Success = true,
                    Data = data
                };
            }
            else
            {
                return null;
            }
            
        }
    }
}
