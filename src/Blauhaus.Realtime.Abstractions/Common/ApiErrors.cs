using Blauhaus.Errors;

namespace Blauhaus.Realtime.Abstractions.Common
{
    public static class ApiErrors
    {
        public static Error UnhandledServerError = Error.Create("An unexpected error occured on the server");
    }
}