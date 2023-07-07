using MediatR;
using NotificationService.Models.Dtos;

namespace NotificationService.MediatR.Queries.GetById
{
    public record GetUserByIdQuerry : IRequest<UserDto>
    {
        public string Id { get; set; }
    }
}