namespace FinanceControl.Shared.Dtos.Response
{
    public class GetUserMeResponseDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PreferredCurrency { get; set; }
        public string PreferredLanguage { get; set; }
        public string? Country { get; set; }
        public bool IsEmailVerified { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
