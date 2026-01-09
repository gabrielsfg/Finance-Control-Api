using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceControl.Shared.Dtos.Respose
{
    public class GetAllAccountResponseDto
    {
        public IEnumerable<GetAccountItemResponseDto>? Accounts { get; set; }
    }
}
