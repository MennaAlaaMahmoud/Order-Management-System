using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ServicesAbstractions;
using Stripe;

namespace Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _config;

        public PaymentService(IConfiguration config)
        {
            _config = config;
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
        }

        public async Task<string> CreatePaymentIntentAsync(decimal amount, string currency)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100), // Stripe uses the smallest currency unit
                Currency = currency,
                PaymentMethodTypes = new List<string> { "card" },
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options);

            return intent.ClientSecret;
        }
    }

}
