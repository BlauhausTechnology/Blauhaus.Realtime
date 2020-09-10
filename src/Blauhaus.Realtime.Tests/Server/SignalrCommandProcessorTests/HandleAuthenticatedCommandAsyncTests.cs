using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Auth.Abstractions.Builders;
using Blauhaus.Auth.Abstractions.Errors;
using Blauhaus.Auth.Abstractions.User;
using Blauhaus.Auth.TestHelpers.MockBuilders;
using Blauhaus.Domain.TestHelpers.Extensions;
using Blauhaus.Domain.TestHelpers.MockBuilders.Common.CommandHandlers;
using Blauhaus.Realtime.Abstractions.Common;
using Blauhaus.Realtime.Server.SignalR.Hubs;
using Blauhaus.Realtime.Tests._TestObjects;
using Blauhaus.Realtime.Tests.Server.SignalrCommandProcessorTests._Base;
using Blauhaus.TestHelpers.MockBuilders;
using Moq;
using NUnit.Framework;

namespace Blauhaus.Realtime.Tests.Server.SignalrCommandProcessorTests
{
    [TestFixture]
    public class HandleAuthenticatedCommandAsync : BaseSignalrCommandProcessorTest
    {

        protected AuthenticatedCommandHandlerMockBuilder<TestPayload, TestCommand, IAuthenticatedUser> MockCommandHandler =>
            Mocks.AddMockAuthenticatedCommandHandler<TestPayload, TestCommand, IAuthenticatedUser>().Invoke();

        public override void Setup()
        {
            base.Setup();

            MockServiceLocator.Where_Resolve_returns(MockCommandHandler.Object);
        }


        [Test]
        public async Task SHOULD_invoke_command_handler_with_Authenticated_User()
        {
            //Arrange
            var claimsPrincipal = new ClaimsPrincipalBuilder().Build();
            MockClientConnection.With(x => x.User, claimsPrincipal);
            var authenticatedUser = new AuthenticatedUserMockBuilder().With(x => x.EmailAddress, "bob@freever.com").Object;
            MockAuthenticatedUserFactory.Where_Create_returns(authenticatedUser);

            //Act
            await ExecuteAsync(Command, Properties, Connection);

            //Assert
            MockAuthenticatedUserFactory.Mock.Verify(x => x.Create(claimsPrincipal));
            MockCommandHandler.Verify_HandleAsync_called_With_User(x => x.EmailAddress == "bob@freever.com");
        }

        [Test]
        public async Task IF_user_factory_fails_To_create_user_SHOULD_return_fail()
        {
            //Arrange
            var claimsPrincipal = new ClaimsPrincipalBuilder().Build();
            MockClientConnection.With(x => x.User, claimsPrincipal);
            MockAuthenticatedUserFactory.Where_Create_fails(AuthErrors.InvalidIdentity);

            //Act
            var result = await ExecuteAsync(Command, Properties, Connection);

            //Assert 
            Assert.That(result.Error, Is.EqualTo(AuthErrors.InvalidIdentity.ToString()));
        }

        
        [Test]
        public async Task IF_handler_succeeds_SHOULD_return_Payload()
        {
            //Arrange
            var payload = new TestPayload();
            MockCommandHandler.Where_HandleAsync_returns(payload);

            //Act
            var result = await Sut.HandleAuthenticatedCommandAsync<TestPayload, TestCommand>(Command, Properties, Connection);

            //Assert 
            Assert.That(result.Value, Is.EqualTo(payload));
        }


        protected override void SetupException(string exceptionMessage)
        {
            MockCommandHandler.Where_HandleAsync_returns_throws(new Exception(exceptionMessage));
        }

        protected override void SetupFail(string errorMessage)
        {
            MockCommandHandler.Where_HandleAsync_returns_fails(errorMessage);
        }


        protected override async Task<ApiResult> ExecuteAsync(TestCommand command, Dictionary<string, string> properties, ISignalrClientConnectionProxy clientConnection)
        {
            
            return await Sut.HandleAuthenticatedCommandAsync<TestPayload, TestCommand>(command, properties, clientConnection);
        }

        protected override void VerifyToken(CancellationToken token)
        {
            MockCommandHandler.Mock.Verify(x => x.HandleAsync(It.IsAny<TestCommand>(), It.IsAny<IAuthenticatedUser>(), token));
        }
    }
}