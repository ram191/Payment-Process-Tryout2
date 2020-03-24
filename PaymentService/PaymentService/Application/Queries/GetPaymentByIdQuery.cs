using System;
using MediatR;

namespace PaymentService.Application.Queries
{
    public class GetPaymentByIdQuery : IRequest<GetPaymentByIdDTO>
    {
        public int Id { get; set; }

        public GetPaymentByIdQuery(int _id)
        {
            Id = _id;
        }
    }
}
