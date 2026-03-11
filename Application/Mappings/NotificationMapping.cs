using Application.Common.NotificationDTOS;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class NotificationMapping : Profile
    {
        public NotificationMapping()
        {
            CreateMap<Notification, NotificationDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
        }
    }
}
