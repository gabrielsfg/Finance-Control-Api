using FinanceControl.Shared.Dtos.Others;
using FinanceControl.Shared.Enums;
using System.Collections.Generic;

namespace FinanceControl.Shared.Dtos.Response
{
    public class GetAccountByIdResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public EnumAccountType AccountType { get; set; }
        public int CurrentAmount { get; set; }
        public int? GoalAmount {  get; set; }
        public bool IsDefaultAccount { get; set; }
        public bool IsExcludedFromNetWorth { get; set; }
        public int? BillingDueDay { get; set; }
        public int? CreditLimit { get; set; }
        public List<RecentTransactionDto> RecentTransactions { get; set; } = [];
    }
}
