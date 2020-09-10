using System;
using CSharpFunctionalExtensions;

namespace Blauhaus.Realtime.Abstractions.Server
{
    public class RealtimeApiResult<T> : RealtimeApiResult, IResult<T>
    {
        public RealtimeApiResult(Result result) : base(result)
        {
        }

        public RealtimeApiResult(Result<T> result) : base(result)
        {
            if (result.IsSuccess)
            {
                Value = result.Value;
            }
        }

        public RealtimeApiResult(bool isSuccess, T value, string? error = null)  : base(isSuccess, error)
        {
            Value = value;
        }
        
        public T Value { get; }

    }

    public class RealtimeApiResult : IResult
    {
        public RealtimeApiResult(bool isSuccess, string? error = null)
        {
            IsSuccess = isSuccess;
            IsFailure = !IsSuccess;
            Error = error;

            if(IsFailure && error == null)
                throw new ArgumentException("Failure results require an Error");
        }
        public RealtimeApiResult(Result result)
        {
            IsSuccess = result.IsSuccess;
            IsFailure = result.IsFailure;
            Error = IsFailure ? result.Error : null;
        }

        public string? Error { get; }
        public bool IsFailure { get; }
        public bool IsSuccess { get; }

    }
}