namespace FinanceControl.Shared.Dtos.Request
{
    public class PatchUserRequestDto
    {
        public string? Name { get; set; }
        public string? PreferredCurrency { get; set; }
        public string? PreferredLanguage { get; set; }
        public string? Country { get; set; }
    }
}
