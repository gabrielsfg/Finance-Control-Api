using FinanceControl.Domain.Common;

namespace FinanceControl.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public int UserId { get; set; }
        public string TokenHash { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; } = false;
        public User User { get; set; }
    }
}
