using Flozacode.Repository;
using FoodOnline.Core.Dtos;
using FoodOnline.Core.Enums;
using FoodOnline.Core.Utils;
using FoodOnline.Repository.Contexts;
using FoodOnline.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodOnline.Core.Helpers;

public class DashboardHelper
{
    private readonly IFlozaRepo<Merchant, AppDbContext> _merchantRepo;
    private readonly IFlozaRepo<Menu, AppDbContext> _menuRepo;
    private readonly IFlozaRepo<Order, AppDbContext> _orderRepo;

    public DashboardHelper(
        IFlozaRepo<Merchant, AppDbContext> merchantRepo, 
        IFlozaRepo<Menu, AppDbContext> menuRepo, 
        IFlozaRepo<Order, AppDbContext> orderRepo)
    {
        _merchantRepo = merchantRepo;
        _menuRepo = menuRepo;
        _orderRepo = orderRepo;
    }

    public Task<DashboardDto> GetDashboardAsync()
    {
        var menus = _menuRepo.AsQueryable.AsNoTracking().Count(q => q.DataStatusId == (int)DataStatusEnum.Active);
        var merchants = _merchantRepo.AsQueryable.AsNoTracking().Count(q => q.DataStatusId == (int)DataStatusEnum.Active);
        var orders = 0;
        var payments = 0;
        
        var currentCode = OrderUtils.GenerateCode();
        
        var activeOrder = _orderRepo.AsQueryable
            .AsNoTracking()
            .Include(q => q.OrderDetails)
            .Include(q => q.OrderPayments)
            .AsSplitQuery()
            .FirstOrDefault(q => q.Code == currentCode);
        
        if (activeOrder != null)
        {
            orders = activeOrder.OrderDetails.Count;
            payments = activeOrder.OrderPayments.AsQueryable().Sum(q => q.TotalPayment);
        }

        var result = new DashboardDto
        {
            MenuCount = menus,
            MerchantCount = merchants,
            OrderCount = orders,
            TotalPayment = payments,
        };

        return Task.FromResult(result);
    }
}