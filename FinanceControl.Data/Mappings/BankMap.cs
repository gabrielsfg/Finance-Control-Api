using FinanceControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceControl.Data.Mappings
{
    public class BankMap : IEntityTypeConfiguration<Bank>
    {
        public void Configure(EntityTypeBuilder<Bank> builder)
        {
            builder.ToTable("Banks");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Name).IsRequired().HasMaxLength(200);
            builder.Property(b => b.Code).HasMaxLength(20);
            builder.Property(b => b.Country).IsRequired().HasMaxLength(2);
            builder.Property(b => b.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("timezone('America/Sao_Paulo', now())")
                .IsRequired()
                .ValueGeneratedOnAdd();
            builder.Property(b => b.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .ValueGeneratedOnAdd();

            builder.HasData(BankSeed.Data);
        }
    }

    public static class BankSeed
    {
        public static readonly Bank[] Data =
        [
            // Brazilian banks
            new() { Id = 1, Name = "Banco do Brasil", Code = "001", Country = "BR", CreatedAt = new DateTime(2024, 1, 1) },
            new() { Id = 2, Name = "Bradesco", Code = "237", Country = "BR", CreatedAt = new DateTime(2024, 1, 1) },
            new() { Id = 3, Name = "Itaú Unibanco", Code = "341", Country = "BR", CreatedAt = new DateTime(2024, 1, 1) },
            new() { Id = 4, Name = "Caixa Econômica Federal", Code = "104", Country = "BR", CreatedAt = new DateTime(2024, 1, 1) },
            new() { Id = 5, Name = "Santander Brasil", Code = "033", Country = "BR", CreatedAt = new DateTime(2024, 1, 1) },
            new() { Id = 6, Name = "Nubank", Code = "260", Country = "BR", CreatedAt = new DateTime(2024, 1, 1) },
            new() { Id = 7, Name = "Inter", Code = "077", Country = "BR", CreatedAt = new DateTime(2024, 1, 1) },
            new() { Id = 8, Name = "C6 Bank", Code = "336", Country = "BR", CreatedAt = new DateTime(2024, 1, 1) },
            new() { Id = 9, Name = "XP Investimentos", Code = "341", Country = "BR", CreatedAt = new DateTime(2024, 1, 1) },
            new() { Id = 10, Name = "PicPay", Code = "380", Country = "BR", CreatedAt = new DateTime(2024, 1, 1) },
            new() { Id = 11, Name = "Sicoob", Code = "756", Country = "BR", CreatedAt = new DateTime(2024, 1, 1) },
            new() { Id = 12, Name = "Sicredi", Code = "748", Country = "BR", CreatedAt = new DateTime(2024, 1, 1) },
            new() { Id = 13, Name = "BTG Pactual", Code = "208", Country = "BR", CreatedAt = new DateTime(2024, 1, 1) },
            new() { Id = 14, Name = "Neon", Code = "735", Country = "BR", CreatedAt = new DateTime(2024, 1, 1) },
            new() { Id = 15, Name = "Mercado Pago", Code = "323", Country = "BR", CreatedAt = new DateTime(2024, 1, 1) },

            // American banks
            new() { Id = 16, Name = "JPMorgan Chase", Code = "CHASUS33", Country = "US", CreatedAt = new DateTime(2024, 1, 1) },
            new() { Id = 17, Name = "Bank of America", Code = "BOFAUS3N", Country = "US", CreatedAt = new DateTime(2024, 1, 1) },
            new() { Id = 18, Name = "Wells Fargo", Code = "WFBIUS6S", Country = "US", CreatedAt = new DateTime(2024, 1, 1) },
            new() { Id = 19, Name = "Citibank", Code = "CITIUS33", Country = "US", CreatedAt = new DateTime(2024, 1, 1) },
            new() { Id = 20, Name = "U.S. Bancorp", Code = "USBKUS44", Country = "US", CreatedAt = new DateTime(2024, 1, 1) },
            new() { Id = 21, Name = "Truist Financial", Code = "SNTRUS3A", Country = "US", CreatedAt = new DateTime(2024, 1, 1) },
            new() { Id = 22, Name = "PNC Financial", Code = "PNCCUS33", Country = "US", CreatedAt = new DateTime(2024, 1, 1) },
            new() { Id = 23, Name = "Goldman Sachs", Code = "GSCOUSS", Country = "US", CreatedAt = new DateTime(2024, 1, 1) },
            new() { Id = 24, Name = "Capital One", Code = "HIBKUS44", Country = "US", CreatedAt = new DateTime(2024, 1, 1) },
            new() { Id = 25, Name = "American Express", Code = "AEIBUS33", Country = "US", CreatedAt = new DateTime(2024, 1, 1) },
        ];
    }
}
