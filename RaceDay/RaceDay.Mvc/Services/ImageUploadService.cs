
using System.Net.Http.Headers;
using System.Text.Json;

namespace RaceDay.Mvc.Services;

public class ImageUploadService
{
    private readonly HttpClient _httpClient;

    public ImageUploadService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> UploadImageAsync(
        string endpoint,
        IFormFile file,
        string? token)
    {
        using var form = new MultipartFormDataContent();
        await using var stream = file.OpenReadStream();

        using var fileContent = new StreamContent(stream);

        fileContent.Headers.ContentType =
            new MediaTypeHeaderValue(file.ContentType);

        form.Add(fileContent, "file", file.FileName);

        using var request = new HttpRequestMessage(
            HttpMethod.Post, endpoint);

        request.Content = form;

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        using var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(json);

        if (document.RootElement.TryGetProperty(
            "imageUrl", out var imageUrl))
        {
            return imageUrl.GetString();
        }

        return null;
    }
}
