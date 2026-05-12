using Microsoft.AspNetCore.Authorization;

namespace AspireTodoApp.Server.Features.Auth;

public static class AuthorizationHandlerExtensions
{
    public static AuthorizationBuilder AddCurrentUserHandler(this AuthorizationBuilder builder)
    {
        builder.Services.AddScoped<IAuthorizationHandler, CheckCurrentUserAuthHandler>();
        return builder;
    }

    public static AuthorizationPolicyBuilder RequireCurrentUser(this AuthorizationPolicyBuilder builder)
    {
        return builder.RequireAuthenticatedUser()
                      .AddRequirements(new CheckCurrentUserRequirement());
    }

    private class CheckCurrentUserRequirement : IAuthorizationRequirement { }

    // Verifies the user still exists in the DB even if the JWT is valid
    private class CheckCurrentUserAuthHandler(CurrentUser currentUser) : AuthorizationHandler<CheckCurrentUserRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CheckCurrentUserRequirement requirement)
        {
            if (currentUser.User is not null)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
