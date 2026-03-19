using FinanceControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceControl.Data.Mappings
{
    public class CurrencyRateMap : IEntityTypeConfiguration<CurrencyRate>
    {
        public void Configure(EntityTypeBuilder<CurrencyRate> builder)
        {
            builder.ToTable("CurrencyRates");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.BaseCurrency).IsRequired().HasMaxLength(10);
            builder.Property(c => c.TargetCurrency).IsRequired().HasMaxLength(10);
            builder.Property(c => c.Rate).IsRequired().HasColumnType("numeric(18,6)");
            builder.Property(c => c.FetchedAt)
                .HasColumnType("timestamp without time zone")
                .IsRequired();
            builder.Property(c => c.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("timezone('America/Sao_Paulo', now())")
                .IsRequired()
                .ValueGeneratedOnAdd();
            builder.Property(c => c.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .ValueGeneratedOnAdd();

            builder.HasIndex(c => new { c.BaseCurrency, c.TargetCurrency }).IsUnique();
        }
    }
}
