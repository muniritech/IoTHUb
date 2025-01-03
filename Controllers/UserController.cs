using Microsoft.AspNetCore.Mvc;
using IoTHub.Models;
using IoTHub.Data;
using BCryptNet = BCrypt.Net.BCrypt;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace IoTHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMongoCollection<User> _users;

        public UserController(IMongoDBContext context)
        {
            _users = context.GetCollection<User>("Users");
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var user = _users.Find(u => u.Email == email).FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
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
                Id = Guid.NewGuid().ToString(),
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
                return Ok("Login successful.");
            else
                return Unauthorized("Invalid password.");
        }
    }
}
