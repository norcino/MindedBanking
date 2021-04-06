using MB.Business.Account;
using MB.Business.Currency;
using MB.Common;
using Minded.Decorator.Validation;
using Minded.Mediator;
using Minded.Validation;
using System;
using System.Threading.Tasks;

namespace MB.Business.Transaction
{
    public class CreateTransactionCommandValidator : ICommandValidator<CreateTransactionCommand>
    {
        private readonly IMediator _mediator;

        public CreateTransactionCommandValidator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IValidationResult> ValidateAsync(CreateTransactionCommand command)
        {
            var validationResult = new ValidationResult();
                       
            if (command.Transaction == null)
            {
                validationResult.ValidationEntries.Add(new CodedValidationEntry(nameof(command.Transaction), "{0} is mandatory", ErrorCodes.TransactionDataIsMandatory));
                return validationResult;
            }

            var getAccountQuery = new GetAccountByIdQuery(command.Transaction.AccountId);
            var account = await _mediator.ProcessQueryAsync(getAccountQuery);
            if (account == null)
            {
                validationResult.ValidationEntries.Add(new CodedValidationEntry(nameof(command.Transaction.AccountId), "{0} does not exist", ErrorCodes.SpecifiedAccountDoesNotExist));
                return validationResult;
            }

            var getCurrencyQuery = new GetCurrencyByIdQuery(command.Transaction.CurrencyId);
            var currency = await _mediator.ProcessQueryAsync(getCurrencyQuery);
            if (currency == null)
            {
                validationResult.ValidationEntries.Add(new CodedValidationEntry(nameof(command.Transaction.CurrencyId), "{0} is not supported", ErrorCodes.SpecifiedCurrencyIsNotSupported));
                return validationResult;
            }

            if (command.Transaction.ID != 0)
            {
                validationResult.ValidationEntries.Add(new CodedValidationEntry(nameof(command.Transaction.ID), "{0} should not be specified on creation", ErrorCodes.CannotSpecifyTransactionId));
            }

            if (string.IsNullOrWhiteSpace(command.Transaction.Description))
            {
                validationResult.ValidationEntries.Add(new CodedValidationEntry(nameof(command.Transaction.Description), "{0} is mandatory", ErrorCodes.TransactionDescriptionIsMandatory));
            }

            if (command.Transaction.Amount != 0)
            {
                validationResult.ValidationEntries.Add(new CodedValidationEntry(nameof(command.Transaction.Amount), "{0} cannot be specified on creation", ErrorCodes.TransactionAmountCannotBeSpefied));
                return validationResult;
            }

            if (command.Transaction.OriginalAmount == 0)
            {
                validationResult.ValidationEntries.Add(new CodedValidationEntry(nameof(command.Transaction.OriginalAmount), "{0} is mandatory", ErrorCodes.TransactionOriginalAmountIsMandatory));
            }
            else if (command.Transaction.OriginalAmount < 0)
            {
                var validationEntry = await ValidateBalance(command.Transaction);

                if (validationEntry != null)
                    validationResult.ValidationEntries.Add(validationEntry);
            }

            return validationResult;
        }

        private async Task<ValidationEntry> ValidateBalance(Data.Entities.Transaction transaction)
        {
            var balance = await _mediator.ProcessQueryAsync(new GetAccountBalanceByAccountIdQuery(transaction.AccountId));
            var amount = await _mediator.ProcessQueryAsync(new GetTransactionAmountQuery(transaction));

            if (Math.Round(balance, 2) + Math.Round(amount, 2) < 0)
                return new CodedValidationEntry(nameof(Data.Entities.Transaction.Amount), "The {0} exceeds the current account balance", ErrorCodes.TransactionAmountExceedsBalance);

            return null;
        }
    }
}
