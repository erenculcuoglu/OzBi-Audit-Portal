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
        public DbSet<TenantComplianceSnapshot> TenantComplianceSnapshots { get; set; } = null!;
        public DbSet<CustomPromptTemplateItem> CustomPromptTemplates { get; set; } = null!;
        public DbSet<FeedbackPushSnapshot> FeedbackPushSnapshots { get; set; } = null!;
        public DbSet<SqlErrorPushSnapshot> SqlErrorPushSnapshots { get; set; } = null!;

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

            modelBuilder.Entity<TenantComplianceSnapshot>()
                .HasKey(tc => tc.TenantId);

            modelBuilder.Entity<CustomPromptTemplateItem>()
                .HasKey(cp => cp.Id);

            modelBuilder.Entity<FeedbackPushSnapshot>()
                .HasKey(fp => fp.MessageId);

            modelBuilder.Entity<SqlErrorPushSnapshot>()
                .HasKey(se => se.MessageId);
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

                using var cmd4 = Database.GetDbConnection().CreateCommand();
                cmd4.CommandText = @"
                    CREATE TABLE IF NOT EXISTS TenantComplianceSnapshots (
                        TenantId TEXT PRIMARY KEY,
                        TenantName TEXT NOT NULL,
                        ErpType TEXT NOT NULL,
                        ErpTypeName TEXT NOT NULL,
                        OverallScore INTEGER NOT NULL,
                        Grade TEXT NOT NULL,
                        GradeLabel TEXT NOT NULL,
                        TotalQueriesEvaluated INTEGER NOT NULL,
                        CompliantCount INTEGER NOT NULL,
                        WarningCount INTEGER NOT NULL,
                        CriticalCount INTEGER NOT NULL,
                        IsPromptSynced INTEGER NOT NULL,
                        PromptVersionLabel TEXT NOT NULL,
                        PromptSyncDetails TEXT,
                        TopViolationsJson TEXT,
                        LastEvaluatedAt TEXT NOT NULL
                    );";
                await cmd4.ExecuteNonQueryAsync();

                using var cmd5 = Database.GetDbConnection().CreateCommand();
                cmd5.CommandText = @"
                    CREATE TABLE IF NOT EXISTS CustomPromptTemplates (
                        Id TEXT PRIMARY KEY,
                        Title TEXT NOT NULL,
                        Prompt TEXT NOT NULL,
                        CategoryId INTEGER NOT NULL,
                        OriginTypeId INTEGER NOT NULL DEFAULT 2,
                        TargetRole TEXT,
                        Complexity TEXT,
                        BusinessImpact TEXT,
                        ExpectedDecision TEXT,
                        ErpCompatibility TEXT,
                        AlternativePhrasingsJson TEXT,
                        CreatedByPortalUserId INTEGER NOT NULL,
                        CreatedAt TEXT NOT NULL
                    );";
                await cmd5.ExecuteNonQueryAsync();

                // Ensure newly added columns exist in case table was created previously
                try
                {
                    using var alterCmd = Database.GetDbConnection().CreateCommand();
                    alterCmd.CommandText = "ALTER TABLE CustomPromptTemplates ADD COLUMN AlternativePhrasingsJson TEXT;";
                    await alterCmd.ExecuteNonQueryAsync();
                }
                catch { }

                try
                {
                    using var alterCmd = Database.GetDbConnection().CreateCommand();
                    alterCmd.CommandText = "ALTER TABLE CustomPromptTemplates ADD COLUMN OriginTypeId INTEGER NOT NULL DEFAULT 2;";
                    await alterCmd.ExecuteNonQueryAsync();
                }
                catch { }

                try
                {
                    using var alterCmd = Database.GetDbConnection().CreateCommand();
                    alterCmd.CommandText = "ALTER TABLE CustomPromptTemplates ADD COLUMN BusinessImpact TEXT;";
                    await alterCmd.ExecuteNonQueryAsync();
                }
                catch { }

                try
                {
                    using var alterCmd = Database.GetDbConnection().CreateCommand();
                    alterCmd.CommandText = "ALTER TABLE CustomPromptTemplates ADD COLUMN ExpectedDecision TEXT;";
                    await alterCmd.ExecuteNonQueryAsync();
                }
                catch { }

                try
                {
                    using var alterCmd = Database.GetDbConnection().CreateCommand();
                    alterCmd.CommandText = "ALTER TABLE CustomPromptTemplates ADD COLUMN ErpCompatibility TEXT;";
                    await alterCmd.ExecuteNonQueryAsync();
                }
                catch { }

                try
                {
                    using var cmd6 = Database.GetDbConnection().CreateCommand();
                    cmd6.CommandText = @"
                        CREATE TABLE IF NOT EXISTS FeedbackPushSnapshots (
                            MessageId TEXT PRIMARY KEY,
                            ChatId TEXT,
                            TenantName TEXT,
                            UserName TEXT,
                            UserEmail TEXT,
                            FeedbackReason TEXT,
                            IsLiked INTEGER,
                            PushedAt TEXT NOT NULL,
                            PushedBy TEXT,
                            Status TEXT NOT NULL
                        );";
                    await cmd6.ExecuteNonQueryAsync();
                }
                catch { }

                try
                {
                    using var cmd7 = Database.GetDbConnection().CreateCommand();
                    cmd7.CommandText = @"
                        CREATE TABLE IF NOT EXISTS SqlErrorPushSnapshots (
                            MessageId TEXT PRIMARY KEY,
                            ChatId TEXT,
                            TenantName TEXT,
                            UserName TEXT,
                            UserEmail TEXT,
                            ErrorMessage TEXT,
                            Prompt TEXT,
                            SqlQuery TEXT,
                            PushedAt TEXT NOT NULL,
                            PushedBy TEXT,
                            Status TEXT NOT NULL
                        );";
                    await cmd7.ExecuteNonQueryAsync();
                }
                catch { }
            }
            catch { }
        }
    }
}
