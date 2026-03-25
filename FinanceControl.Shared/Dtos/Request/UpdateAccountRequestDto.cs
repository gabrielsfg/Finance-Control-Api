using FinanceControl.Shared.Enums;

namespace FinanceControl.Shared.Dtos.Request
{
    public class UpdateAccountRequestDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public EnumAccountType AccountType { get; set; }
        public int? GoalAmount { get; set; }
        public bool IsDefaultAccount { get; set; }
        public bool IsExcludedFromNetWorth { get; set; }
        public int? BillingDueDay { get; set; }
        public int? CreditLimit { get; set; }
    }
}
