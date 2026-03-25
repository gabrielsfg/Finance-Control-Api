namespace FinanceControl.Shared.Dtos.Request
{
    public class GetBudgetsQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? OrderBy { get; set; } = "name_asc";
    }
}
