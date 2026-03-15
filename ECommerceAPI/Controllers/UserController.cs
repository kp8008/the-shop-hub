using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceAPI.Models;
using ECommerceAPI.Services;
using ECommerceAPI.Constants;
using System;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Require authentication for all actions
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IValidator<UserDTO> _validator;

        public UserController(ApplicationDbContext db, IValidator<UserDTO> validator)
        {
            _db = db;
            _validator = validator;
        }

        #region GET ALL USERS - Admin Only
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _db.Users
                    .Include(u => u.UserType)
                    .Select(u => new
                    {
                        u.UserID,
                        u.UserName,
                        u.Email,
                        u.Phone,
                        u.Address,
                        u.UserTypeID,
                        UserTypeName = u.UserType.UserTypeName,
                        u.IsActive,
                        u.Created,
                        u.Modified
                    })
                    .ToListAsync();
                
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting users", error = ex.Message });
            }
        }
        #endregion

        #region GET USER BY ID - Admin or Own Profile
        [HttpGet("{UserId}")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<IActionResult> GetByIdUsers(int UserId)
        {
            try
            {
                // Get current user's ID and role from JWT token
                var currentUserId = User.FindFirst("UserID")?.Value;
                var currentUserRole = User.FindFirst("UserTypeName")?.Value;

                // Admin can view any user, Customer can only view their own profile
                if (currentUserRole != Roles.Admin && currentUserId != UserId.ToString())
                {
                    return Forbid("You can only access your own profile");
                }

                var user = await _db.Users
                    .Include(u => u.UserType)
                    .Where(u => u.UserID == UserId)
                    .Select(u => new
                    {
                        u.UserID,
                        u.UserName,
                        u.Email,
                        u.Phone,
                        u.Address,
                        u.UserTypeID,
                        UserTypeName = u.UserType.UserTypeName,
                        u.IsActive,
                        u.Created,
                        u.Modified
                    })
                    .FirstOrDefaultAsync();

                if (user == null)
                    return NotFound(new { message = "User not found" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting user", error = ex.Message });
            }
        }
        #endregion

        #region INSERT USER - Admin Only (for creating users) or Public (for registration)
        [HttpPost]
        [AllowAnonymous] // Allow public registration
        public async Task<IActionResult> InsertUser(UserDTO user)
        {
            try
            {
                // FluentValidation validation
                var result = await _validator.ValidateAsync(user);
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage });
                    return BadRequest(new { message = "Validation failed", errors });
                }

                // Check if user is authenticated (Admin creating user) or anonymous (self-registration)
                var currentUserRole = User.FindFirst("UserTypeName")?.Value;
                
                // If user is not authenticated, default to Customer registration
                if (string.IsNullOrEmpty(currentUserRole))
                {
                    user.UserTypeID = 2; // Force Customer role for public registration
                    user.IsActive = true; // Auto-activate customer accounts
                }
                // If Admin is creating user, allow any UserTypeID
                else if (currentUserRole != Roles.Admin)
                {
                    return Forbid("Only admins can create users with specific roles");
                }

                // Check duplicate email
                bool emailExists = await _db.Users.AnyAsync(u => u.Email == user.Email);
                if (emailExists)
                    return BadRequest(new { message = "Email already exists" });

                // Check duplicate username
                bool userExists = await _db.Users.AnyAsync(u => u.UserName == user.UserName);
                if (userExists)
                    return BadRequest(new { message = "Username already exists" });

                // Hash password before saving
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password ?? "DefaultPassword123");

                var adduser = new User
                {
                    UserName = user.UserName,
                    Address = user.Address,
                    Phone = user.Phone,
                    Email = user.Email,
                    Password = hashedPassword,
                    UserTypeID = user.UserTypeID,
                    IsActive = user.IsActive,
                    Created = DateTime.Now,
                    Modified = DateTime.Now
                };

                _db.Users.Add(adduser);
                await _db.SaveChangesAsync();

                // Don't return password in response
                var response = new
                {
                    adduser.UserID,
                    adduser.UserName,
                    adduser.Email,
                    adduser.Phone,
                    adduser.Address,
                    adduser.UserTypeID,
                    adduser.IsActive,
                    message = "User created successfully"
                };

                return Created("", response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating user", error = ex.Message });
            }
        }
        #endregion

        #region UPDATE USER - Admin or Own Profile
        [HttpPut("{UserId}")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<IActionResult> UpdateUser(int UserId, UserDTO user)
        {
            if (UserId != user.UserID)
                return BadRequest("Id Mismatch");

            if (UserId == 0)
                return BadRequest("Invalid Id");

            try
            {
                // Get current user's ID and role from JWT token
                var currentUserId = User.FindFirst("UserID")?.Value;
                var currentUserRole = User.FindFirst("UserTypeName")?.Value;

                // Admin can update any user, Customer can only update their own profile
                if (currentUserRole != Roles.Admin && currentUserId != UserId.ToString())
                {
                    return Forbid("You can only update your own profile");
                }

                // validation
                var result = await _validator.ValidateAsync(user);
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage });
                    return BadRequest(new { message = "Validation failed", errors });
                }

                var updateuser = await _db.Users.FindAsync(UserId);
                if (updateuser == null)
                    return NotFound(new { message = "User not found" });

                // Customers cannot change their UserTypeID
                if (currentUserRole != Roles.Admin && user.UserTypeID != updateuser.UserTypeID)
                {
                    return Forbid("You cannot change your user type");
                }

                updateuser.UserName = user.UserName;
                updateuser.Address = user.Address;
                updateuser.Phone = user.Phone;
                updateuser.Email = user.Email;
                
                // Only admin can change UserTypeID and IsActive
                if (currentUserRole == Roles.Admin)
                {
                    updateuser.UserTypeID = user.UserTypeID;
                    updateuser.IsActive = user.IsActive;
                }
                
                updateuser.Modified = DateTime.Now;

                await _db.SaveChangesAsync();

                // Return updated user without password
                var response = new
                {
                    updateuser.UserID,
                    updateuser.UserName,
                    updateuser.Email,
                    updateuser.Phone,
                    updateuser.Address,
                    updateuser.UserTypeID,
                    updateuser.IsActive,
                    message = "User updated successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating user", error = ex.Message });
            }
        }
        #endregion

        #region DELETE USER - Admin Only
        [HttpDelete("{UserId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteUser(int UserId)
        {
            try
            {
                var user = await _db.Users.FindAsync(UserId);
                if (user == null)
                    return NotFound(new { message = "User not found" });

                // Prevent admin from deleting themselves
                var currentUserId = User.FindFirst("UserID")?.Value;
                if (currentUserId == UserId.ToString())
                {
                    return BadRequest(new { message = "You cannot delete your own account" });
                }

                _db.Users.Remove(user);
                await _db.SaveChangesAsync();

                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting user", error = ex.Message });
            }
        }
        #endregion
    }
}
