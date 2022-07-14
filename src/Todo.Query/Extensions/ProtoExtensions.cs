using Google.Protobuf.WellKnownTypes;

namespace Todo.Query.Extensions
{
    public static class ProtoExtensions
    {
        public static Timestamp ToTimestamp(this DateOnly dateOnly)
        {
            var dateTime = dateOnly.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            return Timestamp.FromDateTime(dateTime);
        }

        public static DateOnly ToDate(this Timestamp timestamp)
        {
            return DateOnly.FromDateTime(timestamp.ToDateTime());
        }
    }
}
