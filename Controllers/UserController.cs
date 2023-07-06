using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Models.Dtos;
using NotificationService.Services.Users;

namespace AuthService.Controllers
{
    //[Authorize]
    [EnableCors("apiCorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public List<UserDto> GetAll()
        {
            return _userService.GetAll().Result;
        }

        [HttpGet("{Id}")]
        public UserDto GetById(string Id)
        {
            return _userService.GetById(Id);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            _userService.DeleteAsync(id);
            return NoContent();
        }
    }
}