using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using E_Ticaret.Models;

namespace E_Ticaret.Models
{

    public class eTicaretContext:DbContext
    {
        public eTicaretContext(DbContextOptions<eTicaretContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=RECEP;Initial Catalog=Eticaret;User Id=sa;Password=159753");
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Seller> Sellers { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<ItemStatus> ItemStatus { get; set; }
        public DbSet<OrderDetailStatus> OrderDetailStatuses { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}
