using FinanceControl.Domain.Interfaces.Services;
using FinanceControl.Services.Extensions;
using FinanceControl.Shared.Dtos.Request;
using FinanceControl.WebApi.Controllers.Base;
using FinanceControl.WebApi.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WishlistController : BaseController
    {
        private readonly IWishlistService _wishlistService;
        private readonly IValidator<CreateWishlistItemRequestDto> _createValidator;
        private readonly IValidator<UpdateWishlistItemRequestDto> _updateValidator;
        private readonly IValidator<RegisterWishlistPriceRequestDto> _priceValidator;
        private readonly IValidator<PurchaseWishlistItemRequestDto> _purchaseValidator;
        private readonly IValidator<GetWishlistQueryDto> _queryValidator;

        public WishlistController(
            IWishlistService wishlistService,
            IValidator<CreateWishlistItemRequestDto> createValidator,
            IValidator<UpdateWishlistItemRequestDto> updateValidator,
            IValidator<RegisterWishlistPriceRequestDto> priceValidator,
            IValidator<PurchaseWishlistItemRequestDto> purchaseValidator,
            IValidator<GetWishlistQueryDto> queryValidator)
        {
            _wishlistService = wishlistService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _priceValidator = priceValidator;
            _purchaseValidator = purchaseValidator;
            _queryValidator = queryValidator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateWishlistItemAsync([FromBody] CreateWishlistItemRequestDto requestDto)
        {
            var validationResult = _createValidator.Validate(requestDto);
            if (validationResult.ToActionResult() is { } errorResult)
                return errorResult;

            var userId = GetUserId();
            var result = await _wishlistService.CreateWishlistItemAsync(requestDto, userId);
            if (result.IsFailure)
                return BadRequest(new { error = result.Error });

            return Created($"/api/wishlist/{result.Value!.Id}", result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWishlistItemsAsync([FromQuery] GetWishlistQueryDto query)
        {
            var validationResult = _queryValidator.Validate(query);
            if (validationResult.ToActionResult() is { } errorResult)
                return errorResult;

            var userId = GetUserId();
            var result = await _wishlistService.GetAllWishlistItemsPagedAsync(query, userId);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetWishlistItemByIdAsync([FromRoute] int id)
        {
            var validationId = this.ValidatePositiveId(id, "id");
            if (validationId is not null)
                return validationId;

            var userId = GetUserId();
            var result = await _wishlistService.GetWishlistItemByIdAsync(id, userId);
            if (result is null)
                return NotFound(new { error = "Wishlist item not found." });

            return Ok(result);
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdateWishlistItemAsync([FromRoute] int id, [FromBody] UpdateWishlistItemRequestDto requestDto)
        {
            var validationId = this.ValidatePositiveId(id, "id");
            if (validationId is not null)
                return validationId;

            var validationResult = _updateValidator.Validate(requestDto);
            if (validationResult.ToActionResult() is { } errorResult)
                return errorResult;

            var userId = GetUserId();
            var result = await _wishlistService.UpdateWishlistItemAsync(requestDto, id, userId);
            if (result.IsFailure)
                return NotFound(new { error = result.Error });

            return Ok(result.Value);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteWishlistItemAsync([FromRoute] int id)
        {
            var validationId = this.ValidatePositiveId(id, "id");
            if (validationId is not null)
                return validationId;

            var userId = GetUserId();
            var result = await _wishlistService.DeleteWishlistItemAsync(id, userId);
            if (result.IsFailure)
                return NotFound(new { error = result.Error });

            return Ok();
        }

        [HttpPost("{id:int}/price")]
        public async Task<IActionResult> RegisterPriceAsync([FromRoute] int id, [FromBody] RegisterWishlistPriceRequestDto requestDto)
        {
            var validationId = this.ValidatePositiveId(id, "id");
            if (validationId is not null)
                return validationId;

            var validationResult = _priceValidator.Validate(requestDto);
            if (validationResult.ToActionResult() is { } errorResult)
                return errorResult;

            var userId = GetUserId();
            var result = await _wishlistService.RegisterPriceAsync(requestDto, id, userId);
            if (result.IsFailure)
                return BadRequest(new { error = result.Error });

            return Ok(result.Value);
        }

        [HttpPost("{id:int}/purchase")]
        public async Task<IActionResult> PurchaseWishlistItemAsync([FromRoute] int id, [FromBody] PurchaseWishlistItemRequestDto requestDto)
        {
            var validationId = this.ValidatePositiveId(id, "id");
            if (validationId is not null)
                return validationId;

            var validationResult = _purchaseValidator.Validate(requestDto);
            if (validationResult.ToActionResult() is { } errorResult)
                return errorResult;

            var userId = GetUserId();
            var result = await _wishlistService.PurchaseWishlistItemAsync(requestDto, id, userId);
            if (result.IsFailure)
                return BadRequest(new { error = result.Error });

            return Ok(result.Value);
        }

        [HttpGet("{id:int}/price-history")]
        public async Task<IActionResult> GetPriceHistoryAsync([FromRoute] int id)
        {
            var validationId = this.ValidatePositiveId(id, "id");
            if (validationId is not null)
                return validationId;

            var userId = GetUserId();
            var result = await _wishlistService.GetPriceHistoryAsync(id, userId);
            if (result.IsFailure)
                return NotFound(new { error = result.Error });

            return Ok(result.Value);
        }
    }
}
