namespace FinanceControl.Shared.Dtos.Response
{
    public class WishlistItemLinkDto
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string? StoreName { get; set; }
    }
}
