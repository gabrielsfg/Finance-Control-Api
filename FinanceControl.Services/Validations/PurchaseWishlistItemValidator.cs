using FinanceControl.Shared.Dtos.Request;
using FluentValidation;

namespace FinanceControl.Services.Validations
{
    public class PurchaseWishlistItemValidator : AbstractValidator<PurchaseWishlistItemRequestDto>
    {
        public PurchaseWishlistItemValidator()
        {
            RuleFor(x => x.AccountId)
                .NotNull().WithMessage("AccountId is required when CreateTransaction is true.")
                .GreaterThan(0).WithMessage("AccountId must be greater than 0.")
                .When(x => x.CreateTransaction);

            RuleFor(x => x.SubCategoryId)
                .NotNull().WithMessage("SubCategoryId is required when CreateTransaction is true.")
                .GreaterThan(0).WithMessage("SubCategoryId must be greater than 0.")
                .When(x => x.CreateTransaction);

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }
}
