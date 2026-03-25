namespace FinanceControl.Shared.Dtos.Request
{
    public class PurchaseWishlistItemRequestDto
    {
        public bool CreateTransaction { get; set; }
        public int? AccountId { get; set; }
        public int? SubCategoryId { get; set; }
        public DateOnly? TransactionDate { get; set; }
        public string? Description { get; set; }
    }
}
