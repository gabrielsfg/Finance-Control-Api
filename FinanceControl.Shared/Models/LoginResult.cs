namespace FinanceControl.Shared.Models
{
    public class LoginResult
    {
        public bool IsSuccess { get; private set; }
        public bool IsLocked { get; private set; }
        public string? Token { get; private set; }
        public DateTime? LockoutEnd { get; private set; }

        private LoginResult() { }

        public static LoginResult Success(string token) =>
            new() { IsSuccess = true, Token = token };

        public static LoginResult Locked(DateTime lockoutEnd) =>
            new() { IsLocked = true, LockoutEnd = lockoutEnd };

        public static LoginResult Failed() =>
            new();
    }
}
