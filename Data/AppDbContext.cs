using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OzBiPortalCRM.Models;

namespace OzBiPortalCRM.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<PortalUser> Users { get; set; } = null!;
        public DbSet<FavoriteItem> Favorites { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PortalUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<FavoriteItem>()
                .HasIndex(f => new { f.PortalUserId, f.ItemType, f.ItemId })
                .IsUnique();
        }

        public async Task EnsureTablesCreatedAsync()
        {
            await Database.EnsureCreatedAsync();
            await Database.ExecuteSqlRawAsync(@"
                CREATE TABLE IF NOT EXISTS FavoriteItems (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PortalUserId INTEGER NOT NULL,
                    ItemType TEXT NOT NULL,
                    ItemId TEXT NOT NULL,
                    ItemName TEXT,
                    ItemSubText TEXT,
                    AddedAt TEXT NOT NULL
                );
            ");
        }
    }
}
