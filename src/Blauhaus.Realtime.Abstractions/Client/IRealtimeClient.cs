using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace Blauhaus.Realtime.Abstractions.Client
{
     
    public interface IRealtimeClient
    {
        IObservable<RealtimeClientState> Observe();
        IObservable<TEvent> Connect<TEvent>(string methodName);
        
        Task<Result<TResponse>> InvokeAsync<TResponse>(string methodName, object parameter);
        Task<Result> InvokeAsync(string methodName, object parameter);

    }
}