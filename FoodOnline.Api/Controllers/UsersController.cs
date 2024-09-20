using Flozacode.Models.Paginations;
using FoodOnline.Api.Commons;
using FoodOnline.Api.Models;
using FoodOnline.Core.Dtos;
using FoodOnline.Core.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOnline.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class UsersController : FlozaApiController
{
    private readonly UserHelper _helper;

    public UsersController(UserHelper helper)
    {
        _helper = helper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<Pagination<UserViewDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPaged([FromQuery] UserFilter filter)
    {
        var result = await _helper.GetPagedAsync(filter);
        return ApiOK(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] UserAddDto dto)
    {
        var affected = await _helper.CreateAsync(dto, CurrentUser);
        if (affected <= 0)
        {
            return ApiDataInvalid("User not created.");
        }

        return ApiCreated();
    }
}