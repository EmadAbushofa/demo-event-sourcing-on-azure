using Google.Protobuf.WellKnownTypes;
using Todo.Command.Extensions;

namespace Todo.Command.Test.Helpers
{
    public static class ProtoConverters
    {
        public static Timestamp ToUtcTimestamp(string date)
            => DateOnly.Parse(date).ToTimestamp();
    }
}