//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using ECommerceAPI.Models;
//using ECommerceAPI.Services;
//using ECommerceAPI.Validators;
//using FluentValidation;
//using System;
//using System.Data;
//using System.Linq;

//namespace ECommerceAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ProductController : ControllerBase
//    {
//        private readonly ApplicationDbContext _db;
//        private readonly IValidator<ProductDTO> _validator;

//        public ProductController(ApplicationDbContext db, IValidator<ProductDTO> validator)
//        {
//            _db = db;
//            _validator = validator;
//        }

//        #region get all
//        [HttpGet]
//        public async Task<IActionResult> GetAllProducts()
//        {
//            try
//            {
//                var products = await _db.Products.ToListAsync();
//                return Ok(products);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = "Error getting products", error = ex.Message });
//            }
//        }
//        #endregion

//        #region get by id
//        [HttpGet("{ProductId}")]
//        public async Task<IActionResult> GetByIdProducts(int ProductId)
//        {
//            try
//            {
//                var productId = await _db.Products.FindAsync(ProductId);
//                if (productId == null)
//                    return NotFound(new { message = "Product not found" });

//                return Ok(productId);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = "Error getting product", error = ex.Message });
//            }
//        }
//        #endregion

//        #region insert
//        [HttpPost]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> InsertProduct(ProductDTO product)
//        {
//            try
//            {
//                // FluentValidation validation
//                var result = await _validator.ValidateAsync(product);
//                if (!result.IsValid)
//                {
//                    var errors = result.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage });
//                    return BadRequest(new { message = "Validation failed", errors });
//                }

//                var addproduct = new Product();
//                addproduct.ProductName = product.ProductName;
//                addproduct.ProductCode = product.ProductCode;
//                addproduct.CategoryID = product.CategoryID;
//                addproduct.UserID = product.UserID;
//                addproduct.Price = product.Price;
//                addproduct.Image = product.Image;
//                addproduct.IsActive = product.IsActive;
//                addproduct.Created = DateTime.Now;
//                addproduct.Modified = DateTime.Now;

//                _db.Products.Add(addproduct);
//                await _db.SaveChangesAsync();

//                return Created("", product);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = "Error creating product", error = ex.Message });
//            }
//        }
//        #endregion

//        #region update
//        [HttpPut("{ProductId}")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> UpdateProduct(int ProductId, ProductDTO product)
//        {
//            if (ProductId != product.ProductID)
//            {
//                return BadRequest("Id Mismatch");
//            }

//            if (ProductId == 0)
//            {
//                return BadRequest("Invalid Id");
//            }

//            try
//            {
//                // validation
//                var result = await _validator.ValidateAsync(product);
//                if (!result.IsValid)
//                {
//                    var errors = result.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage });
//                    return BadRequest(new { message = "Validation failed", errors });
//                }

//                var updateproduct = await _db.Products.FindAsync(ProductId);
//                if (updateproduct == null)
//                    return NotFound(new { message = "Product not found" });

//                updateproduct.ProductName = product.ProductName;
//                updateproduct.ProductCode = product.ProductCode;
//                updateproduct.CategoryID = product.CategoryID;
//                updateproduct.UserID = product.UserID;
//                updateproduct.Price = product.Price;
//                updateproduct.Image = product.Image;
//                updateproduct.IsActive = product.IsActive;
//                updateproduct.Modified = DateTime.Now;

//                await _db.SaveChangesAsync();

//                return Ok(updateproduct);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = "Error updating product", error = ex.Message });
//            }
//        }
//        #endregion

//        #region delete
//        [HttpDelete("{ProductId}")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> DeleteProduct(int ProductId)
//        {
//            try
//            {
//                var product = await _db.Products.FindAsync(ProductId);
//                if (product == null)
//                    return NotFound(new { message = "Product not found" });

//                _db.Products.Remove(product);
//                await _db.SaveChangesAsync();

//                return Ok(new { message = "Product deleted successfully" });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = "Error deleting product", error = ex.Message });
//            }
//        }
//        #endregion
//    }
//}


using System;
using System.Data;
using System.Linq;
using System.Security.Claims;
using ECommerceAPI.FileUpload_Services;
using ECommerceAPI.Models;
using ECommerceAPI.Services;
using ECommerceAPI.Validators;
using ECommerceAPI.Constants;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Require authentication for all actions
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IValidator<ProductDTO> _validator;
        private readonly IFileService _fileService;

        public ProductController(ApplicationDbContext db, IValidator<ProductDTO> validator, IFileService fileService)
        {
            _db = db;
            _validator = validator;
            _fileService = fileService;
        }

        #region GET ALL PRODUCTS - Public (for browsing)
        [HttpGet]
        [AllowAnonymous] // Allow public access for browsing products
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _db.Products
                    .Where(p => p.IsActive) // Only show active products to public
                    .Include(p => p.Category)
                    .Include(p => p.User)
                    .Include(p => p.ProductImages)
                    .Select(p => new
                    {
                        p.ProductID,
                        p.ProductName,
                        p.ProductCode,
                        p.Price,
                        p.StockQuantity,
                        p.Image,
                        Images = p.ProductImages.OrderBy(img => img.DisplayOrder).Select(img => new { 
                            img.ProductImageID, 
                            img.ImageUrl 
                        }).ToList(),
                        p.CategoryID,
                        CategoryName = p.Category.CategoryName,
                        p.UserID,
                        CreatedBy = p.User.UserName,
                        p.IsActive,
                        p.Created,
                        p.Modified
                    })
                    .ToListAsync();
                
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting products", error = ex.Message });
            }
        }
        #endregion

        #region GET ALL PRODUCTS FOR ADMIN - Admin Only
        [HttpGet("admin")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllProductsForAdmin()
        {
            try
            {
                var products = await _db.Products
                    .Include(p => p.Category)
                    .Include(p => p.User)
                    .Include(p => p.ProductImages)
                    .Select(p => new
                    {
                        p.ProductID,
                        p.ProductName,
                        p.ProductCode,
                        p.Price,
                        p.StockQuantity,
                        p.Image,
                        Images = p.ProductImages.OrderBy(img => img.DisplayOrder).Select(img => new { 
                            img.ProductImageID, 
                            img.ImageUrl 
                        }).ToList(),
                        p.CategoryID,
                        CategoryName = p.Category.CategoryName,
                        p.UserID,
                        CreatedBy = p.User.UserName,
                        p.IsActive,
                        p.Created,
                        p.Modified
                    })
                    .ToListAsync();
                
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting products", error = ex.Message });
            }
        }
        #endregion

        #region GET PRODUCTS BY CATEGORY - Public
        [HttpGet("category/{categoryId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            try
            {
                var products = await _db.Products
                    .Where(p => p.CategoryID == categoryId && p.IsActive)
                    .Include(p => p.Category)
                    .Include(p => p.User)
                    .Select(p => new
                    {
                        p.ProductID,
                        p.ProductName,
                        p.ProductCode,
                        p.Price,
                        p.StockQuantity,
                        p.Image,
                        p.CategoryID,
                        CategoryName = p.Category.CategoryName,
                        p.UserID,
                        CreatedBy = p.User.UserName,
                        p.IsActive,
                        p.Created,
                        p.Modified
                    })
                    .ToListAsync();
                
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting products by category", error = ex.Message });
            }
        }
        #endregion

        #region GET PRODUCT BY ID - Public
        [HttpGet("{ProductId}")]
        [AllowAnonymous] // Allow public access for viewing product details
        public async Task<IActionResult> GetByIdProducts(int ProductId)
        {
            try
            {
                var product = await _db.Products
                    .Include(p => p.Category)
                    .Include(p => p.User)
                    .Include(p => p.ProductImages)
                    .Where(p => p.ProductID == ProductId && p.IsActive)
                    .Select(p => new
                    {
                        p.ProductID,
                        p.ProductName,
                        p.ProductCode,
                        p.Price,
                        p.StockQuantity,
                        p.Image,
                        Images = p.ProductImages.OrderBy(img => img.DisplayOrder).Select(img => new { 
                            img.ProductImageID, 
                            img.ImageUrl 
                        }).ToList(),
                        p.CategoryID,
                        CategoryName = p.Category.CategoryName,
                        p.UserID,
                        CreatedBy = p.User.UserName,
                        p.IsActive,
                        p.Created,
                        p.Modified
                    })
                    .FirstOrDefaultAsync();

                if (product == null)
                    return NotFound(new { message = "Product not found" });

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting product", error = ex.Message });
            }
        }
        #endregion

        #region SEARCH PRODUCTS - Public
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchProducts([FromQuery] string query, [FromQuery] int? categoryId, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
        {
            try
            {
                var productsQuery = _db.Products
                    .Where(p => p.IsActive)
                    .Include(p => p.Category)
                    .Include(p => p.User)
                    .AsQueryable();

                // Apply search filters
                if (!string.IsNullOrEmpty(query))
                {
                    productsQuery = productsQuery.Where(p => 
                        p.ProductName.Contains(query) || 
                        p.ProductCode.Contains(query) ||
                        p.Category.CategoryName.Contains(query));
                }

                if (categoryId.HasValue)
                {
                    productsQuery = productsQuery.Where(p => p.CategoryID == categoryId.Value);
                }

                if (minPrice.HasValue)
                {
                    productsQuery = productsQuery.Where(p => p.Price >= minPrice.Value);
                }

                if (maxPrice.HasValue)
                {
                    productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);
                }

                var products = await productsQuery
                    .Select(p => new
                    {
                        p.ProductID,
                        p.ProductName,
                        p.ProductCode,
                        p.Price,
                        p.StockQuantity,
                        p.Image,
                        p.CategoryID,
                        CategoryName = p.Category.CategoryName,
                        p.UserID,
                        CreatedBy = p.User.UserName,
                        p.IsActive,
                        p.Created,
                        p.Modified
                    })
                    .ToListAsync();

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error searching products", error = ex.Message });
            }
        }
        #endregion

        #region INSERT PRODUCT - Admin Only
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> InsertProduct([FromForm] ProductDTO product)
        {
            try
            {
                // FluentValidation validation
                var result = await _validator.ValidateAsync(product);
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage });
                    return BadRequest(new { message = "Validation failed", errors });
                }

                // Get current user ID from JWT token
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return BadRequest(new { message = "Unable to identify current user" });
                }

                // Check for duplicate product code
                bool productExists = await _db.Products.AnyAsync(p => p.ProductCode.ToLower() == (product.ProductCode ?? string.Empty).ToLower());
                if (productExists)
                {
                    return BadRequest(new { message = "Product with this code already exists" });
                }

                // Verify category exists and is active
                var category = await _db.Categories.FindAsync(product.CategoryID);
                if (category == null || !category.IsActive)
                {
                    return BadRequest(new { message = "Invalid or inactive category" });
                }

                string? imagePath = null;
                if (product.DocumentFile != null && product.DocumentFile.Length > 0)
                {
                    // Upload image file
                    imagePath = await _fileService.UploadFileAsync(product.DocumentFile, "Products");
                }

                var addproduct = new Product
                {
                    ProductName = product.ProductName ?? string.Empty,
                    ProductCode = product.ProductCode ?? string.Empty,
                    CategoryID = product.CategoryID,
                    UserID = int.Parse(currentUserId), // Use current user's ID
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    Image = imagePath ?? string.Empty,
                    IsActive = product.IsActive,
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    ProductImages = new List<ProductImage>()
                };

                // Add additional images if any (upload in parallel for speed)
                if (product.AdditionalImages != null && product.AdditionalImages.Count > 0)
                {
                    var validFiles = product.AdditionalImages.Where(f => f != null && f.Length > 0).ToList();
                    if (validFiles.Count > 0)
                    {
                        var uploadTasks = validFiles.Select(f => _fileService.UploadFileAsync(f, "Products"));
                        var urls = await Task.WhenAll(uploadTasks);
                        int order = 1;
                        foreach (var url in urls)
                        {
                            addproduct.ProductImages.Add(new ProductImage
                            {
                                ImageUrl = url,
                                DisplayOrder = order++
                            });
                        }
                    }
                }

                _db.Products.Add(addproduct);
                await _db.SaveChangesAsync();

                var response = new
                {
                    addproduct.ProductID,
                    addproduct.ProductName,
                    addproduct.ProductCode,
                    addproduct.Price,
                    addproduct.StockQuantity,
                    addproduct.Image,
                    // Return uploaded images URLs
                    Images = addproduct.ProductImages.Select(pi => pi.ImageUrl).ToList(),
                    addproduct.CategoryID,
                    addproduct.UserID,
                    addproduct.IsActive,
                    imagePath,
                    message = "Product created successfully"
                };

                return Created("", response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating product", error = ex.Message });
            }
        }
        #endregion

        #region UPDATE PRODUCT - Admin Only
        [HttpPut("{ProductId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateProduct(int ProductId, [FromForm] ProductDTO product)
        {
            if (ProductId != product.ProductID)
            {
                return BadRequest("Id Mismatch");
            }

            if (ProductId == 0)
            {
                return BadRequest("Invalid Id");
            }

            try
            {
                // validation
                var result = await _validator.ValidateAsync(product);
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage });
                    return BadRequest(new { message = "Validation failed", errors });
                }

                var updateproduct = await _db.Products
                    .Include(p => p.ProductImages)
                    .FirstOrDefaultAsync(p => p.ProductID == ProductId);

                if (updateproduct == null)
                    return NotFound(new { message = "Product not found" });

                // Check for duplicate product code (excluding current product)
                bool productExists = await _db.Products
                    .AnyAsync(p => p.ProductCode.ToLower() == (product.ProductCode ?? string.Empty).ToLower() && p.ProductID != ProductId);
                if (productExists)
                {
                    return BadRequest(new { message = "Product with this code already exists" });
                }

                // Verify category exists and is active
                var category = await _db.Categories.FindAsync(product.CategoryID);
                if (category == null || !category.IsActive)
                {
                    return BadRequest(new { message = "Invalid or inactive category" });
                }

                // Update main image from string (for clearing or manual update)
                if (product.DocumentFile == null && string.IsNullOrEmpty(product.Image) && !string.IsNullOrEmpty(updateproduct.Image))
                {
                    // If frontend sends empty Image URL and no file, it means main image was deleted
                    await _fileService.DeleteFileAsync(updateproduct.Image);
                    updateproduct.Image = string.Empty;
                }

                // Handle selective image deletion
                if (product.ImagesToDelete != null && product.ImagesToDelete.Any())
                {
                    var imagesToRemove = await _db.ProductImages
                        .Where(pi => product.ImagesToDelete.Contains(pi.ProductImageID) && pi.ProductID == ProductId)
                        .ToListAsync();

                    foreach (var img in imagesToRemove)
                    {
                        if (!string.IsNullOrEmpty(img.ImageUrl))
                        {
                            await _fileService.DeleteFileAsync(img.ImageUrl);
                        }
                    }
                    _db.ProductImages.RemoveRange(imagesToRemove);
                }

                // Handle new image uploads
                if (product.Files != null && product.Files.Any())
                {
                    var newImageUrls = new List<string>();
                    foreach (var file in product.Files)
                    {
                        if (file.Length > 0)
                        {
                            var imageUrl = await _fileService.UploadFileAsync(file, "Products");
                            newImageUrls.Add(imageUrl);
                        }
                    }

                    if (newImageUrls.Any())
                    {
                        // Update main image to the first new upload
                        // Optional: only replace if main image was also deleted or if user wants to change it
                        // For now, let's keep the user's requested logic of replacing main image if new files provided
                        if (!string.IsNullOrEmpty(updateproduct.Image))
                        {
                            await _fileService.DeleteFileAsync(updateproduct.Image);
                        }
                        updateproduct.Image = newImageUrls.First();

                        // Add all new uploads to ProductImages
                        int currentMaxOrder = await _db.ProductImages
                            .Where(pi => pi.ProductID == ProductId)
                            .Select(pi => (int?)pi.DisplayOrder)
                            .MaxAsync() ?? 0;

                        foreach (var url in newImageUrls)
                        {
                            updateproduct.ProductImages.Add(new ProductImage
                            {
                                ImageUrl = url,
                                DisplayOrder = ++currentMaxOrder
                            });
                        }
                    }
                }

                // Update other fields
                updateproduct.ProductName = product.ProductName ?? string.Empty;
                updateproduct.ProductCode = product.ProductCode ?? string.Empty;
                updateproduct.CategoryID = product.CategoryID;
                updateproduct.Price = product.Price;
                updateproduct.StockQuantity = product.StockQuantity;
                updateproduct.IsActive = product.IsActive;
                updateproduct.Modified = DateTime.Now;

                await _db.SaveChangesAsync();

                var response = new
                {
                    updateproduct.ProductID,
                    updateproduct.ProductName,
                    updateproduct.ProductCode,
                    updateproduct.Price,
                    updateproduct.StockQuantity,
                    updateproduct.Image,
                    updateproduct.CategoryID,
                    updateproduct.UserID,
                    updateproduct.IsActive,
                    message = "Product updated successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating product", error = ex.Message });
            }
        }
        #endregion

        #region DELETE PRODUCT - Admin Only
        [HttpDelete("{ProductId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteProduct(int ProductId)
        {
            try
            {
                var product = await _db.Products
                    .Include(p => p.ProductImages)
                    .FirstOrDefaultAsync(p => p.ProductID == ProductId);

                if (product == null)
                    return NotFound(new { message = "Product not found" });

                // Check if product is in any active orders or carts
                bool hasActiveOrders = await _db.OrderDetails.AnyAsync(od => od.ProductID == ProductId);
                bool hasCartItems = await _db.CartItems.AnyAsync(ci => ci.ProductID == ProductId);

                if (hasActiveOrders || hasCartItems)
                {
                    return BadRequest(new { message = "Cannot delete product. It has associated orders or cart items." });
                }

                // Delete associated image file if exists
                if (!string.IsNullOrEmpty(product.Image))
                {
                    await _fileService.DeleteFileAsync(product.Image);
                }

                // Delete additional images
                if (product.ProductImages != null)
                {
                    foreach (var img in product.ProductImages)
                    {
                        await _fileService.DeleteFileAsync(img.ImageUrl);
                    }
                }

                _db.Products.Remove(product);
                await _db.SaveChangesAsync();

                return Ok(new { message = "Product deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting product", error = ex.Message });
            }
        }
        #endregion
    }
}