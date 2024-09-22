using Flozacode.Models.Paginations;
using FoodOnline.Core.Dtos;
using FoodOnline.Core.Models;

namespace FoodOnline.Core.Interfaces;

public interface IPositionService : IFlozaPagination<PositionViewDto, PositionAddDto, PositionUpdDto, PositionFilter>
{
    Task<int> DeleteAsync(long id, CurrentUser currentUser, bool isHardDelete = false);
}