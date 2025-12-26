using FinanceControl.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinanceControl.Shared.Dtos;
using FinanceControl.Shared.Helpers;

namespace FinanceControl.Domain.Entities
{
    public class Transaction : BaseEntity
    {
        public int Value  { get; set; }
        public EnumTransactionType Type { get; set; }
        public int Category {  get; set; }
        public string? Description { get; set; }
        public DateOnly TransactionDate { get; set; }
        public EnumPaymentType PaymentType { get; set; }
        public EnumPaymentRecurrence Reccurence { get; set; }
        
        public Transaction(
            int value,
            EnumTransactionType type,
            int category,
            DateOnly transactionDate,
            EnumPaymentType paymentType,
            EnumPaymentRecurrence recurrence,
            string? description = null
            )
        {
            Value = value;
            Type = type;
            Category = category;
            Description = description;
            TransactionDate = transactionDate;
            PaymentType = paymentType;
            Reccurence = recurrence;
        }
        
        protected  Transaction()
        {
        }
    }
    
    
}
