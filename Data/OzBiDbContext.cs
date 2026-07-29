using Microsoft.EntityFrameworkCore;
using OzBiPortalCRM.Models;

namespace OzBiPortalCRM.Data
{
    public class OzBiDbContext : DbContext
    {
        public OzBiDbContext(DbContextOptions<OzBiDbContext> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public DbSet<OzBiTenant> Tenants { get; set; } = null!;
        public DbSet<OzBiChat> Chats { get; set; } = null!;
        public DbSet<OzBiChatMessage> ChatMessages { get; set; } = null!;
        public DbSet<OzBiAiModel> AiModels { get; set; } = null!;
        public DbSet<OzBiAssistant> Assistants { get; set; } = null!;
        public DbSet<OzBiUser> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OzBiTenant>().ToTable("tenant");
            modelBuilder.Entity<OzBiChat>().ToTable("chat");
            modelBuilder.Entity<OzBiChatMessage>().ToTable("chatmessage");
            modelBuilder.Entity<OzBiAiModel>().ToTable("aimodel");
            modelBuilder.Entity<OzBiAssistant>().ToTable("asistant");
            modelBuilder.Entity<OzBiUser>().ToTable("aspnetusers");

            // Explicitly set char(36) column types for string ID properties
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    if (property.ClrType == typeof(string) && (property.Name == "Id" || property.Name.EndsWith("Id")))
                    {
                        property.SetColumnType("char(36)");
                    }
                }
            }
        }
    }
}
