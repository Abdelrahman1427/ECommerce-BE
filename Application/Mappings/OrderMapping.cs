using Application.Common.OrderDTOS;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class OrderMapping : Profile
    {
        public OrderMapping()
        {
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName));

            CreateMap<OrderStatusHistory, OrderStatusHistoryDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.History, opt => opt.MapFrom(src => src.StatusHistory))
                .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => new AddressDto
                {
                    Street = src.ShippingStreet,
                    City = src.ShippingCity,
                    State = src.ShippingState,
                    PostalCode = src.ShippingPostalCode,
                    Country = src.ShippingCountry
                }));
        }
    }
}
