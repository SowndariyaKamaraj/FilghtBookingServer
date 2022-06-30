
using Airline.Model;
using AirlineAPIServices.Model;
using Microsoft.EntityFrameworkCore;

namespace AirlineApiService.Model
{

    public class AppDbContext : DbContext

    {
        public AppDbContext(DbContextOptions<AppDbContext> option) : base(option)
        {
        }
        public DbSet<Inventory> Inventory { get; set; }

        public DbSet<AirlineLogo> Airline { get; set; }

        public DbSet<Discount> Discount { get; set; }
    }
}
