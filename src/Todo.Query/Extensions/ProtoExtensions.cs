using Google.Protobuf.WellKnownTypes;

namespace Todo.Query.Extensions
{
    public static class ProtoExtensions
    {
        public static Timestamp ToUtcTimestamp(this DateTime dateTime)
        {
            return Timestamp.FromDateTime(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc));
        }

        public static DateTime ToDate(this Timestamp timestamp)
        {
            return timestamp.ToDateTime().Date;
        }
    }
}
