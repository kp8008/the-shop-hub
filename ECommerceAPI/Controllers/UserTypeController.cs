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
    [Authorize(Policy = "AdminOnly")] // All operations Admin only
    public class UserTypeController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IValidator<UserTypeDTO> _validator;

        public UserTypeController(ApplicationDbContext db, IValidator<UserTypeDTO> validator)
        {
            _db = db;
            _validator = validator;
        }

        #region GET ALL USER TYPES - Admin Only
        [HttpGet]
        public async Task<IActionResult> GetAllUserTypes()
        {
            try
            {
                var userTypes = await _db.UserTypes
                    .Select(ut => new
                    {
                        ut.UserTypeID,
                        ut.UserTypeName,
                        UserCount = _db.Users.Count(u => u.UserTypeID == ut.UserTypeID)
                    })
                    .ToListAsync();
                
                return Ok(userTypes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting user types", error = ex.Message });
            }
        }
        #endregion

        #region GET USER TYPE BY ID - Admin Only
        [HttpGet("{UserTypeId}")]
        public async Task<IActionResult> GetByIdUserTypes(int UserTypeId)
        {
            try
            {
                var userType = await _db.UserTypes
                    .Where(ut => ut.UserTypeID == UserTypeId)
                    .Select(ut => new
                    {
                        ut.UserTypeID,
                        ut.UserTypeName,
                        UserCount = _db.Users.Count(u => u.UserTypeID == ut.UserTypeID)
                    })
                    .FirstOrDefaultAsync();

                if (userType == null)
                    return NotFound(new { message = "User type not found" });

                return Ok(userType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting user type", error = ex.Message });
            }
        }
        #endregion

        #region CREATE USER TYPE - Admin Only
        [HttpPost]
        public async Task<IActionResult> InsertUserType(UserTypeDTO userType)
        {
            try
            {
                // FluentValidation validation
                var result = await _validator.ValidateAsync(userType);
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage });
                    return BadRequest(new { message = "Validation failed", errors });
                }

                // Check for duplicate user type name
                bool exists = await _db.UserTypes.AnyAsync(ut => ut.UserTypeName.ToLower() == userType.UserTypeName.ToLower());
                if (exists)
                {
                    return BadRequest(new { message = "User type with this name already exists" });
                }

                var adduserType = new UserType
                {
                    UserTypeName = userType.UserTypeName,
                    Created = DateTime.Now,
                    Modified = DateTime.Now
                };

                _db.UserTypes.Add(adduserType);
                await _db.SaveChangesAsync();

                return Created("", new
                {
                    message = "User type created successfully",
                    userTypeId = adduserType.UserTypeID,
                    userTypeName = adduserType.UserTypeName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating user type", error = ex.Message });
            }
        }
        #endregion

        #region UPDATE USER TYPE - Admin Only
        [HttpPut("{UserTypeId}")]
        public async Task<IActionResult> UpdateUserType(int UserTypeId, UserTypeDTO userType)
        {
            if (UserTypeId != userType.UserTypeID)
            {
                return BadRequest("Id Mismatch");
            }

            if (UserTypeId == 0)
            {
                return BadRequest("Invalid Id");
            }

            try
            {
                // validation
                var result = await _validator.ValidateAsync(userType);
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage });
                    return BadRequest(new { message = "Validation failed", errors });
                }

                var updateuserType = await _db.UserTypes.FindAsync(UserTypeId);
                if (updateuserType == null)
                    return NotFound(new { message = "User type not found" });

                // Check for duplicate user type name (excluding current)
                bool exists = await _db.UserTypes
                    .AnyAsync(ut => ut.UserTypeName.ToLower() == userType.UserTypeName.ToLower() && ut.UserTypeID != UserTypeId);
                if (exists)
                {
                    return BadRequest(new { message = "User type with this name already exists" });
                }

                updateuserType.UserTypeName = userType.UserTypeName;
                updateuserType.Modified = DateTime.Now;

                await _db.SaveChangesAsync();

                return Ok(new
                {
                    message = "User type updated successfully",
                    userTypeId = updateuserType.UserTypeID,
                    userTypeName = updateuserType.UserTypeName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating user type", error = ex.Message });
            }
        }
        #endregion

        #region DELETE USER TYPE - Admin Only
        [HttpDelete("{UserTypeId}")]
        public async Task<IActionResult> DeleteUserType(int UserTypeId)
        {
            try
            {
                var userType = await _db.UserTypes.FindAsync(UserTypeId);
                if (userType == null)
                    return NotFound(new { message = "User type not found" });

                // Check if user type is being used by any users
                bool hasUsers = await _db.Users.AnyAsync(u => u.UserTypeID == UserTypeId);
                if (hasUsers)
                {
                    return BadRequest(new { message = "Cannot delete user type. It is being used by users." });
                }

                // Prevent deletion of core user types (Admin and Customer)
                if (userType.UserTypeName.ToLower() == "admin" || userType.UserTypeName.ToLower() == "customer")
                {
                    return BadRequest(new { message = "Cannot delete core user types (Admin/Customer)" });
                }

                _db.UserTypes.Remove(userType);
                await _db.SaveChangesAsync();

                return Ok(new { message = "User type deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting user type", error = ex.Message });
            }
        }
        #endregion
    }
}
