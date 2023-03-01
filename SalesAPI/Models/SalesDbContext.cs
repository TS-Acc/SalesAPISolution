using Microsoft.EntityFrameworkCore;

namespace SalesAPI.Models
{
    public class SalesDbContext : DbContext
    {
        public DbSet<Customer>? Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }

        public SalesDbContext(DbContextOptions<SalesDbContext> options) : base(options) { }
    }
}
