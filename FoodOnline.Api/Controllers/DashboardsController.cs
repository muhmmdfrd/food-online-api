using FoodOnline.Api.Commons;
using FoodOnline.Api.Models;
using FoodOnline.Core.Dtos;
using FoodOnline.Core.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOnline.Api.Controllers;

[ApiController]
[Route("[controller]")]
// [Authorize]
[AllowAnonymous]
public class DashboardsController : FlozaApiController
{
    private readonly DashboardHelper _helper;

    public DashboardsController(DashboardHelper helper)
    {
        _helper = helper;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<DashboardDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDashboard()
    {
        var result = await _helper.GetDashboardAsync();
        return ApiOK(result);
    }
}