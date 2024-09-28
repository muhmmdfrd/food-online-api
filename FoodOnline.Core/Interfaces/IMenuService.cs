using Flozacode.Models.Paginations;
using FoodOnline.Core.Dtos;

namespace FoodOnline.Core.Interfaces;

public interface IMenuService : IFlozaPagination<MenuViewDto, MenuAddDto, MenuUpdDto, MenuFilter>
{
    
}