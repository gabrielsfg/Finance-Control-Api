using FinanceControl.Shared.Dtos.Request;
using FluentValidation;

namespace FinanceControl.Services.Validations
{
    public class CreateWishlistItemValidator : AbstractValidator<CreateWishlistItemRequestDto>
    {
        public CreateWishlistItemValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

            RuleFor(x => x.CurrentPrice)
                .GreaterThan(0).WithMessage("CurrentPrice must be greater than 0.");

            RuleFor(x => x.TargetPrice)
                .GreaterThan(0).WithMessage("TargetPrice must be greater than 0.")
                .When(x => x.TargetPrice.HasValue);

            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage("Priority must be Low, Medium or High.");

            RuleFor(x => x.ImageUrl)
                .MaximumLength(500).WithMessage("ImageUrl must not exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.ImageUrl));

            RuleFor(x => x.Deadline)
                .GreaterThan(DateOnly.FromDateTime(DateTime.Today))
                .WithMessage("Deadline must be a future date.")
                .When(x => x.Deadline.HasValue);

            RuleForEach(x => x.Links).ChildRules(link =>
            {
                link.RuleFor(l => l.Url)
                    .NotEmpty().WithMessage("Link Url is required.")
                    .MaximumLength(2000).WithMessage("Link Url must not exceed 2000 characters.")
                    .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                    .WithMessage("Link Url must be a valid URL.");

                link.RuleFor(l => l.StoreName)
                    .MaximumLength(200).WithMessage("StoreName must not exceed 200 characters.")
                    .When(l => !string.IsNullOrEmpty(l.StoreName));
            });
        }
    }
}
