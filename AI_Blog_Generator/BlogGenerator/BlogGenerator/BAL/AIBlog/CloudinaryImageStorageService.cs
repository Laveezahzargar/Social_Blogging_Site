


using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using BlogGenerator.Interfaces;

namespace BlogGenerator.BAL;

public class CloudinaryImageStorageService : IImageStorageService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryImageStorageService(
        IConfiguration configuration)
    {
        var cloudName =
            configuration["Cloudinary:CloudName"];

        var apiKey =
            configuration["Cloudinary:ApiKey"];

        var apiSecret =
            configuration["Cloudinary:ApiSecret"];

        if (string.IsNullOrWhiteSpace(cloudName) ||
            string.IsNullOrWhiteSpace(apiKey) ||
            string.IsNullOrWhiteSpace(apiSecret))
        {
            throw new InvalidOperationException(
                "Cloudinary configuration is missing.");
        }

        var account = new Account(
            cloudName,
            apiKey,
            apiSecret);

        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadImageAsync(
        string base64Image,
        string fileName)
    {
        var imageBytes =
            Convert.FromBase64String(base64Image);

        using var stream =
            new MemoryStream(imageBytes);

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(
                fileName,
                stream),

            Folder = "storybloom/blogs"
        };

        var result =
            await _cloudinary.UploadAsync(uploadParams);

        if (result.Error != null)
        {
            throw new InvalidOperationException(
                $"Cloudinary upload failed: {result.Error.Message}");
        }

        return result.SecureUrl.ToString();
    }
}