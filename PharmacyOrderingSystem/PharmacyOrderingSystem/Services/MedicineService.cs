using Microsoft.EntityFrameworkCore;
using PharmacyOrderingSystem.Data;
using PharmacyOrderingSystem.Models;
using System;

namespace PharmacyOrderingSystem.Services
{
    public class MedicineService
    {
        private readonly AppDbContext _context;

        public MedicineService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Medicine>> GetAll()
        {
            return await _context.Medicines
                .Include(m => m.Inventory)
                .Include(m => m.Category)
                .ToListAsync();
        }

        public async Task<Medicine?> GetById(int id)
        {
            return await _context.Medicines
                .Include(m => m.Inventory)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Medicine> Create(Medicine medicine, int stock)
        {
            _context.Medicines.Add(medicine);
            await _context.SaveChangesAsync();

            var inventory = new Inventory
            {
                MedicineId = medicine.Id,
                Stock = stock
            };

            _context.Inventories.Add(inventory);
            await _context.SaveChangesAsync();

            return medicine;
        }

        public async Task Update(Medicine medicine)
        {
            _context.Medicines.Update(medicine);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine != null)
            {
                _context.Medicines.Remove(medicine);
                await _context.SaveChangesAsync();
            }
        }
    }
}