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
public class RolesController : FlozaApiController
{
    private readonly RoleHelper _helper;

    public RolesController(RoleHelper helper)
    {
        _helper = helper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<Pagination<RoleViewDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPaged([FromQuery] RoleFilter filter)
    {
        var result = await _helper.GetPagedAsync(filter);
        return ApiOK(result);
    }
}