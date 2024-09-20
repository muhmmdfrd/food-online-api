using FoodOnline.Core.Dtos;

namespace FoodOnline.Core.Interfaces;

public interface IAuthService
{
    public Task<UserViewDto?> AuthAsync(AuthRequestDto request);
}