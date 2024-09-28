using Flozacode.Models.Paginations;
using FoodOnline.Core.Dtos;

namespace FoodOnline.Core.Interfaces;

public interface IOrderDetailService : IFlozaPagination<OrderDetailViewDto, OrderDetailAddDto, OrderDetailUpdDto, OrderDetailFilter>
{
    public Task<int> CreateMultipleAsync(List<OrderDetailAddDto> values);
}