using FinanceControl.Shared.Enums;

namespace FinanceControl.Shared.Dtos.Response
{
    public class GetWishlistItemByIdResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int CurrentPrice { get; set; }
        public int? TargetPrice { get; set; }
        public EnumWishlistPriority Priority { get; set; }
        public DateOnly? Deadline { get; set; }
        public EnumWishlistStatus Status { get; set; }
        public int? PurchasedTransactionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public List<WishlistItemLinkDto> Links { get; set; } = [];
    }
}
