using AutoMapper;
using AutoMapper.QueryableExtensions;
using Flozacode.Exceptions;
using Flozacode.Extensions.SortExtension;
using Flozacode.Models.Paginations;
using Flozacode.Repository;
using FoodOnline.Core.Dtos;
using FoodOnline.Core.Enums;
using FoodOnline.Core.Interfaces;
using FoodOnline.Core.Models;
using FoodOnline.Repository.Contexts;
using FoodOnline.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodOnline.Core.Services;

public class MenuService : IMenuService
{
    private readonly IFlozaRepo<Menu, AppDbContext> _repo;
    private readonly IMapper _mapper;

    public MenuService(IFlozaRepo<Menu, AppDbContext> repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<Pagination<MenuViewDto>> GetPagedAsync(MenuFilter filter)
    {
        var query = _repo.AsQueryable.AsNoTracking().Where(q => q.DataStatusId == (int)DataStatusEnum.Active);
        var total = query.Count();
        var filtered = total;

        if (!string.IsNullOrEmpty(filter.Keyword))
        {
            query = query.Where(q => q.Name.Contains(filter.Keyword, StringComparison.OrdinalIgnoreCase));
            filtered = query.Count();
        }

        if (filter.MerchantId != null)
        {
            query = query.Where(q => q.MerchantId == filter.MerchantId);
            filtered = query.Count();
        }

        var result = query
            .ProjectTo<MenuViewDto>(_mapper.ConfigurationProvider)
            .SortBy(filter.SortName, filter.SortDir)
            .Skip(filter.Skip)
            .Take(filter.Size)
            .ToList();

        return new Pagination<MenuViewDto>
        {
            Size = filter.Size,
            Data = result,
            Filtered = filtered,
            Total = total,
        };
    }

    public Task<List<MenuViewDto>> GetListAsync()
    {
        var entities = _repo.AsQueryable
            .AsNoTracking()
            .Where(q => q.DataStatusId == (int)DataStatusEnum.Active)
            .ToList();

        return Task.FromResult(_mapper.Map<List<MenuViewDto>>(entities));
    }

    public async Task<MenuViewDto> FindAsync(long id)
    {
        var menu = _repo.AsQueryable.AsNoTracking().ProjectTo<MenuViewDto>(_mapper.ConfigurationProvider).FirstOrDefault(q => q.Id == id);
        if (menu == null)
        {
            throw new RecordNotFoundException("Menu not found.");
        }

        return menu;
    }

    public Task<int> CreateAsync(MenuAddDto value)
    {
        var entity = _mapper.Map<Menu>(value);
        return _repo.AddAsync(entity);
    }

    public Task<int> UpdateAsync(MenuUpdDto value)
    {
        var existing = _repo.AsQueryable.FirstOrDefault(q => q.Id == value.Id);
        if (existing == null)
        {
            throw new RecordNotFoundException("Menu not found.");
        }

        _mapper.Map(value, existing);
        return _repo.UpdateAsync(existing);
    }

    public Task<int> DeleteAsync(long id)
    {
        var entity = _repo.AsQueryable.FirstOrDefault(q => q.Id == id);
        if (entity == null)
        {
            throw new RecordNotFoundException("Menu not found.");
        }

        return _repo.DeleteAsync(entity);
    }

    public Task<int> DeleteAsync(long id, CurrentUser currentUser, bool isHardDelete = false)
    {
        if (isHardDelete)
        {
            return DeleteAsync(id);
        }

        var entity = _repo.AsQueryable.FirstOrDefault(q => q.Id == id);
        if (entity == null)
        {
            throw new RecordNotFoundException("Menu not found.");
        }
        
        entity.ModifiedBy = currentUser.Id;
        entity.ModifiedAt = DateTime.UtcNow;
        entity.DataStatusId = (int)DataStatusEnum.Deleted;

        return _repo.UpdateAsync(entity);
    }
}