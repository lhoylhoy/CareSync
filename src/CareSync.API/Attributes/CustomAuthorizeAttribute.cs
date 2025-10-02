using Microsoft.AspNetCore.Authorization;

namespace CareSync.API.Attributes;

/// <summary>
///     Custom authorization attribute that only requires authentication in production.
///     In development, allows anonymous access for easier testing.
/// </summary>
public class CustomAuthorizeAttribute : AuthorizeAttribute
{
    public CustomAuthorizeAttribute()
    {
        Policy = "CustomOnly";
    }
}

public class CustomAuthorizationHandler : AuthorizationHandler<CustomAuthorizeRequirement>
{
    private readonly IHostEnvironment _environment;

    public CustomAuthorizationHandler(IHostEnvironment environment)
    {
        _environment = environment;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CustomAuthorizeRequirement requirement)
    {
        // In development, automatically succeed (allow anonymous)
        if (_environment.IsDevelopment())
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // In production, require the user to be authenticated
        if (context.User.Identity?.IsAuthenticated == true)
            context.Succeed(requirement);
        else
            context.Fail();

        return Task.CompletedTask;
    }
}

public class CustomAuthorizeRequirement : IAuthorizationRequirement
{
}
