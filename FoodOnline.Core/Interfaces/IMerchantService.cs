using Flozacode.Models.Paginations;
using FoodOnline.Core.Dtos;
using FoodOnline.Core.Models;

namespace FoodOnline.Core.Interfaces;

public interface IMerchantService : IFlozaPagination<MerchantViewDto, MerchantAddDto, MerchantUpdDto, MerchantFilter>
{
    Task<int> DeleteAsync(long id, CurrentUser currentUser, bool isHardDelete = false);
}