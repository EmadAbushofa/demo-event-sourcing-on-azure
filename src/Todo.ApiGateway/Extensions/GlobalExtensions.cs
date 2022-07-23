using System.Security.Claims;

namespace Todo.ApiGateway.Extensions
{
    public static class GlobalExtensions
    {
        public static string GetId(this ClaimsPrincipal claims)
            => claims.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
