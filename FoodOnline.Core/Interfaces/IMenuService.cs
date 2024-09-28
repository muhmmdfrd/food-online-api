using Flozacode.Models.Paginations;
using FoodOnline.Core.Dtos;
using FoodOnline.Core.Models;

namespace FoodOnline.Core.Interfaces;

public interface IMenuService : IFlozaPagination<MenuViewDto, MenuAddDto, MenuUpdDto, MenuFilter>
{
    Task<int> DeleteAsync(long id, CurrentUser currentUser, bool isHardDelete = false);
}