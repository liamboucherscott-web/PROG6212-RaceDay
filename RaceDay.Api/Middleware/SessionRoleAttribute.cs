using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaceDay.Api.Middleware;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class SessionRoleAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _role;

    public SessionRoleAttribute(string role) => _role = role;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var session = context.HttpContext.Session;
        var userId = session.GetString("UserId");
        var userRole = session.GetString("Role");

        if (string.IsNullOrEmpty(userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (_role != "Any" && userRole != _role)
        {
            context.Result = new ForbidResult();
            return;
        }
    }
}
