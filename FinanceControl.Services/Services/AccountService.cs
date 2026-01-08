using FinanceControl.Data.Data;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Interfaces.Service;
using FinanceControl.Shared.Dtos.Request;
using FinanceControl.Shared.Dtos.Respose;
using FinanceControl.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceControl.Services.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;

        public AccountService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<GetAllAccountResponseDto>> CreateAccountAsync(CreateAccountRequestDto requestDto, int userId)
        {
            var account = new Account()
            {
                UserId = userId,
                Name = requestDto.Name,
                CurrentBalance = requestDto.CurrentBalance,
                GoalAmount = requestDto.GoalAmount,
                IsDefaultAccount = true
            };

            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();

            var accounts = await GetAllAccountAsync(userId);
            return Result<GetAllAccountResponseDto>.Success(accounts);
        }

        public async Task<GetAllAccountResponseDto> GetAllAccountAsync(int userId)
        {
            var accounts = await _context.Accounts
                .Where(a => a.UserId.Equals(userId))
                .OrderBy(a => a.Name)
                .Select(a => new GetAccountItemResponseDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    CurrentAmout = a.CurrentBalance
                })
                .ToListAsync();

            return new GetAllAccountResponseDto()
            {
                Accounts = accounts
            };

        }
    }
}
