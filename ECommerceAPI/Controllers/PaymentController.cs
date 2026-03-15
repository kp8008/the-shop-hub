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
    public class PaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IValidator<PaymentDTO> _validator;

        public PaymentController(ApplicationDbContext db, IValidator<PaymentDTO> validator)
        {
            _db = db;
            _validator = validator;
        }

        #region GET ALL PAYMENTS - Admin Only
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllPayments()
        {
            try
            {
                var payments = await _db.Payments
                    .Include(p => p.Order)
                        .ThenInclude(o => o.User)
                    .Include(p => p.PaymentMode)
                    .Select(p => new
                    {
                        p.PaymentID,
                        p.OrderID,
                        OrderNo = p.Order.OrderNo,
                        UserName = p.Order.User.UserName,
                        UserEmail = p.Order.User.Email,
                        p.PaymentModeID,
                        PaymentModeName = p.PaymentMode.PaymentModeName,
                        p.TotalPayment,
                        p.PaymentReference,
                        p.PaymentStatus,
                        p.TransactionID,
                        p.TransactionDate,
                        p.Created,
                        p.Modified
                    })
                    .OrderByDescending(p => p.Created)
                    .ToListAsync();
                
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting payments", error = ex.Message });
            }
        }
        #endregion

        #region GET USER PAYMENTS - Customer Only (Own Payments)
        [HttpGet("MyPayments")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> GetMyPayments()
        {
            try
            {
                var currentUserId = User.FindFirst("UserID")?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return BadRequest(new { message = "Unable to identify current user" });
                }

                int userId = int.Parse(currentUserId);

                var payments = await _db.Payments
                    .Include(p => p.Order)
                    .Include(p => p.PaymentMode)
                    .Where(p => p.Order.UserID == userId)
                    .Select(p => new
                    {
                        p.PaymentID,
                        p.OrderID,
                        OrderNo = p.Order.OrderNo,
                        p.PaymentModeID,
                        PaymentModeName = p.PaymentMode.PaymentModeName,
                        p.TotalPayment,
                        p.PaymentReference,
                        p.PaymentStatus,
                        p.TransactionID,
                        p.TransactionDate,
                        p.Created,
                        p.Modified
                    })
                    .OrderByDescending(p => p.Created)
                    .ToListAsync();

                return Ok(payments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting your payments", error = ex.Message });
            }
        }
        #endregion

        #region GET PAYMENT BY ID - Admin or Own Payment
        [HttpGet("{PaymentId}")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<IActionResult> GetByIdPayments(int PaymentId)
        {
            try
            {
                var currentUserId = User.FindFirst("UserID")?.Value;
                var currentUserRole = User.FindFirst("UserTypeName")?.Value;

                var payment = await _db.Payments
                    .Include(p => p.Order)
                        .ThenInclude(o => o.User)
                    .Include(p => p.PaymentMode)
                    .FirstOrDefaultAsync(p => p.PaymentID == PaymentId);

                if (payment == null)
                    return NotFound(new { message = "Payment not found" });

                // Customer can only access their own payments
                if (currentUserRole != Roles.Admin && payment.Order.UserID.ToString() != currentUserId)
                {
                    return Forbid("You can only access your own payments");
                }

                var response = new
                {
                    payment.PaymentID,
                    payment.OrderID,
                    OrderNo = payment.Order.OrderNo,
                    UserName = payment.Order.User.UserName,
                    UserEmail = payment.Order.User.Email,
                    payment.PaymentModeID,
                    PaymentModeName = payment.PaymentMode.PaymentModeName,
                    payment.TotalPayment,
                    payment.PaymentReference,
                    payment.PaymentStatus,
                    payment.TransactionID,
                    payment.TransactionDate,
                    payment.Created,
                    payment.Modified
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting payment", error = ex.Message });
            }
        }
        #endregion

        #region CREATE PAYMENT - Customer Only (During Checkout)
        [HttpPost]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> InsertPayment(PaymentDTO payment)
        {
            try
            {
                var currentUserId = User.FindFirst("UserID")?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return BadRequest(new { message = "Unable to identify current user" });
                }

                // FluentValidation validation
                var result = await _validator.ValidateAsync(payment);
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage });
                    return BadRequest(new { message = "Validation failed", errors });
                }

                // Verify order belongs to current user
                var order = await _db.Orders.FindAsync(payment.OrderID);
                if (order == null || order.UserID.ToString() != currentUserId)
                {
                    return BadRequest(new { message = "Invalid order or order doesn't belong to you" });
                }

                // Verify payment mode exists
                var paymentMode = await _db.PaymentModes.FindAsync(payment.PaymentModeID);
                if (paymentMode == null)
                {
                    return BadRequest(new { message = "Invalid payment mode" });
                }

                // Generate transaction ID if not provided
                var transactionId = payment.TransactionID ?? $"TXN{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";

                var addpayment = new Payment
                {
                    OrderID = payment.OrderID,
                    PaymentModeID = payment.PaymentModeID,
                    TotalPayment = payment.TotalPayment,
                    PaymentReference = payment.PaymentReference,
                    PaymentStatus = payment.PaymentStatus ?? "Pending",
                    TransactionID = transactionId,
                    TransactionDate = DateTime.Now,
                    Created = DateTime.Now,
                    Modified = DateTime.Now
                };

                _db.Payments.Add(addpayment);
                await _db.SaveChangesAsync();

                return Created("", new
                {
                    message = "Payment created successfully",
                    paymentId = addpayment.PaymentID,
                    transactionId = addpayment.TransactionID,
                    totalPayment = addpayment.TotalPayment,
                    paymentStatus = addpayment.PaymentStatus
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating payment", error = ex.Message });
            }
        }
        #endregion

        #region UPDATE PAYMENT - Admin Only
        [HttpPut("{PaymentId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdatePayment(int PaymentId, PaymentDTO payment)
        {
            if (PaymentId != payment.PaymentID)
            {
                return BadRequest("Id Mismatch");
            }

            if (PaymentId == 0)
            {
                return BadRequest("Invalid Id");
            }

            try
            {
                // validation
                var result = await _validator.ValidateAsync(payment);
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage });
                    return BadRequest(new { message = "Validation failed", errors });
                }

                var updatepayment = await _db.Payments.FindAsync(PaymentId);
                if (updatepayment == null)
                    return NotFound(new { message = "Payment not found" });

                // Admin can update payment status and reference
                updatepayment.PaymentModeID = payment.PaymentModeID;
                updatepayment.TotalPayment = payment.TotalPayment;
                updatepayment.PaymentReference = payment.PaymentReference;
                updatepayment.PaymentStatus = payment.PaymentStatus ?? updatepayment.PaymentStatus;
                updatepayment.TransactionID = payment.TransactionID ?? updatepayment.TransactionID;
                updatepayment.Modified = DateTime.Now;

                await _db.SaveChangesAsync();

                return Ok(new
                {
                    message = "Payment updated successfully",
                    paymentId = updatepayment.PaymentID,
                    paymentStatus = updatepayment.PaymentStatus
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating payment", error = ex.Message });
            }
        }
        #endregion

        #region DELETE PAYMENT - Admin Only
        [HttpDelete("{PaymentId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeletePayment(int PaymentId)
        {
            try
            {
                var payment = await _db.Payments
                    .Include(p => p.Order)
                    .FirstOrDefaultAsync(p => p.PaymentID == PaymentId);

                if (payment == null)
                    return NotFound(new { message = "Payment not found" });

                _db.Payments.Remove(payment);
                await _db.SaveChangesAsync();

                return Ok(new
                {
                    message = "Payment deleted successfully",
                    orderNo = payment.Order.OrderNo
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting payment", error = ex.Message });
            }
        }
        #endregion
    }
}
