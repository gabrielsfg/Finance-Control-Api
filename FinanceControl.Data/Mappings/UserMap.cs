using FinanceControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceControl.Data.Mappings
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Email);
            builder.Property(u => u.PasswordHash);
            builder.Property(u => u.Name);
            builder.Property(u => u.IsActive).HasDefaultValue(true);
            builder.Property(u => u.FailedLoginAttempts).HasDefaultValue(0);
            builder.Property(u => u.LockoutEnd);
            builder.Property(u => u.PreferredCurrency).HasDefaultValue("BRL").HasMaxLength(3);
            builder.Property(u => u.PreferredLanguage).HasDefaultValue("pt-BR").HasMaxLength(10);
            builder.Property(u => u.Country).HasMaxLength(2);
            builder.Property(u => u.IsEmailVerified).HasDefaultValue(false);
            builder.Property(u => u.EmailVerificationTokenHash);
            builder.Property(u => u.EmailVerificationTokenExpiresAt);
            builder.Property(u => u.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("timezone('America/Sao_Paulo', now())")
                .IsRequired()
                .ValueGeneratedOnAdd();
            builder.Property(u => u.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .ValueGeneratedOnAdd();
        }
    }
}
