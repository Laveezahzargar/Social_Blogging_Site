using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace BlogGenerator.Interfaces;

public interface IImageStorageService
{
    Task<string> UploadImageAsync(
        string base64Image,
        string fileName);
}