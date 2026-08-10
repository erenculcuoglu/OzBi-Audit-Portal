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
        public DbSet<TenantSubscription> TenantSubscriptions { get; set; } = null!;

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

            modelBuilder.Entity<TenantSubscription>()
                .HasKey(ts => ts.TenantId);
        }

        public async Task EnsureTablesCreatedAsync()
        {
            try
            {
                await Database.EnsureCreatedAsync();
            }
            catch { }

            try
            {
                await Database.OpenConnectionAsync();
                
                using var cmd1 = Database.GetDbConnection().CreateCommand();
                cmd1.CommandText = @"
                    CREATE TABLE IF NOT EXISTS FavoriteItems (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        PortalUserId INTEGER NOT NULL,
                        ItemType TEXT NOT NULL,
                        ItemId TEXT NOT NULL,
                        ItemName TEXT,
                        ItemSubText TEXT,
                        AddedAt TEXT NOT NULL
                    );";
                await cmd1.ExecuteNonQueryAsync();

                using var cmd2 = Database.GetDbConnection().CreateCommand();
                cmd2.CommandText = @"
                    CREATE TABLE IF NOT EXISTS UserLoginSnapshots (
                        UserId TEXT PRIMARY KEY,
                        LastSeenLoginCount INTEGER NOT NULL,
                        LastUpdatedAt TEXT NOT NULL
                    );";
                await cmd2.ExecuteNonQueryAsync();

                using var cmd3 = Database.GetDbConnection().CreateCommand();
                cmd3.CommandText = @"
                    CREATE TABLE IF NOT EXISTS TenantSubscriptions (
                        TenantId TEXT PRIMARY KEY,
                        SubscriptionEndDate TEXT,
                        SourceCampaign TEXT,
                        LastUpdatedAt TEXT NOT NULL
                    );";
                await cmd3.ExecuteNonQueryAsync();
            }
            catch { }
        }
    }
}
