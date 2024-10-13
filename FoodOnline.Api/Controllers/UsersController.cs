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
    
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<UserViewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Find([FromRoute] long id)
    {
        var result = await _helper.FindAsync(id);
        return ApiOK(result);
    }
    
    [HttpGet("find/{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<UserViewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> FindA([FromRoute] long id)
    {
        var result = await _helper.FindAsync(id);
        return Ok(result);
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
    
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromBody] UserUpdDto dto)
    {
        var affected = await _helper.UpdateAsync(dto, CurrentUser);
        if (affected <= 0)
        {
            return ApiDataInvalid("User not updated.");
        }

        return ApiOK("User updated.");
    }
    
    [HttpPut("firebase-token")]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateFirebaseToken([FromBody] UserUpdTokenRequest dto)
    {
        var affected = await _helper.UpdateFirebaseTokenAsync(CurrentUser.Id, dto.Token);
        if (affected <= 0)
        {
            return ApiDataInvalid("Token not updated.");
        }

        return ApiOK("Token updated.");
    }
    
    [HttpDelete("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromRoute] long id)
    {
        var affected = await _helper.DeleteAsync(id, CurrentUser, false);
        if (affected <= 0)
        {
            return ApiDataInvalid("User not deleted.");
        }

        return ApiOK("User deleted.");
    }
    
}