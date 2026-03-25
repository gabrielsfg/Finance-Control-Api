using FinanceControl.Shared.Dtos.Request;
using FinanceControl.Shared.Enums;
using FluentValidation;
using System.Text.RegularExpressions;

namespace FinanceControl.Services.Validations
{
    public class CreateTransactionValidator : AbstractValidator<CreateTransactionRequestDto>
    {
        public CreateTransactionValidator()
        {
            RuleFor(x => x.SubCategoryId)
                .GreaterThan(0).WithMessage("SubCategoryId must be greater than 0.");

            RuleFor(x => x.AccountId)
                .GreaterThan(0).WithMessage("AccountId must be greater than 0.");

            RuleFor(x => x.Value)
                .GreaterThan(0).WithMessage("Value must be greater than 0.");

            RuleFor(x => x.Type).IsInEnum()
                .WithMessage($"Type must be one of: {string.Join(", ", Enum.GetNames<EnumTransactionType>())}.");

            RuleFor(x => x.PaymentType).IsInEnum()
                .WithMessage($"PaymentType must be one of: {string.Join(", ", Enum.GetNames<EnumPaymentType>())}.");

            RuleFor(x => x.PaymentMethod)
                .MaximumLength(50).WithMessage("PaymentMethod must not exceed 50 characters.")
                .Matches(@"^[A-Z0-9_]+$").WithMessage("PaymentMethod must contain only uppercase letters, digits and underscores.")
                .When(x => !string.IsNullOrEmpty(x.PaymentMethod));

            RuleFor(x => x.TransactionDate)
                .NotEmpty().WithMessage("TransactionDate is required.");

            RuleFor(x => x.TotalInstallments)
                .GreaterThan(1).WithMessage("TotalInstallments must be greater than 1.")
                .When(x => x.PaymentType == EnumPaymentType.Installment);

            RuleFor(x => x.TotalInstallments)
                .Null().WithMessage("TotalInstallments should only be set when PaymentType is Installment.")
                .When(x => x.PaymentType != EnumPaymentType.Installment);

            RuleFor(x => x.Recurrence).IsInEnum()
                .WithMessage($"Recurrence must be one of: {string.Join(", ", Enum.GetNames<EnumRecurrenceType>())}.")
                .When(x => x.Recurrence.HasValue);

            RuleFor(x => x.Recurrence)
                .NotNull().WithMessage("Recurrence is required when PaymentType is Recurring.")
                .When(x => x.PaymentType == EnumPaymentType.Recurring);
        }
    }
}
