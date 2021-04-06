using AnonymousData;
using FluentAssertions;
using MB.Business.Transaction;
using MB.Data.Entities;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minded.Mediator;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection;
using ODataPath = Microsoft.AspNet.OData.Routing.ODataPath;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.OData.UriParser;
using Microsoft.AspNet.OData.Query.Validators;
using Microsoft.AspNetCore.Builder;
using Builder;
using Minded.Common;

namespace MB.Application.Api.Tests
{
    [TestClass]
    public class TransactionsControllerTests
    {
        private TransactionsController _sut;
        private Mock<IMediator> _mediatorMock;        

        [TestInitialize]
        public void TestInitialize()
        {
            _mediatorMock = new Mock<IMediator>();
            _sut = new TransactionsController(_mediatorMock.Object);            
        }

        [TestMethod]
        public async Task Post_should_use_CreateTransactionCommand_to_create_Transaction()
        {
            var transaction = Builder<Transaction>.New().Build();
            var commandResponse = new CommandResponse<int>(transaction.ID);
            commandResponse.Successful = true;

            _mediatorMock.Setup(m => m.ProcessCommandAsync<int>(It.Is<CreateTransactionCommand>(c => c.Transaction == transaction)))
                .ReturnsAsync(commandResponse)
                .Verifiable();

            var result = await _sut.Post(transaction);

            _mediatorMock.VerifyAll();
        }

        [TestMethod]
        public async Task Post_should_return_BadRequest_when_CreateTransactionCommand_fails_validation()
        {
            var propertyName = Any.String();
            var errorMessageTemplate = $"{0}{Any.String()}";

            var commandResponse = new CommandResponse<int>();
            var validationEntry = new Minded.Validation.ValidationEntry(propertyName, errorMessageTemplate);
            commandResponse.ValidationEntries.Add(validationEntry);
            commandResponse.Successful = false;

            _mediatorMock.Setup(m => m.ProcessCommandAsync<int>(
                It.IsAny<CreateTransactionCommand>()))
                .ReturnsAsync(commandResponse);
            
            var result = await _sut.Post(Any.Of<Transaction>());

            result.Should().BeOfType<BadRequestObjectResult>();
            (result as BadRequestObjectResult).Value.Should().BeEquivalentTo(validationEntry.ToString());
        }

        [TestMethod]
        public async Task Post_should_return_CreatedResult_when_CreateTransactionCommand_succeed()
        {
            var transaction = Builder<Transaction>.New().Build();
            var commandResponse = new CommandResponse<int>
            {
                Successful = true
            };

            _mediatorMock.Setup(m => m.ProcessCommandAsync<int>(
                It.IsAny<CreateTransactionCommand>()))
                .ReturnsAsync(commandResponse);

            var result = await _sut.Post(transaction);

            result.Should().BeOfType<CreatedResult>();
            (result as CreatedResult).Value.Should().BeEquivalentTo(transaction);
            (result as CreatedResult).Location.Should().BeEquivalentTo($"/api/Transactions/{transaction.ID}");
        }

        [TestMethod]
        public async Task Post_should_return_CreatedResult_with_Entity_when_CreateTransactionCommand_succeed()
        {
            var transaction = Builder<Transaction>.New().Build();
            var commandResponse = new CommandResponse<int>
            {
                Successful = true
            };

            _mediatorMock.Setup(m => m.ProcessCommandAsync<int>(
                It.IsAny<CreateTransactionCommand>()))
                .ReturnsAsync(commandResponse);

            var result = await _sut.Post(transaction);

            (result as CreatedResult).Value.Should().BeEquivalentTo(transaction);
        }

        [TestMethod]
        public async Task Post_should_return_CreatedResult_with_CreatedLocation_when_CreateTransactionCommand_succeed()
        {
            var transaction = Builder<Transaction>.New().Build();
            var commandResponse = new CommandResponse<int>
            {
                Successful = true
            };

            _mediatorMock.Setup(m => m.ProcessCommandAsync<int>(
                It.IsAny<CreateTransactionCommand>()))
                .ReturnsAsync(commandResponse);

            var result = await _sut.Post(transaction);

            (result as CreatedResult).Location.Should().BeEquivalentTo($"/api/Transactions/{transaction.ID}");
        }
    }
}
