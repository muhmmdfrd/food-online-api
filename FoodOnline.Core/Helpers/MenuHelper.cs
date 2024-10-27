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

    public MenuHelper(IMenuService service, IOptions<RabbitMqConfigs> rabbitMqConfigs)
    {
        _service = service;
        _rabbitMqConfigs = rabbitMqConfigs.Value;
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
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy { ProcessDictionaryKeys = true }
                    },
                    Formatting = Formatting.Indented
                };
                
                var length = value.File.Length;
                if (length < 0)
                {
                    return 0;
                }

                using var fileStream = value.File.OpenReadStream();
                {
                    var cts = new CancellationToken();
                    var bytes = new byte[length];
                    await fileStream.ReadAsync(bytes, 0, (int)value.File.Length, cts);
                    var base64 = Convert.ToBase64String(bytes);

                    var request = JsonConvert.SerializeObject(new MessageMenuRequest
                    {
                        Note = "",
                        ReferenceId = result,
                        UploadType = (int)FileTypeEnum.File,
                        File = base64,
                        UniqueId = value.Code,
                    }, settings);
                
                    publisher.Publish("upload-image", "upload-image-menu", request);
                }
            };
            
            transaction.Complete();
            
            return result;
        }
    }

    public Task<int> UpdateAsync(MenuUpdDto value, CurrentUser currentUser)
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