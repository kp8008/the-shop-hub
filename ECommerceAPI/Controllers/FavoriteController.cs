 using Microsoft.AspNetCore.Authorization;
 using Microsoft.AspNetCore.Mvc;
 using Microsoft.EntityFrameworkCore;
 using ECommerceAPI.Models;
 using ECommerceAPI.Services;
 using ECommerceAPI.Validators;
 using FluentValidation;
 using System;
 using System.Linq;
 using System.Threading.Tasks;
 using System.Security.Claims;
 
 namespace ECommerceAPI.Controllers
 {
     [Route("api/Favorites")]
     [ApiController]
     [Authorize]
     public class FavoriteController : ControllerBase
     {
         private readonly ApplicationDbContext _db;
         private readonly IValidator<FavoriteDTO> _validator;
 
         public FavoriteController(ApplicationDbContext db, IValidator<FavoriteDTO> validator)
         {
             _db = db;
             _validator = validator;
         }
 
         private int? GetCurrentUserId()
         {
             var id = User.FindFirst("UserID")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
             if (string.IsNullOrEmpty(id)) return null;
             if (int.TryParse(id, out var parsed)) return parsed;
             return null;
         }
 
         // GET: api/Favorites/MyFavorites
         [HttpGet("MyFavorites")]
         [Authorize(Policy = "CustomerOnly")]
         public async Task<IActionResult> MyFavorites()
         {
             try
             {
                 var currentUserId = GetCurrentUserId();
                 if (!currentUserId.HasValue)
                     return BadRequest(new { message = "Unable to identify current user" });
 
                 int userId = currentUserId.Value;
 
                 var userExists = await _db.Users.AnyAsync(u => u.UserID == userId && u.IsActive);
                 if (!userExists)
                     return BadRequest(new { message = "User not found or inactive" });
 
                 var favorites = await _db.Favorites
                     .Where(f => f.UserID == userId)
                     .Include(f => f.Product)
                         .ThenInclude(p => p.Category)
                     .Select(f => new
                     {
                         f.FavoriteID,
                         f.ProductID,
                         ProductName = f.Product.ProductName,
                         ProductCode = f.Product.ProductCode,
                         ProductImage = f.Product.Image,
                         ProductPrice = f.Product.Price,
                         CategoryName = f.Product.Category.CategoryName,
                         f.Created
                     })
                     .ToListAsync();
 
                 return Ok(favorites);
             }
             catch (Exception ex)
             {
                 return StatusCode(500, new { message = "Error getting favorites", error = ex.Message });
             }
         }
 
         // POST: api/Favorites
         [HttpPost]
         [Authorize(Policy = "CustomerOnly")]
         public async Task<IActionResult> InsertFavorite([FromBody] FavoriteDTO favorite)
         {
             try
             {
                 var currentUserId = GetCurrentUserId();
                 if (!currentUserId.HasValue)
                     return BadRequest(new { message = "Unable to identify current user" });
 
                 favorite.UserID = currentUserId.Value;
 
                 var result = await _validator.ValidateAsync(favorite);
                 if (!result.IsValid)
                 {
                     var errors = result.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage });
                     return BadRequest(new { message = "Validation failed", errors });
                 }
 
                 var product = await _db.Products.FindAsync(favorite.ProductID);
                 if (product == null || !product.IsActive)
                     return BadRequest(new { message = "Product not found or inactive" });
 
                 var existing = await _db.Favorites.FirstOrDefaultAsync(f => f.UserID == favorite.UserID && f.ProductID == favorite.ProductID);
                 if (existing != null)
                     return Ok(new { message = "Already in favorites" });
 
                 var addFavorite = new Favorite
                 {
                     UserID = favorite.UserID,
                     ProductID = favorite.ProductID,
                     Created = DateTime.Now,
                     Modified = DateTime.Now
                 };
 
                 _db.Favorites.Add(addFavorite);
                 await _db.SaveChangesAsync();
 
                 return Created("", new { message = "Added to favorites", favoriteId = addFavorite.FavoriteID });
             }
             catch (Exception ex)
             {
                 return StatusCode(500, new { message = "Error adding favorite", error = ex.Message });
             }
         }
 
         // POST: api/Favorites/toggle/{productId}
         [HttpPost("toggle/{productId}")]
         [Authorize(Policy = "CustomerOnly")]
         public async Task<IActionResult> ToggleFavorite(int productId)
         {
             try
             {
                 var currentUserId = GetCurrentUserId();
                 if (!currentUserId.HasValue)
                     return BadRequest(new { message = "Unable to identify current user" });
 
                 int userId = currentUserId.Value;
 
                 var existing = await _db.Favorites.FirstOrDefaultAsync(f => f.UserID == userId && f.ProductID == productId);
                 if (existing != null)
                 {
                     _db.Favorites.Remove(existing);
                     await _db.SaveChangesAsync();
                     return Ok(new { message = "Removed from favorites" });
                 }
 
                 var product = await _db.Products.FindAsync(productId);
                 if (product == null || !product.IsActive)
                     return BadRequest(new { message = "Product not found or inactive" });
 
                 var addFavorite = new Favorite
                 {
                     UserID = userId,
                     ProductID = productId,
                     Created = DateTime.Now,
                     Modified = DateTime.Now
                 };
                 _db.Favorites.Add(addFavorite);
                 await _db.SaveChangesAsync();
                 return Created("", new { message = "Added to favorites", favoriteId = addFavorite.FavoriteID });
             }
             catch (Exception ex)
             {
                 return StatusCode(500, new { message = "Error toggling favorite", error = ex.Message });
             }
         }
 
         // DELETE: api/Favorites/{productId}
         [HttpDelete("{productId}")]
         [Authorize(Policy = "CustomerOnly")]
         public async Task<IActionResult> DeleteFavorite(int productId)
         {
             try
             {
                 var currentUserId = GetCurrentUserId();
                 if (!currentUserId.HasValue)
                     return BadRequest(new { message = "Unable to identify current user" });
 
                 int userId = currentUserId.Value;
 
                 var favorite = await _db.Favorites
                     .Include(f => f.Product)
                     .FirstOrDefaultAsync(f => f.UserID == userId && f.ProductID == productId);
 
                 if (favorite == null)
                     return NotFound(new { message = "Favorite not found" });
 
                 _db.Favorites.Remove(favorite);
                 await _db.SaveChangesAsync();
 
                 return Ok(new { message = "Removed from favorites", productName = favorite.Product.ProductName });
             }
             catch (Exception ex)
             {
                 return StatusCode(500, new { message = "Error removing favorite", error = ex.Message });
             }
         }
     }
 }
