using FinanceControl.Shared.Enums;

namespace FinanceControl.Shared.Dtos.Respose
{
    public class GetBudgetByIdResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly FinishDate { get; set; }
        public EnumBudgetRecurrence Recurence { get; set; }
    }
}
