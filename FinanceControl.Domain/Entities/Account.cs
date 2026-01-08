using FinanceControl.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceControl.Domain.Entities
{
    public class Account : OwnedEntity
    {
        public string Name { get; set; }
        public int CurrentBalance { get; set; }
        public int? GoalAmount {  get; set; }
        public bool IsDefaultAccount { get; set; }
    }
}
