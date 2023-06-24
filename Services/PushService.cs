using AuthService.Services;
using AutoMapper;
using NotificationService.Entities;
using NotificationService.Models;
using NotificationService.Repositories;

namespace NotificationService.Services
{
    public interface IPushDataService
    {
        List<PushNotificationDto> GetAll(string? userId);

        PushNotificationDto GetById(int id);

        Task<int> AddNewAsync(PushRequest dto, string userDto);
    }

    public class PushService : IPushDataService
    {
        private readonly IPushRepository _pushRepository;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public PushService(IPushRepository dbContext, IMapper mapper, IUserService userService)
        {
            _pushRepository = dbContext;
            _mapper = mapper;
            _userService = userService;
        }

        public List<PushNotificationDto> GetAll(string? userId)
        {
            if (userId is not null)
            {
                var userDto = _userService.GetById(userId);
                var pushToUser = _pushRepository.GetByUserId(userDto.Id);
                var pushToUserDtos = _mapper.Map<List<PushNotificationDto>>(pushToUser);
                return pushToUserDtos;
            }

            var push = _pushRepository.GetAll();
            var dtos = _mapper.Map<List<PushNotificationDto>>(push);
            return dtos;
        }

        public PushNotificationDto GetById(int id)
        {
            var push = _pushRepository.GetById(id);
            var dto = _mapper.Map<PushNotificationDto>(push);
            return dto;
        }

        public async Task<int> AddNewAsync(PushRequest request, string userId)
        {
            var userDto = _userService.GetById(userId);
            var pushNotification = _mapper.Map<PushNotification>(request);
            await _pushRepository.AddAsync(pushNotification, userDto);
            return pushNotification.Id;
        }
    }
}