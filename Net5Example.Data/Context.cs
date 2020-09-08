using Microsoft.EntityFrameworkCore;
using Net5Example.ViewModels;

namespace Net5Example.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<StockPosition> StockPositions { get; set; }
    }
}
