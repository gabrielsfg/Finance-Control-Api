using FinanceControl.Domain.Interfaces.Service;
using FinanceControl.Services.Services;
using FinanceControl.Services.Validations;
using FinanceControl.Shared.Dtos.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AreaController : BaseController
    {
        private readonly IAreaService _areaService;

        public AreaController(IAreaService areaService)
        {
            _areaService = areaService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAreaAsync([FromBody]CreateAreaRequestDto requestDto)
        {
            var userId = GetUserId();

            var result = await _areaService.CreateAreaAsync(requestDto, userId);

            if (result.IsFailure)
                return NotFound(new { error = result.Error });

            return Ok(result.Value);
        }

        [HttpGet("all/{budgetId:int}")]
        public async Task<IActionResult> GetAllAreaAsync([FromQuery] int budgetId)
        {
            var userId = GetUserId();

            var result = await _areaService.GetAllAreasAsync(budgetId, userId);
            return Ok(result);
        }

        [HttpGet("by-id/{id:int}")]
        public async Task<IActionResult> GetAreaByIdAsync([FromRoute] int id)
        {
            var userId = GetUserId();

            var result = await _areaService.GetAreaByIdAync(id, userId);
            return Ok(result);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateAreaAsync([FromBody] UpdateAreaRequestDto requestDto)
        {
            var userId = GetUserId();

            var result = await _areaService.UpdateAreaAsync(requestDto, userId);

            if (result.IsFailure)
                return NotFound(new { error = result.Error });

            return Ok(result.Value);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAreaAsync([FromRoute] int id)
        {
            var userId = GetUserId();
            var result = await _areaService.DeleteAreaAsync(id, userId);

            if (result.IsFailure)
                return NotFound(new { error = result.Error });

            return Ok(result.Value);
        }
    }
}
