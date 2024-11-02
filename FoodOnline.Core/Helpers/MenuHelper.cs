using System.Transactions;
using Flozacode.Models.Paginations;
using FoodOnline.Core.Dtos;
using FoodOnline.Core.Enums;
using FoodOnline.Core.Interfaces;
using FoodOnline.Core.MessageBroker;
using FoodOnline.Core.Models;
using FoodOnline.Core.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UuidExtensions;

namespace FoodOnline.Core.Helpers;

public class MenuHelper
{
    private readonly IMenuService _service;
    private readonly RabbitMqConfigs _rabbitMqConfigs;
    private readonly JsonSerializerSettings _jsonSettings;

    public MenuHelper(IMenuService service, IOptions<RabbitMqConfigs> rabbitMqConfigs)
    {
        _service = service;
        _rabbitMqConfigs = rabbitMqConfigs.Value;
        _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy { ProcessDictionaryKeys = true }
            },
            Formatting = Formatting.Indented
        };
    }
    
    public Task<Pagination<MenuViewDto>> GetPagedAsync(MenuFilter filter)
    {
        return _service.GetPagedAsync(filter);
    }

    public Task<List<MenuViewDto>> GetListAsync()
    {
        return _service.GetListAsync();
    }

    public Task<MenuViewDto> FindAsync(long id)
    {
        return _service.FindAsync(id);
    }

    public async Task<long> CreateAsync(MenuAddDto value, CurrentUser currentUser)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        {
            var now = DateTime.UtcNow;

            value.CreatedBy = currentUser.Id;
            value.CreatedAt = now;
            value.ModifiedBy = currentUser.Id;
            value.ModifiedAt = now;
            value.Code = Uuid7.Guid().ToString();
        
            var result = await _service.CreateAndGetIdAsync(value);
            if (result <= 0)
            {
                return 0;
            }

            if (value.File == null)
            {
                return 0;
            }

            using var publisher = new Publisher(_rabbitMqConfigs);
            {
                if (!string.IsNullOrEmpty(value.File))
                {
                    var request = JsonConvert.SerializeObject(new MessageMenuRequest
                    {
                        Note = "",
                        ReferenceId = result,
                        UploadType = (int)FileTypeEnum.File,
                        File = value.File,
                        UniqueId = value.Code,
                    }, _jsonSettings);
                
                    publisher.Publish("upload-image", "upload-image-menu", request);
                }
            }
            
            transaction.Complete();
            
            return result;
        }
    }

    public async Task<int> UpdateAsync(MenuUpdDto value, CurrentUser currentUser)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        {
            value.ModifiedBy = currentUser.Id;
            value.ModifiedAt = DateTime.UtcNow;

            var exist = await _service.FindAsync(value.Id);
            if (exist != null && string.IsNullOrEmpty(exist.Code))
            {
                value.Code = Uuid7.Guid().ToString();
            }
            
            using var publisher = new Publisher(_rabbitMqConfigs);
            {
                if (!string.IsNullOrEmpty(value.File))
                {
                    var request = JsonConvert.SerializeObject(new MessageMenuRequest
                    {
                        Note = "",
                        ReferenceId = value.Id,
                        UploadType = (int)FileTypeEnum.File,
                        File = value.File,
                        UniqueId = value.Code!,
                    }, _jsonSettings);
                
                    publisher.Publish("upload-image", "upload-image-menu", request);
                }
            }
            
            var result = await _service.UpdateAsync(value);
            transaction.Complete();
            return result;
        }
    }

    public Task<int> DeleteAsync(long id, CurrentUser currentUser, bool isHardDelete)
    {
        return _service.DeleteAsync(id, currentUser, isHardDelete);
    }
}