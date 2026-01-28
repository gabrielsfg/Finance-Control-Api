using FinanceControl.Shared.Dtos.Request;
using FinanceControl.Shared.Dtos.Respose;
using FinanceControl.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceControl.Domain.Interfaces.Service
{
    public interface IBudgetSubCategoryAllocationService
    {
        Task AddSubCategoryToBudgetAsync(AddSubCategoryToBudgetRequestDto requestDto, int userId);
        Task<Result<IEnumerable<GetAllSubCategoryAllocationByAreaResponseDto>>> GetAllSubCategoryAllocationByAreasAsync(List<int> areasId, int userId);
        Task<Result<IEnumerable<GetAllSubCategoryAllocationByAreaResponseDto>>> GetAllSubCategoryAllocationByBudgetAsync(int budgetId, int userId);
        Task GetSubCategoryAllocationByIdAsync(int id, int userId);
        Task UpdateBudgetSubCategoryAllocationAsync();
        Task RemoveSubCategoryFromBudgetAsync();

    }
}
