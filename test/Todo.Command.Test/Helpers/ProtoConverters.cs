using Google.Protobuf.WellKnownTypes;

namespace Todo.Command.Test.Helpers
{
    public static class ProtoConverters
    {
        public static Timestamp ToUtcTimestamp(string dateTime)
            => DateTime.Parse(dateTime).ToUniversalTime().ToTimestamp();
    }
}