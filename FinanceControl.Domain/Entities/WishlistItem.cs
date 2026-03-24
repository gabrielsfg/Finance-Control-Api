using FinanceControl.Domain.Common;
using FinanceControl.Shared.Enums;

namespace FinanceControl.Domain.Entities
{
    public class WishlistItem : OwnedEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int CurrentPrice { get; set; }
        public int? TargetPrice { get; set; }
        public EnumWishlistPriority Priority { get; set; }
        public DateOnly? Deadline { get; set; }
        public EnumWishlistStatus Status { get; set; } = EnumWishlistStatus.Active;
        public int? PurchasedTransactionId { get; set; }

        public Transaction? PurchasedTransaction { get; set; }
        public ICollection<WishlistItemLink> Links { get; set; } = [];
        public ICollection<WishlistItemPriceHistory> PriceHistory { get; set; } = [];
    }
}
