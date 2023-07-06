using NotificationService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NotificationService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : Controller
    {
        private readonly IDeviceService _deviceService;

        public DeviceController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [HttpPost]
        public async Task<ActionResult> AssignDeviceIdToUser([FromQuery] string deviceId)
        {
            await _deviceService.AssignDeviceIdToUserAsync(deviceId);
            return Ok();
        }
    }
}