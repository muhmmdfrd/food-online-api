using Flozacode.Models.Paginations;
using FoodOnline.Core.Dtos;

namespace FoodOnline.Core.Interfaces;

public interface IOrderService : IFlozaPagination<OrderViewDto, OrderAddDto, OrderUpdDto, OrderFilter>
{
    public bool CheckOrderActiveByCode(string code);
    public OrderViewDto? GetActiveOrderByCode(string code);
    public long? GetOrderActiveId();
    public List<OrderViewHistory> GetMyOrder(long userId);
    public OrderViewDetailHistory? GetOrderViewDetailHistory(long userId, long orderId);
}