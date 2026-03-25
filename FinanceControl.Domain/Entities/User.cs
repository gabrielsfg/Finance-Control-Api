using FinanceControl.Domain.Common;

namespace FinanceControl.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;
        public int FailedLoginAttempts { get; set; } = 0;
        public DateTime? LockoutEnd { get; set; }
        public string PreferredCurrency { get; set; } = "BRL";
        public string PreferredLanguage { get; set; } = "pt-BR";
        public string? Country { get; set; }
        public bool IsEmailVerified { get; set; } = false;
        public string? EmailVerificationTokenHash { get; set; }
        public DateTime? EmailVerificationTokenExpiresAt { get; set; }
    }
}
