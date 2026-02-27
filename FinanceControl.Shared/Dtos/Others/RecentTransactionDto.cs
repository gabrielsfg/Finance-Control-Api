using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceControl.Shared.Dtos.Others
{
    public class RecentTransactionDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Value { get; set; }
        public int Type { get; set; }
        public string SubCategoryName { get; set; }
        public string CategoryName { get; set; }
    }
}
