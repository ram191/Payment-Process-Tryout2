using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaymentService.Model;

namespace PaymentService.Application.Commands
{
    public class DeletePaymentCommandHandler : IRequestHandler<DeletePaymentCommand, BaseDTO>
    {
        private readonly PaymentContext _context;
        public DeletePaymentCommandHandler(PaymentContext context)
        {
            _context = context;
        }

        public async Task<BaseDTO> Handle(DeletePaymentCommand request, CancellationToken cancellationToken)
        {
            var data = await _context.Payments.FindAsync(request.Id);

            if (data != null)
            {
                _context.Remove(data);
                await _context.SaveChangesAsync();
                return new BaseDTO
                {
                    Message = "Data has been deleted",
                    Success = true
                };
            }
            else
            {
                return null;
            }    
        }
    }
}
