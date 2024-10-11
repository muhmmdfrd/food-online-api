using System.Transactions;
using Flozacode.Repository;
using FoodOnline.Core.Dtos;
using FoodOnline.Core.Enums;
using FoodOnline.Core.Interfaces;
using FoodOnline.Core.Models;
using FoodOnline.Core.Utils;
using FoodOnline.Repository.Contexts;
using FoodOnline.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodOnline.Core.Helpers;

public class OrderDetailHelper
{
    private readonly IOrderService _orderService;
    private readonly IOrderDetailService _service;
    private readonly IMenuService _menuService;
    private readonly IOrderPaymentService _orderPaymentService;
    private readonly IFlozaRepo<Menu, AppDbContext> _menuRepo;
    
    public OrderDetailHelper(
        IOrderService orderService, 
        IOrderDetailService service, 
        IMenuService menuService,
        IOrderPaymentService orderPaymentService, 
        IFlozaRepo<Menu, AppDbContext> menuRepo)
    {
        _orderService = orderService;
        _service = service;
        _menuService = menuService;
        _orderPaymentService = orderPaymentService;
        _menuRepo = menuRepo;
    }

    public async Task<List<OrderDetailGroupByUser>> GetOrderToday(CurrentUser currentUser)
    {
        var activeOrderId = _orderService.GetOrderActiveId();
        if (activeOrderId == null)
        {
            return [];
        }
        
        return await _service.GetOrderDetailByOrderIdAsync(activeOrderId ?? 0, currentUser.Id);
    }

    public async Task<int> CreateAsync(OrderDetailAddRequestDto value, CurrentUser currentUser)
    {
        var now = DateTime.UtcNow;
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        {
            var code = OrderUtils.GenerateCode(now);
            var order = _orderService.GetActiveOrderByCode(code);
            if (order == null)
            {
                return 0;
            }
    
            var menus = await _menuService.GetListAsync();
            var orderDetailDtos = new List<OrderDetailAddDto>();
                
            foreach (var item in value.Details)
            {
                var menu = menus.FirstOrDefault(q => q.Id == item.MenuId);
                if (menu == null)
                {
                    return 0;
                }
                
                now = DateTime.UtcNow;
                var dto = new OrderDetailAddDto
                {
                    UserName = currentUser.Name,
                    UserId = currentUser.Id,
                    Price = menu.Price,
                    Qty = item.Qty,
                    Total = menu.Price * item.Qty,
                    MenuId = menu.Id,
                    MenuName = menu.Name,
                    OrderId = order.Id,
                    CreatedBy = currentUser.Id,
                    CreatedAt = now,
                    ModifiedBy = currentUser.Id,
                    ModifiedAt = now,
                };
               
                orderDetailDtos.Add(dto);
            }

            var affected = await _service.CreateMultipleAsync(orderDetailDtos);
            if (affected <= 0)
            {
                return 0;
            }

            var payment = _orderPaymentService.IsPaymentExist(order.Id, currentUser.Id);
            if (payment != null)
            {
                payment.TotalPayment = value.PaymentAmount;
                payment.GrandTotal += orderDetailDtos.Sum(q => q.Total);
                payment.Cashback = payment.GrandTotal - payment.TotalPayment;
                affected = await _orderPaymentService.UpdateGrandTotalAsync(payment);
            }
            else
            {
                var total = orderDetailDtos.Sum(q => q.Total);
                affected = await _orderPaymentService.CreateAsync(new OrderPaymentAddDto
                {
                    OrderId = order.Id,
                    UserId = currentUser.Id,
                    GrandTotal = total,
                    TotalPayment = value.PaymentAmount,
                    Cashback = total - value.PaymentAmount,
                    CreatedBy = currentUser.Id,
                    CreatedAt = now,
                    ModifiedBy = currentUser.Id,
                    ModifiedAt = now,
                });
            }
            
            transaction.Complete();
            
            return affected;
        }
    }

    public Task<OrderDetailCaculateResultDto> CalculateAsync(List<OrderDetailAddChildDto> items)
    {
        var children = new List<OrderDetailCaculateResultItemDto>();
        var menuIds = items.Select(q => q.MenuId).ToList();
        var menus = _menuRepo
            .AsQueryable
            .AsNoTracking()
            .Where(q =>
                q.DataStatusId == (int)DataStatusEnum.Active &&
                menuIds.Contains(q.Id))
            .ToList();

        foreach (var t in menus)
        {
            var qty = items.First(q => q.MenuId == t.Id).Qty;
            var item = new OrderDetailCaculateResultItemDto
            {
                Qty = qty,
                MenuName = t.Name,
                Total = qty * t.Price
            };
            children.Add(item);
        }

        var result = new OrderDetailCaculateResultDto
        {
            Items = children,
            GrandTotal = children.Sum(q => q.Total),
        };

        return Task.FromResult(result);
    }
}