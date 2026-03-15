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
    public class PaymentModeController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IValidator<PaymentModeDTO> _validator;

        public PaymentModeController(ApplicationDbContext db, IValidator<PaymentModeDTO> validator)
        {
            _db = db;
            _validator = validator;
        }

        #region GET ALL PAYMENT MODES - Public (for checkout)
        [HttpGet]
        [AllowAnonymous] // Allow public access for checkout options
        public async Task<IActionResult> GetAllPaymentModes()
        {
            try
            {
                var paymentModes = await _db.PaymentModes
                    .Select(pm => new
                    {
                        pm.PaymentModeID,
                        pm.PaymentModeName
                    })
                    .ToListAsync();
                
                return Ok(paymentModes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting payment modes", error = ex.Message });
            }
        }
        #endregion

        #region GET PAYMENT MODE BY ID - Public
        [HttpGet("{PaymentModeId}")]
        [AllowAnonymous] // Allow public access for checkout
        public async Task<IActionResult> GetByIdPaymentModes(int PaymentModeId)
        {
            try
            {
                var paymentMode = await _db.PaymentModes
                    .Where(pm => pm.PaymentModeID == PaymentModeId)
                    .Select(pm => new
                    {
                        pm.PaymentModeID,
                        pm.PaymentModeName
                    })
                    .FirstOrDefaultAsync();

                if (paymentMode == null)
                    return NotFound(new { message = "Payment mode not found" });

                return Ok(paymentMode);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting payment mode", error = ex.Message });
            }
        }
        #endregion

        #region CREATE PAYMENT MODE - Admin Only
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> InsertPaymentMode(PaymentModeDTO paymentMode)
        {
            try
            {
                // FluentValidation validation
                var result = await _validator.ValidateAsync(paymentMode);
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage });
                    return BadRequest(new { message = "Validation failed", errors });
                }

                // Check for duplicate payment mode name
                bool exists = await _db.PaymentModes.AnyAsync(pm => pm.PaymentModeName.ToLower() == paymentMode.PaymentModeName.ToLower());
                if (exists)
                {
                    return BadRequest(new { message = "Payment mode with this name already exists" });
                }

                var addpaymentMode = new PaymentMode
                {
                    PaymentModeName = paymentMode.PaymentModeName
                };

                _db.PaymentModes.Add(addpaymentMode);
                await _db.SaveChangesAsync();

                return Created("", new
                {
                    message = "Payment mode created successfully",
                    paymentModeId = addpaymentMode.PaymentModeID,
                    paymentModeName = addpaymentMode.PaymentModeName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating payment mode", error = ex.Message });
            }
        }
        #endregion

        #region UPDATE PAYMENT MODE - Admin Only
        [HttpPut("{PaymentModeId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdatePaymentMode(int PaymentModeId, PaymentModeDTO paymentMode)
        {
            if (PaymentModeId != paymentMode.PaymentModeID)
            {
                return BadRequest("Id Mismatch");
            }

            if (PaymentModeId == 0)
            {
                return BadRequest("Invalid Id");
            }

            try
            {
                // validation
                var result = await _validator.ValidateAsync(paymentMode);
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage });
                    return BadRequest(new { message = "Validation failed", errors });
                }

                var updatepaymentMode = await _db.PaymentModes.FindAsync(PaymentModeId);
                if (updatepaymentMode == null)
                    return NotFound(new { message = "Payment mode not found" });

                // Check for duplicate payment mode name (excluding current)
                bool exists = await _db.PaymentModes
                    .AnyAsync(pm => pm.PaymentModeName.ToLower() == paymentMode.PaymentModeName.ToLower() && pm.PaymentModeID != PaymentModeId);
                if (exists)
                {
                    return BadRequest(new { message = "Payment mode with this name already exists" });
                }

                updatepaymentMode.PaymentModeName = paymentMode.PaymentModeName;

                await _db.SaveChangesAsync();

                return Ok(new
                {
                    message = "Payment mode updated successfully",
                    paymentModeId = updatepaymentMode.PaymentModeID,
                    paymentModeName = updatepaymentMode.PaymentModeName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating payment mode", error = ex.Message });
            }
        }
        #endregion

        #region DELETE PAYMENT MODE - Admin Only
        [HttpDelete("{PaymentModeId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeletePaymentMode(int PaymentModeId)
        {
            try
            {
                var paymentMode = await _db.PaymentModes.FindAsync(PaymentModeId);
                if (paymentMode == null)
                    return NotFound(new { message = "Payment mode not found" });

                // Check if payment mode is being used in any payments
                bool hasPayments = await _db.Payments.AnyAsync(p => p.PaymentModeID == PaymentModeId);
                if (hasPayments)
                {
                    return BadRequest(new { message = "Cannot delete payment mode. It is being used in payments." });
                }

                _db.PaymentModes.Remove(paymentMode);
                await _db.SaveChangesAsync();

                return Ok(new { message = "Payment mode deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting payment mode", error = ex.Message });
            }
        }
        #endregion
    }
}
