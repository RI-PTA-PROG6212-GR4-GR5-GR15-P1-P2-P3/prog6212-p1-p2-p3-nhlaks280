using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Filters;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers;

[SessionAuthorize("Participant")]
public class ParticipantController : Controller
{
    private readonly ApiService _api;

    public ParticipantController(ApiService api)
    {
        _api = api;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Participant Dashboard";

        try
        {
            var events = await _api.GetAsync<List<RaceEventViewModel>>(
                "api/Events");

            return View(events ?? new List<RaceEventViewModel>());
        }
        catch (Exception)
        {
            ViewBag.ErrorMessage = "Unable to load events. Please try again.";
            return View(new List<RaceEventViewModel>());
        }
    }

    public async Task<IActionResult> EventCategories(int id)
    {
        var eventCategories =
            await _api.GetAsync<List<EventCategoryViewModel>>(
                "api/EventCategories");

        var selected = eventCategories?
            .Where(ec => ec.EventID == id)
            .ToList() ?? new List<EventCategoryViewModel>();

        ViewBag.EventName = selected.FirstOrDefault()?.Event?.EventName
                            ?? "Available event categories";

        return View(selected);
    }
    public async Task<IActionResult> Enrol(int id)
    {
        var token = HttpContext.Session.GetString("AuthToken");

        var eventCategories =
            await _api.GetAsync<List<EventCategoryViewModel>>(
                "api/EventCategories", token);

        var selected = eventCategories?
            .FirstOrDefault(ec => ec.EventCategoryID == id);

        if (selected == null)
            return NotFound("Event category not found.");

        ViewBag.EventCategory = selected;
        return View(new EnrollmentCreateViewModel
        {
            EventCategoryID = id
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enrol(EnrollmentCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var token = HttpContext.Session.GetString("AuthToken");

        var request = new
        {
            EventCategoryID = model.EventCategoryID
        };

        var result = await _api.PostAsync<object, EnrollmentResponseViewModel>(
            "api/Enrollments", request, token);

        if (result == null)
        {
            TempData["Error"] =
                "Enrolment failed. You may already be enrolled or the category may be unavailable.";

            return RedirectToAction(nameof(Index));
        }

        TempData["Success"] = "Your enrolment was successful!";
        return RedirectToAction(nameof(MyEnrollments));
    }

    public async Task<IActionResult> MyEnrollments()
    {
        var token = HttpContext.Session.GetString("AuthToken");

        try
        {
            var enrollments =
                await _api.GetAsync<List<EnrollmentResponseViewModel>>(
                    "api/Enrollments", token);

            return View(enrollments ?? new List<EnrollmentResponseViewModel>());
        }
        catch (Exception)
        {
            ViewBag.ErrorMessage = "Unable to load your enrolments.";
            return View(new List<EnrollmentResponseViewModel>());
        }
    }
}