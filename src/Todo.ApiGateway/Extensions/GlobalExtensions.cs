using Google.Protobuf.WellKnownTypes;
using System.Security.Claims;

namespace Todo.ApiGateway.Extensions
{
    public static class GlobalExtensions
    {
        public static string GetId(this ClaimsPrincipal claims)
            => claims.FindFirstValue(ClaimTypes.NameIdentifier);

        public static Timestamp DateToTimestamp(this DateTime dateTime)
            => DateTime.SpecifyKind(dateTime.Date, DateTimeKind.Utc).ToTimestamp();
    }
}
