using EchoB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EchoB.Infrastructure.Persistence.Configurations
{
    public class BlockedUserConfiguration : IEntityTypeConfiguration<BlockedUser>
    {
        public void Configure(EntityTypeBuilder<BlockedUser> builder)
        {
            builder.ToTable("BlockedUsers");

            builder.HasKey(bu => bu.Id);

            builder.Property(bu => bu.Id)
                .ValueGeneratedNever();

            builder.Property(bu => bu.UserId)
                .IsRequired();

            builder.Property(bu => bu.BlockedUserId)
                .IsRequired();

            builder.Property(bu => bu.CreatedAt)
                .IsRequired();

            builder.Property(bu => bu.UpdatedAt)
                .IsRequired();

            // Create unique index to prevent duplicate blocks
            builder.HasIndex(bu => new { bu.UserId, bu.BlockedUserId })
                .IsUnique();

            // Configure relationships
            builder.HasOne(bu => bu.User)
                .WithMany(u => u.BlockedUsers)
                .HasForeignKey(bu => bu.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Prevent self-blocking at database level
            builder.ToTable(t => t.HasCheckConstraint("CK_BlockedUsers_NoSelfBlock", "[UserId] != [BlockedUserId]"));
        }
    }
}

