using MediatR;
using NotificationService.Models.Dtos;

namespace NotificationService.MediatR.Queries.GetById
{
    public class GetUserByIdQuerry : IRequest<UserDto>
    {
        public string Id { get; set; }
    }
}