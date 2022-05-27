using Google.Protobuf.WellKnownTypes;

namespace Todo.Query.Test.Live.Helpers
{
    public static class ProtoConverters
    {
        public static Timestamp ToUtcTimestamp(this DateTime dateTime)
            => dateTime.ToUniversalTime().ToTimestamp();
    }
}