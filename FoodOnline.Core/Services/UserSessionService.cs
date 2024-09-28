using Flozacode.Helpers.StringHelper;
using Flozacode.Repository;
using FoodOnline.Core.Constants;
using FoodOnline.Core.Interfaces;
using FoodOnline.Core.Services.RedisCaching;
using FoodOnline.Repository.Contexts;
using FoodOnline.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodOnline.Core.Services;

public class UserSessionService : IUserSessionService
{
    private readonly IFlozaRepo<UserSession, AppDbContext> _repo;

    public UserSessionService(IFlozaRepo<UserSession, AppDbContext> repo)
    {
        _repo = repo;
    }

    public async Task<string?> CreateAsync(long userId)
    {
        var entity = new UserSession
        {
            UserId = userId,
            Code = FlozaString.GenerateRandomString(10),
            CreatedAt = DateTime.UtcNow,
            IsValid = true,
        };

        var affected = await _repo.AddAsync(entity);
        return affected <= 0 ? null : entity.Code;
        return entity.Code;
    }

    public bool CheckCode(string code)
    {
        return _repo.AsQueryable.AsNoTracking().Any(q => q.Code == code && q.IsValid);
    }

    public async Task<int?> InvalidateSessionAsync(string code)
    {
        var existing = _repo.AsQueryable.FirstOrDefault(q => q.Code == code);
        if (existing == null) return null;
       
        existing.IsValid = false;
       
       return await _repo.UpdateAsync(existing);
    }

    public async Task<string?> GetLastSessionAsync(long userId)
    {
        var existing = _repo.AsQueryable.FirstOrDefault(q => q.UserId == userId && q.IsValid);
        if (existing == null) return null;

        return existing.Code;
    }
}