using System;
using CSharpFunctionalExtensions;

namespace Blauhaus.Realtime.Abstractions.Common
{
    public class ApiResult<T> : ApiResult, IResult<T>
    {
        public ApiResult() { }

        public ApiResult(Result result) : base(result)
        {
        }

        public ApiResult(Result<T> result) : base(result)
        {
            if (result.IsSuccess)
            {
                Value = result.Value;
            }
        }

        public ApiResult(bool isSuccess, T value, string? error = null)  : base(isSuccess, error)
        {
            Value = value;
        }
        
        public T Value { get; }

    }

    public class ApiResult : IResult
    {
        public ApiResult() { }

        public ApiResult(bool isSuccess, string? error = null)
        {
            IsSuccess = isSuccess;
            IsFailure = !IsSuccess;
            Error = error;

            if(IsFailure && error == null)
                throw new ArgumentException("Failure results require an Error");
        }
        public ApiResult(Result result)
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