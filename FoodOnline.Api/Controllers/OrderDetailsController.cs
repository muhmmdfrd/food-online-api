using FoodOnline.Api.Commons;
using FoodOnline.Api.Models;
using FoodOnline.Core.Dtos;
using FoodOnline.Core.Enums;
using FoodOnline.Core.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOnline.Api.Controllers;

[ApiController]
[Route("order-details")]
[Authorize]
public class OrderDetailsController : FlozaApiController
{
    private readonly OrderDetailHelper _helper;

    public OrderDetailsController(OrderDetailHelper helper)
    {
        _helper = helper;
    }

    [HttpGet("today")]
    [ProducesResponseType(typeof(ApiResponse<List<OrderDetailGroupByUser>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOrderToday()
    {
        var result = await _helper.GetOrderToday(CurrentUser);
        return ApiOK(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] OrderDetailAddRequestDto dto)
    {
        var result = await _helper.CreateAsync(dto, CurrentUser);

        if (result != OrderResponseEnum.Success)
        {
            return ApiOrderFailed(result);
        }
        
        return ApiCreated();
    }
    
    [HttpPost("calculate")]
    [ProducesResponseType(typeof(ApiResponse<OrderDetailCaculateResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Calculate([FromBody] List<OrderDetailAddChildDto> dto)
    {
        var result = await _helper.CalculateAsync(dto);
        return ApiOK(result);
    }
}