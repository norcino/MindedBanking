using MB.Common;
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
            command.Transaction.DateTime = SystemTime.UtcNow();
            
            var getAmountQuery = new GetTransactionAmountQuery(command.Transaction);
            command.Transaction.Amount = await _mediator.ProcessQueryAsync(getAmountQuery);

            await _context.Transactions.AddAsync(command.Transaction);
            await _context.SaveChangesAsync();

            return new CommandResponse<int>(command.Transaction.ID)
            {
                Successful = true
            };
        }
    }
}
