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

public class UserService : IUserService
{
    private readonly IFlozaRepo<User, AppDbContext> _repo;
    private readonly IMapper _mapper;

    public UserService(IFlozaRepo<User, AppDbContext> repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<Pagination<UserViewDto>> GetPagedAsync(UserFilter filter)
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
            .ProjectTo<UserViewDto>(_mapper.ConfigurationProvider)
            .SortBy(filter.SortName, filter.SortDir)
            .Skip(filter.Skip)
            .Take(filter.Size)
            .ToList();

        return new Pagination<UserViewDto>
        {
            Size = filter.Size,
            Data = result,
            Filtered = filtered,
            Total = total,
        };
    }

    public async Task<List<UserViewDto>> GetListAsync()
    {
        return _repo.AsQueryable
            .AsNoTracking()
            .ProjectTo<UserViewDto>(_mapper.ConfigurationProvider)
            .Where(q => q.DataStatusId == (int)DataStatusEnum.Active)
            .ToList();
    }

    public async Task<UserViewDto> FindAsync(long id)
    {
        var user = _repo.AsQueryable.AsNoTracking().ProjectTo<UserViewDto>(_mapper.ConfigurationProvider).FirstOrDefault(q => q.Id == id);
        if (user == null)
        {
            throw new RecordNotFoundException("User not found.");
        }

        return user;
    }

    public Task<int> CreateAsync(UserAddDto value)
    {
        var entity = _mapper.Map<User>(value);
        return _repo.AddAsync(entity);
    }

    public Task<int> UpdateAsync(UserUpdDto value)
    {
        var existing = _repo.AsQueryable.FirstOrDefault(q => q.Id == value.Id);
        if (existing == null)
        {
            throw new RecordNotFoundException("User not found.");
        }

        _mapper.Map(value, existing);
        return _repo.UpdateAsync(existing);
    }

    public Task<int> DeleteAsync(long id)
    {
        var entity = _repo.AsQueryable.FirstOrDefault(q => q.Id == id);
        if (entity == null)
        {
            throw new RecordNotFoundException("User not found.");
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
            throw new RecordNotFoundException("User not found.");
        }
        
        entity.ModifiedBy = currentUser.Id;
        entity.ModifiedAt = DateTime.UtcNow;
        entity.DataStatusId = (int)DataStatusEnum.Deleted;

        return _repo.UpdateAsync(entity);
    }

    public Task<int> UpdateFirebaseTokenAsync(long id, string token)
    {
        var user = _repo.AsQueryable.FirstOrDefault(q => q.Id == id && q.DataStatusId == (int)DataStatusEnum.Active);
        if (user == null)
        {
            return Task.FromResult(0);
        }

        user.FirebaseToken = token;
        user.ModifiedBy = id;
        user.ModifiedAt = DateTime.UtcNow;
        
        return _repo.UpdateAsync(user);
    }
}