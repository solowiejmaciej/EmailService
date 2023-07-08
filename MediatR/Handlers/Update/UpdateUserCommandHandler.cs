using AutoMapper;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using NotificationService.Exceptions;
using NotificationService.MediatR.Commands.Update;
using NotificationService.Models.Dtos;
using NotificationService.Repositories;
using NotificationService.UserContext;

namespace NotificationService.MediatR.Handlers.Update
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
    {
        private readonly IUsersRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUserContext _userContext;

        public UpdateUserCommandHandler(
            IUsersRepository repository,
            IMapper mapper,
            IUserContext userContext
            )
        {
            _repository = repository;
            _mapper = mapper;
            _userContext = userContext;
        }

        public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (user is null) throw new NotFoundException($"User with id {request.Id} not found");

            var currentUser = _userContext.GetCurrentUser();
            var isAdmin = currentUser.Role == "Admin";
            if (!isAdmin && currentUser.Id != request.Id)
            {
                throw new AccessForbiddenException($"User {currentUser.Id} tried to edit {request.Id} user ");
            }

            if (!request.DeviceId.IsNullOrEmpty())
            {
                user.DeviceId = request.DeviceId;
            }
            if (!request.Firstname.IsNullOrEmpty())
            {
                user.Firstname = request.Firstname;
            }

            if (!request.Surname.IsNullOrEmpty())
            {
                user.Surname = request.Surname;
            }

            if (!request.Email.IsNullOrEmpty())
            {
                user.Email = request.Email;
            }

            if (!request.PhoneNumber.IsNullOrEmpty())
            {
                user.PhoneNumber = request.PhoneNumber;
            }
            await _repository.SaveAsync(cancellationToken);
            var dto = _mapper.Map<UserDto>(user);
            return dto;
        }
    }
}