using Flozacode.Models.Paginations;
using FoodOnline.Core.Dtos;
using FoodOnline.Repository.Entities;

namespace FoodOnline.Core.Interfaces;

public interface IOrderPaymentService : IFlozaPagination<OrderPaymentViewDto, OrderPaymentAddDto, OrderPaymentUpdDto, OrderPaymentFilter>
{
    public OrderPayment? IsPaymentExist(long orderId, long userId);
    public Task<int> UpdateGrandTotalAsync(OrderPayment value);
}