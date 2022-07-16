using Google.Protobuf.WellKnownTypes;
using Todo.Query.Extensions;

namespace Todo.Query.Test.Helpers
{
    public static class ProtoConverters
    {
        public static Timestamp? ToUtcTimestamp(string? date)
            => date == null ? null : DateTime.Parse(date).ToUtcTimestamp();
    }
}