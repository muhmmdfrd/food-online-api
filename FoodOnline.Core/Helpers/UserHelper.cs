using System.Transactions;
using Flozacode.Models.Paginations;
using FoodOnline.Core.Dtos;
using FoodOnline.Core.Interfaces;
using FoodOnline.Core.Models;

namespace FoodOnline.Core.Helpers;

public class UserHelper
{
    private readonly IUserService _service;

    public UserHelper(IUserService service)
    {
        _service = service;
    }

    public Task<Pagination<UserViewDto>> GetPagedAsync(UserFilter filter)
    {
        return _service.GetPagedAsync(filter);
    }

    public Task<List<UserViewDto>> GetListAsync()
    {
        return _service.GetListAsync();
    }

    public Task<UserViewDto> FindAsync(long id)
    {
        return _service.FindAsync(id);
    }

    public async Task<int> CreateAsync(UserAddDto value, CurrentUser currentUser)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        {
            var now = DateTime.UtcNow;

            value.Password = BCrypt.Net.BCrypt.HashPassword(value.Password);
            value.CreatedBy = currentUser.Id;
            value.CreatedAt = now;
            value.ModifiedBy = currentUser.Id;
            value.ModifiedAt = now;
        
            var result = await _service.CreateAsync(value);
            transaction.Complete();
            
            return result;
        }
    }

    public Task<int> UpdateAsync(UserUpdDto value, CurrentUser currentUser)
    {
        value.ModifiedBy = currentUser.Id;
        value.ModifiedAt = DateTime.UtcNow;

        return _service.UpdateAsync(value);
    }
    
    public Task<int> UpdateFirebaseTokenAsync(long id, string token)
    {
        return _service.UpdateFirebaseTokenAsync(id, token);
    }

    public Task<int> DeleteAsync(long id, CurrentUser currentUser, bool isHardDelete)
    {
        return _service.DeleteAsync(id, currentUser, isHardDelete);
    }
}