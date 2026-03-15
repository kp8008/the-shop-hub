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
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IValidator<OrderDTO> _validator;
        private const string PlaceholderImage = "https://via.placeholder.com/300x300.png?text=No+Image";

        public OrderController(ApplicationDbContext db, IValidator<OrderDTO> validator)
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

        #region GET ALL ORDERS - Admin Only
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _db.Orders
                    .Include(o => o.User)
                    .Include(o => o.Address)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Product)
                    .Select(o => new
                    {
                        o.OrderID,
                        o.OrderNo,
                        o.UserID,
                        UserName = o.User.UserName,
                        UserEmail = o.User.Email,
                        o.OrderDate,
                        o.DeliveryDate,
                        o.Status,
                        UserPhone = o.User.Phone,
                        o.AddressID,
                        ShippingAddress = o.Address != null ? new
                        {
                            o.Address.ReceiverName,
                            o.Address.Phone,
                            o.Address.AddressLine1,
                            o.Address.City,
                            o.Address.State,
                            o.Address.Country,
                            o.Address.Pincode,
                            FullAddress = $"{o.Address.AddressLine1}, {o.Address.City}, {o.Address.State} - {o.Address.Pincode}"
                        } : null,
                        o.TotalAmount,
                        o.CouponDiscount,
                        o.NetAmount,
                        OrderItemsCount = o.OrderDetails.Count(),
                        TotalQuantity = o.OrderDetails.Sum(od => od.Quantity),
                        o.Created,
                        o.Modified
                    })
                    .OrderByDescending(o => o.Created)
                    .ToListAsync();
                
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting orders", error = ex.Message });
            }
        }
        #endregion

        #region GET USER ORDERS - Customer Only (Own Orders)
        [HttpGet("MyOrders")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> GetMyOrders()
        {
            try
            {
                var currentUserId = User.FindFirst("UserID")?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return BadRequest(new { message = "Unable to identify current user" });
                }

                int userId = int.Parse(currentUserId);

                var userOrders = await _db.Orders
                    .Where(o => o.UserID == userId)
                    .Include(o => o.Address)
                    .Include(o => o.Payment)
                        .ThenInclude(p => p.PaymentMode)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Product)
                            .ThenInclude(p => p.Category)
                    .OrderByDescending(o => o.Created)
                    .ToListAsync();

                var response = userOrders.Select(o => new
                {
                    o.OrderID,
                    o.OrderNo,
                    o.OrderDate,
                    o.DeliveryDate,
                    o.Status,
                    ShippingAddress = o.Address != null ? new
                    {
                        o.Address.ReceiverName,
                        o.Address.Phone,
                        FullAddress = $"{o.Address.AddressLine1}, {o.Address.City}, {o.Address.State} - {o.Address.Pincode}"
                    } : null,
                    o.TotalAmount,
                    o.CouponDiscount,
                    o.NetAmount,
                    Payment = o.Payment != null ? new
                    {
                        PaymentModeName = o.Payment.PaymentMode?.PaymentModeName ?? "N/A",
                        PaymentReference = o.Payment.PaymentReference,
                        PaymentStatus = o.Payment.PaymentStatus,
                        TotalPayment = o.Payment.TotalPayment,
                        TransactionID = o.Payment.TransactionID
                    } : (object)null,
                    OrderItems = o.OrderDetails.Select(od => new
                    {
                        od.OrderDetailID,
                        od.ProductID,
                        ProductName = od.Product != null ? od.Product.ProductName : "Unknown Product",
                        ProductCode = od.Product != null ? od.Product.ProductCode : string.Empty,
                        ProductImage = BuildImageUrl(od.Product != null ? od.Product.Image : null),
                        CategoryName = od.Product != null && od.Product.Category != null ? od.Product.Category.CategoryName : string.Empty,
                        od.Quantity,
                        od.Amount,
                        od.Discount,
                        od.NetAmount
                    }).ToList(),
                    OrderItemsCount = o.OrderDetails.Count(),
                    TotalQuantity = o.OrderDetails.Sum(od => od.Quantity),
                    o.Created,
                    o.Modified
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting your orders", error = ex.Message });
            }
        }
        #endregion

        #region GET ORDER BY ID - Admin or Own Order
        [HttpGet("{OrderId}")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<IActionResult> GetByIdOrders(int OrderId)
        {
            try
            {
                var currentUserId = User.FindFirst("UserID")?.Value;
                var currentUserRole = User.FindFirst("UserTypeName")?.Value;

                var order = await _db.Orders
                    .Include(o => o.User)
                    .Include(o => o.Address)
                    .Include(o => o.Payment)
                        .ThenInclude(p => p.PaymentMode)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Product)
                            .ThenInclude(p => p.Category)
                    .FirstOrDefaultAsync(o => o.OrderID == OrderId);

                if (order == null)
                    return NotFound(new { message = "Order not found" });

                // Customer can only access their own orders
                if (currentUserRole != Roles.Admin && order.UserID.ToString() != currentUserId)
                {
                    return Forbid("You can only access your own orders");
                }

                // Load reviews by this order's customer for products in this order (for admin to see "review by user")
                var productIdsInOrder = order.OrderDetails.Select(od => od.ProductID).ToList();
                var reviewsByUser = await _db.ProductReviews
                    .Where(pr => pr.UserID == order.UserID && productIdsInOrder.Contains(pr.ProductID))
                    .Select(pr => new { pr.ProductID, pr.ReviewID, pr.Rating, pr.Title, pr.Comment, pr.Image, pr.Created })
                    .ToListAsync();

                var response = new
                {
                    order.OrderID,
                    order.OrderNo,
                    order.UserID,
                    UserName = order.User.UserName,
                    UserEmail = order.User.Email,
                    order.OrderDate,
                    order.DeliveryDate,
                    order.Status,
                    ShippingAddress = order.Address != null ? new
                    {
                        order.Address.ReceiverName,
                        order.Address.Phone,
                        order.Address.AddressLine1,
                        order.Address.Landmark,
                        order.Address.City,
                        order.Address.State,
                        order.Address.Country,
                        order.Address.Pincode,
                        FullAddress = $"{order.Address.AddressLine1}, {order.Address.City}, {order.Address.State} - {order.Address.Pincode}"
                    } : null,
                    order.TotalAmount,
                    order.CouponDiscount,
                    order.NetAmount,
                    Payment = order.Payment != null ? new
                    {
                        PaymentModeName = order.Payment.PaymentMode?.PaymentModeName ?? "N/A",
                        PaymentReference = order.Payment.PaymentReference,
                        PaymentStatus = order.Payment.PaymentStatus,
                        TotalPayment = order.Payment.TotalPayment,
                        TransactionID = order.Payment.TransactionID
                    } : (object)null,
                    OrderItems = order.OrderDetails.Select(od =>
                    {
                        var review = reviewsByUser.FirstOrDefault(r => r.ProductID == od.ProductID);
                        return new
                        {
                            od.OrderDetailID,
                            od.ProductID,
                            ProductName = od.Product.ProductName,
                            ProductCode = od.Product.ProductCode,
                            ProductImage = BuildImageUrl(od.Product.Image),
                            CategoryName = od.Product.Category.CategoryName,
                            od.Quantity,
                            od.Amount,
                            od.Discount,
                            od.NetAmount,
                            Review = review != null ? new
                            {
                                review.ReviewID,
                                review.Rating,
                                review.Title,
                                review.Comment,
                                review.Image,
                                review.Created
                            } : (object)null
                        };
                    }).ToList(),
                    OrderItemsCount = order.OrderDetails.Count(),
                    TotalQuantity = order.OrderDetails.Sum(od => od.Quantity),
                    order.Created,
                    order.Modified
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting order", error = ex.Message });
            }
        }
        #endregion

        #region CREATE ORDER - Customer Only
        [HttpPost]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> InsertOrder(OrderDTO order)
        {
            try
            {
                var currentUserId = User.FindFirst("UserID")?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return BadRequest(new { message = "Unable to identify current user" });
                }

                // FluentValidation validation
                var result = await _validator.ValidateAsync(order);
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage });
                    return BadRequest(new { message = "Validation failed", errors });
                }

                // Verify address belongs to current user
                var address = await _db.Addresses.FindAsync(order.AddressID);
                if (address == null || address.UserID.ToString() != currentUserId)
                {
                    return BadRequest(new { message = "Invalid address or address doesn't belong to you" });
                }

                // Generate unique order number
                var orderNo = $"ORD{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(100, 999)}";

                var addorder = new Order
                {
                    UserID = int.Parse(currentUserId), // Use current user's ID
                    OrderNo = orderNo,
                    OrderDate = DateTime.Now,
                    DeliveryDate = order.DeliveryDate ?? DateTime.Now.AddDays(7), // Default 7 days delivery
                    AddressID = order.AddressID,
                    TotalAmount = order.TotalAmount,
                    CouponDiscount = order.CouponDiscount ?? 0,
                    NetAmount = order.NetAmount,
                    Status = "pending",
                    Created = DateTime.Now,
                    Modified = DateTime.Now
                };

                _db.Orders.Add(addorder);
                await _db.SaveChangesAsync();

                return Created("", new
                {
                    message = "Order created successfully",
                    orderId = addorder.OrderID,
                    orderNo = addorder.OrderNo,
                    totalAmount = addorder.TotalAmount,
                    netAmount = addorder.NetAmount,
                    orderDate = addorder.OrderDate,
                    estimatedDelivery = addorder.DeliveryDate
                });
            }
            catch (Exception ex)
            {
                var innerError = ex.InnerException?.Message;
                return StatusCode(500, new { message = "Error creating order", error = ex.Message, innerError });
            }
        }
        #endregion

        #region UPDATE ORDER - Admin Only
        [HttpPut("{OrderId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateOrder(int OrderId, OrderDTO order)
        {
            if (OrderId != order.OrderID)
            {
                return BadRequest("Id Mismatch");
            }

            if (OrderId == 0)
            {
                return BadRequest("Invalid Id");
            }

            try
            {
                // validation
                var result = await _validator.ValidateAsync(order);
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage });
                    return BadRequest(new { message = "Validation failed", errors });
                }

                var updateorder = await _db.Orders.FindAsync(OrderId);
                if (updateorder == null)
                    return NotFound(new { message = "Order not found" });

                // Admin can update delivery date and amounts
                updateorder.DeliveryDate = order.DeliveryDate ?? updateorder.DeliveryDate;
                updateorder.TotalAmount = order.TotalAmount;
                updateorder.CouponDiscount = order.CouponDiscount ?? updateorder.CouponDiscount;
                updateorder.NetAmount = order.NetAmount;
                if (!string.IsNullOrWhiteSpace(order.Status))
                {
                    updateorder.Status = order.Status;
                }
                updateorder.Modified = DateTime.Now;

                await _db.SaveChangesAsync();

                return Ok(new
                {
                    message = "Order updated successfully",
                    orderId = updateorder.OrderID,
                    orderNo = updateorder.OrderNo,
                    status = updateorder.Status
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating order", error = ex.Message });
            }
        }
        #endregion

        #region UPDATE ORDER STATUS - Admin Only
        [HttpPut("{OrderId}/Status")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateOrderStatus(int OrderId, [FromBody] UpdateOrderStatusDTO request)
        {
            if (OrderId == 0)
            {
                return BadRequest("Invalid Id");
            }

            if (string.IsNullOrWhiteSpace(request.Status))
            {
                return BadRequest(new { message = "Status is required" });
            }

            try
            {
                var updateorder = await _db.Orders.FindAsync(OrderId);
                if (updateorder == null)
                    return NotFound(new { message = "Order not found" });

                updateorder.Status = request.Status;
                updateorder.Modified = DateTime.Now;

                await _db.SaveChangesAsync();

                return Ok(new
                {
                    message = "Order status updated successfully",
                    orderId = updateorder.OrderID,
                    status = updateorder.Status
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating order status", error = ex.Message });
            }
        }
        #endregion

        #region CANCEL ORDER - Customer (Own Order, within time limit) or Admin
        [HttpDelete("{OrderId}")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<IActionResult> DeleteOrder(int OrderId)
        {
            try
            {
                var currentUserId = User.FindFirst("UserID")?.Value;
                var currentUserRole = User.FindFirst("UserTypeName")?.Value;

                var order = await _db.Orders
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.OrderID == OrderId);

                if (order == null)
                    return NotFound(new { message = "Order not found" });

                // Customer can only cancel their own orders
                if (currentUserRole != Roles.Admin && order.UserID.ToString() != currentUserId)
                {
                    return Forbid("You can only cancel your own orders");
                }

                // Customer can only cancel within 24 hours of order creation
                if (currentUserRole != Roles.Admin)
                {
                    var hoursSinceOrder = (DateTime.Now - order.Created).TotalHours;
                    if (hoursSinceOrder > 24)
                    {
                        return BadRequest(new { message = "Orders can only be cancelled within 24 hours of placement" });
                    }
                }

                // Remove order details first
                _db.OrderDetails.RemoveRange(order.OrderDetails);
                
                // Remove the order
                _db.Orders.Remove(order);
                await _db.SaveChangesAsync();

                return Ok(new
                {
                    message = "Order cancelled successfully",
                    orderNo = order.OrderNo
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error cancelling order", error = ex.Message });
            }
        }
        #endregion
    }
}
