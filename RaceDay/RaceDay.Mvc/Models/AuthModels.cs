using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers;

public class AuthController : Controller
{
    private readonly ApiService _api;

    public AuthController(ApiService api)
    {
        _api = api;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var response = await _api.PostAsync<LoginViewModel, ApiLoginResponse>(
                "api/auth/login", model);

            if (response == null || string.IsNullOrWhiteSpace(response.Token))
            {
                ModelState.AddModelError(
                    string.Empty, "Invalid email or password.");
                return View(model);
            }

            HttpContext.Session.Clear();

            HttpContext.Session.SetString("AuthToken", response.Token);
            HttpContext.Session.SetString("UserID", response.UserID.ToString());
            HttpContext.Session.SetString("UserName", response.UserName ?? "");
            HttpContext.Session.SetString("UserEmail", response.Email ?? "");
            HttpContext.Session.SetString("UserRole", response.Role ?? "");

            if (!string.IsNullOrWhiteSpace(response.ProfilePictureUrl))
            {
                HttpContext.Session.SetString(
                    "ProfilePictureUrl", response.ProfilePictureUrl);
            }

            if (response.Role == "Organiser")
                return RedirectToAction("Index", "Organiser");

            if (response.Role == "Participant")
                return RedirectToAction("Index", "Participant");

            HttpContext.Session.Clear();

            ModelState.AddModelError(
                string.Empty, "Your account has an invalid role.");

            return View(model);
        }
        catch
        {
            ModelState.AddModelError(
                string.Empty, "Unable to connect to the RaceDay service.");
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var request = new
            {
                model.FirstName,
                model.LastName,
                model.Email,
                model.Password,
                model.Role
            };

            var response = await _api.PostAsync<object, ApiRegisterResponse>(
                "api/auth/register", request);

            if (response == null)
            {
                ModelState.AddModelError(
                    string.Empty, "Registration failed. Please try again.");
                return View(model);
            }

            TempData["SuccessMessage"] =
                "Registration successful. Please sign in.";

            return RedirectToAction(nameof(Login));
        }
        catch
        {
            ModelState.AddModelError(
                string.Empty, "Unable to complete registration.");
            return View(model);
        }
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login));
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}