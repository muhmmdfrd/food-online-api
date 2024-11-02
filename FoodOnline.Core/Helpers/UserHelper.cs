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

public class UserHelper
{
    private readonly IUserService _service;
    private readonly RabbitMqConfigs _rabbitMqConfigs;
    private readonly JsonSerializerSettings _jsonSettings;

    public UserHelper(IUserService service, IOptions<RabbitMqConfigs> rabbitMqConfigs)
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
            value.Code = Uuid7.Guid().ToString();
        
            var result = await _service.CreateAsync(value);

            if (!string.IsNullOrEmpty(value.File))
            {
                using var publisher = new Publisher(_rabbitMqConfigs);
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
                };
            }
            
            transaction.Complete();
            
            return result;
        }
    }

    public Task<int> UpdateAsync(UserUpdDto value, CurrentUser currentUser)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        {
            value.ModifiedBy = currentUser.Id;
            value.ModifiedAt = DateTime.UtcNow;

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

            var result = _service.UpdateAsync(value);
            transaction.Complete();
            return result;
        }
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