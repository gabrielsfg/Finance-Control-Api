using FinanceControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceControl.Data.Mappings
{
    public class WishlistItemPriceHistoryMap : IEntityTypeConfiguration<WishlistItemPriceHistory>
    {
        public void Configure(EntityTypeBuilder<WishlistItemPriceHistory> builder)
        {
            builder.ToTable("WishlistItemPriceHistory");
            builder.HasKey(h => h.Id);

            builder.Property(h => h.Price)
                .IsRequired();

            builder.Property(h => h.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("timezone('America/Sao_Paulo', now())")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(h => h.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .ValueGeneratedOnAdd();
        }
    }
}
