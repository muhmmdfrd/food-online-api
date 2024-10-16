﻿using FoodOnline.Api.Commons;
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
    public async Task<IActionResult> Login([FromBody] AuthRequestDto request)
    {
        var result = await _authHelper.AuthAsync(request);
        if (result == null)
        {
            return ApiUnauthorized();
        }
        
        return ApiOK(result);
    }
}