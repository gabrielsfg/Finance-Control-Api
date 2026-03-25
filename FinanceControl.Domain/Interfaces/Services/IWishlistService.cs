using FinanceControl.Shared.Dtos.Request;
using FinanceControl.Shared.Dtos.Response;
using FinanceControl.Shared.Models;

namespace FinanceControl.Domain.Interfaces.Services
{
    public interface IWishlistService
    {
        Task<Result<GetWishlistItemByIdResponseDto>> CreateWishlistItemAsync(CreateWishlistItemRequestDto requestDto, int userId);
        Task<PagedResponse<GetWishlistItemResponseDto>> GetAllWishlistItemsPagedAsync(GetWishlistQueryDto query, int userId);
        Task<GetWishlistItemByIdResponseDto?> GetWishlistItemByIdAsync(int id, int userId);
        Task<Result<GetWishlistItemByIdResponseDto>> UpdateWishlistItemAsync(UpdateWishlistItemRequestDto requestDto, int id, int userId);
        Task<Result> DeleteWishlistItemAsync(int id, int userId);
        Task<Result<WishlistPriceHistoryResponseDto>> RegisterPriceAsync(RegisterWishlistPriceRequestDto requestDto, int id, int userId);
        Task<Result<GetWishlistItemByIdResponseDto>> PurchaseWishlistItemAsync(PurchaseWishlistItemRequestDto requestDto, int id, int userId);
        Task<Result<IEnumerable<WishlistPriceHistoryResponseDto>>> GetPriceHistoryAsync(int id, int userId);
    }
}
