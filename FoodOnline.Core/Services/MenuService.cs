using Flozacode.Models.Paginations;
using FoodOnline.Core.Dtos;
using FoodOnline.Core.Interfaces;

namespace FoodOnline.Core.Services;

public class MenuService : IMenuService
{
    public Task<Pagination<MenuViewDto>> GetPagedAsync(MenuFilter filter)
    {
        throw new NotImplementedException();
    }

    public Task<List<MenuViewDto>> GetListAsync()
    {
        throw new NotImplementedException();
    }

    public Task<MenuViewDto> FindAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<int> CreateAsync(MenuAddDto value)
    {
        throw new NotImplementedException();
    }

    public Task<int> UpdateAsync(MenuUpdDto value)
    {
        throw new NotImplementedException();
    }

    public Task<int> DeleteAsync(long id)
    {
        throw new NotImplementedException();
    }
}