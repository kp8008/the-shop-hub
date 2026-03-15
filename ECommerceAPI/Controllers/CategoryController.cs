using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceAPI.Models;
using ECommerceAPI.Services;
using ECommerceAPI.Validators;
using ECommerceAPI.Constants;
using FluentValidation;
using System;
using System.Data;
using System.Linq;
using System.Security.Claims;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Require authentication for all actions
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IValidator<CategoryDTO> _validator;

        public CategoryController(ApplicationDbContext db, IValidator<CategoryDTO> validator)
        {
            _db = db;
            _validator = validator;
        }

        #region GET ALL CATEGORIES - Public (for browsing)
        [HttpGet]
        [AllowAnonymous] // Allow public access for browsing categories
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _db.Categories
                    .Where(c => c.IsActive) // Only show active categories to public
                    .Include(c => c.User)
                    .Select(c => new
                    {
                        c.CategoryID,
                        c.CategoryName,
                        c.UserID,
                        CreatedBy = c.User.UserName,
                        c.IsActive,
                        c.Created,
                        c.Modified
                    })
                    .ToListAsync();
                
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting categories", error = ex.Message });
            }
        }
        #endregion

        #region GET ALL CATEGORIES FOR ADMIN - Admin Only
        [HttpGet("admin")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllCategoriesForAdmin()
        {
            try
            {
                var categories = await _db.Categories
                    .Include(c => c.User)
                    .Select(c => new
                    {
                        c.CategoryID,
                        c.CategoryName,
                        c.UserID,
                        CreatedBy = c.User.UserName,
                        c.IsActive,
                        c.Created,
                        c.Modified
                    })
                    .ToListAsync();
                
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting categories", error = ex.Message });
            }
        }
        #endregion

        #region GET CATEGORY BY ID - Public
        [HttpGet("{CategoryId}")]
        [AllowAnonymous] // Allow public access for viewing category details
        public async Task<IActionResult> GetByIdCategories(int CategoryId)
        {
            try
            {
                var category = await _db.Categories
                    .Include(c => c.User)
                    .Where(c => c.CategoryID == CategoryId && c.IsActive)
                    .Select(c => new
                    {
                        c.CategoryID,
                        c.CategoryName,
                        c.UserID,
                        CreatedBy = c.User.UserName,
                        c.IsActive,
                        c.Created,
                        c.Modified
                    })
                    .FirstOrDefaultAsync();

                if (category == null)
                    return NotFound(new { message = "Category not found" });

                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting category", error = ex.Message });
            }
        }
        #endregion

        #region INSERT CATEGORY - Admin Only
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> InsertCategory(CategoryDTO category)
        {
            try
            {
                // FluentValidation validation
                var result = await _validator.ValidateAsync(category);
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

                // Check for duplicate category name
                bool categoryExists = await _db.Categories.AnyAsync(c => c.CategoryName.ToLower() == (category.CategoryName ?? string.Empty).ToLower());
                if (categoryExists)
                {
                    return BadRequest(new { message = "Category with this name already exists" });
                }

                var addcategory = new Category
                {
                    CategoryName = category.CategoryName ?? string.Empty,
                    UserID = int.Parse(currentUserId), // Use current user's ID
                    IsActive = category.IsActive,
                    Created = DateTime.Now,
                    Modified = DateTime.Now
                };

                _db.Categories.Add(addcategory);
                await _db.SaveChangesAsync();

                var response = new
                {
                    addcategory.CategoryID,
                    addcategory.CategoryName,
                    addcategory.UserID,
                    addcategory.IsActive,
                    message = "Category created successfully"
                };

                return Created("", response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating category", error = ex.Message });
            }
        }
        #endregion

        #region UPDATE CATEGORY - Admin Only
        [HttpPut("{CategoryId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateCategory(int CategoryId, CategoryDTO category)
        {
            if (CategoryId != category.CategoryID)
            {
                return BadRequest("Id Mismatch");
            }

            if (CategoryId == 0)
            {
                return BadRequest("Invalid Id");
            }

            try
            {
                // validation
                var result = await _validator.ValidateAsync(category);
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage });
                    return BadRequest(new { message = "Validation failed", errors });
                }

                var updatecategory = await _db.Categories.FindAsync(CategoryId);
                if (updatecategory == null)
                    return NotFound(new { message = "Category not found" });

                // Check for duplicate category name (excluding current category)
                bool categoryExists = await _db.Categories
                    .AnyAsync(c => c.CategoryName.ToLower() == (category.CategoryName ?? string.Empty).ToLower() && c.CategoryID != CategoryId);
                if (categoryExists)
                {
                    return BadRequest(new { message = "Category with this name already exists" });
                }

                updatecategory.CategoryName = category.CategoryName ?? string.Empty;
                updatecategory.IsActive = category.IsActive;
                updatecategory.Modified = DateTime.Now;

                await _db.SaveChangesAsync();

                var response = new
                {
                    updatecategory.CategoryID,
                    updatecategory.CategoryName,
                    updatecategory.UserID,
                    updatecategory.IsActive,
                    message = "Category updated successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating category", error = ex.Message });
            }
        }
        #endregion

        #region DELETE CATEGORY - Admin Only
        [HttpDelete("{CategoryId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteCategory(int CategoryId)
        {
            try
            {
                var category = await _db.Categories.FindAsync(CategoryId);
                if (category == null)
                    return NotFound(new { message = "Category not found" });

                // Check if category has associated products
                bool hasProducts = await _db.Products.AnyAsync(p => p.CategoryID == CategoryId);
                if (hasProducts)
                {
                    return BadRequest(new { message = "Cannot delete category. It has associated products." });
                }

                _db.Categories.Remove(category);
                await _db.SaveChangesAsync();

                return Ok(new { message = "Category deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting category", error = ex.Message });
            }
        }
        #endregion
    }
}
