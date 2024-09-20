using Newtonsoft.Json;
using StackExchange.Redis;

namespace FoodOnline.Core.Services.RedisCaching;

public class RedisService
{
    private readonly IDatabase _db;

    public RedisService(string connectionString)
    {
        var redis = ConnectionMultiplexer.Connect(connectionString);
        _db = redis.GetDatabase();
    }

    public async Task WriteToListAsync<T>(string key, List<T> data)
    { 
        foreach (var item in data)
        {
            var json = JsonConvert.SerializeObject(item);
            await _db.ListLeftPushAsync(key, json);
        }
    }

    public async Task WriteAsync<T>(string key, T data)
    {
        var json = JsonConvert.SerializeObject(data);
        await _db.ListLeftPushAsync(key, json);
    }

    public async Task<List<T>> ReadFromListAsync<T>(string key)
    {
        var length = await _db.ListLengthAsync(key);
        var result = new List<T>();

        for (var i = 0; i < length; i++)
        {
            var json = await _db.ListRightPopAsync(key);
            if (json.HasValue)
            {
                var data = JsonConvert.DeserializeObject<T>(json!);
                result.Add(data!);
            }
        }

        return result;
    }
}