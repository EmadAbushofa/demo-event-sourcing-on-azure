using Google.Protobuf.WellKnownTypes;

namespace Todo.Query.Test.Helpers
{
    public static class ProtoConverters
    {
        public static Timestamp? ToUtcTimestamp(string? date)
            => date == null ? null : DateTime.SpecifyKind(DateTime.Parse(date), DateTimeKind.Utc).ToTimestamp();
    }
}