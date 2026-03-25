using FinanceControl.Shared.Enums;

namespace FinanceControl.Shared.Dtos.Response
{
    public class GetAccountItemResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public EnumAccountType AccountType { get; set; }
        public int CurrentAmount { get; set; }
        public bool IsDefaultAccount { get; set; }
        public bool IsExcludedFromNetWorth { get; set; }
        public int? BillingDueDay { get; set; }
        public int? CreditLimit { get; set; }
    }
}
