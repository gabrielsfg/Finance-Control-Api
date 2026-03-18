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
        Task<AuthResponseDto?> RegisterUserAsync(CreateUserRequestDto requestDto);

        Task<LoginResult> UserLoginAsync(UserLoginRequestDto requestDto);

        Task<GetUserMeResponseDto?> GetUserByIdAsync(int userId);

        Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken);

        Task<bool> LogoutAsync(string refreshToken);

        Task ForgotPasswordAsync(string email);

        Task<bool> ResetPasswordAsync(string token, string newPassword);

        Task<bool> VerifyEmailAsync(string token);

        Task<GetUserMeResponseDto?> UpdateUserAsync(int userId, PatchUserRequestDto requestDto);

        Task<bool> DeleteUserAsync(int userId, string password);

        Task<bool> ResetUserDataAsync(int userId, string password);
    }
}
