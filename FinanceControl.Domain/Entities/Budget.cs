using FinanceControl.Domain.Common;
using FinanceControl.Domain.Enums;

namespace FinanceControl.Domain.Entities
{
    public class Budget : OwnedEntity
    {
        public string Name { get; set; }
        public int StartDate {  get; set; }
        public EnumBudgetRecurrence Reccurrence { get; set; }

    }
}
