using FinanceControl.Data.Data;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Services.Services
{
    public class BankService : IBankService
    {
        private readonly ApplicationDbContext _context;

        public BankService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Bank>> GetBanksAsync(string? country)
        {
            var query = _context.Banks.AsQueryable();

            if (!string.IsNullOrWhiteSpace(country))
                query = query.Where(b => b.Country == country.ToUpper());

            return await query.OrderBy(b => b.Name).ToListAsync();
        }
    }
}
