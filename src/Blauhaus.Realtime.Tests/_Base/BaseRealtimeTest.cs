using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Analytics.TestHelpers.MockBuilders;
using Blauhaus.Ioc.Abstractions;
using Blauhaus.Ioc.TestHelpers;
using Blauhaus.Realtime.Abstractions.Client;
using Blauhaus.Realtime.Client.SignalR.HubProxy;
using Blauhaus.Realtime.Tests._MockBuilders;
using Blauhaus.TestHelpers.BaseTests;
using Blauhaus.TestHelpers.MockBuilders;
using NUnit.Framework;

namespace Blauhaus.Realtime.Tests._Base
{
    public abstract class BaseRealtimeTest<TSut> : BaseServiceTest<TSut> where TSut : class
    {
        [SetUp]
        public virtual void Setup()
        {
            base.Cleanup();

            AddService(x => MockClientConfig.Object);
            AddService(x => MockAnalyticsService.Object);
            AddService(x => MockServiceLocator.Object);

            MockServiceLocator.Where_Resolve_returns(MockHubConnectionProxy.Object);

        }

        protected MockBuilder<IRealtimeClientConfig> MockClientConfig => AddMock<IRealtimeClientConfig>().Invoke();
        protected AnalyticsServiceMockBuilder MockAnalyticsService => AddMock<AnalyticsServiceMockBuilder, IAnalyticsService>().Invoke();
        protected ServiceLocatorMockBuilder MockServiceLocator => AddMock<ServiceLocatorMockBuilder, IServiceLocator>().Invoke();
        protected SignalrServerConnectionProxyMockBuilder MockHubConnectionProxy => AddMock<SignalrServerConnectionProxyMockBuilder, ISignalrServerConnectionProxy>().Invoke();
    }
}