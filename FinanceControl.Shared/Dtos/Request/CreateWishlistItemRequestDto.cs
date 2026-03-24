using FinanceControl.Shared.Enums;

namespace FinanceControl.Shared.Dtos.Request
{
    public class CreateWishlistItemRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int CurrentPrice { get; set; }
        public int? TargetPrice { get; set; }
        public EnumWishlistPriority Priority { get; set; }
        public DateOnly? Deadline { get; set; }
        public List<CreateWishlistItemLinkDto> Links { get; set; } = [];
    }

    public class CreateWishlistItemLinkDto
    {
        public string Url { get; set; } = string.Empty;
        public string? StoreName { get; set; }
    }
}
