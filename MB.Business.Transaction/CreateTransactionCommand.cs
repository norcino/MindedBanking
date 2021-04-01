using Minded.Common;
using Minded.Decorator.Validation;
using Minded.Log;

namespace MB.Business.Transaction
{
    [ValidateCommand]
    public class CreateTransactionCommand : ICommand
    {
        public Data.Entities.Transaction Transaction { get; set; }

        public CreateTransactionCommand(Data.Entities.Transaction transaction)
        {
            Transaction = transaction;
        }

        public LogInfo ToLog()
        {
            const string template = "Transaction: {Description}";
            return new LogInfo(template, Transaction.Description);
        }
    }
}
