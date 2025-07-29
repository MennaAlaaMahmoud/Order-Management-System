using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstractions;
using Shared;

namespace Presentation
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("card")]
        public async Task<IActionResult> PayWithCard(decimal amount, string email, string phone)
        {
            var url = await _paymentService.PayWithCardAsync(amount, email, phone);
            return Ok(new { redirectUrl = url });
        }
        // Assuming the wallet payment method is similar to card payment
        [HttpPost("wallet")]
        public async Task<IActionResult> PayWithWallet(decimal amount, string phone)
        {
            var url = await _paymentService.PayWithWalletAsync(amount, phone);
            return Ok(new { redirectUrl = url });
        }

    }

}

