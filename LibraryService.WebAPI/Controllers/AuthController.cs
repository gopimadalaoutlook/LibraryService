using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LibraryService.WebAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private static readonly Dictionary<string, string> users = new Dictionary<string, string>
        {

            { "admin", "adminpassword" },
            { "user", "password" }
        };

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            if (users.ContainsKey(loginRequest.Username) && users[loginRequest.Username] == loginRequest.Password)
            {
                // Simulate JWT token creation after user successfully logs in
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, loginRequest.Username),
                    new Claim(ClaimTypes.Role, loginRequest.Username == "admin" ? "Admin" : "User")
                };

                var secretKey = _configuration["JwtSettings:SecretKey"];
                var expirationMinutes = double.Parse(_configuration["JwtSettingS:ExpirationInMinutes"]);

               var key = new SymmetricSecurityKey(Convert.FromBase64String(secretKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["JwtSettings:Issuer"],
                    audience: _configuration["JwtSettings:Audience"],
                    claims: claims,
                    expires: System.DateTime.Now.AddMinutes(expirationMinutes),
                    signingCredentials: credentials
                );

                var jwt = new JwtSecurityTokenHandler().WriteToken(token);
                return Ok(new { access_token = jwt });
            }
            return Unauthorized("Invalid username or password.");
        }
        
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    
    
}
