using AutoMapper;
using FoodOnline.Core.Dtos;
using FoodOnline.Core.Enums;
using FoodOnline.Repository.Entities;

namespace FoodOnline.Api.Mappings;

public class GeneralProfile : Profile
{
    private static readonly string[] IgnoredPropertyNames = ["CreatedAt", "CreatedBy", "ModifiedAt", "ModifiedBy"];

    public GeneralProfile()
    {
        ShouldMapProperty = p => !IgnoredPropertyNames.Contains(p.Name);
            
        CreateMap<User, UserViewDto>()
            .ForMember(d => d.RoleName, conf => conf.MapFrom(e => e.Role.Name))
            .ForMember(d => d.PositionName, conf => conf.MapFrom(e => e.Position == null ? null : e.Position.Name))
            .ReverseMap();
        CreateMap<UserAddDto, User>();
        CreateMap<UserUpdDto, User>();
        
        CreateMap<Role, RoleViewDto>()
            .ReverseMap();
        CreateMap<RoleAddDto, Role>();
        CreateMap<RoleUpdDto, Role>();

        CreateMap<Position, PositionViewDto>()
            .ReverseMap();
        CreateMap<PositionAddDto, Position>();
        CreateMap<PositionUpdDto, Position>();
        
        CreateMap<Order, OrderViewDto>()
            .ForMember(d => d.StatusName, conf => conf.MapFrom(e => ((OrderStatusEnum)e.StatusId).ToString()))
            .ReverseMap();
        CreateMap<OrderAddDto, Order>();
        CreateMap<OrderUpdDto, Order>();

        CreateMap<OrderDetail, OrderDetailDto>()
            .ReverseMap();
        CreateMap<OrderDetailAddDto, OrderDetail>();
        CreateMap<OrderDetailUpdDto, OrderDetail>();

        CreateMap<OrderPayment, OrderPaymentDto>()
            .ReverseMap();
        CreateMap<OrderPaymentAddDto, OrderPayment>();
        CreateMap<OrderPaymentUpdDto, OrderPayment>();
    }
}