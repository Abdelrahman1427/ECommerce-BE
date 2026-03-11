using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Application.Helper
{
    public static class FileUploadHelper
    {
        private static string _uploadPath = string.Empty;
        private static string _folder = string.Empty;

        public static void Initialize(IWebHostEnvironment env, IConfiguration config)
        {
            var rootPath = env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            if (!Directory.Exists(rootPath))
                Directory.CreateDirectory(rootPath);

            //_folder = config.GetValue<string>("UploadSettings:UploadPath") ?? "wwwroot/floors";
            _uploadPath = Path.Combine(env.WebRootPath, _folder);
        }
        public static async Task<(string FilePath, string ErrorMessage)> UploadFileAsync(IFormFile file, string[] allowedExtensions, long maxFileSizeInBytes = 10 * 1024 * 1024) // Default: 10MB
        {
            try
            {
                if (file == null || file.Length == 0)
                    return (null, "No file provided.");

                if (file.Length > maxFileSizeInBytes)
                    return (null, $"File size exceeds the maximum limit of {maxFileSizeInBytes / (1024 * 1024)}MB.");

                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                    return (null, $"Invalid file type. Allowed types: {string.Join(", ", allowedExtensions)}.");

                //var uploadsFolder = config.GetValue<string>("UploadSettings:UploadPath");
                if (string.IsNullOrEmpty(_uploadPath))
                    return (null, "Upload path is not configured.");

                if (!Directory.Exists(_uploadPath))
                    Directory.CreateDirectory(_uploadPath);

                var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(_uploadPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return relative path for storage in database
                return (Path.Combine(_folder, uniqueFileName).Replace("\\", "/"), "uploaded success");
            }
            catch (Exception ex)
            {
                return (null, $"Upload failed: {ex.Message}");
            }
        }

        public static bool DeleteFile(string filePath)
        {
            try
            {
                //var uploadsFolder = config.GetValue<string>("UploadSettings:UploadPath");
                var fullPath = Path.Combine(_uploadPath, filePath.TrimStart('/'));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}