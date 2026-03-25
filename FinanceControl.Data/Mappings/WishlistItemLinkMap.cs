using FinanceControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceControl.Data.Mappings
{
    public class WishlistItemLinkMap : IEntityTypeConfiguration<WishlistItemLink>
    {
        public void Configure(EntityTypeBuilder<WishlistItemLink> builder)
        {
            builder.ToTable("WishlistItemLinks");
            builder.HasKey(l => l.Id);

            builder.Property(l => l.Url)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(l => l.StoreName)
                .HasMaxLength(200);

            builder.Property(l => l.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("timezone('America/Sao_Paulo', now())")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(l => l.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .ValueGeneratedOnAdd();
        }
    }
}
