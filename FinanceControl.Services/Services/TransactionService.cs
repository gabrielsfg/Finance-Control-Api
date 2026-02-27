using FinanceControl.Data.Data;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Enums;
using FinanceControl.Domain.Interfaces.Services;
using FinanceControl.Shared.Dtos.Others;
using FinanceControl.Shared.Dtos.Request;
using FinanceControl.Shared.Dtos.Response;
using FinanceControl.Shared.Helpers;
using FinanceControl.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceControl.Services.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public TransactionService(ApplicationDbContext context, IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _context = context;
            _contextFactory = contextFactory;
        }

        public async Task<Result<CreateTransactionResponseDto>> CreateTransactionAsync(CreateTransactionRequestDto requestDto, int userId)
        {
            if (!Enum.TryParse<EnumTransactionType>(requestDto.Type, out var type))
                return Result<CreateTransactionResponseDto>.Failure("Invalid transaction type.");

            if (!Enum.TryParse<EnumPaymentType>(requestDto.PaymentType, out var paymentType))
                return Result<CreateTransactionResponseDto>.Failure("Invalid payment type.");

            var accountExists = await _context.Accounts
                .AnyAsync(a => a.Id == requestDto.AccountId && a.UserId == userId);
            if (!accountExists)
                return Result<CreateTransactionResponseDto>.Failure("Invalid parameters.");

            var subCategoryExists = await _context.SubCategories
                .AnyAsync(sc => sc.Id == requestDto.SubCategoryId && sc.UserId == userId);
            if (!subCategoryExists)
                return Result<CreateTransactionResponseDto>.Failure("Invalid parameters.");

            if (requestDto.BudgetId.HasValue)
            {
                var budgetExists = await _context.Budgets
                    .AnyAsync(b => b.Id == requestDto.BudgetId && b.UserId == userId);
                if (!budgetExists)
                    return Result<CreateTransactionResponseDto>.Failure("Invalid parameters.");
            }

            await using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var createdTransactions = paymentType switch
                {
                    EnumPaymentType.OneTime => await CreateOneTimeAsync(requestDto, userId, type),
                    EnumPaymentType.Installment => await CreateInstallmentsAsync(requestDto, userId, type),
                    EnumPaymentType.Recurring => await CreateRecurringAsync(requestDto, userId, type),
                    _ => null
                };

                if (createdTransactions is null || createdTransactions.Count == 0)
                    return Result<CreateTransactionResponseDto>.Failure("Invalid payment type.");

                await _context.SaveChangesAsync(); // único SaveChanges de toda a operação
                await dbTransaction.CommitAsync();

                var transactionIds = createdTransactions.Select(t => t.Id).ToList();
                var response = await BuildCreateResponseAsync(transactionIds);

                return Result<CreateTransactionResponseDto>.Success(response);
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Result<IEnumerable<GetTransactionResponseDto>>> GetAllTransactionsAsync(int userId)
        {
            var transactions = await GetTransactionQuery(userId)
                .ToListAsync();

            return Result<IEnumerable<GetTransactionResponseDto>>.Success(transactions);
        }

        public async Task<Result<IEnumerable<GetTransactionResponseDto>>> GetAllTransactionsByBudgetAsync(int budgetId, int userId)
        {
            var budgetExists = await _context.Budgets
                .AnyAsync(b => b.Id == budgetId && b.UserId == userId);
            if (!budgetExists)
                return Result<IEnumerable<GetTransactionResponseDto>>.Failure("Invalid parameters.");

            var transactions = await GetTransactionQuery(userId)
                .Where(t => t.BudgetId == budgetId)
                .ToListAsync();

            return Result<IEnumerable<GetTransactionResponseDto>>.Success(transactions);
        }

        public async Task<Result<IEnumerable<GetTransactionResponseDto>>> GetAllTransactionsByAccountAsync(int accountId, int userId)
        {
            var accountExists = await _context.Accounts
                .AnyAsync(a => a.Id == accountId && a.UserId == userId);
            if (!accountExists)
                return Result<IEnumerable<GetTransactionResponseDto>>.Failure("Invalid parameters.");

            var transactions = await GetTransactionQuery(userId)
                .Where(t => t.AccountId == accountId)
                .ToListAsync();

            return Result<IEnumerable<GetTransactionResponseDto>>.Success(transactions);
        }

        public async Task<Result<IEnumerable<GetTransactionResponseDto>>> GetAllTransactionsBySubCategoryAsync(int subCategoryId, int userId)
        {
            var subCategoryExists = await _context.SubCategories
                .AnyAsync(sc => sc.Id == subCategoryId && sc.UserId == userId);
            if (!subCategoryExists)
                return Result<IEnumerable<GetTransactionResponseDto>>.Failure("Invalid parameters.");

            var transactions = await GetTransactionQuery(userId)
                .Where(t => t.SubCategoryId == subCategoryId)
                .ToListAsync();

            return Result<IEnumerable<GetTransactionResponseDto>>.Success(transactions);
        }

        public async Task<Result<GetTransactionByIdResponseDto>> GetTransactionByIdAsync(int id, int userId)
        {
            var transaction = await _context.Transactions
                .Where(t => t.Id == id && t.UserId == userId)
                .Select(t => new GetTransactionByIdResponseDto
                {
                    Id = t.Id,
                    BudgetId = t.BudgetId,
                    SubCategoryId = t.SubCategoryId,
                    SubCategoryName = t.SubCategory.Name,
                    AccountId = t.AccountId,
                    AccountName = t.Account.Name,
                    RecurringTransactionId = t.RecurringTransactionId,
                    ParentTransactionId = t.ParentTransactionId,
                    Value = t.Value,
                    Type = t.Type.ToString(),
                    Description = t.Description,
                    TransactionDate = t.TransactionDate,
                    PaymentType = t.PaymentType.ToString(),
                    InstallmentNumber = t.InstallmentNumber,
                    TotalInstallments = t.TotalInstallments,
                    IsPaid = t.IsPaid
                })
                .FirstOrDefaultAsync();

            if (transaction is null)
                return Result<GetTransactionByIdResponseDto>.Failure("Transaction not found.");

            return Result<GetTransactionByIdResponseDto>.Success(transaction);
        }

        public async Task<Result<IEnumerable<GetTransactionResponseDto>>> UpdateTransactionAsync(UpdateTransactionRequestDto requestDto, int id, int userId)
        {
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (transaction is null)
                return Result<IEnumerable<GetTransactionResponseDto>>.Failure("Transaction not found.");

            var accountExists = await _context.Accounts
                .AnyAsync(a => a.Id == requestDto.AccountId && a.UserId == userId);
            if (!accountExists)
                return Result<IEnumerable<GetTransactionResponseDto>>.Failure("Invalid parameters.");

            var subCategoryExists = await _context.SubCategories
                .AnyAsync(sc => sc.Id == requestDto.SubCategoryId && sc.UserId == userId);
            if (!subCategoryExists)
                return Result<IEnumerable<GetTransactionResponseDto>>.Failure("Invalid parameters.");

            if (requestDto.BudgetId.HasValue)
            {
                var budgetExists = await _context.Budgets
                    .AnyAsync(b => b.Id == requestDto.BudgetId && b.UserId == userId);
                if (!budgetExists)
                    return Result<IEnumerable<GetTransactionResponseDto>>.Failure("Invalid parameters.");
            }

            transaction.AccountId = requestDto.AccountId;
            transaction.SubCategoryId = requestDto.SubCategoryId;
            transaction.BudgetId = requestDto.BudgetId;
            transaction.Value = requestDto.Value;
            transaction.Description = requestDto.Description;
            transaction.TransactionDate = requestDto.TransactionDate;

            await _context.SaveChangesAsync();

            return await GetAllTransactionsAsync(userId);
        }

        public async Task<Result<IEnumerable<GetTransactionResponseDto>>> DeleteTransactionAsync(int id, int userId)
        {
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (transaction is null)
                return Result<IEnumerable<GetTransactionResponseDto>>.Failure("Transaction not found.");

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return await GetAllTransactionsAsync(userId);
        }

        public async Task<Result<IEnumerable<GetTransactionResponseDto>>> UpdateRecurringTransactionAsync(UpdateRecurringTransactionRequestDto requestDto, int recurringId, int userId)
        {
            var recurring = await _context.RecurringTransactions
                .FirstOrDefaultAsync(rt => rt.Id == recurringId && rt.UserId == userId);

            if (recurring is null)
                return Result<IEnumerable<GetTransactionResponseDto>>.Failure("Recurring transaction not found.");

            var accountExists = await _context.Accounts
                .AnyAsync(a => a.Id == requestDto.AccountId && a.UserId == userId);
            if (!accountExists)
                return Result<IEnumerable<GetTransactionResponseDto>>.Failure("Invalid parameters.");

            var subCategoryExists = await _context.SubCategories
                .AnyAsync(sc => sc.Id == requestDto.SubCategoryId && sc.UserId == userId);
            if (!subCategoryExists)
                return Result<IEnumerable<GetTransactionResponseDto>>.Failure("Invalid parameters.");

            if (requestDto.BudgetId.HasValue)
            {
                var budgetExists = await _context.Budgets
                    .AnyAsync(b => b.Id == requestDto.BudgetId && b.UserId == userId);
                if (!budgetExists)
                    return Result<IEnumerable<GetTransactionResponseDto>>.Failure("Invalid parameters.");
            }

            if (requestDto.EndDate.HasValue && requestDto.EndDate.Value <= DateOnly.FromDateTime(DateTime.UtcNow))
                return Result<IEnumerable<GetTransactionResponseDto>>.Failure("EndDate must be a future date.");

            recurring.AccountId = requestDto.AccountId;
            recurring.SubCategoryId = requestDto.SubCategoryId;
            recurring.BudgetId = requestDto.BudgetId;
            recurring.Value = requestDto.Value;
            recurring.Description = requestDto.Description;
            recurring.EndDate = requestDto.EndDate;

            await _context.SaveChangesAsync();

            return await GetAllTransactionsAsync(userId);
        }

        public async Task<Result<IEnumerable<GetTransactionResponseDto>>> CancelRecurringTransactionAsync(int recurringId, int userId)
        {
            var recurring = await _context.RecurringTransactions
                .FirstOrDefaultAsync(rt => rt.Id == recurringId && rt.UserId == userId);

            if (recurring is null)
                return Result<IEnumerable<GetTransactionResponseDto>>.Failure("Recurring transaction not found.");

            if (!recurring.IsActive)
                return Result<IEnumerable<GetTransactionResponseDto>>.Failure("Recurring transaction is already inactive.");

            recurring.IsActive = false;

            await _context.SaveChangesAsync();

            return await GetAllTransactionsAsync(userId);
        }

        /// <summary>
        /// Main Page Endpoints
        /// </summary>
        public async Task<BalanceSummaryDto> GetSummaryBalanceAsync(MainPageSummaryRequestDto requestDto)
        {
            await using var context = _contextFactory.CreateDbContext();

            var result = await context.Transactions
                    .Where(t => t.UserId == requestDto.UserId)
                    .WhereIf(requestDto.BudgetId.HasValue, t => t.BudgetId == requestDto.BudgetId)
                    .Where(t => t.TransactionDate >= requestDto.StartDate && t.TransactionDate <= requestDto.FinishDate)
                    .GroupBy(_ => 1)
                    .Select(g => new BalanceSummaryDto
                    {
                        TotalIncome = g
                            .Where(t => t.Type == EnumTransactionType.Income)
                            .Sum(t => (int?)t.Value) ?? 0,
                        TotalExpenses = g
                            .Where(t => t.Type == EnumTransactionType.Expense)
                            .Sum(t => (int?)t.Value) ?? 0
                    })
                    .FirstOrDefaultAsync();

            if (result is null)
                return new BalanceSummaryDto();

            result.Balance = result.TotalIncome - result.TotalExpenses;
            return result;
        }

        public async Task<List<RecentTransactionDto>> GetRecentTransactionsAsync(MainPageSummaryRequestDto requestDto)
        {
            await using var context = _contextFactory.CreateDbContext();

            return await context.Transactions
                .Where(t => t.UserId == requestDto.UserId)
                .WhereIf(requestDto.BudgetId.HasValue, t => t.BudgetId == requestDto.BudgetId)
                .Where(t => t.TransactionDate >= requestDto.StartDate && t.TransactionDate <= requestDto.FinishDate)
                .OrderByDescending(t => t.TransactionDate)
                .ThenByDescending(t => t.CreatedAt)
                .Take(5)
                .Select(t => new RecentTransactionDto
                {
                    Id = t.Id,
                    Description = t.Description,
                    Value = t.Value,
                    Type = (int)t.Type,
                    SubCategoryName = t.SubCategory.Name,
                    CategoryName = t.SubCategory.Category.Name
                })
                .ToListAsync();
        }

        public async Task<BudgetSummaryDto> GetBudgetSummaryAsync(MainPageSummaryRequestDto requestDto)
        {
            await using var context = _contextFactory.CreateDbContext();

            var totalExpected = await context.BudgetSubcategoryAllocations
                .Where(a => a.Budget.UserId == requestDto.UserId)
                .WhereIf(requestDto.BudgetId.HasValue, a => a.BudgetId == requestDto.BudgetId)
                .SumAsync(a => (int?)a.ExpectedValue) ?? 0;

            var totalSpent = await context.Transactions
                .Where(t => t.UserId == requestDto.UserId)
                .Where(t => t.Type == EnumTransactionType.Expense)
                .WhereIf(requestDto.BudgetId.HasValue, t => t.BudgetId == requestDto.BudgetId)
                .Where(t => t.TransactionDate >= requestDto.StartDate && t.TransactionDate <= requestDto.FinishDate)
                .SumAsync(t => (int?)t.Value) ?? 0;

            var spentPercentage = totalExpected > 0
                ? Math.Round((decimal)totalSpent / totalExpected * 100, 2)
                : 0m;

            return new BudgetSummaryDto
            {
                TotalExpected = totalExpected,
                TotalSpent = totalSpent,
                SpentPercentage = spentPercentage
            };
        }

        public async Task<List<TopCategoryItemDto>> GetTopCategoriesAsync(MainPageSummaryRequestDto requestDto)
        {
            await using var context = _contextFactory.CreateDbContext();

            return await context.Transactions
                .Where(t => t.UserId == requestDto.UserId)
                .Where(t => t.Type == EnumTransactionType.Expense)
                .WhereIf(requestDto.BudgetId.HasValue, t => t.BudgetId == requestDto.BudgetId)
                .Where(t => t.TransactionDate >= requestDto.StartDate && t.TransactionDate <= requestDto.FinishDate)
                .GroupBy(t => t.SubCategory.Category.Name)
                .Select(g => new TopCategoryItemDto
                {
                    CategoryName = g.Key,
                    TotalSpent = g.Sum(t => t.Value)
                })
                .OrderByDescending(x => x.TotalSpent)
                .Take(5)
                .ToListAsync();
        }

        /// <summary>
        /// Private methods
        /// </summary>
        private async Task<List<Transaction>> CreateOneTimeAsync(CreateTransactionRequestDto dto, int userId, EnumTransactionType type)
        {
            var transaction = new Transaction
            {
                UserId = userId,
                BudgetId = dto.BudgetId,
                SubCategoryId = dto.SubCategoryId,
                AccountId = dto.AccountId,
                Value = dto.Value,
                Type = type,
                Description = dto.Description,
                TransactionDate = dto.TransactionDate,
                PaymentType = EnumPaymentType.OneTime,
                IsPaid = false
            };

            _context.Transactions.Add(transaction);
            return [transaction];
        }

        private async Task<List<Transaction>> CreateInstallmentsAsync(CreateTransactionRequestDto dto, int userId, EnumTransactionType type)
        {
            if (!dto.TotalInstallments.HasValue || dto.TotalInstallments <= 0)
                return [];

            var (firstValue, otherValue) = CalculateInstallmentValues(dto.Value, dto.TotalInstallments.Value);
            var transactions = new List<Transaction>();

            var parent = new Transaction
            {
                UserId = userId,
                BudgetId = dto.BudgetId,
                SubCategoryId = dto.SubCategoryId,
                AccountId = dto.AccountId,
                Value = firstValue,
                Type = type,
                Description = dto.Description,
                TransactionDate = dto.TransactionDate,
                PaymentType = EnumPaymentType.Installment,
                InstallmentNumber = 1,
                TotalInstallments = dto.TotalInstallments,
                IsPaid = false
            };

            _context.Transactions.Add(parent);
            await _context.SaveChangesAsync();

            transactions.Add(parent);

            for (int i = 2; i <= dto.TotalInstallments; i++)
            {
                var installment = new Transaction
                {
                    UserId = userId,
                    BudgetId = dto.BudgetId,
                    SubCategoryId = dto.SubCategoryId,
                    AccountId = dto.AccountId,
                    ParentTransactionId = parent.Id,
                    Value = otherValue,
                    Type = type,
                    Description = dto.Description,
                    TransactionDate = dto.TransactionDate.AddMonths(i - 1),
                    PaymentType = EnumPaymentType.Installment,
                    InstallmentNumber = i,
                    TotalInstallments = dto.TotalInstallments,
                    IsPaid = false
                };

                _context.Transactions.Add(installment);
                transactions.Add(installment);
            }

            return transactions;
        }

        private async Task<List<Transaction>> CreateRecurringAsync(CreateTransactionRequestDto dto, int userId, EnumTransactionType type)
        {
            if (!Enum.TryParse<EnumRecurrenceType>(dto.Recurrence, out var recurrence))
                return [];

            if (recurrence == EnumRecurrenceType.None)
                return [];

            var recurringTemplate = new RecurringTransaction
            {
                UserId = userId,
                BudgetId = dto.BudgetId,
                SubCategoryId = dto.SubCategoryId,
                AccountId = dto.AccountId,
                Value = dto.Value,
                Type = type,
                Description = dto.Description,
                Recurrence = recurrence,
                StartDate = dto.TransactionDate,
                EndDate = dto.RecurringEndDate,
                IsActive = true
            };

            _context.RecurringTransactions.Add(recurringTemplate);
            await _context.SaveChangesAsync();

            var transaction = new Transaction
            {
                UserId = userId,
                BudgetId = dto.BudgetId,
                SubCategoryId = dto.SubCategoryId,
                AccountId = dto.AccountId,
                RecurringTransactionId = recurringTemplate.Id,
                Value = dto.Value,
                Type = type,
                Description = dto.Description,
                TransactionDate = dto.TransactionDate,
                PaymentType = EnumPaymentType.Recurring,
                IsPaid = false
            };

            _context.Transactions.Add(transaction);
            return [transaction];
        }

        private IQueryable<GetTransactionResponseDto> GetTransactionQuery(int userId)
        {
            return _context.Transactions
                .Where(t => t.UserId == userId)
                .Select(t => new GetTransactionResponseDto
                {
                    Id = t.Id,
                    BudgetId = t.BudgetId,
                    SubCategoryId = t.SubCategoryId,
                    SubCategoryName = t.SubCategory.Name,
                    AccountId = t.AccountId,
                    AccountName = t.Account.Name,
                    RecurringTransactionId = t.RecurringTransactionId,
                    ParentTransactionId = t.ParentTransactionId,
                    Value = t.Value,
                    Type = t.Type.ToString(),
                    Description = t.Description,
                    TransactionDate = t.TransactionDate,
                    PaymentType = t.PaymentType.ToString(),
                    InstallmentNumber = t.InstallmentNumber,
                    TotalInstallments = t.TotalInstallments,
                    IsPaid = t.IsPaid
                });
        }

        private async Task<CreateTransactionResponseDto> BuildCreateResponseAsync(List<int> transactionIds)
        {
            var transactions = await _context.Transactions
                .Where(t => transactionIds.Contains(t.Id))
                .Select(t => new GetTransactionResponseDto
                {
                    Id = t.Id,
                    BudgetId = t.BudgetId,
                    SubCategoryId = t.SubCategoryId,
                    SubCategoryName = t.SubCategory.Name,
                    AccountId = t.AccountId,
                    AccountName = t.Account.Name,
                    RecurringTransactionId = t.RecurringTransactionId,
                    ParentTransactionId = t.ParentTransactionId,
                    Value = t.Value,
                    Type = t.Type.ToString(),
                    Description = t.Description,
                    TransactionDate = t.TransactionDate,
                    PaymentType = t.PaymentType.ToString(),
                    InstallmentNumber = t.InstallmentNumber,
                    TotalInstallments = t.TotalInstallments,
                    IsPaid = t.IsPaid
                })
                .ToListAsync();

            return new CreateTransactionResponseDto { Transactions = transactions };
        }

        private static (int firstValue, int otherValue) CalculateInstallmentValues(int total, int installments)
        {
            var baseValue = total / installments;
            var remainder = total % installments;
            return (baseValue + remainder, baseValue);
        }
    }
}
