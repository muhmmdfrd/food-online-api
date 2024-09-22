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
public class PositionsController : FlozaApiController
{
    private readonly PositionHelper _helper;

    public PositionsController(PositionHelper helper)
    {
        _helper = helper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<Pagination<PositionViewDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPaged([FromQuery] PositionFilter filter)
    {
        var result = await _helper.GetPagedAsync(filter);
        return ApiOK(result);
    }
    
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<PositionViewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Find([FromRoute] long id)
    {
        var result = await _helper.FindAsync(id);
        return ApiOK(result);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] PositionAddDto dto)
    {
        var affected = await _helper.CreateAsync(dto, CurrentUser);
        if (affected <= 0)
        {
            return ApiDataInvalid("Position not created.");
        }

        return ApiCreated();
    }
    
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromBody] PositionUpdDto dto)
    {
        var affected = await _helper.UpdateAsync(dto, CurrentUser);
        if (affected <= 0)
        {
            return ApiDataInvalid("Position not updated.");
        }

        return ApiOK("Position updated.");
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
            return ApiDataInvalid("Position not deleted.");
        }

        return ApiOK("Position deleted.");
    }
}