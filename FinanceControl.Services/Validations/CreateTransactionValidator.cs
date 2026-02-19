using FinanceControl.Domain.Enums;
using FinanceControl.Shared.Dtos.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceControl.Services.Validations
{
    public class CreateTransactionValidator : AbstractValidator<CreateTransactionRequestDto>
    {
        private static readonly string[] ValidTypes =
            Enum.GetNames(typeof(EnumTransactionType));

        private static readonly string[] ValidPaymentTypes =
            Enum.GetNames(typeof(EnumPaymentType));

        private static readonly string[] ValidRecurrenceTypes =
            Enum.GetNames(typeof(EnumRecurrenceType));

        public CreateTransactionValidator()
        {
            RuleFor(x => x.SubCategoryId)
                .GreaterThan(0).WithMessage("SubCategoryId must be greater than 0.");

            RuleFor(x => x.AccountId)
                .GreaterThan(0).WithMessage("AccountId must be greater than 0.");

            RuleFor(x => x.Value)
                .GreaterThan(0).WithMessage("Value must be greater than 0.");

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Type is required.")
                .Must(t => ValidTypes.Contains(t, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Type must be one of: {string.Join(", ", ValidTypes)}.");

            RuleFor(x => x.PaymentType)
                .NotEmpty().WithMessage("PaymentType is required.")
                .Must(p => ValidPaymentTypes.Contains(p, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"PaymentType must be one of: {string.Join(", ", ValidPaymentTypes)}.");

            RuleFor(x => x.TransactionDate)
                .NotEmpty().WithMessage("TransactionDate is required.");

            RuleFor(x => x.TotalInstallments)
                .GreaterThan(1).WithMessage("TotalInstallments must be greater than 1.")
                .When(x => string.Equals(x.PaymentType, nameof(EnumPaymentType.Installment), StringComparison.OrdinalIgnoreCase));

            RuleFor(x => x.TotalInstallments)
                .Null().WithMessage("TotalInstallments should only be set when PaymentType is Installment.")
                .When(x => !string.Equals(x.PaymentType, nameof(EnumPaymentType.Installment), StringComparison.OrdinalIgnoreCase));

            RuleFor(x => x.Recurrence)
                .Must(r => ValidRecurrenceTypes.Contains(r, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Recurrence must be one of: {string.Join(", ", ValidRecurrenceTypes)}.")
                .When(x => !string.IsNullOrEmpty(x.Recurrence));

            RuleFor(x => x.Recurrence)
                .NotEmpty().WithMessage("Recurrence is required when PaymentType is Recurring.")
                .When(x => string.Equals(x.PaymentType, nameof(EnumPaymentType.Recurring), StringComparison.OrdinalIgnoreCase));

            RuleFor(x => x.RecurringEndDate)
                .GreaterThan(x => x.TransactionDate)
                .WithMessage("RecurringEndDate must be after TransactionDate.")
                .When(x => x.RecurringEndDate.HasValue);
        }
    }
}
