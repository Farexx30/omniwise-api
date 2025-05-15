using Omniwise.Domain.Entities;
using AutoMapper;

namespace Omniwise.Application.Notifications.Dtos;

public class NotificationsMappingProfile : Profile
{
    public NotificationsMappingProfile()
    {
        CreateMap<Notification, NotificationDto>();
    }
}
