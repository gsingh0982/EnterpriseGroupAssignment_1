using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Assignment3_EquipmentRental_Team2.Models;
using Assignment3_EquipmentRental_Team2.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Assignment3_EquipmentRental_Team2.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public AuthController(IUnitOfWork unitOfWork, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _config = config;
        }

        // POST /api/auth/login  (admin username + password)
        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            var user = _unitOfWork.Users.GetByUsername(request.Username);

            // Same 401 whether the user is missing, has no password (Google user) or the password is wrong
            if (user == null || user.PasswordHash == null ||
                !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            return Ok(new { token = GenerateToken(user) });
        }

        // GET /api/auth/me  (who is the signed-in user? read from the JWT, then looked up locally)
        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(idClaim, out var userId))
            {
                return Unauthorized();
            }

            var user = _unitOfWork.Users.GetById(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            return Ok(new
            {
                id = user.Id,
                name = user.Name ?? user.Username,
                email = user.Email,
                role = user.Role ?? "User"
            });
        }

        
        [HttpGet("google/login")]
        public IActionResult GoogleLogin(string returnUrl)
        {
            // After Google signs the user in, the OpenID Connect middleware (CallbackPath /signin-google)
            // validates the response and then sends the browser to our callback below.
            var props = new AuthenticationProperties
            {
                RedirectUri = "/api/auth/google/callback?returnUrl=" + Uri.EscapeDataString(returnUrl)
            };

            return Challenge(props, OpenIdConnectDefaults.AuthenticationScheme);
        }

        
        [HttpGet("google/callback")]
        public async Task<IActionResult> GoogleCallback(string returnUrl)
        {
            // Read the temporary cookie the middleware created after a successful Google sign-in
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded || result.Principal == null)
            {
                return Redirect(returnUrl + "#error=" + Uri.EscapeDataString("Google sign-in failed"));
            }

            // Google's claim names may be raw ("sub") or mapped to .NET names, so accept either
            var principal = result.Principal;
            var sub = principal.FindFirstValue("sub") ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = principal.FindFirstValue("email") ?? principal.FindFirstValue(ClaimTypes.Email);
            var name = principal.FindFirstValue("name") ?? principal.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(sub))
            {
                return Redirect(returnUrl + "#error=" + Uri.EscapeDataString("Google did not return a user id"));
            }

            // Link by Google's "sub", never by email. First sign-in creates the local user with the User role;
            // the role is set here in code, so a user can never choose or change it.
            var user = _unitOfWork.Users.GetByProviderSubject("Google", sub);
            if (user == null)
            {
                user = new User
                {
                    Provider = "Google",
                    ProviderSubject = sub,
                    Email = email,
                    Name = name,
                    Role = "User"
                };
                _unitOfWork.Users.AddUser(user);
                _unitOfWork.Complete();
            }

            // The temporary Google cookie has done its job, so remove it
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Hand our JWT to the UI in the URL fragment (#), which is never sent to servers or written to logs
            return Redirect(returnUrl + "#token=" + GenerateToken(user));
        }

        // Builds the application's own signed JWT (used by the admin login and, later, the Google callback)
        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name ?? user.Username ?? "user"),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpiryMinutes"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
