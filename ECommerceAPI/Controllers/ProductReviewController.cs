using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceAPI.Models;
using ECommerceAPI.Services;
using ECommerceAPI.Validators;
using ECommerceAPI.Constants;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Data;
using System.Linq;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductReviewController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IValidator<ProductReviewDTO> _validator;

        public ProductReviewController(ApplicationDbContext db, IValidator<ProductReviewDTO> validator)
        {
            _db = db;
            _validator = validator;
        }

        #region GET ALL PRODUCT REVIEWS - Public (for browsing)
        [HttpGet]
        [AllowAnonymous] // Allow public access to view reviews
        public async Task<IActionResult> GetAllProductReviews()
        {
            try
            {
                var productReviews = await _db.ProductReviews
                    .Include(pr => pr.User)
                    .Include(pr => pr.Product)
                    .Select(pr => new
                    {
                        pr.ReviewID,
                        pr.ProductID,
                        ProductName = pr.Product.ProductName,
                        pr.UserID,
                        UserName = pr.User.UserName,
                        pr.Rating,
                        pr.Title,
                        pr.Comment,
                        pr.Image,
                        pr.Created
                    })
                    .ToListAsync();
                
                return Ok(productReviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting product reviews", error = ex.Message });
            }
        }
        #endregion

        #region GET REVIEWS BY PRODUCT ID - Public (for product detail page)
        [HttpGet("product/{productId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewsByProductId(int productId)
        {
            try
            {
                var reviews = await _db.ProductReviews
                    .Include(pr => pr.User)
                    .Include(pr => pr.Product)
                    .Where(pr => pr.ProductID == productId)
                    .OrderByDescending(pr => pr.Created)
                    .Select(pr => new
                    {
                        pr.ReviewID,
                        pr.ProductID,
                        pr.UserID,
                        UserName = pr.User.UserName,
                        pr.Rating,
                        pr.Title,
                        pr.Comment,
                        pr.Image,
                        pr.Created
                    })
                    .ToListAsync();

                var averageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;
                var count = reviews.Count;

                return Ok(new { reviews, averageRating = Math.Round(averageRating, 1), count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting product reviews", error = ex.Message });
            }
        }
        #endregion

        #region CAN REVIEW PRODUCT - Customer only (check if user can review after delivered)
        [HttpGet("CanReview/{productId}")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> CanReviewProduct(int productId)
        {
            try
            {
                var currentUserId = User.FindFirst("UserID")?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                    return Unauthorized(new { message = "User ID not found in token" });

                var userId = int.Parse(currentUserId);
                var hasDeliveredOrder = await (from od in _db.OrderDetails
                    join o in _db.Orders on od.OrderID equals o.OrderID
                    where o.UserID == userId && od.ProductID == productId
                          && (o.Status == "Delivered" || o.Status == "delivered")
                    select 1).AnyAsync();
                if (!hasDeliveredOrder)
                    return Ok(new { canReview = false, message = "You can review only after the order is delivered." });

                var alreadyReviewed = await _db.ProductReviews
                    .AnyAsync(pr => pr.ProductID == productId && pr.UserID == userId);
                if (alreadyReviewed)
                    return Ok(new { canReview = false, message = "You have already reviewed this product." });

                return Ok(new { canReview = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error checking review eligibility", error = ex.Message });
            }
        }
        #endregion

        #region GET PRODUCT REVIEW BY ID - Public
        [HttpGet("{ReviewId}")]
        [AllowAnonymous] // Allow public access to view specific review
        public async Task<IActionResult> GetByIdProductReviews(int ReviewId)
        {
            try
            {
                var review = await _db.ProductReviews
                    .Include(pr => pr.User)
                    .Include(pr => pr.Product)
                    .Where(pr => pr.ReviewID == ReviewId)
                    .Select(pr => new
                    {
                        pr.ReviewID,
                        pr.ProductID,
                        ProductName = pr.Product.ProductName,
                        pr.UserID,
                        UserName = pr.User.UserName,
                        pr.Rating,
                        pr.Title,
                        pr.Comment,
                        pr.Image,
                        pr.Created
                    })
                    .FirstOrDefaultAsync();

                if (review == null)
                    return NotFound(new { message = "Product review not found" });

                return Ok(review);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting product review", error = ex.Message });
            }
        }
        #endregion

        #region CREATE PRODUCT REVIEW - Customer Only
        [HttpPost]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> InsertProductReview(ProductReviewDTO productReview)
        {
            try
            {
                // Get current user ID from JWT token
                var currentUserId = User.FindFirst("UserID")?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Unauthorized(new { message = "User ID not found in token" });
                }

                // FluentValidation validation
                var result = await _validator.ValidateAsync(productReview);
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage });
                    return BadRequest(new { message = "Validation failed", errors });
                }

                // Verify product exists
                var productExists = await _db.Products.AnyAsync(p => p.ProductID == productReview.ProductID);
                if (!productExists)
                {
                    return BadRequest(new { message = "Product not found" });
                }

                var userId = int.Parse(currentUserId);

                // Customer can review only if they have received this product (order status = Delivered)
                var hasDeliveredOrder = await (from od in _db.OrderDetails
                    join o in _db.Orders on od.OrderID equals o.OrderID
                    where o.UserID == userId && od.ProductID == productReview.ProductID
                          && (o.Status == "Delivered" || o.Status == "delivered")
                    select 1).AnyAsync();
                if (!hasDeliveredOrder)
                {
                    return BadRequest(new { message = "You can submit a review only after the order is delivered." });
                }

                // Check if user already reviewed this product
                var existingReview = await _db.ProductReviews
                    .AnyAsync(pr => pr.ProductID == productReview.ProductID && pr.UserID == userId);
                if (existingReview)
                {
                    return BadRequest(new { message = "You have already reviewed this product" });
                }

                var addproductReview = new ProductReview
                {
                    ProductID = productReview.ProductID,
                    UserID = userId,
                    Rating = productReview.Rating,
                    Title = productReview.Title,
                    Comment = productReview.Comment,
                    Image = string.IsNullOrWhiteSpace(productReview.Image) ? null : productReview.Image.Trim(),
                    Created = DateTime.Now,
                    Modified = DateTime.Now
                };

                _db.ProductReviews.Add(addproductReview);
                await _db.SaveChangesAsync();

                return Created("", new
                {
                    message = "Product review created successfully",
                    reviewId = addproductReview.ReviewID,
                    productId = addproductReview.ProductID,
                    rating = addproductReview.Rating,
                    title = addproductReview.Title
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating product review", error = ex.Message });
            }
        }
        #endregion

        #region UPDATE PRODUCT REVIEW - Admin or Own Review
        [HttpPut("{ReviewId}")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<IActionResult> UpdateProductReview(int ReviewId, ProductReviewDTO productReview)
        {
            if (ReviewId != productReview.ReviewID)
            {
                return BadRequest("Id Mismatch");
            }

            if (ReviewId == 0)
            {
                return BadRequest("Invalid Id");
            }

            try
            {
                // Get current user info from JWT token
                var currentUserId = User.FindFirst("UserID")?.Value;
                var currentUserRole = User.FindFirst("UserTypeName")?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Unauthorized(new { message = "User ID not found in token" });
                }

                // validation
                var result = await _validator.ValidateAsync(productReview);
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage });
                    return BadRequest(new { message = "Validation failed", errors });
                }

                var updateproductReview = await _db.ProductReviews.FindAsync(ReviewId);
                if (updateproductReview == null)
                    return NotFound(new { message = "Product review not found" });

                // Check ownership - only admin or review owner can update
                if (currentUserRole != Roles.Admin && updateproductReview.UserID != int.Parse(currentUserId))
                {
                    return Forbid("You can only update your own reviews");
                }

                updateproductReview.Rating = productReview.Rating;
                updateproductReview.Title = productReview.Title;
                updateproductReview.Comment = productReview.Comment;
                updateproductReview.Image = string.IsNullOrWhiteSpace(productReview.Image) ? null : productReview.Image.Trim();
                updateproductReview.Modified = DateTime.Now;

                await _db.SaveChangesAsync();

                return Ok(new
                {
                    message = "Product review updated successfully",
                    reviewId = updateproductReview.ReviewID,
                    rating = updateproductReview.Rating,
                    title = updateproductReview.Title
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating product review", error = ex.Message });
            }
        }
        #endregion

        #region DELETE PRODUCT REVIEW - Admin or Own Review
        [HttpDelete("{ReviewId}")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<IActionResult> DeleteProductReview(int ReviewId)
        {
            try
            {
                // Get current user info from JWT token
                var currentUserId = User.FindFirst("UserID")?.Value;
                var currentUserRole = User.FindFirst("UserTypeName")?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Unauthorized(new { message = "User ID not found in token" });
                }

                var productReview = await _db.ProductReviews.FindAsync(ReviewId);
                if (productReview == null)
                    return NotFound(new { message = "Product review not found" });

                // Check ownership - only admin or review owner can delete
                if (currentUserRole != Roles.Admin && productReview.UserID != int.Parse(currentUserId))
                {
                    return Forbid("You can only delete your own reviews");
                }

                _db.ProductReviews.Remove(productReview);
                await _db.SaveChangesAsync();

                return Ok(new { message = "Product review deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting product review", error = ex.Message });
            }
        }
        #endregion
    }
}
