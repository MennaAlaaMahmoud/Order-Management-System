using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Presentation
{

    [ApiController]
    [Route("api/webhook/paymob")]
    public class PaymobWebhookController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public PaymobWebhookController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpPost]
        public async Task<IActionResult> HandleWebhook([FromBody] dynamic payload)
        {
            // Parse the order ID & success status
            try
            {
                var json = JsonConvert.SerializeObject(payload);
                var data = JsonConvert.DeserializeObject<dynamic>(json);

                var success = data.obj.success;
                var orderId = (int)data.obj.order.id;

                if (success == true)
                {
                    var order = await _orderRepository.GetByIdAsync(orderId);
                    if (order != null)
                    {
                        order.Status = "Paid";
                        await _orderRepository.UpdateAsync(order);
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Invalid payload", error = ex.Message });
            }
        }
    }
}
