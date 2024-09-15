using FoodOnline.Core.Interfaces;

namespace FoodOnline.Core.Helpers;

public class UserSessionHelper
{
    private readonly IUserSessionService _service;

    public UserSessionHelper(IUserSessionService service)
    {
        _service = service;
    }

    public Task<string?> CreateAsync(long userId)
    {
        return _service.CreateAsync(userId);
    }

    public bool CheckCodeAsync(string code)
    {
        return _service.CheckCode(code);
    }
}