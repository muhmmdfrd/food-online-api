using System.Transactions;
using Flozacode.Models.Paginations;
using FoodOnline.Core.Dtos;
using FoodOnline.Core.Interfaces;
using FoodOnline.Core.Models;

namespace FoodOnline.Core.Helpers;

public class PositionHelper
{
    private readonly IPositionService _service;

    public PositionHelper(IPositionService service)
    {
        _service = service;
    }

    public Task<Pagination<PositionViewDto>> GetPagedAsync(PositionFilter filter)
    {
        return _service.GetPagedAsync(filter);
    }

    public Task<List<PositionViewDto>> GetListAsync()
    {
        return _service.GetListAsync();
    }

    public Task<PositionViewDto> FindAsync(long id)
    {
        return _service.FindAsync(id);
    }

    public async Task<int> CreateAsync(PositionAddDto value, CurrentUser currentUser)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        {
            var now = DateTime.UtcNow;

            value.CreatedBy = currentUser.Id;
            value.CreatedAt = now;
            value.ModifiedBy = currentUser.Id;
            value.ModifiedAt = now;
        
            var result = await _service.CreateAsync(value);
            transaction.Complete();
            
            return result;
        }
    }

    public Task<int> UpdateAsync(PositionUpdDto value, CurrentUser currentUser)
    {
        value.ModifiedBy = currentUser.Id;
        value.ModifiedAt = DateTime.UtcNow;

        return _service.UpdateAsync(value);
    }

    public Task<int> DeleteAsync(long id, CurrentUser currentUser, bool isHardDelete)
    {
        return _service.DeleteAsync(id, currentUser, isHardDelete);
    }
}