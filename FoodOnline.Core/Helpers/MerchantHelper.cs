using System.Transactions;
using Flozacode.Models.Paginations;
using FoodOnline.Core.Dtos;
using FoodOnline.Core.Interfaces;
using FoodOnline.Core.Models;

namespace FoodOnline.Core.Helpers;

public class MerchantHelper
{
    private readonly IMerchantService _service;

    public MerchantHelper(IMerchantService service)
    {
        _service = service;
    }

    public Task<Pagination<MerchantViewDto>> GetPagedAsync(MerchantFilter filter)
    {
        return _service.GetPagedAsync(filter);
    }

    public Task<List<MerchantViewDto>> GetListAsync()
    {
        return _service.GetListAsync();
    }

    public Task<MerchantViewDto> FindAsync(long id)
    {
        return _service.FindAsync(id);
    }

    public async Task<int> CreateAsync(MerchantAddDto value, CurrentUser currentUser)
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

    public Task<int> UpdateAsync(MerchantUpdDto value, CurrentUser currentUser)
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