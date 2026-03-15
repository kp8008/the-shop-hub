using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ECommerceAPI.Models;
using ECommerceAPI.Services;
using BCrypt.Net;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _cache;
        private const string OtpCacheKeyPrefix = "pwreset:";
        private static readonly TimeSpan OtpExpiry = TimeSpan.FromMinutes(10);

        public AuthController(ApplicationDbContext db, IConfiguration configuration, IEmailService emailService, IMemoryCache cache)
        {
            _db = db;
            _configuration = configuration;
            _emailService = emailService;
            _cache = cache;
        }

        #region LOGIN
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            try
            {
                // Find user by email
                var user = await _db.Users
                    .Include(u => u.UserType)
                    .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.IsActive == true);

                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                // Verify password
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password);
                
                if (!isPasswordValid)
                {
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                // Generate JWT Token
                var token = GenerateJwtToken(user);

                // Send thank-you email (fire and forget so login is not delayed)
                var customerName = user.UserName?.Trim() ?? user.Email?.Split('@')[0] ?? "there";
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _emailService.SendLoginThankYouAsync(user.Email, customerName);
                    }
                    catch { /* ignore email errors */ }
                });

                var response = new LoginResponseDTO
                {
                    UserID = user.UserID,
                    UserName = user.UserName,
                    Email = user.Email,
                    UserTypeID = user.UserTypeID,
                    UserTypeName = user.UserType?.UserTypeName ?? "",
                    Phone = user.Phone,
                    Token = token,
                    IsActive = user.IsActive
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error during login", error = ex.Message });
            }
        }
        #endregion

        #region PASSWORD RESET (OTP via Gmail)
        [HttpPost("request-password-reset-otp")]
        public async Task<IActionResult> RequestPasswordResetOtp([FromBody] RequestPasswordResetOtpDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { message = "Email is required" });

            var email = dto.Email.Trim().ToLowerInvariant();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email && u.IsActive == true);
            if (user == null)
                return NotFound(new { message = "No account found with this email" });

            var otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
            var cacheKey = OtpCacheKeyPrefix + email;
            _cache.Set(cacheKey, otp, OtpExpiry);

            try
            {
                await _emailService.SendPasswordResetOtpAsync(user.Email, otp);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to send OTP email. Check server email config.", error = ex.Message });
            }

            return Ok(new { message = "OTP sent to your email. Valid for 10 minutes." });
        }

        [HttpPost("verify-otp-reset-password")]
        public async Task<IActionResult> VerifyOtpAndResetPassword([FromBody] VerifyOtpResetPasswordDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Otp) || string.IsNullOrWhiteSpace(dto.NewPassword))
                return BadRequest(new { message = "Email, OTP and new password are required" });

            if (dto.NewPassword.Length < 6)
                return BadRequest(new { message = "Password must be at least 6 characters" });

            var email = dto.Email.Trim().ToLowerInvariant();
            var cacheKey = OtpCacheKeyPrefix + email;
            if (!_cache.TryGetValue(cacheKey, out string? storedOtp) || storedOtp != dto.Otp.Trim())
                return BadRequest(new { message = "Invalid or expired OTP. Please request a new one." });

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email);
            if (user == null)
                return NotFound(new { message = "User not found" });

            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.Modified = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            _cache.Remove(cacheKey);

            return Ok(new { message = "Password reset successfully. You can now sign in." });
        }
        #endregion

        #region REGISTER (Customer only)
        private const int CustomerUserTypeId = 2;

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { message = "Invalid registration data" });

                var email = (dto.Email ?? "").Trim();
                if (string.IsNullOrEmpty(email))
                    return BadRequest(new { message = "Email is required" });

                var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (existingUser != null)
                    return BadRequest(new { message = "An account with this email already exists" });

                var userName = $"{dto.FirstName?.Trim()} {dto.LastName?.Trim()}".Trim();
                if (string.IsNullOrWhiteSpace(userName))
                    userName = email;

                var password = dto.Password ?? "";
                if (password.Length < 6)
                    return BadRequest(new { message = "Password must be at least 6 characters" });
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

                var phone = (dto.Mobile ?? "").Trim();
                if (string.IsNullOrEmpty(phone))
                    return BadRequest(new { message = "Mobile number is required" });

                var user = new User
                {
                    UserName = userName,
                    Email = email,
                    Password = hashedPassword,
                    Phone = phone,
                    Address = "", // DB column is NOT NULL; customer can add address later in profile
                    UserTypeID = CustomerUserTypeId,
                    IsActive = true,
                    Created = DateTime.UtcNow
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();

                var cart = new Cart
                {
                    UserID = user.UserID,
                    Created = DateTime.UtcNow
                };
                _db.Carts.Add(cart);
                await _db.SaveChangesAsync();

                await _db.Entry(user).Reference(u => u.UserType).LoadAsync();
                var token = GenerateJwtToken(user);
                var response = new LoginResponseDTO
                {
                    UserID = user.UserID,
                    UserName = user.UserName,
                    Email = user.Email,
                    UserTypeID = user.UserTypeID,
                    UserTypeName = user.UserType?.UserTypeName ?? "Customer",
                    Phone = user.Phone,
                    Token = token,
                    IsActive = user.IsActive
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Registration failed", error = ex.Message });
            }
        }
        #endregion

        #region GENERATE JWT TOKEN
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong!";
            var issuer = jwtSettings["Issuer"] ?? "ECommerceAPI";
            var audience = jwtSettings["Audience"] ?? "ECommerceAPI";
            var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim("UserID", user.UserID.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("UserTypeID", user.UserTypeID.ToString()),
                new Claim("UserTypeName", user.UserType?.UserTypeName ?? ""),
                new Claim(ClaimTypes.Role, user.UserType?.UserTypeName ?? "")
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion
    }
}
