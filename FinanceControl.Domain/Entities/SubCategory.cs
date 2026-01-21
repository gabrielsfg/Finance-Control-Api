using FinanceControl.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceControl.Domain.Entities
{
    public class SubCategory : OwnedEntity
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
    }
}
