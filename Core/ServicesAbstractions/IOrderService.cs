using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Shared;

namespace ServicesAbstractions
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(int customerId, List<OrderItem> items, string paymentMethod);
        Task UpdateOrderStatusAsync(int orderId, string status);

    }
}
