using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyMvcApp.Authorization.Requirements;
using MyMvcApp.Repositories;

namespace MyMvcApp.Authorization.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class AuthorizeResourceOwnerAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
{
    private readonly string _resourceIdParameter;

    public AuthorizeResourceOwnerAttribute(string resourceIdParameter = "id")
    {
        _resourceIdParameter = resourceIdParameter;
        Policy = "ResourceOwner";
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new ChallengeResult();
            return;
        }

        // Get resource ID from route or query
        var resourceIdValue = context.RouteData.Values[_resourceIdParameter]?.ToString() 
            ?? context.HttpContext.Request.Query[_resourceIdParameter].ToString();

        if (string.IsNullOrEmpty(resourceIdValue) || !int.TryParse(resourceIdValue, out int resourceId))
        {
            context.Result = new BadRequestResult();
            return;
        }

        var authorizationService = context.HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
        var requirement = new ResourceOwnerRequirement();

        var result = await authorizationService.AuthorizeAsync(user, resourceId, requirement).ConfigureAwait(false);

        if (!result.Succeeded)
        {
            context.Result = new ForbidResult();
        }
    }
}





