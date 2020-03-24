using System;
namespace PaymentService.Application
{
    public class PaymentData
    {
        public int Order_id { get; set; }
        public int Transaction_id { get; set; }
        public string Payment_type { get; set; }
        public string Gross_amount { get; set; }
        public string Transaction_time { get; set; }
        public string Transaction_status { get; set; }
    }
}
