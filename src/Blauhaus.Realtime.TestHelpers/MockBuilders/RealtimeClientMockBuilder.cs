using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Blauhaus.Realtime.Abstractions.Client;
using Blauhaus.TestHelpers.MockBuilders;

namespace Blauhaus.Realtime.TestHelpers.MockBuilders
{
    public class RealtimeClientMockBuilder : BaseMockBuilder<RealtimeClientMockBuilder, IRealtimeClient>
    {

        public RealtimeClientMockBuilder()
        {
            Where_Observe_returns(RealtimeClientState.Connected);
        }

        public RealtimeClientMockBuilder Where_Observe_returns(RealtimeClientState state)
        {
            Mock.Setup(x => x.Observe())
                .Returns(Observable.Create<RealtimeClientState>(observer =>
                {
                    observer.OnNext(state);
                    return Disposable.Empty;
                }));
            return this;
        }
        public RealtimeClientMockBuilder Where_Observe_returns(IEnumerable<RealtimeClientState> values)
        {
            Mock.Setup(x => x.Observe())
                .Returns(Observable.Create<RealtimeClientState>(observer =>
                {
                    foreach (var state in values)
                    {
                        observer.OnNext(state);
                    }
                    return Disposable.Empty;
                }));
            return this;
        }
        public RealtimeClientMockBuilder Where_Observe_returns(IDisposable token)
        {
            Mock.Setup(x => x.Observe())
                .Returns(Observable.Create<RealtimeClientState>(observer => token));
            return this;
        }
        public RealtimeClientMockBuilder Where_Observe_fails(Exception e)
        {
            Mock.Setup(x => x.Observe())
                .Returns(Observable.Create<RealtimeClientState>(observer =>
                {
                    observer.OnError(e);
                    return Disposable.Empty;
                }));
            return this;
        }
         
    }
}