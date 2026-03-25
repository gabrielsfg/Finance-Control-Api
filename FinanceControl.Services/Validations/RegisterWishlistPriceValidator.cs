using FinanceControl.Shared.Dtos.Request;
using FluentValidation;

namespace FinanceControl.Services.Validations
{
    public class RegisterWishlistPriceValidator : AbstractValidator<RegisterWishlistPriceRequestDto>
    {
        public RegisterWishlistPriceValidator()
        {
            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");
        }
    }
}
