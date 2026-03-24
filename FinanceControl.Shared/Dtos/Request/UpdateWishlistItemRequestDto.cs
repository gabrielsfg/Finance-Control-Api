using FinanceControl.Shared.Enums;

namespace FinanceControl.Shared.Dtos.Request
{
    public class UpdateWishlistItemRequestDto
    {
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
        public int? TargetPrice { get; set; }
        public EnumWishlistPriority? Priority { get; set; }
        public DateOnly? Deadline { get; set; }
    }
}
