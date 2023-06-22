using AuthService.Services;
using AutoMapper;
using NotificationService.Entities;
using NotificationService.Models;
using NotificationService.Repositories;

namespace NotificationService.Services
{
    public interface IPushDataService
    {
        List<PushNotificationDto> GetAllByCurrentUser();

        PushNotificationDto GetById(int id);

        Task<int> AddNewAsync(PushRequest dto, string userDto);

        List<PushNotificationDto> GetAll();
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

        public List<PushNotificationDto> GetAllByCurrentUser()
        {
            throw new NotImplementedException();
        }

        public PushNotificationDto GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<int> AddNewAsync(PushRequest request, string userId)
        {
            var userDto = _userService.GetById(userId);
            var pushNotification = _mapper.Map<PushNotification>(request);
            await _pushRepository.AddAsync(pushNotification, userDto);
            return pushNotification.Id;
        }

        public List<PushNotificationDto> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}