using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Domain.TestHelpers.Extensions;
using Blauhaus.Domain.TestHelpers.MockBuilders.Common.CommandHandlers;
using Blauhaus.Realtime.Abstractions.Common;
using Blauhaus.Realtime.Server.SignalR.ConnectionProxy;
using Blauhaus.Realtime.Tests._TestObjects;
using Blauhaus.Realtime.Tests.Server.SignalrCommandProcessorTests._Base;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Blauhaus.Realtime.Tests.Server.SignalrCommandProcessorTests
{
    [TestFixture]
    public class HandleCommandAsyncTests : BaseSignalrCommandProcessorTest
    {
        protected CommandHandlerMockBuilder<TestPayload, TestCommand> MockCommandHandler => Mocks.AddMockCommandHandler<TestPayload, TestCommand>().Invoke();

        public override void Setup()
        {
            base.Setup();

            MockServiceLocator.Where_Resolve_returns(MockCommandHandler.Object);
        }

        
        
        [Test]
        public async Task IF_handler_succeeds_SHOULD_return_Payload()
        {
            //Arrange
            var payload = new TestPayload();
            MockCommandHandler.Where_HandleAsync_returns(payload);

            //Act
            var result = await Sut.HandleCommandAsync<TestPayload, TestCommand>(Command, Properties, Connection);

            //Assert 
            Assert.That(result.Value, Is.EqualTo(payload));
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
            
            return await Sut.HandleCommandAsync<TestPayload, TestCommand>(command, properties, clientConnection);
        }

        protected override void VerifyToken(CancellationToken token)
        {
            MockCommandHandler.Mock.Verify(x => x.HandleAsync(It.IsAny<TestCommand>(), token));
        }
    }
}