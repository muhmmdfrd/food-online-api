using AutoMapper;
using AutoMapper.QueryableExtensions;
using Flozacode.Extensions.SortExtension;
using Flozacode.Models.Paginations;
using Flozacode.Repository;
using FoodOnline.Core.Dtos;
using FoodOnline.Core.Interfaces;
using FoodOnline.Repository.Contexts;
using FoodOnline.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodOnline.Core.Services;

public class OrderDetailService : IOrderDetailService
{
    private readonly IFlozaRepo<OrderDetail, AppDbContext> _repo;
    private readonly IMapper _mapper;

    public OrderDetailService(IFlozaRepo<OrderDetail, AppDbContext> repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<Pagination<OrderDetailViewDto>> GetPagedAsync(OrderDetailFilter filter)
    {
        if (filter.OrderId <= 0)
        {
            throw new ArgumentException("OrderId must be included.");
        }
        
        var query = _repo.AsQueryable.AsNoTracking().Where(q => q.OrderId == filter.OrderId);
        var total = query.Count();
        var filtered = total;

        if (!string.IsNullOrEmpty(filter.Keyword))
        {
            query = query.Where(q => 
                q.MenuName.Contains(filter.Keyword, StringComparison.OrdinalIgnoreCase) ||
                q.UserName.Contains(filter.Keyword, StringComparison.OrdinalIgnoreCase));
            filtered = query.Count();
        }

        var result = query
            .ProjectTo<OrderDetailViewDto>(_mapper.ConfigurationProvider)
            .SortBy(filter.SortName, filter.SortDir)
            .Skip(filter.Skip)
            .Take(filter.Size)
            .ToList();

        return new Pagination<OrderDetailViewDto>
        {
            Size = filter.Size,
            Data = result,
            Filtered = filtered,
            Total = total,
        };
    }

    public Task<List<OrderDetailViewDto>> GetListAsync()
    {
        throw new NotImplementedException();
    }

    public Task<OrderDetailViewDto> FindAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<int> CreateAsync(OrderDetailAddDto value)
    {
        var entity = _mapper.Map<OrderDetail>(value);
        return _repo.AddAsync(entity);
    }

    public Task<int> UpdateAsync(OrderDetailUpdDto value)
    {
        throw new NotImplementedException();
    }

    public Task<int> DeleteAsync(long id)
    {
        throw new NotImplementedException();
    }
}