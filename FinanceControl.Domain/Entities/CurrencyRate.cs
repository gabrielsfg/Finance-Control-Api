using FinanceControl.Domain.Common;

namespace FinanceControl.Domain.Entities
{
    public class CurrencyRate : BaseEntity
    {
        public string BaseCurrency { get; set; } = string.Empty;
        public string TargetCurrency { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public DateTime FetchedAt { get; set; }
    }
}
