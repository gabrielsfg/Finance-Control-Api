using FinanceControl.Shared.Dtos.Request;
using FinanceControl.Shared.Dtos.Respose;
using FinanceControl.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceControl.Domain.Interfaces.Service
{
    public interface IAccountService
    {
        Task<Result<GetAllAccountResponseDto>> CreateAccountAsync(CreateAccountRequestDto requestDto, int userId);
        Task<GetAllAccountResponseDto> GetAllAccountAsync(int userId);
    }
}
