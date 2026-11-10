using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Api.Services;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImageUploadsController : ControllerBase
{
    private readonly AzureBlobStorageService _storage;

    public ImageUploadsController(AzureBlobStorageService storage)
    {
        _storage = storage;
    }

    [HttpPost("event-banner")]
    [Authorize(Roles = "Organiser")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<IActionResult> UploadEventBanner(
        IFormFile file)
    {
        if (file == null)
            return BadRequest(new { message = "Please select an image." });

        try
        {
            var url = await _storage.UploadAsync(
                file, "event-banners");

            return Ok(new
            {
                message = "Event banner uploaded successfully.",
                imageUrl = url
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch
        {
            return StatusCode(500, new
            {
                message = "The event banner could not be uploaded."
            });
        }
    }

    [HttpPost("profile-picture")]
    [Authorize(Roles = "Participant")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<IActionResult> UploadProfilePicture(
        IFormFile file)
    {
        if (file == null)
            return BadRequest(new { message = "Please select an image." });

        try
        {
            var url = await _storage.UploadAsync(
                file, "profile-pictures");

            return Ok(new
            {
                message = "Profile picture uploaded successfully.",
                imageUrl = url
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch
        {
            return StatusCode(500, new
            {
                message = "The profile picture could not be uploaded."
            });
        }
    }
}