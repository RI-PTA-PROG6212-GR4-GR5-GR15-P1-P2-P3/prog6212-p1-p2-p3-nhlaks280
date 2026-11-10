
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Filters;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers;

[SessionAuthorize("Organiser")]
public class OrganiserController : Controller
{
    private readonly ApiService _api;
    private readonly ImageUploadService _imageUpload;

    public OrganiserController(
        ApiService api,
        ImageUploadService imageUpload)
    {
        _api = api;
        _imageUpload = imageUpload;
    }

    private string? Token =>
        HttpContext.Session.GetString("AuthToken");

    private int? OrganiserID =>
        HttpContext.Session.GetInt32("UserID");

    public IActionResult Dashboard()
    {
        ViewData["Title"] = "Organiser Dashboard";
        return View();
    }

    public async Task<IActionResult> Events()
    {
        var events = await _api.GetAsync<List<RaceEventViewModel>>(
            "api/Events", Token);

        return View(events ?? new());
    }

    public IActionResult CreateEvent()
    {
        return View(new RaceEventViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEvent(
        RaceEventViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (OrganiserID == null)
        {
            ViewBag.ErrorMessage = "Your session has expired.";
            return View(model);
        }

        model.OrganiserID = OrganiserID.Value;

        try
        {
            if (model.BannerImage != null &&
                model.BannerImage.Length > 0)
            {
                model.BannerUrl = await _imageUpload.UploadImageAsync(
                    "api/ImageUploads/event-banner",
                    model.BannerImage,
                    Token);

                if (string.IsNullOrWhiteSpace(model.BannerUrl))
                {
                    ViewBag.ErrorMessage = "Banner upload failed.";
                    return View(model);
                }
            }

            var request = new
            {
                model.OrganiserID,
                model.EventName,
                model.Description,
                model.EventDate,
                model.Location,
                model.DistanceKm,
                model.EventType,
                model.BannerUrl
            };

            var result = await _api.PostAsync<object, RaceEventViewModel>(
                "api/Events", request, Token);

            if (result == null)
            {
                ViewBag.ErrorMessage = "Unable to create event.";
                return View(model);
            }

            TempData["SuccessMessage"] = "Event created successfully.";
            return RedirectToAction(nameof(Events));
        }
        catch
        {
            ViewBag.ErrorMessage = "An error occurred while creating the event.";
            return View(model);
        }
    }

    // Display an existing event for editing.
    public async Task<IActionResult> EditEvent(int id)
    {
        var raceEvent = await _api.GetAsync<RaceEventViewModel>(
            $"api/Events/{id}", Token);

        if (raceEvent == null)
            return NotFound();

        return View(raceEvent);
    }

    // Save event changes.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditEvent(
        int id, RaceEventViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (OrganiserID == null)
        {
            ViewBag.ErrorMessage = "Your session has expired.";
            return View(model);
        }

        try
        {
            model.OrganiserID = OrganiserID.Value;

            if (model.BannerImage != null &&
                model.BannerImage.Length > 0)
            {
                var uploadedUrl = await _imageUpload.UploadImageAsync(
                    "api/ImageUploads/event-banner",
                    model.BannerImage,
                    Token);

                if (string.IsNullOrWhiteSpace(uploadedUrl))
                {
                    ViewBag.ErrorMessage = "Banner upload failed.";
                    return View(model);
                }

                model.BannerUrl = uploadedUrl;
            }

            var request = new
            {
                model.EventID,
                model.OrganiserID,
                model.EventName,
                model.Description,
                model.EventDate,
                model.Location,
                model.DistanceKm,
                model.EventType,
                model.BannerUrl
            };

            var success = await _api.PutAsync(
                $"api/Events/{id}", request, Token);

            if (!success)
            {
                ViewBag.ErrorMessage = "Unable to update event.";
                return View(model);
            }

            TempData["SuccessMessage"] = "Event updated successfully.";
            return RedirectToAction(nameof(Events));
        }
        catch
        {
            ViewBag.ErrorMessage = "An error occurred while updating the event.";
            return View(model);
        }
    }

    // Delete an event.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var success = await _api.DeleteAsync(
            $"api/Events/{id}", Token);

        TempData[success ? "SuccessMessage" : "ErrorMessage"] =
            success
                ? "Event deleted successfully."
                : "Unable to delete event.";

        return RedirectToAction(nameof(Events));
    }

    public async Task<IActionResult> Enrolments()
    {
        var enrolments =
            await _api.GetAsync<List<ParticipantEnrollmentViewModel>>(
                "api/Enrollments", Token);

        return View(enrolments ?? new());
    }

    public async Task<IActionResult> Categories()
    {
        var categories =
            await _api.GetAsync<List<ParticipantEventCategoryViewModel>>(
                "api/EventCategories");

        return View(categories ?? new());
    }

    public IActionResult CreateCategory()
    {
        return View(new ParticipantEventCategoryViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(
        ParticipantEventCategoryViewModel model)
    {
        var result =
            await _api.PostAsync<object, ParticipantEventCategoryViewModel>(
                "api/EventCategories",
                new { model.EventID, model.CategoryID },
                Token);

        if (result == null)
        {
            ViewBag.ErrorMessage =
                "Unable to assign category. Check the event and category IDs.";

            return View(model);
        }

        TempData["SuccessMessage"] = "Category assigned successfully.";
        return RedirectToAction(nameof(Categories));
    }

    public async Task<IActionResult> Results()
    {
        var results = await _api.GetAsync<List<ResultViewModel>>(
            "api/Results", Token);

        return View(results ?? new());
    }

    public IActionResult ResultsEntry()
    {
        return View(new ResultViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResultsEntry(
        ResultViewModel model)
    {
        if (!ModelState.IsValid ||
            model.FinishTime <= TimeSpan.Zero)
        {
            ModelState.AddModelError(
                nameof(model.FinishTime),
                "Finish time must be greater than zero.");

            return View(model);
        }

        var result =
            await _api.PostAsync<ResultViewModel, ResultViewModel>(
                "api/Results", model, Token);

        if (result == null)
        {
            ViewBag.ErrorMessage = "Unable to save result.";
            return View(model);
        }

        TempData["SuccessMessage"] = "Result recorded successfully.";
        return RedirectToAction(nameof(Results));
    }
}
