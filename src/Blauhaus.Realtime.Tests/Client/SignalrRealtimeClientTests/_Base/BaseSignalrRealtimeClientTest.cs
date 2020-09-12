using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Realtime.Client.SignalR.Client;
using Blauhaus.Realtime.Client.SignalR.ConnectionProxy;
using Blauhaus.Realtime.Tests._Base;
using Moq;
using NUnit.Framework;

namespace Blauhaus.Realtime.Tests.Client.SignalrRealtimeClientTests._Base
{
    public abstract class BaseSignalrRealtimeClientTest : BaseRealtimeTest<SignalrClient>
    {

        public override void Setup()
        {
            base.Setup(); 
        }

        [Test]
        public async Task IF_Hub_has_not_been_initialized_yet_SHOULD_start_it()
        {
            //Arrange
            MockClientConfig.With(x => x.AccessToken, "accessToken");
            MockClientConfig.With(x => x.Url, "http://www.google.com/chat");

            //Act
            await ExecuteAsync();

            //Assert
            VerifyHubProxyInitialized();
            MockAnalyticsService.VerifyTrace("SignalR client hub initialized for the first time");
        }

        private void VerifyHubProxyInitialized()
        {
            MockServiceLocator.Mock.Verify(x => x.Resolve<ISignalrServerConnectionProxy>(), Times.Once);
            MockHubConnectionProxy.Mock.Verify(x => x.Configure(It.Is<ClientConnectionConfig>(y => 
                y.AccessToken == "accessToken" &&
                y.Url == "http://www.google.com/chat")));
            MockHubConnectionProxy.Mock.Verify(x => x.StartAsync(It.IsAny<CancellationToken>()));
        }


        protected abstract Task ExecuteAsync();

    }
}