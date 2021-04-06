using AnonymousData;
using FluentAssertions;
using MB.Business.Transaction;
using MB.Data.Entities;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minded.Mediator;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ODataPath = Microsoft.AspNet.OData.Routing.ODataPath;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.OData.UriParser;
using Microsoft.AspNet.OData.Query.Validators;
using Microsoft.AspNetCore.Builder;
using Builder;
using MB.Business.Account;

namespace MB.Application.Api.Tests
{
    [TestClass]
    public class AccountsControllerTests
    {
        private AccountsController _sut;
        private Mock<IMediator> _mediatorMock;
        private ServiceProvider _provider;

        [TestInitialize]
        public void TestInitialize()
        {
            _mediatorMock = new Mock<IMediator>();

            var collection = new ServiceCollection();
            collection.AddOData();
            collection.AddODataQueryFilter();
            collection.AddTransient<ODataUriResolver>();
            collection.AddTransient<ODataQueryValidator>();
            collection.AddTransient<TopQueryValidator>();

            _provider = collection.BuildServiceProvider();

            var routeBuilder = new RouteBuilder(Mock.Of<IApplicationBuilder>(x => x.ApplicationServices == _provider));
            routeBuilder.EnableDependencyInjection();

            _sut = new AccountsController(_mediatorMock.Object);            
        }

        [TestMethod]
        public async Task GetTransactionsByAccountId_should_return_Ok_with_no_results_when_query_does_not_return_transactions()
        {
            _mediatorMock.Setup(m => m.ProcessQueryAsync(It.IsAny<GetAccountByIdQuery>()))
                .ReturnsAsync(new Account());
            _mediatorMock.Setup(m => m.ProcessQueryAsync(It.IsAny<GetTransactionsByAccountIdQuery>()))
                .ReturnsAsync(new List<Transaction>());

            var result = await _sut.GetTransactionsByAccountId(Any.Int(), GetODataQueryOptions<Transaction>());

            result.Should().BeOfType<OkObjectResult>();
            (result as OkObjectResult).Value.Should().BeOfType<List<Transaction>>();
            ((result as OkObjectResult).Value as List<Transaction>).Should().BeEmpty();
        }

        [TestMethod]
        public async Task GetTransactionsByAccountId_should_return_ExpectedTransactions_when_query_returns_Account()
        {
            var accountId = Any.Int();
            var expectedTransactions = Builder<Transaction>.New().BuildMany(10, (t,i) => { t.AccountId = accountId; });

            _mediatorMock.Setup(m => m.ProcessQueryAsync(It.IsAny<GetAccountByIdQuery>()))
                .ReturnsAsync(new Account());
            _mediatorMock.Setup(m => m.ProcessQueryAsync(It.IsAny<GetTransactionsByAccountIdQuery>()))
                            .ReturnsAsync(expectedTransactions);

            var result = await _sut.GetTransactionsByAccountId(accountId, GetODataQueryOptions<Transaction>());

            result.Should().BeOfType<OkObjectResult>();
            (result as OkObjectResult).Value.Should().BeOfType<List<Transaction>>();
            ((result as OkObjectResult).Value as List<Transaction>).Should().BeEquivalentTo(expectedTransactions);
        }

        [TestMethod]
        public async Task GetTransactionsByAccountId_should_return_NotFound_when_account_does_not_exist()
        {
            var accountId = Any.Int();
            var expectedTransactions = Builder<Transaction>.New().BuildMany(10, (t, i) => { t.AccountId = accountId; });

            _mediatorMock.Setup(m => m.ProcessQueryAsync(It.IsAny<GetAccountByIdQuery>()))
                .ReturnsAsync((Account)null);
            _mediatorMock.Setup(m => m.ProcessQueryAsync(It.IsAny<GetTransactionsByAccountIdQuery>()))
                            .ReturnsAsync(expectedTransactions);

            var result = await _sut.GetTransactionsByAccountId(accountId, GetODataQueryOptions<Transaction>());

            result.Should().BeOfType<NotFoundResult>();
        }

        private ODataQueryOptions<T> GetODataQueryOptions<T>()
        {
            var defaultHttpContext = new DefaultHttpContext
            {
                RequestServices = _provider,
            };

            var modelBuilder = new ODataConventionModelBuilder(_provider);
            var entitySet = modelBuilder.EntitySet<Transaction>($"{typeof(T).Name}s");
            entitySet.EntityType.HasKey(entity => entity.ID);
            var model = modelBuilder.GetEdmModel();
            var queryOptions = new ODataQueryOptions<T>(new ODataQueryContext(model, typeof(T), new ODataPath()), defaultHttpContext.Request);

            return queryOptions;
        }
    }
}
