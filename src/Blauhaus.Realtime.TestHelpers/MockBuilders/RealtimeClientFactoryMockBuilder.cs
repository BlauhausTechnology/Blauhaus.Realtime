using Blauhaus.Realtime.Abstractions.Client;
using Blauhaus.TestHelpers.MockBuilders;
using CSharpFunctionalExtensions;
using Moq;

namespace Blauhaus.Realtime.TestHelpers.MockBuilders
{
    public class RealtimeClientFactoryMockBuilder : BaseMockBuilder<RealtimeClientFactoryMockBuilder, IRealtimeClientFactory>
    {
        public RealtimeClientFactoryMockBuilder()
        {
            Mock.Setup(x => x.AddRuntimeClient(It.IsAny<string>(), It.IsAny<IRealtimeClientConfig>()))
                .Returns(Result.Success);
        }


        public RealtimeClientFactoryMockBuilder Where_GetClient_returns(IRealtimeClient client)
        {
            Mock.Setup(x => x.GetClient(It.IsAny<string>())).Returns(Result.Success(client));
            return this;
        }
        public RealtimeClientFactoryMockBuilder Where_GetClient_returns(IRealtimeClient client, string clientName)
        {
            Mock.Setup(x => x.GetClient(clientName)).Returns(Result.Success(client));
            return this;
        }
        
        public RealtimeClientFactoryMockBuilder Where_GetClient_fails(string error)
        {
            Mock.Setup(x => x.GetClient(It.IsAny<string>())).Returns(Result.Failure<IRealtimeClient>(error));
            return this;
        }

        
        public RealtimeClientFactoryMockBuilder Where_AddRuntimeClient_fails(string error)
        {
            Mock.Setup(x => x.AddRuntimeClient(It.IsAny<string>(), It.IsAny<IRealtimeClientConfig>()))
                .Returns(Result.Failure(error));
            return this;
        }
    }
}