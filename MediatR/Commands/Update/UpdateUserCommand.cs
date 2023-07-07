using MediatR;
using NotificationService.Models.Dtos;

namespace NotificationService.MediatR.Commands.Update
{
    public record UpdateUserCommand : IRequest<UserDto>
    {
        public string Id { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? DeviceId { get; set; }
        public string? Firstname { get; set; }
        public string? Surname { get; set; }
    }
}