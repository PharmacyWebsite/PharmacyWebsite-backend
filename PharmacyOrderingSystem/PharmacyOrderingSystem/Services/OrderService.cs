using Microsoft.EntityFrameworkCore;
using PharmacyOrderingSystem.Data;
using PharmacyOrderingSystem.DTOs;
using PharmacyOrderingSystem.Enums;
using PharmacyOrderingSystem.Models;
using PharmacyOrderingSystem.Helpers;

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

                var user = await _context.Users.FindAsync(dto.UserId);
                if (user == null)
                    throw new Exception("User not found");

                var order = new Order
                {
                    UserId = dto.UserId,
                    Status = OrderStatus.Placed,
                    TotalAmount = 0,
                    Items = new List<OrderItem>()
                };

                foreach (var item in dto.Items)
                {
                    var medicine = await _context.Medicines.FindAsync(item.MedicineId);
                    if (medicine == null)
                        throw new Exception($"Medicine not found: {item.MedicineId}");

                    var inventory = await _context.Inventories
                        .FirstOrDefaultAsync(i => i.MedicineId == item.MedicineId);

                    if (inventory == null)
                        throw new Exception("Inventory not found");

                    if (inventory.Stock < item.Quantity)
                        throw new Exception("Not enough stock");

                    inventory.Stock -= item.Quantity;

                    order.Items.Add(new OrderItem
                    {
                        MedicineId = item.MedicineId,
                        Quantity = item.Quantity,
                        
                    });

                    order.TotalAmount += medicine.Price * item.Quantity;
                }

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                Console.WriteLine("[ORDER] Order created successfully");

                
                try
                {
                    await _emailService.SendEmailAsync(
                        user.Email,
                        "Order Confirmed",
                        $"Your order #{order.Id} has been placed successfully."
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[EMAIL ERROR] " + ex.Message);
                   
                }

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
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new Exception("Order not found");

            order.Status = status;

            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<List<Order>> GetAll()
        {
            return await _context.Orders
                .Include(o => o.Items)
                .ToListAsync();
        }
    }
}