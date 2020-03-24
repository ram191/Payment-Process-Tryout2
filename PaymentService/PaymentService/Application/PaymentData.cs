using System;
using System.Collections.Generic;

namespace PaymentService.Application
{
    public class PaymentData
    {
        public int Order_id { get; set; }
        public string Transaction_id { get; set; }
        public string Payment_type { get; set; }
        public string Gross_amount { get; set; }
        public string Transaction_time { get; set; }
        public string Transaction_status { get; set; }
    }

    public class MidtransDTO
    {
        public string Payment_type { get; set; }
        public Transaction_detail Transaction_details { get; set; }
        public Bank_transfer Bank_transfer { get; set; }
    }

    public class Transaction_detail
    {
        public int Order_id { get; set; }
        public string Gross_amount { get; set; }
    }

    public class Bank_transfer
    {
        public string Bank { get; set; }
    }

    public class BCAMidtransResponse
    {
        public string Status_code { get; set; }
        public string Status_message { get; set; }
        public string Transaction_id { get; set; }
        public string Order_id { get; set; }
        public string Merchant_id { get; set; }
        public string Gross_amount { get; set; }
        public string Currency { get; set; }
        public string Payment_type { get; set; }
        public string Transaction_time { get; set; }
        public string Transaction_status { get; set; }
        public List<VirtualAccount> Va_numbers { get; set; }
        public string Fraud_status { get; set; }
    }

    public class VirtualAccount
    {
        public string Bank { get; set; }
        public string Va_number { get; set; }
    }

    public class WebhookResponse
    {
        public List<VirtualAccount> va_numbers { get; set; }
        public string transaction_time { get; set; }
        public string transaction_status { get; set; }
        public string transaction_id { get; set; }
        public string status_message { get; set; }
        public string status_code { get; set; }
        public string signature_key { get; set; }
        public string payment_type { get; set; }
        public List<object> payment_amounts { get; set; }
        public string order_id { get; set; }
        public string merchant_id { get; set; }
        public string gross_amount { get; set; }
        public string fraud_status { get; set; }
        public string currency { get; set; }
    }
}
