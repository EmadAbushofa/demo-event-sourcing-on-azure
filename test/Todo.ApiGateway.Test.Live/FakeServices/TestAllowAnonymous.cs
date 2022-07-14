using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;

namespace Todo.ApiGateway.Test.Live.FakeServices
{
    public class TestAllowAnonymous : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            foreach (IAuthorizationRequirement requirement in context.PendingRequirements.ToList())
            {
                if (requirement is ScopeAuthorizationRequirement)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}