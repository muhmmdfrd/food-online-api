using System.Transactions;
using Flozacode.Models.Paginations;
using FoodOnline.Core.Dtos;
using FoodOnline.Core.Enums;
using FoodOnline.Core.Interfaces;
using FoodOnline.Core.Models;

namespace FoodOnline.Core.Helpers;

public class OrderHelper
{
    private readonly IOrderService _service;

    public OrderHelper(IOrderService service)
    {
        _service = service;
    }

    public Task<Pagination<OrderViewDto>> GetPagedAsync(OrderFilter filter)
    {
        return _service.GetPagedAsync(filter);
    }

    public List<OrderViewHistory> GetMyOrder(long userId)
    {
        return _service.GetMyOrder(userId);
    }

    public OrderViewDetailHistory? GetOrderViewDetailHistory(long userId, long orderId)
    {
        return _service.GetOrderViewDetailHistory(userId, orderId);
    }

    public async Task<int> CreateAsync(OrderAddDto value, CurrentUser currentUser)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        {
            var now = DateTime.UtcNow;

            var isOrderActive = _service.CheckOrderActiveByCode(value.Code);
            if (isOrderActive)
            {
                return (int)OrderResponseEnum.Active;
            }
            
            value.CreatedBy = currentUser.Id;
            value.CreatedAt = now;
            value.ModifiedBy = currentUser.Id;
            value.ModifiedAt = now;
        
            var result = await _service.CreateAsync(value);
            transaction.Complete();
            
            return result;
        }
    }
}