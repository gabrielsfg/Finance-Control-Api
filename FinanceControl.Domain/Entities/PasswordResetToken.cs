using FinanceControl.Domain.Common;

namespace FinanceControl.Domain.Entities
{
    public class PasswordResetToken : BaseEntity
    {
        public int UserId { get; set; }
        public string TokenHash { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;
        public User User { get; set; }
    }
}
