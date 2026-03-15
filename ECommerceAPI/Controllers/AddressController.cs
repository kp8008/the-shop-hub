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
    public class AddressController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IValidator<AddressDTO> _validator;

        public AddressController(ApplicationDbContext db, IValidator<AddressDTO> validator)
        {
            _db = db;
            _validator = validator;
        }

        #region GET ALL ADDRESSES - Admin Only
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllAddresses()
        {
            try
            {
                var addresses = await _db.Addresses
                    .Include(a => a.User)
                    .Select(a => new
                    {
                        a.AddressID,
                        a.UserID,
                        UserName = a.User.UserName,
                        a.ReceiverName,
                        a.Phone,
                        a.AddressLine1,
                        a.Landmark,
                        a.City,
                        a.State,
                        a.Country,
                        a.Pincode,
                        a.IsDefault,
                        a.Created
                    })
                    .ToListAsync();
                
                return Ok(addresses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting addresses", error = ex.Message });
            }
        }
        #endregion

        #region GET ADDRESS BY ID - Admin or Own Address
        [HttpGet("{AddressId:int}")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<IActionResult> GetByIdAddresses(int AddressId)
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

                var address = await _db.Addresses
                    .Include(a => a.User)
                    .Where(a => a.AddressID == AddressId)
                    .Select(a => new
                    {
                        a.AddressID,
                        a.UserID,
                        UserName = a.User.UserName,
                        a.ReceiverName,
                        a.Phone,
                        a.AddressLine1,
                        a.Landmark,
                        a.City,
                        a.State,
                        a.Country,
                        a.Pincode,
                        a.IsDefault,
                        a.Created
                    })
                    .FirstOrDefaultAsync();

                if (address == null)
                    return NotFound(new { message = "Address not found" });

                // Check ownership - only admin or address owner can view
                if (currentUserRole != Roles.Admin && address.UserID != int.Parse(currentUserId))
                {
                    return Forbid("You can only view your own addresses");
                }

                return Ok(address);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting address", error = ex.Message });
            }
        }
        #endregion

        #region GET USER ADDRESSES - Customer Only (Own Addresses)
        [HttpGet("MyAddresses")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> GetMyAddresses()
        {
            try
            {
                var currentUserId = User.FindFirst("UserID")?.Value;
                if (!int.TryParse(currentUserId, out var userId))
                {
                    // If user id cannot be resolved, return empty list instead of 400
                    return Ok(Array.Empty<object>());
                }

                var addresses = await _db.Addresses
                    .Where(a => a.UserID == userId)
                    .OrderByDescending(a => a.Created)
                    .Take(2)
                    .Select(a => new
                    {
                        a.AddressID,
                        a.UserID,
                        a.ReceiverName,
                        a.Phone,
                        a.AddressLine1,
                        a.Landmark,
                        a.City,
                        a.State,
                        a.Country,
                        a.Pincode,
                        a.IsDefault,
                        a.Created
                    })
                    .ToListAsync();

                return Ok(addresses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting your addresses", error = ex.Message });
            }
        }
        #endregion

        #region CREATE ADDRESS - Customer (own) or Admin
        [HttpPost]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<IActionResult> InsertAddress(AddressDTO address)
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

                // FluentValidation validation
                var result = await _validator.ValidateAsync(address);
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage });
                    return BadRequest(new { message = "Validation failed", errors });
                }

                // For customers, force UserID to be their own ID
                if (currentUserRole == Roles.Customer)
                {
                    address.UserID = int.Parse(currentUserId);
                }
                // For admin, verify the target user exists
                else if (currentUserRole == Roles.Admin)
                {
                    var userExists = await _db.Users.AnyAsync(u => u.UserID == address.UserID);
                    if (!userExists)
                    {
                        return BadRequest(new { message = "Target user not found" });
                    }
                }

                var addaddress = new Address
                {
                    UserID = address.UserID,
                    ReceiverName = address.ReceiverName,
                    Phone = address.Phone,
                    AddressLine1 = address.AddressLine1,
                    Landmark = address.Landmark,
                    City = address.City,
                    State = address.State,
                    Country = address.Country,
                    Pincode = address.Pincode,
                    IsDefault = address.IsDefault,
                    Created = DateTime.Now,
                    Modified = DateTime.Now
                };

                // If this is set as default, unset other default addresses for this user
                if (address.IsDefault)
                {
                    var existingDefaults = await _db.Addresses
                        .Where(a => a.UserID == address.UserID && a.IsDefault)
                        .ToListAsync();
                    
                    foreach (var existing in existingDefaults)
                    {
                        existing.IsDefault = false;
                    }
                }

                _db.Addresses.Add(addaddress);
                await _db.SaveChangesAsync();

                return Created("", new
                {
                    message = "Address created successfully",
                    addressId = addaddress.AddressID,
                    receiverName = addaddress.ReceiverName,
                    city = addaddress.City,
                    isDefault = addaddress.IsDefault
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating address", error = ex.Message });
            }
        }
        #endregion

        #region UPDATE ADDRESS - Admin or Own Address
        [HttpPut("{AddressId:int}")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<IActionResult> UpdateAddress(int AddressId, AddressDTO address)
        {
            if (AddressId != address.AddressID)
            {
                return BadRequest("Id Mismatch");
            }

            if (AddressId == 0)
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
                var result = await _validator.ValidateAsync(address);
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage });
                    return BadRequest(new { message = "Validation failed", errors });
                }

                var updateaddress = await _db.Addresses.FindAsync(AddressId);
                if (updateaddress == null)
                    return NotFound(new { message = "Address not found" });

                // Check ownership - only admin or address owner can update
                if (currentUserRole != Roles.Admin && updateaddress.UserID != int.Parse(currentUserId))
                {
                    return Forbid("You can only update your own addresses");
                }

                // If this is set as default, unset other default addresses for this user
                if (address.IsDefault && !updateaddress.IsDefault)
                {
                    var existingDefaults = await _db.Addresses
                        .Where(a => a.UserID == updateaddress.UserID && a.IsDefault && a.AddressID != AddressId)
                        .ToListAsync();
                    
                    foreach (var existing in existingDefaults)
                    {
                        existing.IsDefault = false;
                    }
                }

                updateaddress.ReceiverName = address.ReceiverName;
                updateaddress.Phone = address.Phone;
                updateaddress.AddressLine1 = address.AddressLine1;
                updateaddress.Landmark = address.Landmark;
                updateaddress.City = address.City;
                updateaddress.State = address.State;
                updateaddress.Country = address.Country;
                updateaddress.Pincode = address.Pincode;
                updateaddress.IsDefault = address.IsDefault;
                updateaddress.Modified = DateTime.Now;

                await _db.SaveChangesAsync();

                return Ok(new
                {
                    message = "Address updated successfully",
                    addressId = updateaddress.AddressID,
                    receiverName = updateaddress.ReceiverName,
                    city = updateaddress.City,
                    isDefault = updateaddress.IsDefault
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating address", error = ex.Message });
            }
        }
        #endregion

        #region DELETE ADDRESS - Admin or Own Address
        [HttpDelete("{AddressId:int}")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<IActionResult> DeleteAddress(int AddressId)
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

                var address = await _db.Addresses.FindAsync(AddressId);
                if (address == null)
                    return NotFound(new { message = "Address not found" });

                // Check ownership - only admin or address owner can delete
                if (currentUserRole != Roles.Admin && address.UserID != int.Parse(currentUserId))
                {
                    return Forbid("You can only delete your own addresses");
                }

                // Check if address is being used in any orders
                bool hasOrders = await _db.Orders.AnyAsync(o => o.AddressID == AddressId);
                if (hasOrders)
                {
                    return BadRequest(new { message = "Cannot delete address. It is being used in orders." });
                }

                _db.Addresses.Remove(address);
                await _db.SaveChangesAsync();

                return Ok(new { message = "Address deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting address", error = ex.Message });
            }
        }
        #endregion
    }
}
