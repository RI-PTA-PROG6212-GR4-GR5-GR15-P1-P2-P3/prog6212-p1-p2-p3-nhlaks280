
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers;

/// <summary>
/// Handles registration, login and logout for the RaceDay MVC application.
/// Authentication is performed by the Part 2 API.
/// </summary>
public class AuthController : Controller
{
    private readonly ApiService _apiService;

    public AuthController(ApiService apiService)
    {
        _apiService = apiService;
    }

    // Displays the login page.
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // Sends login details to the Part 2 API.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var request = new
        {
            model.Email,
            model.Password
        };

        try
        {
            var result = await _apiService.PostAsync<object, ApiLoginResponse>(
                "api/auth/login", request);

            if (result == null || string.IsNullOrWhiteSpace(result.Token))
            {
                ModelState.AddModelError(
                    "", "Login failed. Check your email and password.");

                return View(model);
            }

            // Store authenticated user information in the session.
            HttpContext.Session.SetString("AuthToken", result.Token);
            HttpContext.Session.SetString("UserID", result.UserID.ToString());
            HttpContext.Session.SetString(
                "UserName", $"{result.FirstName} {result.LastName}");
            HttpContext.Session.SetString("UserEmail", result.Email ?? "");
            HttpContext.Session.SetString("UserRole", result.Role ?? "");

            // Redirect users according to their account role.
            if (string.Equals(
                result.Role,
                "Organiser",
                StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Dashboard", "Organiser");
            }

            return RedirectToAction("Index", "Participant");
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(
                "", "The RaceDay API could not be reached. Start the API and try again.");

            return View(model);
        }
        catch (TaskCanceledException)
        {
            ModelState.AddModelError(
                "", "The API request timed out. Please try again.");

            return View(model);
        }
    }

    // Displays the registration page.
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // Registers a new user through the Part 2 API.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (model.Role != "Organiser" && model.Role != "Participant")
        {
            ModelState.AddModelError(
                "Role", "Select a valid account type.");

            return View(model);
        }

        var request = new
        {
            model.FirstName,
            model.LastName,
            model.Email,
            model.Password,
            model.Role
        };

        try
        {
            var result = await _apiService.PostAsync<object, ApiRegisterResponse>(
                "api/auth/register", request);

            if (result == null)
            {
                ModelState.AddModelError(
                    "", "Registration failed. The email may already be registered.");

                return View(model);
            }

            TempData["SuccessMessage"] =
                "Your account has been created. You can now log in.";

            return RedirectToAction(nameof(Login));
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(
                "", "The RaceDay API could not be reached. Start the API and try again.");

            return View(model);
        }
        catch (TaskCanceledException)
        {
            ModelState.AddModelError(
                "", "The API request timed out. Please try again.");

            return View(model);
        }
    }

    // Ends the current session and returns the user to login.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        return RedirectToAction(nameof(Login));
    }

    // Displays the access denied page.
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}


