﻿using AutoMapper;
using FoodOnline.Core.Dtos;
using FoodOnline.Core.Enums;
using FoodOnline.Repository.Entities;

namespace FoodOnline.Api.Mappings;

public class GeneralProfile : Profile
{
    private static readonly string[] IgnoredPropertyNames = ["CreatedAt", "CreatedBy", "ModifiedAt", "ModifiedBy", "DataStatusId"];

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
            .ForMember(d => d.DataStatusName, conf => conf.MapFrom(e => ((DataStatusEnum)e.DataStatusId).ToString()))
            .ReverseMap();
        CreateMap<PositionAddDto, Position>();
        CreateMap<PositionUpdDto, Position>();
        
        CreateMap<Order, OrderViewDto>()
            .ForMember(d => d.StatusName, conf => conf.MapFrom(e => ((OrderStatusEnum)e.StatusId).ToString()))
            .ReverseMap();
        CreateMap<Order, OrderViewHistory>()
            .ForMember(d => d.Menus, conf => conf.MapFrom((q, _, _, context) => q.OrderDetails.Where(x => x.UserId == (long)context.Items["UserId"]).Select(o => o.MenuName).ToList()))
            .ForMember(d => d.Total, conf => conf.MapFrom((q, _, _, context) => q.OrderDetails.Where(x => x.UserId == (long)context.Items["UserId"]).Sum(o => o.Total)));
        CreateMap<OrderAddDto, Order>();
        CreateMap<OrderUpdDto, Order>();

        CreateMap<OrderDetail, OrderDetailViewDto>()
            .ReverseMap();
        CreateMap<OrderDetailAddDto, OrderDetail>();
        CreateMap<OrderDetailUpdDto, OrderDetail>();

        CreateMap<OrderPayment, OrderPaymentDto>()
            .ReverseMap();
        CreateMap<OrderPaymentAddDto, OrderPayment>();
        CreateMap<OrderPaymentUpdDto, OrderPayment>();
        
        CreateMap<Menu, MenuViewDto>()
            .ForMember(d => d.DataStatusName, conf => conf.MapFrom(e => ((DataStatusEnum)e.DataStatusId).ToString()))
            .ForMember(d => d.MerchantName, conf => conf.MapFrom(e => e.Merchant.Name))
            .ReverseMap();
        CreateMap<MenuAddDto, Menu>();
        CreateMap<MenuUpdDto, Menu>();
        
        CreateMap<Merchant, MerchantViewDto>()
            .ForMember(d => d.DataStatusName, conf => conf.MapFrom(e => ((DataStatusEnum)e.DataStatusId).ToString()))
            .ReverseMap();
        CreateMap<MerchantAddDto, Merchant>();
        CreateMap<MerchantUpdDto, Merchant>();
    }
}