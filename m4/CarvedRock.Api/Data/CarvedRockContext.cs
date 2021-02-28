using Microsoft.EntityFrameworkCore;
using CarvedRock.Api.Models;

namespace CarvedRock.Api.Data
{
    public class CarvedRockContext : DbContext
    {
        public CarvedRockContext(DbContextOptions<CarvedRockContext> options)
            : base(options)
        {
            
        }

        public DbSet<WishlistItem> WishlistItems { get; set; }
    }
}