using FinanceControl.Domain.Common;

namespace FinanceControl.Domain.Entities
{
    public class WishlistItemLink : BaseEntity
    {
        public int WishlistItemId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string? StoreName { get; set; }

        public WishlistItem WishlistItem { get; set; } = null!;
    }
}
