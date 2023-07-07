using System.Security.Claims;

namespace NotificationService.UserContext
{
    public interface IUserContext
    {
        CurrentUser GetCurrentUser();
    }

    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public CurrentUser GetCurrentUser()
        {
            var user = _httpContextAccessor.HttpContext!.User;

            if (user is null)
            {
                //throw new UnauthorizedAccessException("currentUser is null");
            }

            if (user.Identity == null)
            {
                //throw new UnauthorizedAccessException("user.Identity is null");
            }

            if (!user.Identity.IsAuthenticated)
            {
                //throw new UnauthorizedAccessException("user.Identity.IsAuthenticated isn't auth");
            }

            var userName = user.Identity.Name!;
            var userId = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
            var userRole = user.FindFirst(c => c.Type == ClaimTypes.Role)!.Value;

            return new CurrentUser(userId, userName, userRole);
        }
    }
}