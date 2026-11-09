
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Filters;

namespace RaceDay.Mvc.Controllers;

/// <summary>
/// Provides the role-specific interface for RaceDay organisers.
/// </summary>
[SessionAuthorize("Organiser")]
public class OrganiserController : Controller
{
    /// <summary>
    /// Displays the organiser dashboard.
    /// </summary>
    public IActionResult Dashboard()
    {
        ViewData["Title"] = "Organiser Dashboard";

        return View();
    }
}
