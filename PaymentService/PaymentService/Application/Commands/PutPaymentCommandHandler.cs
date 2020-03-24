using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using PaymentService.Model;
using RabbitMQ.Client;
using static PaymentService.Model.NotificationLogs;

namespace PaymentService.Application.Commands
{
    public class PutPaymentCommandHandler : IRequestHandler<PutPaymentCommand, PaymentData>
    {
        private readonly PaymentContext _context;

        public PutPaymentCommandHandler(PaymentContext context)
        {
            _context = context;
        }

        public async Task<PaymentData> Handle(PutPaymentCommand request, CancellationToken cancellationToken)
        {
            var payment = await _context.Payments.FindAsync(request.Data.Attributes.Id);

            payment.Transaction_status = request.Data.Attributes.Transaction_status;

            await _context.SaveChangesAsync();

            if (payment.Transaction_status.ToLower() == "success")
            {
                SendQueue();
            }

            return new PaymentData
            {
                Order_id = payment.Order_id,
                Transaction_id = payment.Transaction_id,
                Payment_type = payment.Payment_type,
                Gross_amount = payment.Gross_amount,
                Transaction_time = payment.Transaction_time,
                Transaction_status = payment.Transaction_status
            };
        }

        //Sending queue when transaction_status is changed
        public void SendQueue()
        {
            var target = new TargetCommand { Id = 1, Email_destination = "buyer1@thisapplication.com" };
            PostCommand command = new PostCommand()
            {
                Title = "Payment Successfull",
                Message = "Your payment has been accepted",
                Type = "email",
                From = 1,
                Targets = new List<TargetCommand>() { target }
            };

            var attributes = new Data<PostCommand>()
            { Attributes = command };

            var httpContent = new CommandDTO<PostCommand>()
            { Data = attributes };

            var mObj = JsonConvert.SerializeObject(httpContent);

            var factory = new ConnectionFactory() { HostName = "some-rabbit" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("directExchange", "direct");
                channel.QueueDeclare("notification", true, false, false, null);
                channel.QueueBind("notification", "directExchange", string.Empty);

                var body = Encoding.UTF8.GetBytes(mObj);

                channel.BasicPublish(
                    exchange: "directExchange",
                    routingKey: "",
                    basicProperties: null,
                    body: body
                    );
                Console.WriteLine("Data has been forwarded");
            }
        }
    }
}
