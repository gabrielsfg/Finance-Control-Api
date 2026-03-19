using FinanceControl.Domain.Entities;

namespace FinanceControl.Domain.Interfaces.Services
{
    public interface IBankService
    {
        Task<IEnumerable<Bank>> GetBanksAsync(string? country);
    }
}
