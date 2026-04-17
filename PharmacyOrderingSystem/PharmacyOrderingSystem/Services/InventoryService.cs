using Microsoft.EntityFrameworkCore;
using PharmacyOrderingSystem.Data;
using PharmacyOrderingSystem.Models;
using System;

namespace PharmacyOrderingSystem.Services
{
    public class InventoryService
    {
        private readonly AppDbContext _context;

        public InventoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Inventory?> GetByMedicineId(int medicineId)
        {
            return await _context.Inventories
                .FirstOrDefaultAsync(i => i.MedicineId == medicineId);
        }

        public async Task UpdateStock(int medicineId, int quantity)
        {
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.MedicineId == medicineId);

            if (inventory != null)
            {
                inventory.Stock = quantity;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ReduceStock(int medicineId, int quantity)
        {
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.MedicineId == medicineId);

            if (inventory != null)
            {
                inventory.Stock -= quantity;
                await _context.SaveChangesAsync();
            }
        }
    }
}