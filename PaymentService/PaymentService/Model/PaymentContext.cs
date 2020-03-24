using System;
using Microsoft.EntityFrameworkCore;

namespace PaymentService.Model
{
    public class PaymentContext : DbContext
    {
        public PaymentContext(DbContextOptions<PaymentContext> options) : base(options)
        {
        }

        public DbSet <Payment>Payments { get; set; }
    }
}
