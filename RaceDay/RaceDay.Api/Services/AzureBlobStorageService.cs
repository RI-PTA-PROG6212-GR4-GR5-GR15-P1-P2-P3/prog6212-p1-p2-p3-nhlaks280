using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace RaceDay.Api.Services;

public class AzureBlobStorageService
{
    private readonly IConfiguration _configuration;

    public AzureBlobStorageService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> UploadAsync(
        IFormFile file,
        string containerName)
    {
        var connectionString =
            _configuration["AzureBlobStorage:ConnectionString"];

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Azure Blob Storage connection string is not configured.");
        }

        var allowedTypes = new[]
        {
            "image/jpeg",
            "image/png",
            "image/webp"
        };

        if (!allowedTypes.Contains(file.ContentType))
        {
            throw new InvalidOperationException(
                "Only JPG, PNG and WEBP images are allowed.");
        }

        if (file.Length == 0 || file.Length > 5 * 1024 * 1024)
        {
            throw new InvalidOperationException(
                "The image must be smaller than 5 MB.");
        }

        var blobServiceClient =
            new BlobServiceClient(connectionString);

        var containerClient =
            blobServiceClient.GetBlobContainerClient(containerName);

        await containerClient.CreateIfNotExistsAsync(
            PublicAccessType.Blob);

        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";

        var blobClient = containerClient.GetBlobClient(fileName);

        await using var stream = file.OpenReadStream();

        await blobClient.UploadAsync(
            stream,
            new BlobHttpHeaders
            {
                ContentType = file.ContentType
            });

        return blobClient.Uri.ToString();
    }
}