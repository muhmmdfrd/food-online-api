using Flozacode.Models.Paginations;
using FoodOnline.Core.Dtos;

namespace FoodOnline.Core.Interfaces;

public interface IOrderService : IFlozaPagination<OrderViewDto, OrderAddDto, OrderUpdDto, OrderFilter>
{
    public bool CheckOrderActiveByCode(string code);
}