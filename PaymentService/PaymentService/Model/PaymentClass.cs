using System;
namespace PaymentService.Model
{
    public class Payment
    {
        public int Id { get; set; }
        public int Order_id { get; set; }
        public int Transaction_id { get; set; }
        public string Payment_type { get; set; }
        public string Gross_amount { get; set; }
        public string Transaction_time { get; set; }
        public string Transaction_status { get; set; }
        public DateTime Created_at { get; set; } = DateTime.Now;
        public DateTime Updated_at { get; set; } = DateTime.Now;
    }
}
