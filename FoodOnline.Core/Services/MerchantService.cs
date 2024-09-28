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

public class MerchantService : IMerchantService
{
    private readonly IFlozaRepo<Merchant, AppDbContext> _repo;
    private readonly IMapper _mapper;

    public MerchantService(IFlozaRepo<Merchant, AppDbContext> repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<Pagination<MerchantViewDto>> GetPagedAsync(MerchantFilter filter)
    {
        var query = _repo.AsQueryable.AsNoTracking().Where(q => q.DataStatusId == (int)DataStatusEnum.Active);
        var total = query.Count();
        var filtered = total;

        if (!string.IsNullOrEmpty(filter.Keyword))
        {
            query = query.Where(q => q.Name.Contains(filter.Keyword, StringComparison.OrdinalIgnoreCase));
            filtered = query.Count();
        }

        var result = query
            .ProjectTo<MerchantViewDto>(_mapper.ConfigurationProvider)
            .SortBy(filter.SortName, filter.SortDir)
            .Skip(filter.Skip)
            .Take(filter.Size)
            .ToList();

        return new Pagination<MerchantViewDto>
        {
            Size = filter.Size,
            Data = result,
            Filtered = filtered,
            Total = total,
        };
    }

    public async Task<List<MerchantViewDto>> GetListAsync()
    {
        return _repo.AsQueryable
            .AsNoTracking()
            .ProjectTo<MerchantViewDto>(_mapper.ConfigurationProvider)
            .Where(q => q.DataStatusId == (int)DataStatusEnum.Active)
            .ToList();
    }

    public async Task<MerchantViewDto> FindAsync(long id)
    {
        var user = _repo.AsQueryable.AsNoTracking().FirstOrDefault(q => q.Id == id);
        if (user == null)
        {
            throw new RecordNotFoundException("Merchant not found.");
        }
        
        return _mapper.Map<MerchantViewDto>(user);
    }

    public Task<int> CreateAsync(MerchantAddDto value)
    {
        var entity = _mapper.Map<Merchant>(value);
        return _repo.AddAsync(entity);
    }

    public Task<int> UpdateAsync(MerchantUpdDto value)
    {
        var existing = _repo.AsQueryable.FirstOrDefault(q => q.Id == value.Id);
        if (existing == null)
        {
            throw new RecordNotFoundException("Merchant not found.");
        }

        _mapper.Map(value, existing);
        return _repo.UpdateAsync(existing);
    }

    public Task<int> DeleteAsync(long id)
    {
        var entity = _repo.AsQueryable.FirstOrDefault(q => q.Id == id);
        if (entity == null)
        {
            throw new RecordNotFoundException("Merchant not found.");
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
            throw new RecordNotFoundException("Merchant not found.");
        }
        
        entity.ModifiedBy = currentUser.Id;
        entity.ModifiedAt = DateTime.UtcNow;
        entity.DataStatusId = (int)DataStatusEnum.Deleted;

        return _repo.UpdateAsync(entity);
    }
}