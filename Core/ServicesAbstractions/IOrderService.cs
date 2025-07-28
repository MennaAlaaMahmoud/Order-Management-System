using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace ServicesAbstractions
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(int customerId, List<OrderItem> items, string paymentMethod);
        Task UpdateOrderStatusAsync(int orderId, string status);

    }
}
