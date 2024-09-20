using Flozacode.Models.Paginations;
using FoodOnline.Core.Dtos;
using FoodOnline.Core.Models;

namespace FoodOnline.Core.Interfaces;

public interface IRoleService : IFlozaPagination<RoleViewDto, RoleAddDto, RoleUpdDto, RoleFilter>
{
    
    Task<int> DeleteAsync(long id, CurrentUser currentUser, bool isHardDelete = false);
}