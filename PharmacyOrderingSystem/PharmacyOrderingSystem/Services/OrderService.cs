using Microsoft.EntityFrameworkCore;
using PharmacyOrderingSystem.Data;
using PharmacyOrderingSystem.DTOs;
using PharmacyOrderingSystem.Enums;
using PharmacyOrderingSystem.Helpers;
using PharmacyOrderingSystem.Models;

namespace PharmacyOrderingSystem.Services
{
    public class OrderService
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public OrderService(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<Order> CreateOrder(OrderDto dto)
        {
            try
            {
                Console.WriteLine("[ORDER] Creating order...");

                var order = new Order
                {
                    UserId = dto.UserId,
                    Items = dto.Items.Select(i => new OrderItem
                    {
                        MedicineId = i.MedicineId,
                        Quantity = i.Quantity
                    }).ToList(),
                    TotalAmount = 0,
                    Status = OrderStatus.Placed
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                Console.WriteLine("[ORDER] Order created successfully");

                await _emailService.SendEmailAsync(
                    "user@mail.com",
                    "Order Confirmed",
                    $"Your order #{order.Id} has been placed successfully."
                );

                return order;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ORDER ERROR] {ex.Message}");
                throw;
            }
        }

        public async Task<Order> UpdateOrderStatus(int orderId, OrderStatus status)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Items)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    throw new Exception("Order not found");
                }

                order.Status = status;

                await _context.SaveChangesAsync();

                Console.WriteLine($"[ORDER] Status updated to {status} for Order ID {orderId}");

                return order;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ORDER STATUS ERROR] {ex.Message}");
                throw;
            }
        }
        public async Task<List<Order>> GetAll()
        {
            return await _context.Orders.Include(o => o.Items).ToListAsync();
        }
    }
}