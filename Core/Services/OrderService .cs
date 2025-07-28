using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using ServicesAbstractions;

namespace Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IProductRepository _productRepo;
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly OrderManagementDbContext _context;

        public OrderService(
            IOrderRepository orderRepo,
            IProductRepository productRepo,
            IInvoiceRepository invoiceRepo,
            OrderManagementDbContext context)
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _invoiceRepo = invoiceRepo;
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(int customerId, List<OrderItem> items, string paymentMethod)
        {
            decimal total = 0;

            foreach (var item in items)
            {
                var product = await _productRepo.GetByIdAsync(item.ProductId);
                if (product == null || product.Stock < item.Quantity)
                    throw new Exception("Product not found or insufficient stock");

                item.UnitPrice = product.Price;
                item.Discount = CalculateDiscount(product.Price * item.Quantity);
                total += (item.UnitPrice * item.Quantity) - item.Discount;

                // Update stock
                product.Stock -= item.Quantity;
                await _productRepo.UpdateAsync(product);
            }

            var order = new Order
            {
                CustomerId = customerId,
                OrderDate = DateTime.UtcNow,
                OrderItems = items,
                TotalAmount = total,
                PaymentMethod = paymentMethod,
                Status = "Pending"
            };

            await _orderRepo.AddAsync(order);

            // Generate Invoice
            var invoice = new Invoice
            {
                OrderId = order.Id, // Id comes from BaseEntity
                InvoiceDate = DateTime.UtcNow,
                TotalAmount = total
            };

            await _invoiceRepo.AddAsync(invoice);

            await _context.SaveChangesAsync();

            return order;
        }

        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            await _orderRepo.UpdateStatusAsync(orderId, status);
        }

        private decimal CalculateDiscount(decimal orderTotal)
        {
            if (orderTotal > 200)
                return orderTotal * 0.10m;
            else if (orderTotal > 100)
                return orderTotal * 0.05m;
            return 0;
        }
    }

}
