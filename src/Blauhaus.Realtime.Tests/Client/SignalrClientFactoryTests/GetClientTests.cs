using System;
using System.Collections.Generic;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Analytics.TestHelpers.Extensions;
using Blauhaus.Realtime.Abstractions.Client;
using Blauhaus.Realtime.Abstractions.Errors;
using Blauhaus.Realtime.Client.SignalR.Client;
using Blauhaus.Realtime.Tests._Base;
using Blauhaus.TestHelpers.MockBuilders;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Blauhaus.Realtime.Tests.Client.SignalrClientFactoryTests
{
    public class GetClientTests : BaseRealtimeTest<SignalrClientFactory>
    {
        protected MockBuilder<IRealtimeClientDefinitions> MockClientDefinitions => AddMock<IRealtimeClientDefinitions>().Invoke();
        protected MockBuilder<IRealtimeClient> MockResolvedClient;

        public override void Setup()
        {
            base.Setup();

            MockResolvedClient = new MockBuilder<IRealtimeClient>();
            MockServiceLocator.Where_Resolve_returns(MockResolvedClient.Object);

            AddService(MockClientDefinitions.Object);
        }

        [Test]
        public void IF_there_are_no_injected_or_runtime_definitions_SHOULD_return_error()
        {
            //Arrange
            MockClientDefinitions.Mock.Setup(x => x.GetConfig("")).Returns(default(IRealtimeClientConfig));

            //Act
            var result = Sut.GetClient();

            //Asserrt
            result.VerifyResultError(RealtimeErrors.NoClientConfiguration(""), MockAnalyticsService);
        }

        [Test]
        public void IF_there_are_matching_injected_and_runtime_definitions_SHOULD_give_runtime_preference()
        {
            //Arrange
            var injectedConfig = new MockBuilder<IRealtimeClientConfig>().Object;
            var runtimeConfig = new MockBuilder<IRealtimeClientConfig>().Object;
            MockClientDefinitions.Mock.Setup(x => x.GetConfig("myClient")).Returns(injectedConfig);
            Sut.AddRuntimeClient("myClient", runtimeConfig);

            //Act
            var result = Sut.GetClient("myClient");

            //Asserrt
            MockResolvedClient.Mock.Verify(x => x.Configure(runtimeConfig));
            Assert.That(result.Value, Is.EqualTo(MockResolvedClient.Object));
            MockAnalyticsService.VerifyTrace("New myClient realtime client created", LogSeverity.Information);
        }
        
        [Test]
        public void IF_there_is_a_matching_injected_definition_SHOULD_use_it_to_configure_client()
        {
            //Arrange
            var injectedConfig = new MockBuilder<IRealtimeClientConfig>().Object;
            MockClientDefinitions.Mock.Setup(x => x.GetConfig("myClient")).Returns(injectedConfig);

            //Act
            var result = Sut.GetClient("myClient");

            //Asserrt
            MockResolvedClient.Mock.Verify(x => x.Configure(injectedConfig));
            Assert.That(result.Value, Is.EqualTo(MockResolvedClient.Object));
            MockAnalyticsService.VerifyTrace("New myClient realtime client created", LogSeverity.Information);
        }

        [Test]
        public void IF_client_with_name_has_been_returned_before_SHOULD_return_same_instance()
        {
            //Arrange
            var injectedConfig = new MockBuilder<IRealtimeClientConfig>().Object;
            MockClientDefinitions.Mock.Setup(x => x.GetConfig("myClient")).Returns(injectedConfig);

            //Act
            Sut.GetClient("myClient");
            Sut.GetClient("myClient");

            //Asserrt
            MockResolvedClient.Mock.Verify(x => x.Configure(injectedConfig), Times.Once);
            MockServiceLocator.Mock.Verify(x => x.Resolve<IRealtimeClient>(), Times.Once);
        }
    }
}