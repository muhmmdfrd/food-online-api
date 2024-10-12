using FoodOnline.Core.Dtos;
using FoodOnline.Repository.Entities;

namespace FoodOnline.Core.Interfaces;

public interface IAuthService
{
    public Task<User?> AuthAsync(AuthRequestDto request);
}