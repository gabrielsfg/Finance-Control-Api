using FinanceControl.Domain.Common;

namespace FinanceControl.Domain.Entities
{
    public class WishlistItemPriceHistory : BaseEntity
    {
        public int WishlistItemId { get; set; }
        public int Price { get; set; }

        public WishlistItem WishlistItem { get; set; } = null!;
    }
}
