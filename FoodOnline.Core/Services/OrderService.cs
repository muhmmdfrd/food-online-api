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

    public List<OrderViewDto> GetMyOrder(long userId)
    {
        return _repo.AsQueryable.AsNoTracking()
            .Where(q => q.OrderDetails.Any(o => o.UserId == userId))
            .ProjectTo<OrderViewDto>(_mapper.ConfigurationProvider)
            .ToList();
    }
}