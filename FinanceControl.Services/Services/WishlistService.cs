using FinanceControl.Data.Data;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Interfaces.Services;
using FinanceControl.Shared.Dtos.Request;
using FinanceControl.Shared.Dtos.Response;
using FinanceControl.Shared.Enums;
using FinanceControl.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Services.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly ApplicationDbContext _context;

        public WishlistService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<GetWishlistItemByIdResponseDto>> CreateWishlistItemAsync(CreateWishlistItemRequestDto requestDto, int userId)
        {
            var item = new WishlistItem
            {
                UserId = userId,
                Name = requestDto.Name,
                ImageUrl = requestDto.ImageUrl,
                CurrentPrice = requestDto.CurrentPrice,
                TargetPrice = requestDto.TargetPrice,
                Priority = requestDto.Priority,
                Deadline = requestDto.Deadline,
                Status = EnumWishlistStatus.Active,
                Links = requestDto.Links.Select(l => new WishlistItemLink
                {
                    Url = l.Url,
                    StoreName = l.StoreName
                }).ToList(),
                PriceHistory =
                [
                    new WishlistItemPriceHistory { Price = requestDto.CurrentPrice }
                ]
            };

            _context.WishlistItems.Add(item);
            await _context.SaveChangesAsync();

            var response = await BuildItemByIdResponseAsync(item.Id, userId);
            return Result<GetWishlistItemByIdResponseDto>.Success(response!);
        }

        public async Task<PagedResponse<GetWishlistItemResponseDto>> GetAllWishlistItemsPagedAsync(GetWishlistQueryDto query, int userId)
        {
            var q = _context.WishlistItems.Where(w => w.UserId == userId);

            if (query.Status.HasValue)
                q = q.Where(w => w.Status == query.Status.Value);

            q = query.OrderBy?.ToLower() switch
            {
                "created_asc" => q.OrderBy(w => w.CreatedAt),
                "price_asc" => q.OrderBy(w => w.CurrentPrice),
                "price_desc" => q.OrderByDescending(w => w.CurrentPrice),
                "priority_desc" => q.OrderByDescending(w => w.Priority),
                _ => q.OrderByDescending(w => w.CreatedAt)
            };

            var totalItems = await q.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);

            var items = await q
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(w => new GetWishlistItemResponseDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    ImageUrl = w.ImageUrl,
                    CurrentPrice = w.CurrentPrice,
                    TargetPrice = w.TargetPrice,
                    Priority = w.Priority,
                    Deadline = w.Deadline,
                    Status = w.Status,
                    CreatedAt = w.CreatedAt
                })
                .ToListAsync();

            return new PagedResponse<GetWishlistItemResponseDto>
            {
                CurrentPage = query.Page,
                PageSize = query.PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                RowCount = items.Count,
                Items = items
            };
        }

        public async Task<GetWishlistItemByIdResponseDto?> GetWishlistItemByIdAsync(int id, int userId)
        {
            return await BuildItemByIdResponseAsync(id, userId);
        }

        public async Task<Result<GetWishlistItemByIdResponseDto>> UpdateWishlistItemAsync(UpdateWishlistItemRequestDto requestDto, int id, int userId)
        {
            var item = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (item is null)
                return Result<GetWishlistItemByIdResponseDto>.Failure("Wishlist item not found.");

            if (item.Status == EnumWishlistStatus.Purchased)
                return Result<GetWishlistItemByIdResponseDto>.Failure("A purchased item cannot be edited.");

            if (requestDto.Name != null) item.Name = requestDto.Name;
            if (requestDto.ImageUrl != null) item.ImageUrl = requestDto.ImageUrl;
            if (requestDto.TargetPrice.HasValue) item.TargetPrice = requestDto.TargetPrice;
            if (requestDto.Priority.HasValue) item.Priority = requestDto.Priority.Value;
            if (requestDto.Deadline.HasValue) item.Deadline = requestDto.Deadline;

            await _context.SaveChangesAsync();

            var response = await BuildItemByIdResponseAsync(id, userId);
            return Result<GetWishlistItemByIdResponseDto>.Success(response!);
        }

        public async Task<Result> DeleteWishlistItemAsync(int id, int userId)
        {
            var item = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (item is null)
                return Result.Failure("Wishlist item not found.");

            _context.WishlistItems.Remove(item);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<WishlistPriceHistoryResponseDto>> RegisterPriceAsync(RegisterWishlistPriceRequestDto requestDto, int id, int userId)
        {
            var item = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (item is null)
                return Result<WishlistPriceHistoryResponseDto>.Failure("Wishlist item not found.");

            if (item.Status == EnumWishlistStatus.Purchased)
                return Result<WishlistPriceHistoryResponseDto>.Failure("Cannot update price of a purchased item.");

            item.CurrentPrice = requestDto.Price;

            var history = new WishlistItemPriceHistory
            {
                WishlistItemId = id,
                Price = requestDto.Price
            };

            _context.WishlistItemPriceHistory.Add(history);
            await _context.SaveChangesAsync();

            return Result<WishlistPriceHistoryResponseDto>.Success(new WishlistPriceHistoryResponseDto
            {
                Id = history.Id,
                Price = history.Price,
                RecordedAt = history.CreatedAt
            });
        }

        public async Task<Result<GetWishlistItemByIdResponseDto>> PurchaseWishlistItemAsync(PurchaseWishlistItemRequestDto requestDto, int id, int userId)
        {
            var item = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (item is null)
                return Result<GetWishlistItemByIdResponseDto>.Failure("Wishlist item not found.");

            if (item.Status != EnumWishlistStatus.Active)
                return Result<GetWishlistItemByIdResponseDto>.Failure("Only active items can be marked as purchased.");

            await using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (requestDto.CreateTransaction)
                {
                    var accountExists = await _context.Accounts
                        .AnyAsync(a => a.Id == requestDto.AccountId && a.UserId == userId);
                    if (!accountExists)
                        return Result<GetWishlistItemByIdResponseDto>.Failure("Account not found.");

                    var subCategoryExists = await _context.SubCategories
                        .AnyAsync(sc => sc.Id == requestDto.SubCategoryId && sc.UserId == userId);
                    if (!subCategoryExists)
                        return Result<GetWishlistItemByIdResponseDto>.Failure("SubCategory not found.");

                    var transaction = new Transaction
                    {
                        UserId = userId,
                        AccountId = requestDto.AccountId!.Value,
                        SubCategoryId = requestDto.SubCategoryId!.Value,
                        Value = item.CurrentPrice,
                        Type = EnumTransactionType.Expense,
                        PaymentType = EnumPaymentType.OneTime,
                        TransactionDate = requestDto.TransactionDate ?? DateOnly.FromDateTime(DateTime.Today),
                        Description = requestDto.Description ?? item.Name
                    };

                    _context.Transactions.Add(transaction);
                    await _context.SaveChangesAsync();

                    item.PurchasedTransactionId = transaction.Id;
                }

                item.Status = EnumWishlistStatus.Purchased;
                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                var response = await BuildItemByIdResponseAsync(id, userId);
                return Result<GetWishlistItemByIdResponseDto>.Success(response!);
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Result<IEnumerable<WishlistPriceHistoryResponseDto>>> GetPriceHistoryAsync(int id, int userId)
        {
            var itemExists = await _context.WishlistItems
                .AnyAsync(w => w.Id == id && w.UserId == userId);

            if (!itemExists)
                return Result<IEnumerable<WishlistPriceHistoryResponseDto>>.Failure("Wishlist item not found.");

            var history = await _context.WishlistItemPriceHistory
                .Where(h => h.WishlistItemId == id)
                .OrderBy(h => h.CreatedAt)
                .Select(h => new WishlistPriceHistoryResponseDto
                {
                    Id = h.Id,
                    Price = h.Price,
                    RecordedAt = h.CreatedAt
                })
                .ToListAsync();

            return Result<IEnumerable<WishlistPriceHistoryResponseDto>>.Success(history);
        }

        private async Task<GetWishlistItemByIdResponseDto?> BuildItemByIdResponseAsync(int id, int userId)
        {
            var item = await _context.WishlistItems
                .Include(w => w.Links)
                .Include(w => w.PriceHistory)
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (item is null) return null;

            return new GetWishlistItemByIdResponseDto
            {
                Id = item.Id,
                Name = item.Name,
                ImageUrl = item.ImageUrl,
                CurrentPrice = item.CurrentPrice,
                TargetPrice = item.TargetPrice,
                Priority = item.Priority,
                Deadline = item.Deadline,
                Status = item.Status,
                PurchasedTransactionId = item.PurchasedTransactionId,
                CreatedAt = item.CreatedAt,
                MinPrice = item.PriceHistory.Count > 0 ? item.PriceHistory.Min(h => h.Price) : null,
                MaxPrice = item.PriceHistory.Count > 0 ? item.PriceHistory.Max(h => h.Price) : null,
                Links = item.Links.Select(l => new WishlistItemLinkDto
                {
                    Id = l.Id,
                    Url = l.Url,
                    StoreName = l.StoreName
                }).ToList()
            };
        }
    }
}
