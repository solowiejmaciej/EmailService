using EventsLibrary;
using MediatR;
using Microsoft.AspNetCore.Identity;
using NotificationService.Entities;
using NotificationService.Events;
using NotificationService.MediatR.Commands.CreateNew;
using NotificationService.MediatR.Queries.GetToken;
using NotificationService.Repositories;
using NotificationService.Services.Auth;
using NotificationService.Models.Responses;

namespace NotificationService.MediatR.Handlers.CreateNew
{
    public class CreateNewUserCommandHandler : IRequestHandler<CreateNewUserCommand, TokenResponse>
    {
        private readonly IUsersRepository _userRepository;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly IJWTManager _jwtManager;
        private readonly IEventsPublisher _eventsPublisher;
        private readonly IMediator _mediator;

        public CreateNewUserCommandHandler(
            IUsersRepository userRepository,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IJWTManager jwtManager,
            IEventsPublisher eventsPublisher,
            IMediator mediator
                )
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtManager = jwtManager;
            _eventsPublisher = eventsPublisher;
            _mediator = mediator;
        }

        public async Task<TokenResponse> Handle(CreateNewUserCommand request, CancellationToken cancellationToken)
        {
            var newUser = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email,
                NormalizedEmail = request.Email.ToUpper(),
                NormalizedUserName = request.Email.ToUpper(),
                DeviceId = request.DeviceId,
                PhoneNumber = request.PhoneNumber,
                Firstname = request.Firstname,
                Surname = request.Surname,
            };

            var hashedPass = _passwordHasher.HashPassword(newUser, request.Password);
            newUser.PasswordHash = hashedPass;

            await _userRepository.AddAsyncWithDefaultRole(newUser);

            var querry = new GetTokenQuerry()
            {
                Email = request.Email,
                Password = request.Password,
            };

            var response = await _mediator.Send(querry, cancellationToken);

            await _eventsPublisher.Publish(new UserCreatedEvent(newUser.Firstname, newUser.Surname, newUser.Email));

            return response;
        }
    }
}