using FinanceControl.Shared.Dtos.Response;

namespace FinanceControl.Shared.Models
{
    public class LoginResult
    {
        public bool IsSuccess { get; private set; }
        public bool IsLocked { get; private set; }
        public AuthResponseDto? Auth { get; private set; }
        public DateTime? LockoutEnd { get; private set; }

        private LoginResult() { }

        public static LoginResult Success(AuthResponseDto auth) =>
            new() { IsSuccess = true, Auth = auth };

        public static LoginResult Locked(DateTime lockoutEnd) =>
            new() { IsLocked = true, LockoutEnd = lockoutEnd };

        public static LoginResult Failed() =>
            new();
    }
}
