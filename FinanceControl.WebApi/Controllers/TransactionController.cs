using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Interfaces.Service;
using FinanceControl.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionController : Controller
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateTransactionAsync([FromBody] CreateTransactionRequestDto requestDto)
        {
            var response = await _transactionService.CreateTransactionAsync(requestDto);
            return Ok(new {message = @$"TransactionId: {response.ToString()}" });
        }

        [HttpGet("by-id/{transactionId:int}")]
        public async Task<IActionResult> GetTransactionByIdAsync([FromRoute] int transactionId)
        {
            TransactionResponseDto? transaction = await _transactionService.GetTransactionByIdAsync(transactionId);
            if (transaction == null)
                return NotFound(new {message = "Transaction not found!"});

            return Ok(transaction);
        }

        [HttpGet("paged/{page:int}&{pageSize:int}")]
        public async Task<IActionResult> GetAllTransactionsPagedAsync([FromRoute] int page, [FromRoute] int pageSize)
        {
            var response = await _transactionService.GetAllTransactionsPagedAsync(page, pageSize);
            return Ok(response);
        }

        [HttpPatch("update-by-id")]
        public async Task<IActionResult> UpdateTransactionAsyncById([FromBody] UpdateTransactionRequestDto requestDto)
        {
            await _transactionService.UpdateTransactionAsyncById(requestDto);
            return Ok();
        }

        [HttpDelete("delete-by-id/{transactionId:int}")]
        public async Task<IActionResult> DeleteTransactionById([FromRoute] int transactionId)
        {
            await _transactionService.DeleteTransactionById(transactionId);
            return Ok();
        }
    }
}
