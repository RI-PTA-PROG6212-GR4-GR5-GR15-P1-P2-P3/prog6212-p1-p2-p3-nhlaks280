
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace RaceDay.Mvc.Services;

/// <summary>
/// Central service for communicating with the RaceDay Part 2 API.
/// All application data operations are performed through the API.
/// </summary>
public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Creates an HTTP request and attaches the user's JWT token
    /// when one is supplied.
    /// </summary>
    private HttpRequestMessage CreateRequest(
        HttpMethod method,
        string endpoint,
        string? token = null)
    {
        var request = new HttpRequestMessage(method, endpoint);

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return request;
    }

    /// <summary>
    /// Retrieves information from the API.
    /// </summary>
    public async Task<T?> GetAsync<T>(
        string endpoint,
        string? token = null)
    {
        using var request = CreateRequest(HttpMethod.Get, endpoint, token);

        using var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return default;

        return await response.Content.ReadFromJsonAsync<T>();
    }

    /// <summary>
    /// Sends JSON data to the API.
    /// </summary>
    public async Task<TResponse?> PostAsync<TRequest, TResponse>(
        string endpoint,
        TRequest data,
        string? token = null)
    {
        using var request = CreateRequest(HttpMethod.Post, endpoint, token);

        request.Content = JsonContent.Create(data);

        using var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return default;

        return await response.Content.ReadFromJsonAsync<TResponse>();
    }

    /// <summary>
    /// Updates existing information through the API.
    /// </summary>
    public async Task<bool> PutAsync<TRequest>(
        string endpoint,
        TRequest data,
        string? token = null)
    {
        using var request = CreateRequest(HttpMethod.Put, endpoint, token);

        request.Content = JsonContent.Create(data);

        using var response = await _httpClient.SendAsync(request);

        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Deletes information through the API.
    /// </summary>
    public async Task<bool> DeleteAsync(
        string endpoint,
        string? token = null)
    {
        using var request = CreateRequest(HttpMethod.Delete, endpoint, token);

        using var response = await _httpClient.SendAsync(request);

        return response.IsSuccessStatusCode;
    }
}
