using FinanceControl.Domain.Common;

namespace FinanceControl.Domain.Entities
{
    public class Bank : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string Country { get; set; } = string.Empty;
    }
}
