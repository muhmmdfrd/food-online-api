﻿using System.Transactions;
using Flozacode.Repository;
using FoodOnline.Core.Dtos;
using FoodOnline.Core.Enums;
using FoodOnline.Core.Interfaces;
using FoodOnline.Core.Models;
using FoodOnline.Repository.Contexts;
using FoodOnline.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodOnline.Core.Helpers;

public class OrderDetailHelper
{
    private readonly IOrderService _orderService;
    private readonly IOrderDetailService _service;
    private readonly IUserService _userService;
    private readonly IMenuService _menuService;
    private readonly IOrderPaymentService _orderPaymentService;
    private readonly IFlozaRepo<Menu, AppDbContext> _menuRepo;
    
    public OrderDetailHelper(
        IOrderService orderService, 
        IOrderDetailService service, 
        IUserService userService, 
        IMenuService menuService,
        IOrderPaymentService orderPaymentService, 
        IFlozaRepo<Menu, AppDbContext> menuRepo)
    {
        _orderService = orderService;
        _service = service;
        _userService = userService;
        _menuService = menuService;
        _orderPaymentService = orderPaymentService;
        _menuRepo = menuRepo;
    }

    public async Task<int> CreateAsync(OrderDetailAddRequestDto value, CurrentUser currentUser)
    {
        var now = DateTime.UtcNow;
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        {
            var order = await _orderService.FindAsync(value.OrderId);
            if (order == null)
            {
                return 0;
            }
    
            var user = await _userService.FindAsync(value.UserId);
            if (user == null)
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
                    UserName = user.Name,
                    UserId = user.Id,
                    Price = menu.Price,
                    Qty = item.Qty,
                    Total = menu.Price * item.Qty,
                    MenuId = menu.Id,
                    MenuName = menu.Name,
                    OrderId = value.OrderId,
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

            var payment = _orderPaymentService.IsPaymentExist(value.OrderId, value.UserId);
            if (payment != null)
            {
                payment.GrandTotal = orderDetailDtos.Sum(q => q.Total);
                affected = await _orderPaymentService.UpdateGrandTotalAsync(payment);
            }
            else
            {
                affected = await _orderPaymentService.CreateAsync(new OrderPaymentAddDto
                {
                    OrderId = value.OrderId,
                    UserId = value.UserId,
                    GrandTotal = orderDetailDtos.Sum(q => q.Total), 
                    TotalPayment = 0,
                    Cashback = 0,
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

        for (var i = 0; i < menus.Count; i++)
        {
            var qty = items.First(q => q.MenuId == menus[i].Id).Qty;
            var item = new OrderDetailCaculateResultItemDto
            {
                Qty = qty,
                MenuName = menus[i].Name,
                Total = qty * menus[i].Price
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