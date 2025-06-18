using EchoB.Domain.Entities;
using EchoB.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EchoB.Infrastructure.Persistence.Context
{
    public class EchoBDbContext : IdentityDbContext<EchoBUser, IdentityRole<Guid>, Guid>
    {
        public EchoBDbContext(DbContextOptions<EchoBDbContext> options) : base(options)
        {
        }

        
        public DbSet<BlockedUser> BlockedUsers { get; set; } = null!;
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageReaction> MessagesReactions { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<CallSession> Calls { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Conversation>()
                 .HasOne(c => c.User1)
                 .WithMany()
                 .HasForeignKey(c => c.User1Id)
                 .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.User2)
                .WithMany()
                .HasForeignKey(c => c.User2Id)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Conversation>()
            .HasIndex(c => new { c.User1Id, c.User2Id });
            modelBuilder.Entity<Message>()
                .HasIndex(m => m.ConversationId);
            // MESSAGE REACTIONS
            modelBuilder.Entity<MessageReaction>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.HasIndex(r => new { r.MessageId, r.UserId, r.ReactionType }).IsUnique();

                entity.HasOne(r => r.Message)
                    .WithMany(m => m.Reactions)
                    .HasForeignKey(r => r.MessageId)
                    .OnDelete(DeleteBehavior.Restrict); // ?? ??? ???????

                entity.HasOne(r => r.User)
                    .WithMany()
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Restrict); // ?? ??? ???????
            });
            // Apply all configurations from the current assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            modelBuilder.Entity<CallSession>().HasKey(c => c.Id);
            // Additional global configurations
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Configure all DateTime properties to use UTC
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetColumnType("datetime2");
                    }
                }
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Update timestamps for entities that inherit from BaseEntity
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        // CreatedAt and UpdatedAt are set in the constructor
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdateTimestamp();
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}

