using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using OnlineShopping.Domain.Entities;

namespace OnlineShopping.Domain.Concrete
{
    public class EFDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<OnlineTransaction> Transactions { get; set; }
        public DbSet<OnlineTransactionDetail> TransactionDetails { get; set; }
        public DbSet<Outlet> Outlets { get; set;}
        public DbSet<OutletInventory> OutletInventories { get; set; }
    }
}
