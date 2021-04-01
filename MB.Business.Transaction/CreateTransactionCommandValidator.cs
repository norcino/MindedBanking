using Minded.Decorator.Validation;
using Minded.Validation;
using System.Threading.Tasks;

namespace MB.Business.Transaction
{
    public class CreateTransactionCommandValidator : ICommandValidator<CreateTransactionCommand>
    {
        public CreateTransactionCommandValidator()
        {
        }

        public async Task<IValidationResult> ValidateAsync(CreateTransactionCommand command)
        {
            var validationResult = new ValidationResult();
                       
            if (command.Transaction == null)
            {
                validationResult.ValidationEntries.Add(new ValidationEntry(nameof(command.Transaction), "{0} is mandatory"));

                return validationResult;
            }

            if (command.Transaction.ID != 0)
            {
                validationResult.ValidationEntries.Add(new ValidationEntry(nameof(command.Transaction.ID), "{0} should not be specified on creation"));
            }

            if (string.IsNullOrWhiteSpace(command.Transaction.Description))
            {
                validationResult.ValidationEntries.Add(new ValidationEntry(nameof(command.Transaction.Description), "{0} is mandatory"));
            }

            if (command.Transaction.Amount == 0)
            {
                validationResult.ValidationEntries.Add(new ValidationEntry(nameof(command.Transaction.Description), "{0} is mandatory"));
            }

            if (command.Transaction.OriginalAmount != 0)
            {
                validationResult.ValidationEntries.Add(new ValidationEntry(nameof(command.Transaction.OriginalAmount), "{0} cannot be specified on creation"));
            }
            
            // TODO check if account is valid
            // TODO validate if currency is supported
            return validationResult;
        }
    }
}
