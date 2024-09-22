using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Transactions;
using FoodOnline.Core.Dtos;
using FoodOnline.Core.Interfaces;
using FoodOnline.Core.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FoodOnline.Core.Helpers;

public class AuthHelper
{
    private readonly IAuthService _service;
    private readonly IUserService _userService;
    private readonly UserSessionHelper _userSessionHelper;
    private readonly JwtConfigs _jwtConfigs;

    public AuthHelper(IAuthService service, UserSessionHelper userSessionHelper, IOptions<JwtConfigs> jwtConfigs, IUserService userService)
    {
        _service = service;
        _userSessionHelper = userSessionHelper;
        _userService = userService;
        _jwtConfigs = jwtConfigs.Value;
    }

    public async Task<AuthResponseDto?> AuthAsync(AuthRequestDto request)
    {
        var user = await _service.AuthAsync(request);
        if (user == null)
        {
            return null;
        }

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        {
            var sessionCode = await _userSessionHelper.CreateAsync(user.Id);
            if (string.IsNullOrEmpty(sessionCode))
            {
                return null;
            }

            var token = GenerateToken(user, sessionCode);

            transaction.Complete();

            return new AuthResponseDto
            {
                User = user,
                Code = sessionCode,
                Token = token,
            };
        }
    }
    
    public async Task<AuthRevokeResponseDto?> RevokeAsync(AuthRevokeRequestDto request)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        {
            var valid = _userSessionHelper.CheckCode(request.Code);
            if (valid == false)
            {
                return null;
            }
            
            var existing = await _userSessionHelper.InvalidateSessionAsync(request.Code);
            if (existing <= 0)
            {
                return null;
            }
            
            var sessionCode = await _userSessionHelper.CreateAsync(request.UserId);
            if (string.IsNullOrEmpty(sessionCode))
            {
                return null;
            }

            var user = await _userService.FindAsync(request.UserId);
            if (user.Id <= 0)
            {
                return null;
            }
            
            var token = GenerateToken(user, sessionCode);
            transaction.Complete();

            return new AuthRevokeResponseDto
            {
                Token = token,
                Code = sessionCode,
            };
        }
    }
    
    private string GenerateToken(UserViewDto user, string sessionCode)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtConfigs.TokenSecret);

        var claims = new List<Claim> 
        {
            new Claim("Id", user.Id.ToString()),
            new Claim("Username", user.Username),
            new Claim("Name", user.Name),
            new Claim("sessionCode", sessionCode)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims.ToArray()),
            Expires = DateTime.UtcNow.AddSeconds(_jwtConfigs.TokenLifeTimes),
            Issuer = _jwtConfigs.Issuer,
            Audience = _jwtConfigs.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}