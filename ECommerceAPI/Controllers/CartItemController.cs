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

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Require authentication for all actions
    public class CartItemController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IValidator<CartItemDTO> _validator;

        public CartItemController(ApplicationDbContext db, IValidator<CartItemDTO> validator)
        {
            _db = db;
            _validator = validator;
        }

        #region GET ALL CART ITEMS - Admin Only
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllCartItems()
        {
            try
            {
                var cartItems = await _db.CartItems
                    .Include(ci => ci.Product)
                        .ThenInclude(p => p.Category)
                    .Include(ci => ci.Cart)
                        .ThenInclude(c => c.User)
                    .Select(ci => new
                    {
                        ci.CartItemID,
                        ci.CartID,
                        ci.ProductID,
                        ProductName = ci.Product.ProductName,
                        ProductCode = ci.Product.ProductCode,
                        CategoryName = ci.Product.Category.CategoryName,
                        ci.Quantity,
                        ci.Price,
                        TotalPrice = ci.Quantity * ci.Price,
                        UserName = ci.Cart.User.UserName,
                        UserEmail = ci.Cart.User.Email,
                        ci.Created,
                        ci.Modified
                    })
                    .ToListAsync();
                
                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting cart items", error = ex.Message });
            }
        }
        #endregion

        #region GET CART ITEMS BY CART ID - Admin or Own Cart
        [HttpGet("cart/{cartId}")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<IActionResult> GetCartItemsByCartId(int cartId)
        {
            try
            {
                var currentUserId = User.FindFirst("UserID")?.Value;
                var currentUserRole = User.FindFirst("UserTypeName")?.Value;

                // Verify cart ownership for customers
                if (currentUserRole != Roles.Admin)
                {
                    var cart = await _db.Carts.FindAsync(cartId);
                    if (cart == null || cart.UserID.ToString() != currentUserId)
                    {
                        return Forbid("You can only access your own cart items");
                    }
                }

                var cartItems = await _db.CartItems
                    .Where(ci => ci.CartID == cartId)
                    .Include(ci => ci.Product)
                        .ThenInclude(p => p.Category)
                    .Select(ci => new
                    {
                        ci.CartItemID,
                        ci.CartID,
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
                    })
                    .ToListAsync();

                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting cart items", error = ex.Message });
            }
        }
        #endregion

        #region GET CART ITEM BY ID - Admin or Own Cart Item
        [HttpGet("{CartItemId}")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<IActionResult> GetByIdCartItems(int CartItemId)
        {
            try
            {
                var currentUserId = User.FindFirst("UserID")?.Value;
                var currentUserRole = User.FindFirst("UserTypeName")?.Value;

                var cartItem = await _db.CartItems
                    .Include(ci => ci.Product)
                        .ThenInclude(p => p.Category)
                    .Include(ci => ci.Cart)
                        .ThenInclude(c => c.User)
                    .FirstOrDefaultAsync(ci => ci.CartItemID == CartItemId);

                if (cartItem == null)
                    return NotFound(new { message = "Cart item not found" });

                // Customer can only access their own cart items
                if (currentUserRole != Roles.Admin && cartItem.Cart.UserID.ToString() != currentUserId)
                {
                    return Forbid("You can only access your own cart items");
                }

                var response = new
                {
                    cartItem.CartItemID,
                    cartItem.CartID,
                    cartItem.ProductID,
                    ProductName = cartItem.Product.ProductName,
                    ProductCode = cartItem.Product.ProductCode,
                    ProductImage = cartItem.Product.Image,
                    ProductPrice = cartItem.Product.Price,
                    CategoryName = cartItem.Product.Category.CategoryName,
                    cartItem.Quantity,
                    cartItem.Price,
                    TotalPrice = cartItem.Quantity * cartItem.Price,
                    UserName = cartItem.Cart.User.UserName,
                    cartItem.Created,
                    cartItem.Modified
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting cart item", error = ex.Message });
            }
        }
        #endregion

        #region ADD TO CART - Customer Only
        [HttpPost]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> InsertCartItem(CartItemDTO cartItem)
        {
            try
            {
                var currentUserId = User.FindFirst("UserID")?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return BadRequest(new { message = "Unable to identify current user" });
                }

                // FluentValidation validation
                var result = await _validator.ValidateAsync(cartItem);
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage });
                    return BadRequest(new { message = "Validation failed", errors });
                }

                // Verify cart ownership
                var cart = await _db.Carts.FindAsync(cartItem.CartID);
                if (cart == null || cart.UserID.ToString() != currentUserId)
                {
                    return Forbid("You can only add items to your own cart");
                }

                // Verify product exists and is active
                var product = await _db.Products.FindAsync(cartItem.ProductID);
                if (product == null || !product.IsActive)
                {
                    return BadRequest(new { message = "Product not found or inactive" });
                }

                // Check if item already exists in cart
                var existingItem = await _db.CartItems
                    .FirstOrDefaultAsync(ci => ci.CartID == cartItem.CartID && ci.ProductID == cartItem.ProductID);

                if (existingItem != null)
                {
                    // Update quantity if item already exists
                    existingItem.Quantity += cartItem.Quantity;
                    existingItem.Modified = DateTime.Now;
                    await _db.SaveChangesAsync();

                    return Ok(new
                    {
                        message = "Cart item quantity updated",
                        cartItemId = existingItem.CartItemID,
                        quantity = existingItem.Quantity
                    });
                }

                // Add new cart item
                var addcartItem = new CartItem
                {
                    CartID = cartItem.CartID,
                    ProductID = cartItem.ProductID,
                    Quantity = cartItem.Quantity,
                    Price = product.Price, // Use current product price
                    Created = DateTime.Now,
                    Modified = DateTime.Now
                };

                _db.CartItems.Add(addcartItem);
                await _db.SaveChangesAsync();

                return Created("", new
                {
                    message = "Item added to cart successfully",
                    cartItemId = addcartItem.CartItemID,
                    productName = product.ProductName,
                    quantity = addcartItem.Quantity,
                    price = addcartItem.Price
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error adding item to cart", error = ex.Message });
            }
        }
        #endregion

        #region UPDATE CART ITEM - Customer Only (Own Cart Item)
        [HttpPut("{CartItemId}")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> UpdateCartItem(int CartItemId, CartItemDTO cartItem)
        {
            if (CartItemId != cartItem.CartItemID)
            {
                return BadRequest("Id Mismatch");
            }

            if (CartItemId == 0)
            {
                return BadRequest("Invalid Id");
            }

            try
            {
                var currentUserId = User.FindFirst("UserID")?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return BadRequest(new { message = "Unable to identify current user" });
                }

                // validation
                var result = await _validator.ValidateAsync(cartItem);
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage });
                    return BadRequest(new { message = "Validation failed", errors });
                }

                var updatecartItem = await _db.CartItems
                    .Include(ci => ci.Cart)
                    .FirstOrDefaultAsync(ci => ci.CartItemID == CartItemId);

                if (updatecartItem == null)
                    return NotFound(new { message = "Cart item not found" });

                // Verify ownership
                if (updatecartItem.Cart.UserID.ToString() != currentUserId)
                {
                    return Forbid("You can only update your own cart items");
                }

                // Update quantity only (price should remain as per product price at time of adding)
                updatecartItem.Quantity = cartItem.Quantity;
                updatecartItem.Modified = DateTime.Now;

                await _db.SaveChangesAsync();

                return Ok(new
                {
                    message = "Cart item updated successfully",
                    cartItemId = updatecartItem.CartItemID,
                    quantity = updatecartItem.Quantity,
                    totalPrice = updatecartItem.Quantity * updatecartItem.Price
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating cart item", error = ex.Message });
            }
        }
        #endregion

        #region DELETE CART ITEM - Customer Only (Own Cart Item)
        [HttpDelete("{CartItemId}")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> DeleteCartItem(int CartItemId)
        {
            try
            {
                var currentUserId = User.FindFirst("UserID")?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return BadRequest(new { message = "Unable to identify current user" });
                }

                var cartItem = await _db.CartItems
                    .Include(ci => ci.Cart)
                    .Include(ci => ci.Product)
                    .FirstOrDefaultAsync(ci => ci.CartItemID == CartItemId);

                if (cartItem == null)
                    return NotFound(new { message = "Cart item not found" });

                // Verify ownership
                if (cartItem.Cart.UserID.ToString() != currentUserId)
                {
                    return Forbid("You can only delete your own cart items");
                }

                _db.CartItems.Remove(cartItem);
                await _db.SaveChangesAsync();

                return Ok(new
                {
                    message = "Cart item removed successfully",
                    productName = cartItem.Product.ProductName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting cart item", error = ex.Message });
            }
        }
        #endregion
    }
}
