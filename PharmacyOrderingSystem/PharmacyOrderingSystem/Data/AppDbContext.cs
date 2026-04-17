
using Microsoft.EntityFrameworkCore;
using PharmacyOrderingSystem.Models;

namespace PharmacyOrderingSystem.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }
}