using FinanceControl.Shared.Dtos;
using FinanceControl.Shared.Dtos.Request;
using FinanceControl.Shared.Dtos.Response;
using FinanceControl.Shared.Models;
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

        Task<LoginResult> UserLoginAsync(UserLoginRequestDto requestDto);

        Task<GetUserMeResponseDto?> GetUserByIdAsync(int userId);
    }
}
