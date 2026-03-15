using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ECommerceAPI.Constants;
using ECommerceAPI.FileUpload_Services;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UploadController : ControllerBase
    {
        private readonly IFileService _fileService;

        public UploadController(IFileService fileService)
        {
            _fileService = fileService;
        }

        #region UPLOAD IMAGE - Admin Only
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { message = "No file uploaded" });

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { message = "Invalid file type. Only image files are allowed." });
                }

                // Validate file size (max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { message = "File size too large. Maximum 5MB allowed." });
                }

                var imageUrl = await _fileService.UploadFileAsync(file, "General");
                
                return Ok(new 
                { 
                    message = "Image uploaded successfully",
                    imageUrl = imageUrl,
                    fileName = Path.GetFileName(imageUrl),
                    fileSize = file.Length
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error uploading image", error = ex.Message });
            }
        }
        #endregion

        #region UPLOAD REVIEW IMAGE - Customer Only (for product review image)
        [HttpPost("ReviewImage")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> UploadReviewImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { message = "No file uploaded" });

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                    return BadRequest(new { message = "Invalid file type. Only image files are allowed." });

                if (file.Length > 5 * 1024 * 1024)
                    return BadRequest(new { message = "File size too large. Maximum 5MB allowed." });

                var imageUrl = await _fileService.UploadFileAsync(file, "Reviews");
                return Ok(new { message = "Image uploaded successfully", imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error uploading review image", error = ex.Message });
            }
        }
        #endregion
    }
}
