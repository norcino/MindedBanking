using AnonymousData;
using FluentAssertions;
using MB.Business.Account;
using MB.Business.Currency;
using MB.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minded.Mediator;
using Moq;
using System.Threading.Tasks;

namespace MB.Business.Transaction.Tests
{
    [TestClass]
    public class CreateTransactionCommandValidatorTests
    {
        private CreateTransactionCommandValidator _sut;
        private Mock<IMediator> _mediatorMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _mediatorMock = new Mock<IMediator>();
            _sut = new CreateTransactionCommandValidator(_mediatorMock.Object);
        }

        [TestMethod]
        public async Task Validate_should_fail_when_CommandTransaction_is_null()
        {
            var result = await _sut.ValidateAsync(new CreateTransactionCommand(null));

            result.ValidationEntries
                .Should()
                .Contain(ve =>
                    ve.PropertyName == nameof(CreateTransactionCommand.Transaction) &&
                    ve.ErrorMessage == "{0} is mandatory" &&
                    ve.ErrorCode == ErrorCodes.TransactionDataIsMandatory);
        }

        [TestMethod]
        public async Task Validate_should_fail_when_CommandTransactionAccountId_does_not_exist()
        {
            _mediatorMock.Setup(m => m.ProcessQueryAsync(It.IsAny<GetAccountByIdQuery>()))
                .ReturnsAsync((Data.Entities.Account)null);

            var result = await _sut.ValidateAsync(new CreateTransactionCommand(new Data.Entities.Transaction()));

            result.ValidationEntries
                .Should()
                .Contain(ve =>
                    ve.PropertyName == nameof(Data.Entities.Transaction.AccountId) &&
                    ve.ErrorMessage == "{0} does not exist" &&
                    ve.ErrorCode == ErrorCodes.SpecifiedAccountDoesNotExist);
        }

        [TestMethod]
        public async Task Validate_should_fail_when_CommandTransactionCurrencyIf_is_not_supported()
        {
            _mediatorMock.Setup(m => m.ProcessQueryAsync(It.IsAny<GetAccountByIdQuery>()))
                .ReturnsAsync(new Data.Entities.Account());
            _mediatorMock.Setup(m => m.ProcessQueryAsync(It.IsAny<GetCurrencyByIdQuery>()))
                .ReturnsAsync((Data.Entities.Currency)null);

            var result = await _sut.ValidateAsync(new CreateTransactionCommand(new Data.Entities.Transaction()));

            result.ValidationEntries
                .Should()
                .Contain(ve =>
                    ve.PropertyName == nameof(Data.Entities.Transaction.CurrencyId) &&
                    ve.ErrorMessage == "{0} is not supported" &&
                    ve.ErrorCode == ErrorCodes.SpecifiedCurrencyIsNotSupported);
        }

        [TestMethod]
        public async Task Validate_should_fail_when_CommandTransactionId_is_specified()
        {
            _mediatorMock.Setup(m => m.ProcessQueryAsync(It.IsAny<GetAccountByIdQuery>()))
                .ReturnsAsync(new Data.Entities.Account());
            _mediatorMock.Setup(m => m.ProcessQueryAsync(It.IsAny<GetCurrencyByIdQuery>()))
                .ReturnsAsync(new Data.Entities.Currency());

            var result = await _sut.ValidateAsync(new CreateTransactionCommand(new Data.Entities.Transaction { ID = Any.Int() }));

            result.ValidationEntries
                .Should()
                .Contain(ve =>
                    ve.PropertyName == nameof(Data.Entities.Transaction.ID) &&
                    ve.ErrorMessage == "{0} should not be specified on creation" &&
                    ve.ErrorCode == ErrorCodes.CannotSpecifyTransactionId);
        }

        [TestMethod]
        public async Task Validate_should_fail_when_CommandTransactionDescription_not_provided()
        {
            _mediatorMock.Setup(m => m.ProcessQueryAsync(It.IsAny<GetAccountByIdQuery>()))
                .ReturnsAsync(new Data.Entities.Account());
            _mediatorMock.Setup(m => m.ProcessQueryAsync(It.IsAny<GetCurrencyByIdQuery>()))
                .ReturnsAsync(new Data.Entities.Currency());

            var result = await _sut.ValidateAsync(new CreateTransactionCommand(new Data.Entities.Transaction()));

            result.ValidationEntries
                .Should()
                .Contain(ve =>
                    ve.PropertyName == nameof(Data.Entities.Transaction.Description) &&
                    ve.ErrorMessage == "{0} is mandatory" &&
                    ve.ErrorCode == ErrorCodes.TransactionDescriptionIsMandatory);
        }

        [TestMethod]
        public async Task Validate_should_fail_when_CommandTransactionOriginalAmount_is_zero()
        {
            _mediatorMock.Setup(m => m.ProcessQueryAsync(It.IsAny<GetAccountByIdQuery>()))
                .ReturnsAsync(new Data.Entities.Account());
            _mediatorMock.Setup(m => m.ProcessQueryAsync(It.IsAny<GetCurrencyByIdQuery>()))
                .ReturnsAsync(new Data.Entities.Currency());

            var result = await _sut.ValidateAsync(new CreateTransactionCommand(new Data.Entities.Transaction { OriginalAmount = 0 }));

            result.ValidationEntries
                .Should()
                .Contain(ve =>
                    ve.PropertyName == nameof(Data.Entities.Transaction.OriginalAmount) &&
                    ve.ErrorMessage == "{0} is mandatory" &&
                    ve.ErrorCode == ErrorCodes.TransactionOriginalAmountIsMandatory);
        }

        [TestMethod]
        public async Task Validate_should_fail_when_CommandTransactionOriginalAmount_lower_than_account_balance_for_withdrawal()
        {
            var withdrawalAmount = Any.Decimal() * -1m;
            var insufficiantBalance = withdrawalAmount * -1m - 0.01m;

            _mediatorMock.Setup(m => m.ProcessQueryAsync(It.IsAny<GetAccountByIdQuery>()))
                .ReturnsAsync(new Data.Entities.Account());
            _mediatorMock.Setup(m => m.ProcessQueryAsync(It.IsAny<GetCurrencyByIdQuery>()))
                .ReturnsAsync(new Data.Entities.Currency());
            _mediatorMock.Setup(m => m.ProcessQueryAsync(It.IsAny<GetAccountBalanceByAccountIdQuery>()))
                .ReturnsAsync(insufficiantBalance);
            _mediatorMock.Setup(m => m.ProcessQueryAsync(It.IsAny<GetTransactionAmountQuery>()))
                .ReturnsAsync(withdrawalAmount);

            var result = await _sut.ValidateAsync(new CreateTransactionCommand(
                new Data.Entities.Transaction {
                   OriginalAmount = withdrawalAmount,
                   Description = Any.String()
                }));

            result.ValidationEntries
                .Should()
                .Contain(ve =>
                    ve.PropertyName == nameof(Data.Entities.Transaction.Amount) &&
                    ve.ErrorMessage == "The {0} exceeds the current account balance" &&
                    ve.ErrorCode == ErrorCodes.TransactionAmountExceedsBalance);
        }

        [TestMethod]
        public async Task Validate_should_use_Amount_caulculated_from_OriginalAmount_to_validate_the_balance()
        {
            var withdrawalAmount = Any.Decimal() * -1m;
            var insufficiantBalance = withdrawalAmount * -1m - 0.01m;

            _mediatorMock.Setup(m => m.ProcessQueryAsync(It.IsAny<GetAccountByIdQuery>()))
                .ReturnsAsync(new Data.Entities.Account());
            _mediatorMock.Setup(m => m.ProcessQueryAsync(It.IsAny<GetCurrencyByIdQuery>()))
                .ReturnsAsync(new Data.Entities.Currency());
            _mediatorMock.Setup(m => m.ProcessQueryAsync(It.IsAny<GetAccountBalanceByAccountIdQuery>()))
                .ReturnsAsync(insufficiantBalance);
            _mediatorMock.Setup(m => m.ProcessQueryAsync(It.IsAny<GetTransactionAmountQuery>()))
                .ReturnsAsync(withdrawalAmount);

            var transactionUnderTest = new CreateTransactionCommand(
                new Data.Entities.Transaction
                {
                    OriginalAmount = withdrawalAmount,
                    Description = Any.String()
                });

            var result = await _sut.ValidateAsync(transactionUnderTest);

            result.ValidationEntries
                .Should()
                .Contain(ve =>
                    ve.PropertyName == nameof(Data.Entities.Transaction.Amount) &&
                    ve.ErrorMessage == "The {0} exceeds the current account balance" &&
                    ve.ErrorCode == ErrorCodes.TransactionAmountExceedsBalance);

            _mediatorMock.Verify(m => m.ProcessQueryAsync(
                It.Is<GetTransactionAmountQuery>(q => q.Transaction == transactionUnderTest.Transaction)),
                Times.Once);
        }

        [TestMethod]
        public async Task Validate_should_fail_when_CommandTransactionAmount_is_specified()
        {
            var withdrawalAmount = Any.Decimal() * -1m;
            var insufficiantBalance = withdrawalAmount * -1m - 0.01m;

            _mediatorMock.Setup(m => m.ProcessQueryAsync(It.IsAny<GetAccountByIdQuery>()))
                .ReturnsAsync(new Data.Entities.Account());
            _mediatorMock.Setup(m => m.ProcessQueryAsync(It.IsAny<GetCurrencyByIdQuery>()))
                .ReturnsAsync(new Data.Entities.Currency());
            _mediatorMock.Setup(m => m.ProcessQueryAsync(It.IsAny<GetAccountBalanceByAccountIdQuery>()))
                .ReturnsAsync(insufficiantBalance);
            _mediatorMock.Setup(m => m.ProcessQueryAsync(It.IsAny<GetTransactionAmountQuery>()))
                .ReturnsAsync(withdrawalAmount);

            var result = await _sut.ValidateAsync(new CreateTransactionCommand(
                new Data.Entities.Transaction
                {
                    OriginalAmount = withdrawalAmount,
                    Description = Any.String(),
                    Amount = Any.Decimal()
                }));

            result.ValidationEntries
                .Should()
                .Contain(ve =>
                    ve.PropertyName == nameof(Data.Entities.Transaction.Amount) &&
                    ve.ErrorMessage == "{0} cannot be specified on creation" &&
                    ve.ErrorCode == ErrorCodes.TransactionAmountCannotBeSpefied);
        }
    }
}
