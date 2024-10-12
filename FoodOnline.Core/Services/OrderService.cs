using AutoMapper;
using AutoMapper.QueryableExtensions;
using Flozacode.Extensions.SortExtension;
using Flozacode.Models.Paginations;
using Flozacode.Repository;
using FoodOnline.Core.Dtos;
using FoodOnline.Core.Enums;
using FoodOnline.Core.Interfaces;
using FoodOnline.Repository.Contexts;
using FoodOnline.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodOnline.Core.Services;

public class OrderService : IOrderService
{
    private readonly IFlozaRepo<Order, AppDbContext> _repo;
    private readonly IMapper _mapper;

    public OrderService(IFlozaRepo<Order, AppDbContext> repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<Pagination<OrderViewDto>> GetPagedAsync(OrderFilter filter)
    {
        var query = _repo.AsQueryable.AsNoTracking();
        var total = query.Count();
        var filtered = total;

        if (!string.IsNullOrEmpty(filter.Keyword))
        {
            query = query.Where(q => q.Code.Contains(filter.Keyword, StringComparison.OrdinalIgnoreCase));
            filtered = query.Count();
        }

        var result = query
            .ProjectTo<OrderViewDto>(_mapper.ConfigurationProvider)
            .SortBy(filter.SortName, filter.SortDir)
            .Skip(filter.Skip)
            .Take(filter.Size)
            .ToList();

        return new Pagination<OrderViewDto>
        {
            Size = filter.Size,
            Data = result,
            Filtered = filtered,
            Total = total,
        };
    }

    public Task<List<OrderViewDto>> GetListAsync()
    {
        return _repo.GetListAsync<OrderViewDto>();
    }

    public async Task<OrderViewDto> FindAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<int> CreateAsync(OrderAddDto value)
    {
        var entity = _mapper.Map<Order>(value);
        return _repo.AddAsync(entity);
    }

    public async Task<int> UpdateAsync(OrderUpdDto value)
    {
        throw new NotImplementedException();
    }

    public async Task<int> DeleteAsync(long id)
    {
        throw new NotImplementedException();
    }

    public bool CheckOrderActiveByCode(string code)
    {
        return _repo.AsQueryable.AsNoTracking().Any(q => q.Code == code && q.StatusId == (int)OrderStatusEnum.Active);
    }

    public OrderViewDto? GetActiveOrderByCode(string code)
    {
        return _repo.AsQueryable
            .AsNoTracking()
            .ProjectTo<OrderViewDto>(_mapper.ConfigurationProvider)
            .FirstOrDefault(q => q.Code == code && q.StatusId == (int)OrderStatusEnum.Active);
    }

    public long? GetOrderActiveId()
    {
        return _repo.AsQueryable.AsNoTracking().FirstOrDefault(q => q.StatusId == (int)OrderStatusEnum.Active)?.Id;
    }

    public List<OrderViewHistory> GetMyOrder(long userId)
    {
        var entities = _repo.AsQueryable.AsNoTracking()
            .Include(q => q.OrderDetails)
            .Where(q => q.OrderDetails.Any(x => x.UserId == userId))
            .OrderByDescending(q => q.Id)
            .ToList();

        return _mapper.Map<List<OrderViewHistory>>(entities, opt => opt.Items["UserId"] = userId);
    }

    public OrderViewDetailHistory? GetOrderViewDetailHistory(long userId, long orderId)
    {
        var entities = _repo.AsQueryable.AsNoTracking()
            .Include(q => q.OrderDetails)
            .Include(q => q.OrderPayments)
            .AsSplitQuery()
            .FirstOrDefault(q => q.Id == orderId);
        
        if (entities == null)
        {
            return null;
        }
        
        var details = (
            from d in entities.OrderDetails
            where d.OrderId == orderId && d.UserId == userId
            select new OrderViewDetailItemHistory
            {
                Name = d.MenuName,
                Total = d.Total,
                Price = d.Price,
                Qty = d.Qty,
            }).ToList();

        var payment = (
            from p in entities.OrderPayments
            where p.OrderId == orderId && p.UserId == userId
            select new OrderViewDetailPaymentHistory
            {
                Cashback = p.Cashback,
                TotalPayment = p.TotalPayment
            }).FirstOrDefault();

        return new OrderViewDetailHistory
        {
            Code = entities.Code,
            Date = entities.Date,
            Total = details.Sum(q => q.Total),
            StatusName = ((OrderStatusEnum)entities.StatusId).ToString(),
            OrderDetails = details,
            OrderPayment = payment
        };
    }
}