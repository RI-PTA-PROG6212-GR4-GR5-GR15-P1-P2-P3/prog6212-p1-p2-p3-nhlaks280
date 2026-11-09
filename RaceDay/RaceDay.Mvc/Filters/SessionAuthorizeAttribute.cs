
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaceDay.Mvc.Filters;

/// <summary>
/// Protects MVC pages using the authenticated user's session.
/// An optional role can restrict access to a specific user type.
/// </summary>
public class SessionAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly string? _requiredRole;

    public SessionAuthorizeAttribute(string? requiredRole = null)
    {
        _requiredRole = requiredRole;
    }

    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var session = context.HttpContext.Session;

        // Check whether the user has an active login session.
        var token = session.GetString("AuthToken");

        if (string.IsNullOrWhiteSpace(token))
        {
            context.Result = new RedirectToActionResult(
                "Login", "Auth", null);

            return Task.CompletedTask;
        }

        // Check whether the user's role permits access to this page.
        var currentRole = session.GetString("UserRole");

        if (_requiredRole != null &&
            !string.Equals(currentRole, _requiredRole,
                StringComparison.OrdinalIgnoreCase))
        {
            context.Result = new RedirectToActionResult(
                "AccessDenied", "Auth", null);
        }

        return Task.CompletedTask;
    }
}
