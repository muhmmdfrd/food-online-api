using Flozacode.Models.Paginations;
using FoodOnline.Core.Dtos;
using FoodOnline.Core.Models;

namespace FoodOnline.Core.Interfaces;

public interface IUserService : IFlozaPagination<UserViewDto, UserAddDto, UserUpdDto, UserFilter>
{
    Task<int> DeleteAsync(long id, CurrentUser currentUser, bool isHardDelete = false);
}