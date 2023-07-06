using AutoMapper;
using NotificationService.Entities;
using NotificationService.Models;

namespace NotificationService.Services
{
    public interface IRecipientService
    {
        Recipient GetRecipientFromUserId(string userId);
    }

    public class RecipientService : IRecipientService
    {
        private readonly NotificationDbContext _dbContext;
        private readonly IMapper _mapper;

        public RecipientService(
            NotificationDbContext dbContext,
            IMapper mapper
            )
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public Recipient GetRecipientFromUserId(string userId)
        {
            var dbUser = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
            var recipient = _mapper.Map<Recipient>(dbUser);
            return recipient;
        }
    }
}