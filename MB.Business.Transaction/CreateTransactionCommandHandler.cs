using MB.Business.Account;
using MB.Business.Currency;
using MB.Business.Exchange;
using MB.Data.Access;
using Minded.Common;
using Minded.Decorator.Validation;
using Minded.Mediator;
using System;
using System.Threading.Tasks;

namespace MB.Business.Transaction
{
    [ValidateCommand]
    public class CreateTransactionCommandHandler : ICommandHandler<CreateTransactionCommand>
    {
        private readonly IMindedBankingContext _context;
        private readonly IMediator _mediator;

        public CreateTransactionCommandHandler(IMindedBankingContext context, IMediator mediator)
        {
            _mediator = mediator;
            _context = context;
        }

        public async Task<ICommandResponse> HandleAsync(CreateTransactionCommand command)
        {
            var getAccountQuery = new GetAccountByIdQuery(command.Transaction.AccountId);
            var account = await _mediator.ProcessQueryAsync(getAccountQuery);

            var getCurrencyByIdQuery = new GetCurrencyByIdQuery(account.DefaultCurrencyId);
            var defaultCurrency = await _mediator.ProcessQueryAsync(getCurrencyByIdQuery);
            command.Transaction.OriginalAmount = command.Transaction.Amount;
            command.Transaction.DateTime = DateTime.UtcNow;

            command = await HandleNonDefaultCurrency(command, defaultCurrency);

            await _context.Transactions.AddAsync(command.Transaction);
            await _context.SaveChangesAsync();

            return new CommandResponse<int>(command.Transaction.ID)
            {
                Successful = true
            };
        }

        private async Task<CreateTransactionCommand> HandleNonDefaultCurrency(CreateTransactionCommand command, Data.Entities.Currency defaultCurrency)
        {
            if (command.Transaction.CurrencyId != defaultCurrency.ID)
            {
                var getCurrencyByIdQuery = new GetCurrencyByIdQuery(command.Transaction.CurrencyId);
                var transactionCurrency = await _mediator.ProcessQueryAsync(getCurrencyByIdQuery);

                var getExchangeRateQuery = new GetExchangeRateQuery(transactionCurrency.Code, defaultCurrency.Code, command.Transaction.Amount);
                command.Transaction.OriginalAmount = await _mediator.ProcessQueryAsync(getExchangeRateQuery);
            }

            return command;
        }
    }
}
