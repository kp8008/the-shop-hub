using System;
using System.Linq;
using System.Threading.Tasks;
using ECommerceAPI.Models;
using ECommerceAPI.Services;
using ECommerceAPI.Validators;
using ECommerceAPI.Constants;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Require authentication for all actions
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IValidator<CartDTO> _validator;

        public CartController(ApplicationDbContext db, IValidator<CartDTO> validator)
        {
            _db = db;
            _validator = validator;
        }

        #region GET CURRENT USER CART - Customer Only
        [HttpGet("MyCart")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> MyCart()
        {
            try
            {
                var currentUserId = User.FindFirst("UserID")?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return BadRequest(new { message = "Unable to identify current user" });
                }

                int userId = int.Parse(currentUserId);

                var cart = await _db.Carts
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                            .ThenInclude(p => p.Category)
                    .Where(c => c.UserID == userId)
                    .Select(c => new
                    {
                        c.CartID,
                        c.UserID,
                        CartItems = c.CartItems.Select(ci => new
                        {
                            ci.CartItemID,
                            ci.ProductID,
                            ProductName = ci.Product.ProductName,
                            ProductCode = ci.Product.ProductCode,
                            ProductImage = ci.Product.Image,
                            ProductPrice = ci.Product.Price,
                            CategoryName = ci.Product.Category.CategoryName,
                            ci.Quantity,
                            ci.Price,
                            TotalPrice = ci.Quantity * ci.Price,
                            ci.Created,
                            ci.Modified
                        }).ToList(),
                        TotalItems = c.CartItems.Sum(ci => ci.Quantity),
                        TotalAmount = c.CartItems.Sum(ci => ci.Quantity * ci.Price),
                        c.Created,
                        c.Modified
                    })
                    .FirstOrDefaultAsync();

                if (cart == null)
                {
                    return Ok(new { message = "Cart is empty", cart = (object)null });
                }

                return Ok(cart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting cart", error = ex.Message });
            }
        }
        #endregion

        #region CREATE OR GET CART - Customer Only
        [HttpPost("Initialize")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> InitializeCart()
        {
            try
            {
                var currentUserId = User.FindFirst("UserID")?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return BadRequest(new { message = "Unable to identify current user" });
                }

                int userId = int.Parse(currentUserId);

                var existingCart = await _db.Carts
                    .FirstOrDefaultAsync(c => c.UserID == userId);

                if (existingCart != null)
                {
                    return Ok(new
                    {
                        message = "Cart already exists",
                        cartId = existingCart.CartID
                    });
                }

                var newCart = new Cart
                {
                    UserID = userId,
                    Created = DateTime.Now,
                    Modified = DateTime.Now
                };

                _db.Carts.Add(newCart);
                await _db.SaveChangesAsync();

                return Created("", new
                {
                    message = "Cart created successfully",
                    cartId = newCart.CartID
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error initializing cart", error = ex.Message });
            }
        }
        #endregion

        #region GET ALL CARTS - Admin Only
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllCarts()
        {
            try
            {
                var carts = await _db.Carts
                    .Include(c => c.User)
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                    .Select(c => new
                    {
                        c.CartID,
                        c.UserID,
                        UserName = c.User.UserName,
                        UserEmail = c.User.Email,
                        TotalItems = c.CartItems.Sum(ci => ci.Quantity),
                        TotalAmount = c.CartItems.Sum(ci => ci.Quantity * ci.Price),
                        c.Created,
                        c.Modified
                    })
                    .ToListAsync();

                return Ok(carts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting carts", error = ex.Message });
            }
        }
        #endregion

        #region GET CART BY ID - Admin or Own Cart
        [HttpGet("{cartId}")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<IActionResult> GetCartById(int cartId)
        {
            try
            {
                var currentUserId = User.FindFirst("UserID")?.Value;
                var currentUserRole = User.FindFirst("UserTypeName")?.Value;

                var cart = await _db.Carts
                    .Include(c => c.User)
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                            .ThenInclude(p => p.Category)
                    .FirstOrDefaultAsync(c => c.CartID == cartId);

                if (cart == null)
                    return NotFound(new { message = "Cart not found" });

                // Customer can only access their own cart
                if (currentUserRole != Roles.Admin && cart.UserID.ToString() != currentUserId)
                {
                    return Forbid("You can only access your own cart");
                }

                var response = new
                {
                    cart.CartID,
                    cart.UserID,
                    UserName = cart.User.UserName,
                    UserEmail = cart.User.Email,
                    CartItems = cart.CartItems.Select(ci => new
                    {
                        ci.CartItemID,
                        ci.ProductID,
                        ProductName = ci.Product.ProductName,
                        ProductCode = ci.Product.ProductCode,
                        ProductImage = ci.Product.Image,
                        ProductPrice = ci.Product.Price,
                        CategoryName = ci.Product.Category.CategoryName,
                        ci.Quantity,
                        ci.Price,
                        TotalPrice = ci.Quantity * ci.Price,
                        ci.Created,
                        ci.Modified
                    }).ToList(),
                    TotalItems = cart.CartItems.Sum(ci => ci.Quantity),
                    TotalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.Price),
                    cart.Created,
                    cart.Modified
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting cart", error = ex.Message });
            }
        }
        #endregion

        #region CLEAR CART - Customer Only (Own Cart)
        [HttpDelete("Clear")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> ClearMyCart()
        {
            try
            {
                var currentUserId = User.FindFirst("UserID")?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return BadRequest(new { message = "Unable to identify current user" });
                }

                int userId = int.Parse(currentUserId);

                var cart = await _db.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.UserID == userId);

                if (cart == null)
                    return NotFound(new { message = "Cart not found" });

                // Remove all cart items
                _db.CartItems.RemoveRange(cart.CartItems);
                await _db.SaveChangesAsync();

                return Ok(new { message = "Cart cleared successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error clearing cart", error = ex.Message });
            }
        }
        #endregion

        #region DELETE CART - Admin Only
        [HttpDelete("{cartId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteCart(int cartId)
        {
            try
            {
                var cart = await _db.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.CartID == cartId);

                if (cart == null)
                    return NotFound(new { message = "Cart not found" });

                // Remove all cart items first
                _db.CartItems.RemoveRange(cart.CartItems);
                
                // Remove the cart
                _db.Carts.Remove(cart);
                await _db.SaveChangesAsync();

                return Ok(new { message = "Cart deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting cart", error = ex.Message });
            }
        }
        #endregion
    }
}
