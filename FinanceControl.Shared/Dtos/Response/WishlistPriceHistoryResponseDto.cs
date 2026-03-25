namespace FinanceControl.Shared.Dtos.Response
{
    public class WishlistPriceHistoryResponseDto
    {
        public int Id { get; set; }
        public int Price { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}
