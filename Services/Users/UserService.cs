using NotificationService.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NotificationService.Entities;
using NotificationService.Models.Dtos;
using NotificationService.Models.Responses;
using NotificationService.Models.Requests;
using NotificationService.Services.Auth;

namespace NotificationService.Services.Users
{
    public interface IUserService
    {
        UserDto GetById(string id);

        Task<List<UserDto>> GetAll();

        Task DeleteAsync(string id);

        TokenResponse Register(UserRegisterRequest userBodyResponse);
    }

    internal class UserService : IUserService
    {
        private readonly NotificationDbContext _dbContext;
        private readonly ILogger<UserService> _logger;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly IJWTManager _jwtManager;

        public UserService(
            NotificationDbContext dbContext,
            ILogger<UserService> logger,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IJWTManager jwtManager

            )
        {
            _dbContext = dbContext;
            _logger = logger;
            _passwordHasher = passwordHasher;
            _jwtManager = jwtManager;
        }

        private ApplicationUser GetUserFromDb(string id)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.Id.Equals(id));
            if (user == null)
            {
                throw new NotFoundException($"User with {id} not found");
            }
            return user;
        }

        private string GetRoleIdByUserId(string userId)
        {
            try
            {
                return _dbContext.UserRoles.FirstOrDefault(r => r.UserId == userId).RoleId;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return null;
            }
        }

        private string GetRoleNameByRoleId(string roleId)
        {
            try
            {
                return _dbContext.Roles.FirstOrDefault(r => r.Id == roleId).Name;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return null;
            }
        }

        private List<UserDto> GetUsersWithRoles(List<ApplicationUser> users)
        {
            List<UserDto> usersDtos = new List<UserDto>();
            foreach (var user in users)
            {
                string? roleId = GetRoleIdByUserId(user.Id);
                string? roleName = GetRoleNameByRoleId(roleId);
                usersDtos.Add(new UserDto
                {
                    Id = user.Id,
                    RoleId = roleId,
                    RoleName = roleName,
                    Email = user.Email,
                    DeviceId = user.DeviceId,
                });
            }
            return usersDtos;
        }

        public UserDto GetById(string id)
        {
            var user = GetUserFromDb(id);
            var roleId = GetRoleIdByUserId(user.Id);
            var roleName = GetRoleNameByRoleId(roleId);
            var userDto = new UserDto
            {
                Id = user.Id,
                RoleId = roleId,
                RoleName = roleName,
                Email = user.Email,
                DeviceId = user.DeviceId,
            };
            return userDto;
        }

        public async Task<List<UserDto>> GetAll()
        {
            var allUsers = await _dbContext.Users.ToListAsync();
            var allUsersDtos = GetUsersWithRoles(allUsers);
            return allUsersDtos;
        }

        public async Task DeleteAsync(string id)
        {
            var userToDelete = GetUserFromDb(id);
            _dbContext.Users.Remove(userToDelete);
            await _dbContext.SaveChangesAsync();
        }

        public TokenResponse Register(UserRegisterRequest userBodyResponse)
        {
            var newUser = new ApplicationUser
            {
                Email = userBodyResponse.Email,
                UserName = userBodyResponse.Email,
                NormalizedEmail = userBodyResponse.Email.ToUpper(),
                NormalizedUserName = userBodyResponse.Email.ToUpper(),
                DeviceId = userBodyResponse.DeviceId,
                PhoneNumber = userBodyResponse.PhoneNumber
            };

            var hashedPass = _passwordHasher.HashPassword(newUser, userBodyResponse.Password);
            newUser.PasswordHash = hashedPass;

            var userInDb = _dbContext.Users.Add(newUser);
            var userRoleId = _dbContext.Roles.FirstOrDefault(r => r.Name == "User").Id;
            _dbContext.UserRoles.AddAsync(new()
            {
                RoleId = userRoleId,
                UserId = userInDb.Entity.Id
            });

            _dbContext.SaveChanges();

            var response = _jwtManager.GenerateJWT(new UserLoginRequest()
            {
                Email = userBodyResponse.Email,
                Password = userBodyResponse.Password,
            });

            return response;
        }
    }
}