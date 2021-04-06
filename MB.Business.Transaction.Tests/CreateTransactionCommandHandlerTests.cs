using AnonymousData;
using Builder;
using FluentAssertions;
using MB.Common;
using MB.Data.Access;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minded.Common;
using Minded.Mediator;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace MB.Business.Transaction.Tests
{
    [TestClass]
    public class CreateTransactionCommandHandlerTests
    {
        private CreateTransactionCommandHandler _sut;
        private Mock<IMediator> _mediatorMock;
        private Mock<IMindedBankingContext> _mindedBankingContextMock;
        private Mock<DbSet<Data.Entities.Transaction>> _transactionsDbSetMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _mediatorMock = new Mock<IMediator>();
            _mindedBankingContextMock = new Mock<IMindedBankingContext>();
            _transactionsDbSetMock = new Mock<DbSet<Data.Entities.Transaction>>();

            _mindedBankingContextMock.SetupGet(c => c.Transactions).Returns(_transactionsDbSetMock.Object);

            _sut = new CreateTransactionCommandHandler(_mindedBankingContextMock.Object, _mediatorMock.Object);
        }

        [TestMethod]
        public async Task HandleAsync_should_set_DateTime()
        {
            var expectedDateTime = Any.DateTime();
            SystemTime.UtcNow = () => expectedDateTime;
                        
            var result = await _sut.HandleAsync(new CreateTransactionCommand(Any.Of<Data.Entities.Transaction>()));

            _transactionsDbSetMock.Verify(t => t.AddAsync(It.Is<Data.Entities.Transaction>(t => t.DateTime == expectedDateTime), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task HandleAsync_should_add_the_Transaction_to_the_dbset_and_Save()
        {
            SystemTime.UtcNow = () => Any.DateTime();
            var expectedTransaction = Builder<Data.Entities.Transaction>.New().Build(t => t.DateTime = SystemTime.UtcNow());

            var result = await _sut.HandleAsync(new CreateTransactionCommand(expectedTransaction));

            _transactionsDbSetMock.Verify(t => t.AddAsync(expectedTransaction, It.IsAny<CancellationToken>()), Times.Once);
            _mindedBankingContextMock.Verify(c => c.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task HandleAsync_should_return_successful_CommandResponse_with_ID()
        {
            SystemTime.UtcNow = () => Any.DateTime();
            var expectedTransaction = Builder<Data.Entities.Transaction>.New().Build(t => t.DateTime = SystemTime.UtcNow());

            var result = await _sut.HandleAsync(new CreateTransactionCommand(expectedTransaction)) as CommandResponse<int>;

            result.Successful.Should().BeTrue();
            result.Result.Should().Be(expectedTransaction.ID);
        }
    }
}
