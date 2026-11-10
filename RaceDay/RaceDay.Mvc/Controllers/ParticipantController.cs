
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Filters;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers;

[SessionAuthorize("Participant")]
public class ParticipantController : Controller
{
    private readonly ApiService _api;
    private readonly ImageUploadService _imageUpload;

    public ParticipantController(
        ApiService api,
        ImageUploadService imageUpload)
    {
        _api = api;
        _imageUpload = imageUpload;
    }

    private string? Token =>
        HttpContext.Session.GetString("AuthToken");

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Participant Dashboard";

        try
        {
            var categories =
                await _api.GetAsync<List<ParticipantEventCategoryViewModel>>(
                    "api/EventCategories");

            return View(categories ?? new());
        }
        catch
        {
            ViewBag.ErrorMessage = "Unable to load events.";
            return View(new List<ParticipantEventCategoryViewModel>());
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enrol(CreateEnrollmentViewModel model)
    {
        if (!ModelState.IsValid || model.EventCategoryID <= 0)
        {
            TempData["ErrorMessage"] = "Please select a valid category.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var request = new { EventCategoryID = model.EventCategoryID };

            var result =
                await _api.PostAsync<object, ParticipantEnrollmentViewModel>(
                    "api/Enrollments", request, Token);

            TempData[result == null ? "ErrorMessage" : "SuccessMessage"] =
                result == null
                    ? "Enrolment failed. You may already be enrolled."
                    : "You have successfully enrolled.";
        }
        catch
        {
            TempData["ErrorMessage"] = "Unable to complete enrolment.";
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> MyEnrolments()
    {
        try
        {
            var enrolments =
                await _api.GetAsync<List<ParticipantEnrollmentViewModel>>(
                    "api/Enrollments", Token);

            var categories =
                await _api.GetAsync<List<ParticipantEventCategoryViewModel>>(
                    "api/EventCategories");

            ViewBag.Categories = categories ?? new();

            return View(enrolments ?? new());
        }
        catch
        {
            ViewBag.ErrorMessage = "Unable to load your enrolments.";
            ViewBag.Categories =
                new List<ParticipantEventCategoryViewModel>();

            return View(new List<ParticipantEnrollmentViewModel>());
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelEnrolment(int id)
    {
        try
        {
            var success =
                await _api.DeleteAsync($"api/Enrollments/{id}", Token);

            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success
                    ? "Enrolment cancelled."
                    : "Cancellation failed. An enrolment with a result cannot be deleted.";
        }
        catch
        {
            TempData["ErrorMessage"] = "Unable to cancel enrolment.";
        }

        return RedirectToAction(nameof(MyEnrolments));
    }

    public async Task<IActionResult> Results()
    {
        try
        {
            var results =
                await _api.GetAsync<List<ResultViewModel>>(
                    "api/Results", Token);

            return View(results ?? new());
        }
        catch
        {
            ViewBag.ErrorMessage = "Unable to load your results.";
            return View(new List<ResultViewModel>());
        }
    }

    public IActionResult Profile()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadProfilePicture(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["ErrorMessage"] = "Please select an image.";
            return RedirectToAction(nameof(Profile));
        }

        try
        {
            // Upload the image to Azure through the API.
            var imageUrl = await _imageUpload.UploadImageAsync(
                "api/ImageUploads/profile-picture",
                file,
                Token);

            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                TempData["ErrorMessage"] = "Image upload failed.";
                return RedirectToAction(nameof(Profile));
            }

            // Save the image URL permanently in the participant database record.
            var saved = await _api.PutAsync(
                "api/auth/profile-picture",
                new { ImageUrl = imageUrl },
                Token);

            if (!saved)
            {
                TempData["ErrorMessage"] =
                    "The image uploaded, but its URL could not be saved to your profile.";

                return RedirectToAction(nameof(Profile));
            }

            HttpContext.Session.SetString("ProfilePictureUrl", imageUrl);

            TempData["SuccessMessage"] =
                "Profile picture uploaded and saved successfully.";
        }
        catch
        {
            TempData["ErrorMessage"] =
                "Unable to upload or save your profile picture.";
        }

        return RedirectToAction(nameof(Profile));
    }
}
