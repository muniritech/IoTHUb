using Microsoft.AspNetCore.Mvc;
using IoTHub.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using BCryptNet = BCrypt.Net.BCrypt;
using MongoDB.Driver;
using IoTHub.Data;
using MongoDB.Bson; // Add this namespace

namespace IoTHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoCollection<User> _users;

        public AccountController(IConfiguration configuration, IMongoDBContext context)
        {
            _configuration = configuration;
            _users = context.GetCollection<User>("Users");
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegistrationModel registrationModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_users.Find(u => u.Email == registrationModel.Email).FirstOrDefault() != null)
                return Conflict("User with this email already exists.");

            var user = new User
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Username = registrationModel.Username,
                Email = registrationModel.Email,
                PasswordHash = BCryptNet.HashPassword(registrationModel.Password, 12)
            };

            _users.InsertOne(user);
            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            var existingUser = _users.Find(u => u.Email == loginModel.Email).FirstOrDefault();
            if (existingUser == null)
                return NotFound("User not found.");

            if (BCryptNet.Verify(loginModel.Password, existingUser.PasswordHash))
            {
                var token = GenerateJwtToken(existingUser);
                return Ok(new { token });
            }
            else
            {
                return Unauthorized("Invalid password.");
            }
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = _configuration["Jwt:Key"];
            if (key == null)
            {
                throw new ArgumentNullException("Jwt:Key");
            }

            var keyBytes = Encoding.UTF8.GetBytes(key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
