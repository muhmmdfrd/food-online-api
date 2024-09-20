using AutoMapper;
using FoodOnline.Core.Dtos;
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
    }
}