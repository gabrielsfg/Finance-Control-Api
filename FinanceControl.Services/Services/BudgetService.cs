using FinanceControl.Data.Data;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Interfaces.Service;
using FinanceControl.Shared.Dtos.Request;
using FinanceControl.Shared.Dtos.Respose;
using FinanceControl.Shared.Enums;
using FinanceControl.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Services.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly ApplicationDbContext _context;

        public BudgetService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<GetBudgetByIdResponseDto>> CreateBudgetAsync(CreateBudgetResquestDto requestDto, int userId)
        {
            Budget budget = new Budget
            {
                Name = requestDto.Name,
                StartDate = requestDto.StartDate,
                Recurrence = requestDto.Recurrence,
                UserId = userId,
                IsActive = requestDto.IsActive
            };

            var hasAnyBudget = await _context.Budgets.AnyAsync(b => b.UserId == userId);
            if (!hasAnyBudget)
            {
                budget.IsActive = true;
            }
            else if (budget.IsActive)
            {
                var currentActive = await _context.Budgets.FirstOrDefaultAsync(b => b.UserId == userId && b.IsActive);
                if (currentActive != null)
                    currentActive.IsActive = false;
            }

            await _context.Budgets.AddAsync(budget);
            await _context.SaveChangesAsync();

            var result = await GetBudgetByIdAsync(budget.Id, userId);

            return Result<GetBudgetByIdResponseDto>.Success(result);
        }

        public async Task<IEnumerable<GetAllBudgetResponseDto>> GetAllBudgetAsync(int userId)
        {
            var budgets = await _context.Budgets.Where(b => b.UserId == userId).Select(b => new GetAllBudgetResponseDto
            {
                Id = b.Id,
                Name = b.Name,
                Recurrence = b.Recurrence
            }).ToListAsync();

            return budgets;
        }

        public async Task<GetBudgetByIdResponseDto> GetBudgetByIdAsync(int id, int userId)
        {
            var budget = await _context.Budgets.FirstOrDefaultAsync(b => b.UserId == userId && b.Id == id);

            if (budget == null)
                return null;

            var startDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, budget.StartDate);

            var finishDate = budget.Recurrence switch
            {
                EnumBudgetRecurrence.Weekly => startDate.AddDays(7),
                EnumBudgetRecurrence.Biweekly => startDate.AddDays(14),
                EnumBudgetRecurrence.Monthly => startDate.AddMonths(1),
                EnumBudgetRecurrence.Semiannually => startDate.AddMonths(6),
                EnumBudgetRecurrence.Annually => startDate.AddYears(1),
                _ => startDate
            };

            return new GetBudgetByIdResponseDto
            {
                Id = budget.Id,
                Name = budget.Name,
                StartDate = startDate,
                FinishDate = finishDate,
                Recurence = budget.Recurrence
            };
        }

        public async Task<Result<GetBudgetByIdResponseDto>> UpdateBudgetAsync(UpdateBudgetRequestDto requestDto, int userId)
        {
            var budget = _context.Budgets.FirstOrDefault(b => b.UserId == userId && b.Id == requestDto.Id);

            if (budget == null)
                return Result<GetBudgetByIdResponseDto>.Failure("Budget not found.");

            budget.Name = requestDto.Name;
            budget.StartDate = requestDto.StartDate;
            budget.Recurrence = requestDto.Recurrence;
            budget.IsActive = requestDto.IsActive;

            if (requestDto.IsActive)
            {
                var currentActive = await _context.Budgets
                    .FirstOrDefaultAsync(b => b.UserId == userId && b.IsActive && b.Id != requestDto.Id);
                if (currentActive != null)
                    currentActive.IsActive = false;
            }

            await _context.SaveChangesAsync();
            var budgetResult = await GetBudgetByIdAsync(requestDto.Id, userId);
            return Result<GetBudgetByIdResponseDto>.Success(budgetResult);
        }

        public async Task<Result<IEnumerable<GetAllBudgetResponseDto>>> DeleteBudgetAsync(int id, int userId)
        {
            var budget = await _context.Budgets.FirstOrDefaultAsync(b => b.UserId == userId && b.Id == id);

            if(budget == null)
                return Result<IEnumerable<GetAllBudgetResponseDto>>.Failure("Budget not found.");

            _context.Remove(budget);
            await _context.SaveChangesAsync();

            var budgets = await GetAllBudgetAsync(userId);

            return Result<IEnumerable<GetAllBudgetResponseDto>>.Success(budgets);
        }
    }
}
