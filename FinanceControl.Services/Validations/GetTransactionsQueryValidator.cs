using FinanceControl.Shared.Dtos.Request;
using FluentValidation;

namespace FinanceControl.Services.Validations
{
    public class GetTransactionsQueryValidator : AbstractValidator<GetTransactionsQueryDto>
    {
        public GetTransactionsQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0).WithMessage("Page must be greater than 0.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100.");

            RuleFor(x => x.MinValue)
                .GreaterThan(0).WithMessage("MinValue must be greater than 0.")
                .When(x => x.MinValue.HasValue);

            RuleFor(x => x.MaxValue)
                .GreaterThan(0).WithMessage("MaxValue must be greater than 0.")
                .When(x => x.MaxValue.HasValue);

            RuleFor(x => x)
                .Must(x => !x.MinValue.HasValue || !x.MaxValue.HasValue || x.MinValue <= x.MaxValue)
                .WithMessage("MinValue must be less than or equal to MaxValue.");

            RuleFor(x => x)
                .Must(x => !x.StartDate.HasValue || !x.EndDate.HasValue || x.StartDate <= x.EndDate)
                .WithMessage("StartDate must be before or equal to EndDate.");
        }
    }
}
