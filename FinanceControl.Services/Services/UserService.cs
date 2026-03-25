using FinanceControl.Data.Data;
using FinanceControl.Data.Seed;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Interfaces.Service;
using FinanceControl.Shared.Dtos;
using FinanceControl.Shared.Dtos.Request;
using FinanceControl.Shared.Dtos.Response;
using FinanceControl.Shared.Enums;
using FinanceControl.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FinanceControl.Services.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public UserService(
            ApplicationDbContext context,
            IConfiguration configuration,
            IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<AuthResponseDto?> RegisterUserAsync(CreateUserRequestDto requestDto)
        {
            requestDto.Email = requestDto.Email.ToLower();

            if (await _context.Users.AnyAsync(u => u.Email == requestDto.Email))
                return null;

            var user = new User();
            var hasedPassword = new PasswordHasher<User>().HashPassword(user, requestDto.Password);

            user.Email = requestDto.Email;
            user.Name = requestDto.Name;
            user.PasswordHash = hasedPassword;

            _context.Add(user);
            await _context.SaveChangesAsync();

            // System category for balance updates
            var systemCategory = new Category
            {
                UserId = user.Id,
                Name = "BalanceUpdate",
                IsSystem = true
            };
            _context.Categories.Add(systemCategory);
            await _context.SaveChangesAsync();

            var systemSubCategory = new SubCategory
            {
                UserId = user.Id,
                CategoryId = systemCategory.Id,
                Name = "BalanceUpdate",
                IsSystem = true
            };
            _context.SubCategories.Add(systemSubCategory);
            await _context.SaveChangesAsync();

            // Default categories and subcategories
            foreach (var (categoryName, _, subCategories) in DefaultUserDataSeed.GetDefaultCategories())
            {
                var category = new Category
                {
                    UserId = user.Id,
                    Name = categoryName,
                    IsSystem = true
                };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                foreach (var subName in subCategories)
                {
                    _context.SubCategories.Add(new SubCategory
                    {
                        UserId = user.Id,
                        CategoryId = category.Id,
                        Name = subName,
                        IsSystem = true
                    });
                }
                await _context.SaveChangesAsync();
            }

            // Default account "Conta Principal"
            var defaultAccount = new Account
            {
                UserId = user.Id,
                Name = "Conta Principal",
                AccountType = EnumAccountType.Checking,
                IsDefaultAccount = true,
                IsExcludedFromNetWorth = false
            };
            _context.Accounts.Add(defaultAccount);
            await _context.SaveChangesAsync();

            var rawVerificationToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            user.EmailVerificationTokenHash = HashToken(rawVerificationToken);
            user.EmailVerificationTokenExpiresAt = DateTime.UtcNow.AddHours(24);
            await _context.SaveChangesAsync();

            var backendUrl = _configuration["AppSettings:BackendUrl"];
            var verificationLink = $"{backendUrl}/api/user/verify-email?token={Uri.EscapeDataString(rawVerificationToken)}";
            await _emailService.SendVerificationEmailAsync(user.Email, verificationLink);

            return await CreateAuthResponseAsync(user);
        }

        public async Task<LoginResult> UserLoginAsync(UserLoginRequestDto requestDto)
        {
            requestDto.Email = requestDto.Email.ToLower();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == requestDto.Email);
            if (user is null)
                return LoginResult.Failed();

            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
                return LoginResult.Locked(user.LockoutEnd.Value);

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, requestDto.Password) == PasswordVerificationResult.Failed)
            {
                user.FailedLoginAttempts++;

                if (user.FailedLoginAttempts >= 5)
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);

                await _context.SaveChangesAsync();
                return LoginResult.Failed();
            }

            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            await _context.SaveChangesAsync();

            return LoginResult.Success(await CreateAuthResponseAsync(user));
        }

        public async Task<GetUserMeResponseDto?> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
                return null;

            return new GetUserMeResponseDto
            {
                Name = user.Name,
                Email = user.Email,
                PreferredCurrency = user.PreferredCurrency,
                PreferredLanguage = user.PreferredLanguage,
                Country = user.Country,
                IsEmailVerified = user.IsEmailVerified,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken)
        {
            var tokenHash = HashToken(refreshToken);

            var stored = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);

            if (stored is null || stored.IsRevoked || stored.ExpiresAt <= DateTime.UtcNow)
                return null;

            stored.IsRevoked = true;
            await _context.SaveChangesAsync();

            return await CreateAuthResponseAsync(stored.User);
        }

        public async Task<bool> LogoutAsync(string refreshToken)
        {
            var tokenHash = HashToken(refreshToken);

            var stored = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash && !rt.IsRevoked);

            if (stored is null)
                return false;

            stored.IsRevoked = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task ForgotPasswordAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email.ToLower());

            // Resposta sempre igual para não revelar se o e-mail existe
            if (user is null)
                return;

            // Invalida tokens anteriores pendentes do mesmo usuário
            var existing = await _context.PasswordResetTokens
                .Where(t => t.UserId == user.Id && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();
            foreach (var t in existing)
                t.IsUsed = true;

            var raw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var entity = new PasswordResetToken
            {
                UserId = user.Id,
                TokenHash = HashToken(raw),
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            _context.PasswordResetTokens.Add(entity);
            await _context.SaveChangesAsync();

            var frontendUrl = _configuration["AppSettings:FrontendUrl"];
            var resetLink = $"{frontendUrl}/reset-password?token={Uri.EscapeDataString(raw)}";

            await _emailService.SendPasswordResetEmailAsync(user.Email, resetLink);
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            var tokenHash = HashToken(token);

            var stored = await _context.PasswordResetTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow);

            if (stored is null)
                return false;

            stored.IsUsed = true;

            var user = stored.User;
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, newPassword);
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;

            // Revoga todos os refresh tokens ativos do usuário
            var activeRefreshTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == user.Id && !rt.IsRevoked)
                .ToListAsync();
            foreach (var rt in activeRefreshTokens)
                rt.IsRevoked = true;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> VerifyEmailAsync(string token)
        {
            var tokenHash = HashToken(token);

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.EmailVerificationTokenHash == tokenHash &&
                u.EmailVerificationTokenExpiresAt > DateTime.UtcNow &&
                !u.IsEmailVerified);

            if (user is null)
                return false;

            user.IsEmailVerified = true;
            user.EmailVerificationTokenHash = null;
            user.EmailVerificationTokenExpiresAt = null;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<GetUserMeResponseDto?> UpdateUserAsync(int userId, PatchUserRequestDto requestDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
                return null;

            if (requestDto.Name is not null)
                user.Name = requestDto.Name;
            if (requestDto.PreferredCurrency is not null)
                user.PreferredCurrency = requestDto.PreferredCurrency;
            if (requestDto.PreferredLanguage is not null)
                user.PreferredLanguage = requestDto.PreferredLanguage;
            if (requestDto.Country is not null)
                user.Country = requestDto.Country.ToUpper();

            await _context.SaveChangesAsync();

            return new GetUserMeResponseDto
            {
                Name = user.Name,
                Email = user.Email,
                PreferredCurrency = user.PreferredCurrency,
                PreferredLanguage = user.PreferredLanguage,
                Country = user.Country,
                IsEmailVerified = user.IsEmailVerified,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<bool> DeleteUserAsync(int userId, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
                return false;

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, password) == PasswordVerificationResult.Failed)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ResetUserDataAsync(int userId, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
                return false;

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, password) == PasswordVerificationResult.Failed)
                return false;

            // Remove todos os dados financeiros do usuário
            // A ordem respeita as FKs: filhos antes dos pais que não têm cascade pelo EF
            _context.Transactions.RemoveRange(_context.Transactions.Where(t => t.UserId == userId));
            _context.RecurringTransactions.RemoveRange(_context.RecurringTransactions.Where(t => t.UserId == userId));
            _context.Budgets.RemoveRange(_context.Budgets.Where(b => b.UserId == userId));
            _context.Accounts.RemoveRange(_context.Accounts.Where(a => a.UserId == userId));
            _context.SubCategories.RemoveRange(_context.SubCategories.Where(sc => sc.UserId == userId));
            _context.Categories.RemoveRange(_context.Categories.Where(c => c.UserId == userId));
            _context.Areas.RemoveRange(_context.Areas.Where(a => a.UserId == userId));

            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<AuthResponseDto> CreateAuthResponseAsync(User user)
        {
            var accessToken = CreateAccessToken(user);
            var (rawRefreshToken, refreshTokenEntity) = CreateRefreshToken(user.Id);

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = rawRefreshToken
            };
        }

        private string CreateAccessToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token")!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
                audience: _configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        private (string raw, RefreshToken entity) CreateRefreshToken(int userId)
        {
            var raw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            var entity = new RefreshToken
            {
                UserId = userId,
                TokenHash = HashToken(raw),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            return (raw, entity);
        }

        private static string HashToken(string token)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToHexString(bytes);
        }
    }
}
