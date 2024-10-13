using System.Transactions;
using FirebaseAdmin.Messaging;
using Flozacode.Models.Paginations;
using Flozacode.Repository;
using FoodOnline.Core.Dtos;
using FoodOnline.Core.Enums;
using FoodOnline.Core.Fcm;
using FoodOnline.Core.Interfaces;
using FoodOnline.Core.Models;
using FoodOnline.Repository.Contexts;
using FoodOnline.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodOnline.Core.Helpers;

public class OrderHelper
{
    private readonly IOrderService _service;
    private readonly IFlozaRepo<Order, AppDbContext> _repo;
    private readonly IFlozaRepo<User, AppDbContext> _userRepo;
    private readonly FirebaseHelper _firebaseHelper;

    public OrderHelper(IOrderService service, IFlozaRepo<Order, AppDbContext> repo, IFlozaRepo<User, AppDbContext> userRepo, FirebaseHelper firebaseHelper)
    {
        _service = service;
        _repo = repo;
        _userRepo = userRepo;
        _firebaseHelper = firebaseHelper;
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

            await _repo.AsQueryable
                .Where(q => q.Code != value.Code && q.StatusId == (int)OrderStatusEnum.Active)
                .ExecuteUpdateAsync(f => f.SetProperty(
                    x => x.StatusId, x => (int)OrderStatusEnum.Close));
            
            value.CreatedBy = currentUser.Id;
            value.CreatedAt = now;
            value.ModifiedBy = currentUser.Id;
            value.ModifiedAt = now;
        
            var result = await _service.CreateAsync(value);
            transaction.Complete();

            await Task.Run(() =>
            {
                var tokens = _userRepo.AsQueryable
                    .AsNoTracking()
                    .Where(q => !string.IsNullOrEmpty(q.FirebaseToken))
                    .Select(t => t.FirebaseToken).ToList();

                _firebaseHelper.SendBroadcastAsync(new Notification
                {
                    Title = "Order telah dibuka",
                    Body = "Halo gusy, orderan telah dibuka yaa! Silakan pesan biar ngga laper"
                }, tokens!);
            });
            
            return result;
        }
    }
}