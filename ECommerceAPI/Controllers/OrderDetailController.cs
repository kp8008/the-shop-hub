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
    public class OrderDetailController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IValidator<OrderDetailDTO> _validator;
        private const string PlaceholderImage = "https://via.placeholder.com/300x300.png?text=No+Image";

        public OrderDetailController(ApplicationDbContext db, IValidator<OrderDetailDTO> validator)
        {
            _db = db;
            _validator = validator;
        }
        
        private string BuildImageUrl(string? path)
        {
            if (string.IsNullOrWhiteSpace(path)) return PlaceholderImage;
            if (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || path.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                if (path.Contains("example.com", StringComparison.OrdinalIgnoreCase)) return PlaceholderImage;
                return path;
            }
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var normalized = path.Replace("\\", "/").TrimStart('/');
            return $"{baseUrl}/{normalized}";
        }

        #region GET ALL ORDER DETAILS - Admin Only
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllOrderDetails()
        {
            try
            {
                var orderDetails = await _db.OrderDetails
                    .Include(od => od.Order)
                        .ThenInclude(o => o.User)
                    .Include(od => od.Product)
                        .ThenInclude(p => p.Category)
                    .Include(od => od.Address)
                    .Select(od => new
                    {
                        od.OrderDetailID,
                        od.OrderID,
                        OrderNo = od.Order.OrderNo,
                        UserName = od.Order.User.UserName,
                        od.ProductID,
                        ProductName = od.Product.ProductName,
                        ProductCode = od.Product.ProductCode,
                        ProductImage = BuildImageUrl(od.Product.Image),
                        CategoryName = od.Product.Category.CategoryName,
                        od.AddressID,
                        ShippingAddress = od.Address != null ? $"{od.Address.AddressLine1}, {od.Address.City}" : "",
                        od.Quantity,
                        od.Amount,
                        od.Discount,
                        od.NetAmount,
                        od.Created,
                        od.Modified
                    })
                    .ToListAsync();
                
                return Ok(orderDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting order details", error = ex.Message });
            }
        }
        #endregion

        #region GET ORDER DETAILS BY ORDER ID - Admin or Own Order
        [HttpGet("order/{orderId}")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<IActionResult> GetOrderDetailsByOrderId(int orderId)
        {
            try
            {
                var currentUserId = User.FindFirst("UserID")?.Value;
                var currentUserRole = User.FindFirst("UserTypeName")?.Value;

                // Verify order ownership for customers
                if (currentUserRole != Roles.Admin)
                {
                    var order = await _db.Orders.FindAsync(orderId);
                    if (order == null || order.UserID.ToString() != currentUserId)
                    {
                        return Forbid("You can only access your own order details");
                    }
                }

                var orderDetails = await _db.OrderDetails
                    .Where(od => od.OrderID == orderId)
                    .Include(od => od.Product)
                        .ThenInclude(p => p.Category)
                    .Include(od => od.Address)
                    .Select(od => new
                    {
                        od.OrderDetailID,
                        od.OrderID,
                        od.ProductID,
                        ProductName = od.Product.ProductName,
                        ProductCode = od.Product.ProductCode,
                        ProductImage = BuildImageUrl(od.Product.Image),
                        CategoryName = od.Product.Category.CategoryName,
                        od.AddressID,
                        ShippingAddress = od.Address != null ? new
                        {
                            od.Address.ReceiverName,
                            od.Address.Phone,
                            FullAddress = $"{od.Address.AddressLine1}, {od.Address.City}, {od.Address.State} - {od.Address.Pincode}"
                        } : null,
                        od.Quantity,
                        od.Amount,
                        od.Discount,
                        od.NetAmount,
                        od.Created,
                        od.Modified
                    })
                    .ToListAsync();

                return Ok(orderDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting order details", error = ex.Message });
            }
        }
        #endregion

        #region GET ORDER DETAIL BY ID - Admin or Own Order Detail
        [HttpGet("{OrderDetailId}")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<IActionResult> GetByIdOrderDetails(int OrderDetailId)
        {
            try
            {
                var currentUserId = User.FindFirst("UserID")?.Value;
                var currentUserRole = User.FindFirst("UserTypeName")?.Value;

                var orderDetail = await _db.OrderDetails
                    .Include(od => od.Order)
                        .ThenInclude(o => o.User)
                    .Include(od => od.Product)
                        .ThenInclude(p => p.Category)
                    .Include(od => od.Address)
                    .FirstOrDefaultAsync(od => od.OrderDetailID == OrderDetailId);

                if (orderDetail == null)
                    return NotFound(new { message = "Order detail not found" });

                // Customer can only access their own order details
                if (currentUserRole != Roles.Admin && orderDetail.Order.UserID.ToString() != currentUserId)
                {
                    return Forbid("You can only access your own order details");
                }

                var response = new
                {
                    orderDetail.OrderDetailID,
                    orderDetail.OrderID,
                    OrderNo = orderDetail.Order.OrderNo,
                    UserName = orderDetail.Order.User.UserName,
                    orderDetail.ProductID,
                    ProductName = orderDetail.Product.ProductName,
                    ProductCode = orderDetail.Product.ProductCode,
                    ProductImage = BuildImageUrl(orderDetail.Product.Image),
                    CategoryName = orderDetail.Product.Category.CategoryName,
                    orderDetail.AddressID,
                    ShippingAddress = orderDetail.Address != null ? new
                    {
                        orderDetail.Address.ReceiverName,
                        orderDetail.Address.Phone,
                        orderDetail.Address.AddressLine1,
                        orderDetail.Address.City,
                        orderDetail.Address.State,
                        orderDetail.Address.Pincode,
                        FullAddress = $"{orderDetail.Address.AddressLine1}, {orderDetail.Address.City}, {orderDetail.Address.State} - {orderDetail.Address.Pincode}"
                    } : null,
                    orderDetail.Quantity,
                    orderDetail.Amount,
                    orderDetail.Discount,
                    orderDetail.NetAmount,
                    orderDetail.Created,
                    orderDetail.Modified
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting order detail", error = ex.Message });
            }
        }
        #endregion

        #region CREATE ORDER DETAIL - Customer Only (During Order Creation)
        [HttpPost]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> InsertOrderDetail(OrderDetailDTO orderDetail)
        {
            try
            {
                var currentUserId = User.FindFirst("UserID")?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return BadRequest(new { message = "Unable to identify current user" });
                }

                // FluentValidation validation
                var result = await _validator.ValidateAsync(orderDetail);
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage });
                    return BadRequest(new { message = "Validation failed", errors });
                }

                // Verify order belongs to current user
                var order = await _db.Orders.FindAsync(orderDetail.OrderID);
                if (order == null || order.UserID.ToString() != currentUserId)
                {
                    return BadRequest(new { message = "Invalid order or order doesn't belong to you" });
                }

                // Verify product exists and is active
                var product = await _db.Products.FindAsync(orderDetail.ProductID);
                if (product == null || !product.IsActive)
                {
                    return BadRequest(new { message = "Product not found or inactive" });
                }

                // Verify address belongs to current user
                var address = await _db.Addresses.FindAsync(orderDetail.AddressID);
                if (address == null || address.UserID.ToString() != currentUserId)
                {
                    return BadRequest(new { message = "Invalid address or address doesn't belong to you" });
                }

                var addorderDetail = new OrderDetail
                {
                    OrderID = orderDetail.OrderID,
                    ProductID = orderDetail.ProductID,
                    AddressID = orderDetail.AddressID,
                    Quantity = orderDetail.Quantity,
                    Amount = product.Price, // Use current product price
                    Discount = orderDetail.Discount,
                    NetAmount = (product.Price * orderDetail.Quantity) - orderDetail.Discount,
                    Created = DateTime.Now,
                    Modified = DateTime.Now
                };

                _db.OrderDetails.Add(addorderDetail);
                await _db.SaveChangesAsync();

                return Created("", new
                {
                    message = "Order detail created successfully",
                    orderDetailId = addorderDetail.OrderDetailID,
                    productName = product.ProductName,
                    quantity = addorderDetail.Quantity,
                    amount = addorderDetail.Amount,
                    netAmount = addorderDetail.NetAmount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating order detail", error = ex.Message });
            }
        }
        #endregion

        #region UPDATE ORDER DETAIL - Admin Only
        [HttpPut("{OrderDetailId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateOrderDetail(int OrderDetailId, OrderDetailDTO orderDetail)
        {
            if (OrderDetailId != orderDetail.OrderDetailID)
            {
                return BadRequest("Id Mismatch");
            }

            if (OrderDetailId == 0)
            {
                return BadRequest("Invalid Id");
            }

            try
            {
                // validation
                var result = await _validator.ValidateAsync(orderDetail);
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage });
                    return BadRequest(new { message = "Validation failed", errors });
                }

                var updateorderDetail = await _db.OrderDetails.FindAsync(OrderDetailId);
                if (updateorderDetail == null)
                    return NotFound(new { message = "Order detail not found" });

                // Admin can update quantities, discounts, and amounts
                updateorderDetail.Quantity = orderDetail.Quantity;
                updateorderDetail.Amount = orderDetail.Amount;
                updateorderDetail.Discount = orderDetail.Discount;
                updateorderDetail.NetAmount = orderDetail.NetAmount;
                updateorderDetail.Modified = DateTime.Now;

                await _db.SaveChangesAsync();

                return Ok(new
                {
                    message = "Order detail updated successfully",
                    orderDetailId = updateorderDetail.OrderDetailID
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating order detail", error = ex.Message });
            }
        }
        #endregion

        #region DELETE ORDER DETAIL - Admin Only
        [HttpDelete("{OrderDetailId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteOrderDetail(int OrderDetailId)
        {
            try
            {
                var orderDetail = await _db.OrderDetails
                    .Include(od => od.Product)
                    .FirstOrDefaultAsync(od => od.OrderDetailID == OrderDetailId);

                if (orderDetail == null)
                    return NotFound(new { message = "Order detail not found" });

                _db.OrderDetails.Remove(orderDetail);
                await _db.SaveChangesAsync();

                return Ok(new
                {
                    message = "Order detail deleted successfully",
                    productName = orderDetail.Product.ProductName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting order detail", error = ex.Message });
            }
        }
        #endregion
    }
}
