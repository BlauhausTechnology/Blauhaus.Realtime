using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Auth.Abstractions.User;
using Blauhaus.Domain.TestHelpers.Extensions;
using Blauhaus.Domain.TestHelpers.MockBuilders.Common.CommandHandlers;
using Blauhaus.Realtime.Abstractions.Common;
using Blauhaus.Realtime.Server.SignalR.ConnectionProxy;
using Blauhaus.Realtime.Tests._TestObjects;
using Blauhaus.Realtime.Tests.Server.SignalrCommandProcessorTests._Base;
using CSharpFunctionalExtensions;
using Moq;
using NUnit.Framework;

namespace Blauhaus.Realtime.Tests.Server.SignalrCommandProcessorTests
{
    [TestFixture]
    public class HandleVoidCommandAsyncTests : BaseSignalrCommandProcessorTest
    {

        protected VoidCommandHandlerMockBuilder<TestCommand> MockCommandHandler => Mocks.AddMockVoidCommandHandler<TestCommand>().Invoke();

        public override void Setup()
        {
            base.Setup();

            MockServiceLocator.Where_Resolve_returns(MockCommandHandler.Object);
        }
        
        [Test]
        public async Task IF_handler_succeeds_SHOULD_return_sucess()
        {
            //Arrange
            var payload = new TestPayload();
            MockCommandHandler.Where_HandleAsync_returns_result(Result.Success());

            //Act
            var result = await Sut.HandleVoidCommandAsync(Command, Properties, Connection);

            //Assert 
            Assert.That(result.IsSuccess);
        }


        protected override void SetupException(string exceptionMessage)
        {
            MockCommandHandler.Where_HandleAsync_throws(new Exception(exceptionMessage));
        }
        protected override void SetupFail(string errorMessage)
        {
            MockCommandHandler.Where_HandleAsync_returns_fail(errorMessage);
        }

        protected override async Task<ApiResult> ExecuteAsync(TestCommand command, Dictionary<string, string> properties, ISignalrClientConnectionProxy clientConnection)
        {
            return await Sut.HandleVoidCommandAsync(command, properties, clientConnection);
        }

        protected override void VerifyToken(CancellationToken token)
        {
            MockCommandHandler.Mock.Verify(x => x.HandleAsync(It.IsAny<TestCommand>(), token));
        }
    }
}