using FinanceControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceControl.Data.Mappings
{
    public class WishlistItemMap : IEntityTypeConfiguration<WishlistItem>
    {
        public void Configure(EntityTypeBuilder<WishlistItem> builder)
        {
            builder.ToTable("WishlistItems");
            builder.HasKey(w => w.Id);

            builder.Property(w => w.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(w => w.ImageUrl)
                .HasMaxLength(500);

            builder.Property(w => w.CurrentPrice)
                .IsRequired();

            builder.Property(w => w.TargetPrice);

            builder.Property(w => w.Priority)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(w => w.Deadline);

            builder.Property(w => w.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(w => w.PurchasedTransactionId);

            builder.Property(w => w.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("timezone('America/Sao_Paulo', now())")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(w => w.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .ValueGeneratedOnAdd();

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(w => w.PurchasedTransaction)
                .WithMany()
                .HasForeignKey(w => w.PurchasedTransactionId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(w => w.Links)
                .WithOne(l => l.WishlistItem)
                .HasForeignKey(l => l.WishlistItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(w => w.PriceHistory)
                .WithOne(h => h.WishlistItem)
                .HasForeignKey(h => h.WishlistItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
