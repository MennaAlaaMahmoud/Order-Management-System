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
        private readonly IPaymobServices _paymobService;

        public PaymentService(IPaymobServices paymobService)
        {
            _paymobService = paymobService;
        }

        public async Task<string> PayWithCardAsync(decimal amount, string email, string phone)
        {
            return await _paymobService.PayWithCardAsync(amount, email, phone);
        }

        public async Task<string> PayWithWalletAsync(decimal amount, string phone)
        {
            return await _paymobService.PayWithWalletAsync(amount, phone);
        }
    }


}
