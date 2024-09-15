using AutoMapper;
using BCrypt.Net;
using Flozacode.Repository;
using FoodOnline.Core.Dtos;
using FoodOnline.Core.Enums;
using FoodOnline.Core.Interfaces;
using FoodOnline.Repository.Contexts;
using FoodOnline.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodOnline.Core.Services;

public class AuthService : IAuthService
{
    private readonly IFlozaRepo<User, AppDbContext> _repo;
    private readonly IMapper _mapper;

    public AuthService(IFlozaRepo<User, AppDbContext> repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }
    
    public async Task<UserViewDto?> AuthAsync(AuthRequestDto request)
    {
        var user = _repo.AsQueryable
            .AsNoTracking()
            .FirstOrDefault(q => 
                q.Username == request.Username &&
                q.DataStatusId == (int)DataStatusEnum.Active);

        if (user == null)
        {
            return null;
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            return null;
        }
        
        return _mapper.Map<UserViewDto>(user);
    }
}