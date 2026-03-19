using FinanceControl.Shared.Enums;

namespace FinanceControl.Shared.Dtos.Request
{
    public class GetTransactionsQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? OrderBy { get; set; } = "date_desc";
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public EnumTransactionType? Type { get; set; }
        public EnumPaymentType? PaymentType { get; set; }
        public int? AccountId { get; set; }
        public int? SubCategoryId { get; set; }
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
        public string? Search { get; set; }
    }
}
