using System;
using MediatR;

namespace PaymentService.Application.Queries
{
    public class GetPaymentsQuery : IRequest<GetPaymentsDTO>
    {
    }
}
