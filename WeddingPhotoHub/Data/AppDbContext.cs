using Microsoft.EntityFrameworkCore;
using WeddingPhotoHub.Models;

namespace WeddingPhotoHub.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Foto> Fotos { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
