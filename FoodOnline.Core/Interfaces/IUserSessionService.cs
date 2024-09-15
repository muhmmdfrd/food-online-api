namespace FoodOnline.Core.Interfaces;

public interface IUserSessionService
{
    public Task<string?> CreateAsync(long userId);
    public bool CheckCode(string code);
}