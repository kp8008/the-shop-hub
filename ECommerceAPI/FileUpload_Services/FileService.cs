
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ECommerceAPI.Models;
using Microsoft.Extensions.Options;

namespace ECommerceAPI.FileUpload_Services
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file, string subFolder);
        Task<bool> DeleteFileAsync(string? fileUrl);
        void DeleteFile(string? relativePath); // Keep for backward compatibility if needed, but will be empty or handle local
    }

    public class FileService : IFileService
    {
        private readonly Cloudinary _cloudinary;

        public FileService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        public async Task<string> UploadFileAsync(IFormFile file, string subFolder)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            var uploadResult = new ImageUploadResult();

            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = $"ECommerce/{subFolder}",
                    // Remove auto transformations to keep original quality
                    // Transformation = new Transformation().Quality("auto").FetchFormat("auto")
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            if (uploadResult.Error != null)
            {
                throw new Exception(uploadResult.Error.Message);
            }

            return uploadResult.SecureUrl.ToString();
        }

        public async Task<bool> DeleteFileAsync(string? fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl)) return true;

            // Check if it's a valid Cloudinary URL
            if (!Uri.TryCreate(fileUrl, UriKind.Absolute, out var uri) || 
                !uri.Host.Contains("cloudinary.com"))
            {
                // Not a Cloudinary URL (possibly a local path from old data), skip deletion
                return true;
            }

            try
            {
                // Extract public ID from Cloudinary URL
                // Example: https://res.cloudinary.com/demo/image/upload/v12345678/ECommerce/Products/public_id.jpg
                var publicId = Path.GetFileNameWithoutExtension(uri.LocalPath);
            
                // If the folder is included in the path, we need to include it in the publicId
                // Cloudinary publicId usually includes the folder path if specified during upload
                var segments = uri.Segments;
                if (segments.Length > 2)
                {
                    // Find where the version (v12345678) or 'upload' is and take everything after it
                    int uploadIndex = Array.FindIndex(segments, s => s.Trim('/') == "upload");
                    if (uploadIndex != -1 && uploadIndex + 2 < segments.Length)
                    {
                        // Skip 'upload/' and 'v12345678/'
                        var pathSegments = segments.Skip(uploadIndex + 2).Select(s => s.Trim('/'));
                        publicId = string.Join("/", pathSegments);
                        publicId = publicId.Substring(0, publicId.LastIndexOf('.'));
                    }
                }

                var deleteParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);

                return result.Result == "ok";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete Cloudinary file: {fileUrl}. Error: {ex.Message}");
                return false;
            }
        }

        public void DeleteFile(string? relativePath)
        {
            // This was for local files. We can keep it empty or try to handle it.
            // Since we are moving to Cloudinary, new files will use DeleteFileAsync.
            if (string.IsNullOrEmpty(relativePath)) return;
            
            if (relativePath.StartsWith("http"))
            {
                // It's a Cloudinary URL, call the async version (fire and forget or sync wait)
                _ = DeleteFileAsync(relativePath);
                return;
            }

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);
            if (File.Exists(fullPath))
            {
                try
                {
                    File.Delete(fullPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to delete file: {fullPath}. Error: {ex.Message}");
                }
            }
        }
    }
}