using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Contracts;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using ServicesAbstractions;
using Shared;

namespace Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IProductRepository _productRepo;
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly OrderManagementDbContext _context;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepo,
            IProductRepository productRepo,
            IInvoiceRepository invoiceRepo,
            OrderManagementDbContext context,
            IMapper mapper
            )
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _invoiceRepo = invoiceRepo;
            _context = context;
            _mapper = mapper;
        }

        public async Task<OrderResponseDto> CreateOrderAsync(int customerId, List<OrderItem> items, string paymentMethod)
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

            return _mapper.Map<OrderResponseDto>(order);

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
