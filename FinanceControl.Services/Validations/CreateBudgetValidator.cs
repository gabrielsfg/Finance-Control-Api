using FinanceControl.Shared.Dtos.Request;
using FinanceControl.Shared.Enums;
using FluentValidation;

namespace FinanceControl.Services.Validations
{
    public class CreateBudgetValidator : AbstractValidator<CreateBudgetResquestDto>
    {
        public CreateBudgetValidator()
        {
            RuleFor(b => b.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(b => b.StartDate).LessThanOrEqualTo(31).GreaterThan(0).WithMessage("Start date must be a valid Day");
            RuleFor(b => b.Recurrence).IsInEnum()
                .WithMessage($"Recurrence must be one of: {string.Join(", ", Enum.GetNames<EnumBudgetRecurrence>())}");
        }
    }
}
