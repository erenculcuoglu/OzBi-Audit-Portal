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
        public DbSet<UserLoginSnapshot> UserLoginSnapshots { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PortalUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<FavoriteItem>()
                .HasIndex(f => new { f.PortalUserId, f.ItemType, f.ItemId })
                .IsUnique();

            modelBuilder.Entity<UserLoginSnapshot>()
                .HasKey(s => s.UserId);
        }

        public async Task EnsureTablesCreatedAsync()
        {
            await Database.EnsureCreatedAsync();

            try
            {
                await Database.OpenConnectionAsync();
                using var cmd = Database.GetDbConnection().CreateCommand();
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS FavoriteItems (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        PortalUserId INTEGER NOT NULL,
                        ItemType TEXT NOT NULL,
                        ItemId TEXT NOT NULL,
                        ItemName TEXT,
                        ItemSubText TEXT,
                        AddedAt TEXT NOT NULL
                    );

                    CREATE TABLE IF NOT EXISTS UserLoginSnapshots (
                        UserId TEXT PRIMARY KEY,
                        LastSeenLoginCount INTEGER NOT NULL,
                        LastUpdatedAt TEXT NOT NULL
                    );
                ";
                await cmd.ExecuteNonQueryAsync();
            }
            catch { }
        }
    }
}
