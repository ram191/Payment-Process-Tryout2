using System;
using System.Collections.Generic;
using System.Net.Http;
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
    public class PostPaymentCommandHandler : IRequestHandler<PostPaymentCommand, BaseDTO>
    {
        private PaymentContext _context;
        public PostPaymentCommandHandler(PaymentContext context)
        {
            _context = context;
        }

        public async Task<BaseDTO> Handle(PostPaymentCommand request, CancellationToken cancellationToken)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", "Basic U0ItTWlkLXNlcnZlci1BOEtVUy00Q1ZPNjJGeVJmdDc4Yk9kM0I=");
            

            var transactionDetails = new Transaction_detail { Order_id = request.Data.Attributes.Order_id, Gross_amount = request.Data.Attributes.Gross_amount };
            var bankTransfer = new Bank_transfer { Bank = request.Data.Attributes.Bank };

            var midtransDTO = new MidtransDTO()
            {
                Payment_type = "bank_transfer",
                Transaction_details = transactionDetails,
                Bank_transfer = bankTransfer
            };

            //Sending HTTP Post to Midtrans
            var jObj = JsonConvert.SerializeObject(midtransDTO);
            var content = new StringContent(jObj, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://api.sandbox.midtrans.com/v2/charge", content);
            var returnObject = await response.Content.ReadAsStringAsync();

            var rObj = JsonConvert.DeserializeObject<BCAMidtransResponse>(returnObject);

            //Adding to database
            var paymentData = new Payment
            {
                Payment_type = rObj.Payment_type,
                Gross_amount = rObj.Gross_amount,
                Transaction_id = rObj.Transaction_id,
                Transaction_time = rObj.Transaction_time,
                Transaction_status = rObj.Transaction_status,
                Order_id = request.Data.Attributes.Order_id
            };
            await _context.Payments.AddAsync(paymentData);
            await _context.SaveChangesAsync();

            //Sending Queue to RabbitMQ
            var target = new TargetCommand { Id = 1, Email_destination = "buyer1@thisapplication.com" };

            PostCommand command = new PostCommand()
            {
                Title = "Welcome to the sample app",
                Message = "Please verify your identity by sending us your credit card security number",
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
                    routingKey: "notificationKey",
                    basicProperties: null,
                    body: body
                    );
                Console.WriteLine("Data has been forwarded");
            }

            return new BaseDTO
            {
                Message = "Payment has been sent",
                Success = true
            };
        }
    }
}
