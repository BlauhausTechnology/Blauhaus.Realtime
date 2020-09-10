using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Http;
using Blauhaus.Analytics.Abstractions.Operation;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Auth.TestHelpers.MockBuilders;
using Blauhaus.Errors.Extensions;
using Blauhaus.Realtime.Abstractions.Common;
using Blauhaus.Realtime.Server.SignalR.CommandProcessor;
using Blauhaus.Realtime.Server.SignalR.Hubs;
using Blauhaus.Realtime.Tests._Base;
using Blauhaus.Realtime.Tests._MockBuilders;
using Blauhaus.Realtime.Tests._TestObjects;
using Blauhaus.TestHelpers.MockBuilders;
using Moq;
using NUnit.Framework;

namespace Blauhaus.Realtime.Tests.Server.SignalrCommandProcessorTests._Base
{
    public abstract class BaseSignalrCommandProcessorTest : BaseRealtimeTest<SignalrCommandProcessor>
    {
        protected TestCommand Command;
        protected Dictionary<string, string> Properties;
        protected SignalrClientConnectionProxyMockBuilder MockClientConnection;
        protected ISignalrClientConnectionProxy Connection => MockClientConnection.Object;
        protected MockBuilder<IDisposable> MockScope;

        protected AuthenticatedUserFactoryMockBuilder MockAuthenticatedUserFactory => AddMock<AuthenticatedUserFactoryMockBuilder, IAuthenticatedUserFactory>().Invoke();

        public override void Setup()
        {
            base.Setup();

            Command = new TestCommand();
            Properties = new Dictionary<string, string>();
            MockClientConnection = new SignalrClientConnectionProxyMockBuilder();

            MockScope = new MockBuilder<IDisposable>();
            MockServiceLocator.Mock.Setup(x => x.ResetScope()).Returns(MockScope.Object);
            MockServiceLocator.Where_Resolve_returns(MockAnalyticsService.Object);
            MockServiceLocator.Where_Resolve_returns(MockAuthenticatedUserFactory.Object);
        }
        
        [Test]
        public async Task SHOULD_Reset_and_dispose_service_scope()
        {
            //Act
            await ExecuteAsync(Command, Properties, Connection);

            //Assert
            MockScope.Mock.Verify(x => x.Dispose());
        }

        [Test]
        public async Task SHOULD_Start_and_dispose_request_operation()
        {
            //Arrange
            var analyticsOperation = new MockBuilder<IAnalyticsOperation>();
            MockAnalyticsService.Mock.Setup(x => x.StartRequestOperation(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<string>()))
                .Returns(analyticsOperation.Object);
            Properties["Prop"] = "prop";

            //Act
            await ExecuteAsync(Command, Properties, Connection);

            //Assert
            analyticsOperation.Mock.Verify(x => x.Dispose());
            MockAnalyticsService.VerifyStartRequestOperation("TestCommand");
            MockAnalyticsService.Mock.Verify(x => x.StartRequestOperation(Sut, It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<string>()));
            MockAnalyticsService.Mock.Verify(x => x.StartRequestOperation(It.IsAny<object>(), It.IsAny<string>(), It.Is<Dictionary<string, string>>(y => 
                y["Prop"] == "prop"), It.IsAny<string>()));
        }

        [Test]
        public async Task SHOULD_Add_ConnectionId_to_analytics()
        {
            //Arrange 
            MockClientConnection.With(x => x.ConnectionId, "connection");

            //Act
            await ExecuteAsync(Command, Properties, Connection);

            //Assert 
            MockAnalyticsService.Mock.Verify(x => x.StartRequestOperation(It.IsAny<object>(), It.IsAny<string>(), It.Is<Dictionary<string, string>>(y => 
                y[$"{AnalyticsHeaders.Prefix}ConnectionId"] == "connection"), It.IsAny<string>()));
        }

        [Test]
        public async Task IF_handler_throws_exception_SHOULD_log_and_return_error()
        {
            //Arrange
            SetupException("oopsy");

            //Act
            var result = await ExecuteAsync(Command, Properties, Connection);

            //Assert
            MockAnalyticsService.VerifyLogException<Exception>("oopsy");
            Assert.That(result.IsFailure);
            Assert.That(result.Error.IsError(ApiErrors.UnhandledServerError));
        }
        
        [Test]
        public async Task IF_handler_returns_error_SHOULD_return_failure_ApiResult()
        {
            //Arrange
            SetupFail("oopsy");

            //Act
            var result = await ExecuteAsync(Command, Properties, Connection);

            //Assert
            Assert.That(result.IsFailure);
            Assert.That(result.Error, Is.EqualTo("oopsy"));
        }

        [Test]
        public async Task SHOULD_invoke_Handler_with_Client_CancellationToken()
        {
            //Arrange
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            MockClientConnection.With(x => x.ConnectionAborted, token);

            //Act
            await ExecuteAsync(Command, Properties, Connection);

            //Assert
            VerifyToken(token);
        }

        
        protected abstract void SetupException(string errorMessage);
        protected abstract void SetupFail(string errorMessage);
        protected abstract Task<ApiResult> ExecuteAsync(TestCommand command, Dictionary<string, string> properties, ISignalrClientConnectionProxy clientConnection);
        protected abstract void VerifyToken(CancellationToken token);


    }
}