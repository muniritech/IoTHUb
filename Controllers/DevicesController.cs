using Microsoft.AspNetCore.Mvc;
using IoTHub.Models;
using IoTHub.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authorization;

namespace IoTHub.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class DevicesController : ControllerBase
    {
        private readonly IMongoCollection<Device> _devices;
        private readonly JwtSettings _jwtSettings;

        public DevicesController(IMongoDBContext context, IOptions<JwtSettings> jwtSettings)
        {
            _devices = context.GetCollection<Device>("Devices");
            _jwtSettings = jwtSettings.Value;
        }

        // CRUD actions for devices
    }
}
