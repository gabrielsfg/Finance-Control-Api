using FinanceControl.Shared.Enums;

namespace FinanceControl.Shared.Dtos.Respose
{
    public class SubCategoryAllocationItemByCategoryIdDto
    {
        public int AllocationId { get; set; }
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public int SubCategoryExpectedValue { get; set; }
        public EnumAllocationType AllocationType { get; set; }
    }
}
