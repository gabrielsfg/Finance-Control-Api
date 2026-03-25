using FinanceControl.Shared.Dtos.Request;
using FluentValidation;

namespace FinanceControl.Services.Validations
{
    public class GetWishlistQueryValidator : AbstractValidator<GetWishlistQueryDto>
    {
        public GetWishlistQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0).WithMessage("Page must be greater than 0.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Status must be Active, Purchased or Archived.")
                .When(x => x.Status.HasValue);
        }
    }
}
