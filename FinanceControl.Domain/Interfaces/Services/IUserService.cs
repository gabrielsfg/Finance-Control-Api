using FinanceControl.Shared.Dtos;
using FinanceControl.Shared.Dtos.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceControl.Domain.Interfaces.Service
{
    public interface IUserService
    {
        Task<string?> RegisterUserAsync(CreateUserRequestDto requestDto);

        Task<string?> UserLoginAsync(UserLoginRequestDto requestDto);
    }
}
