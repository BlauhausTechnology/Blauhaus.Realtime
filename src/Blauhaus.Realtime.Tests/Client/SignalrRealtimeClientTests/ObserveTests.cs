using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Realtime.Abstractions.Client;
using Blauhaus.Realtime.Client.SignalR.ConnectionProxy;
using Blauhaus.Realtime.Tests.Client.SignalrRealtimeClientTests._Base;
using Microsoft.AspNetCore.SignalR.Client;
using NUnit.Framework;

namespace Blauhaus.Realtime.Tests.Client.SignalrRealtimeClientTests
{
    public class ObserveTests : BaseSignalrRealtimeClientTest
    {
        protected override Task ExecuteAsync()
        {
            Sut.Observe().Subscribe(observer => {});
            return Task.CompletedTask;
        }

        
        [TestCase(HubConnectionState.Connecting, RealtimeClientState.Connecting)]
        [TestCase(HubConnectionState.Reconnecting, RealtimeClientState.Reconnecting)]
        [TestCase(HubConnectionState.Connected, RealtimeClientState.Connected)]
        [TestCase(HubConnectionState.Disconnected, RealtimeClientState.Disconnected)]
        public async Task SHOULD_publish_initial_hub_state_as_RealtimeClientState(HubConnectionState hubState, RealtimeClientState clientState)
        {
            //Arrange
            MockHubConnectionProxy.With(x => x.CurrentState, hubState);

            //Act
            var tcs = new TaskCompletionSource<RealtimeClientState>();
            Sut.Observe().Subscribe(clientstate =>
            {
                tcs.SetResult(clientstate);
            });
            var publishedState = await tcs.Task;

            //Assert
            Assert.That(publishedState, Is.EqualTo(clientState));
        }

        [Test]
        public async Task SHOULD_publish_hub_state_WHEN_it_updates()
        {
            //Arrange
            var tcs = new TaskCompletionSource<List<RealtimeClientState>>();

            //Act
            var publishedStates = new List<RealtimeClientState>();
            Sut.Observe().Subscribe(clientstate =>
            {
                publishedStates.Add(clientstate);
                if (publishedStates.Count == 4)
                {
                    tcs.SetResult(publishedStates);
                }
            });
            MockHubConnectionProxy.Mock.Raise(x => x.StateChanged += null!, new ClientConnectionStateChangeEventArgs(HubConnectionState.Connecting, null));
            MockHubConnectionProxy.Mock.Raise(x => x.StateChanged += null!, new ClientConnectionStateChangeEventArgs(HubConnectionState.Connected, null));
            MockHubConnectionProxy.Mock.Raise(x => x.StateChanged += null!, new ClientConnectionStateChangeEventArgs(HubConnectionState.Disconnected, null));
            await tcs.Task;

            //Assert
            Assert.That(publishedStates[1], Is.EqualTo(RealtimeClientState.Connecting));
            Assert.That(publishedStates[2], Is.EqualTo(RealtimeClientState.Connected));
            Assert.That(publishedStates[3], Is.EqualTo(RealtimeClientState.Disconnected));
        }


        [Test]
        public async Task Dispose_SHOULD_unsubscribe_from_hub_state_changes()
        {
            //Act
            var publishedStates = new List<RealtimeClientState>();
            var subscription = Sut.Observe().Subscribe(clientstate =>
            {
                publishedStates.Add(clientstate); 
            });
            MockHubConnectionProxy.Mock.Raise(x => x.StateChanged += null!, new ClientConnectionStateChangeEventArgs(HubConnectionState.Connecting, null));
            MockHubConnectionProxy.Mock.Raise(x => x.StateChanged += null!, new ClientConnectionStateChangeEventArgs(HubConnectionState.Connected, null));
            subscription.Dispose();
            MockHubConnectionProxy.Mock.Raise(x => x.StateChanged += null!, new ClientConnectionStateChangeEventArgs(HubConnectionState.Disconnected, null));
            await Task.Delay(20);

            //Assert
            Assert.That(publishedStates.Count, Is.EqualTo(3));
        }

        [Test]
        public async Task Dispos_SHOULD_not_unsubscribe_other_observers()
        {
            //Act
            var statesOne = new List<RealtimeClientState>();
            var statesTwo = new List<RealtimeClientState>();
            var subscriptionOne = Sut.Observe().Subscribe(clientstate =>
            {
                statesOne.Add(clientstate);
            });
            Sut.Observe().Subscribe(clientstate =>
            {
                statesTwo.Add(clientstate);
            });  
            subscriptionOne.Dispose();
            MockHubConnectionProxy.Mock.Raise(x => x.StateChanged += null!, new ClientConnectionStateChangeEventArgs(HubConnectionState.Disconnected, null));
            await Task.Delay(20);

            //Assert
            Assert.That(statesOne.Count, Is.EqualTo(1));
            Assert.That(statesTwo.Count, Is.EqualTo(2));
        }
        
        [Test]
        public async Task IF_Reconnecting_state_is_published_with_Exception_SHOULD_trace_Warning()
        {
            //Arrange
            var tcs = new TaskCompletionSource<List<RealtimeClientState>>();
            var publishedStates = new List<RealtimeClientState>();

            //Act 
            Sut.Observe().Subscribe(clientstate =>
            {
                publishedStates.Add(clientstate);
                if(publishedStates.Count == 2) tcs.SetResult(publishedStates);
            });
            MockHubConnectionProxy.Mock.Raise(x => x.StateChanged += null!, new ClientConnectionStateChangeEventArgs(HubConnectionState.Reconnecting, new Exception("transient error")));
            await tcs.Task;

            //Assert 
            Assert.That(publishedStates[1], Is.EqualTo(RealtimeClientState.Reconnecting));
            MockAnalyticsService.VerifyTrace("SignalR client hub Reconnecting due to exception: transient error", LogSeverity.Warning);
        }

        [Test]
        public async Task IF_Reconnecting_state_is_published_without_Exception_SHOULD_trace_Verbose()
        {
            //Arrange
            var tcs = new TaskCompletionSource<List<RealtimeClientState>>();
            var publishedStates = new List<RealtimeClientState>();

            //Act 
            Sut.Observe().Subscribe(clientstate =>
            {
                publishedStates.Add(clientstate);
                if(publishedStates.Count == 2) tcs.SetResult(publishedStates);
            });
            MockHubConnectionProxy.Mock.Raise(x => x.StateChanged += null!, new ClientConnectionStateChangeEventArgs(HubConnectionState.Reconnecting, null));
            await tcs.Task;

            //Assert 
            Assert.That(publishedStates[1], Is.EqualTo(RealtimeClientState.Reconnecting));
            MockAnalyticsService.VerifyTrace("SignalR client hub Reconnecting");
        }

        [Test]
        public async Task IF_Disconnected_state_is_published_with_Exception_SHOULD_log_exception()
        {
            //Arrange
            var tcs = new TaskCompletionSource<List<RealtimeClientState>>();
            var publishedStates = new List<RealtimeClientState>();

            //Act 
            Sut.Observe().Subscribe(clientstate =>
            {
                publishedStates.Add(clientstate);
                if(publishedStates.Count == 2) tcs.SetResult(publishedStates);
            });
            MockHubConnectionProxy.Mock.Raise(x => x.StateChanged += null!, new ClientConnectionStateChangeEventArgs(HubConnectionState.Disconnected, new Exception("fatal error")));
            await tcs.Task;

            //Assert 
            Assert.That(publishedStates[1], Is.EqualTo(RealtimeClientState.Disconnected));
            MockAnalyticsService.VerifyLogException<Exception>("fatal error");
            MockAnalyticsService.VerifyTrace("SignalR client hub Disconnected", LogSeverity.Warning);
        }

        [Test]
        public async Task IF_Disconnected_state_is_published_without_Exception_SHOULD_trace_warning()
        {
            //Arrange
            var tcs = new TaskCompletionSource<List<RealtimeClientState>>();
            var publishedStates = new List<RealtimeClientState>();

            //Act 
            Sut.Observe().Subscribe(clientstate =>
            {
                publishedStates.Add(clientstate);
                if(publishedStates.Count == 2) tcs.SetResult(publishedStates);
            });
            MockHubConnectionProxy.Mock.Raise(x => x.StateChanged += null!, new ClientConnectionStateChangeEventArgs(HubConnectionState.Disconnected, null));
            await tcs.Task;

            //Assert 
            Assert.That(publishedStates[1], Is.EqualTo(RealtimeClientState.Disconnected));
            MockAnalyticsService.VerifyTrace("SignalR client hub Disconnected", LogSeverity.Warning);
        }

        [Test]
        public async Task IF_Connected_state_is_published_SHOULD_trace()
        {
            //Arrange
            var tcs = new TaskCompletionSource<List<RealtimeClientState>>();
            var publishedStates = new List<RealtimeClientState>();

            //Act 
            Sut.Observe().Subscribe(clientstate =>
            {
                publishedStates.Add(clientstate);
                if(publishedStates.Count == 2) tcs.SetResult(publishedStates);
            });
            MockHubConnectionProxy.Mock.Raise(x => x.StateChanged += null!, new ClientConnectionStateChangeEventArgs(HubConnectionState.Connected, null));
            await tcs.Task;

            //Assert 
            Assert.That(publishedStates[1], Is.EqualTo(RealtimeClientState.Connected));
            MockAnalyticsService.VerifyTrace("SignalR client hub Connected");
        }

        [Test]
        public async Task IF_Connecting_state_is_published_SHOULD_trace()
        {
            //Arrange
            var tcs = new TaskCompletionSource<List<RealtimeClientState>>();
            var publishedStates = new List<RealtimeClientState>();

            //Act 
            Sut.Observe().Subscribe(clientstate =>
            {
                publishedStates.Add(clientstate);
                if(publishedStates.Count == 2) tcs.SetResult(publishedStates);
            });
            MockHubConnectionProxy.Mock.Raise(x => x.StateChanged += null!, new ClientConnectionStateChangeEventArgs(HubConnectionState.Connecting, null));
            await tcs.Task;

            //Assert 
            Assert.That(publishedStates[1], Is.EqualTo(RealtimeClientState.Connecting));
            MockAnalyticsService.VerifyTrace("SignalR client hub Connecting");
        }

    }
}