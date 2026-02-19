using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceControl.Shared.Dtos.Response
{
    public class GetTransactionResponseDto
    {
        public int Id { get; set; }
        public int? BudgetId { get; set; }
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public int? RecurringTransactionId { get; set; }
        public int? ParentTransactionId { get; set; }
        public int Value { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateOnly TransactionDate { get; set; }
        public string PaymentType { get; set; }
        public int? InstallmentNumber { get; set; }
        public int? TotalInstallments { get; set; }
        public bool IsPaid { get; set; }
    }
}
