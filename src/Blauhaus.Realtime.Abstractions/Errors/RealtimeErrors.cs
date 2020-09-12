using Blauhaus.Errors;

namespace Blauhaus.Realtime.Abstractions.Errors
{
    public static class RealtimeErrors
    {
        public static Error NoClientConfiguration(string requestedClientName) => Error.Create
            ("No realtime clients are configured " + requestedClientName != string.Empty ? "for the name " + requestedClientName : "") ;
    }
}