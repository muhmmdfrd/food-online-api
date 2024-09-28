using FoodOnline.Api.Commons;
using FoodOnline.Api.Models;
using FoodOnline.Core.Dtos;
using FoodOnline.Core.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOnline.Api.Controllers;

[ApiController]
[Route("[controller]")]
[AllowAnonymous]
public class AuthController : FlozaApiController
{
    private readonly AuthHelper _authHelper;

    public AuthController(AuthHelper authHelper)
    {
        _authHelper = authHelper;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] AuthRequestDto request)
    {
        var result = await _authHelper.AuthAsync(request);
        if (result == null)
        {
            return ApiUnauthorized();
        }
        
        return ApiOK(result);
    }
    
    [HttpPost("revoke")]
    [ProducesResponseType(typeof(ApiResponse<AuthRevokeResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Revoke([FromBody] AuthRevokeRequestDto request)
    {
        var result = await _authHelper.RevokeAsync(request);
        if (result == null)
        {
            return ApiUnauthorized();
        }
        
        return ApiOK(result);
    }
    
    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResponse<AuthRevokeResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout([FromBody] LogoutRequestDto request)
    {
        var result = await _authHelper.LogoutAsync(request);
        if (result)
        {
            return ApiOK("User logged out.");
        }

        return ApiDataInvalid("Logout failed.");
    }
}