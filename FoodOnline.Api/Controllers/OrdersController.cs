using Flozacode.Models.Paginations;
using FoodOnline.Api.Commons;
using FoodOnline.Api.Models;
using FoodOnline.Core.Dtos;
using FoodOnline.Core.Enums;
using FoodOnline.Core.Helpers;
using FoodOnline.Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOnline.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class OrdersController : FlozaApiController
{
    private readonly OrderHelper _helper;

    public OrdersController(OrderHelper helper)
    {
        _helper = helper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<Pagination<OrderViewDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPaged([FromQuery] OrderFilter filter)
    {
        var result = await _helper.GetPagedAsync(filter);
        return ApiOK(result);
    }
    
    [HttpGet("user/{userId:long}")]
    [ProducesResponseType(typeof(ApiResponse<List<OrderViewDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status500InternalServerError)]
    public IActionResult GetMyOrder([FromRoute] long userId)
    {
        var result = _helper.GetMyOrder(userId);
        return ApiOK(result);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create()
    {
        var dto = new OrderAddDto
        {
            Code = OrderUtils.GenerateCode(),
            Date = DateTime.UtcNow,
            StatusId = (int)OrderStatusEnum.Active,
        };
        
        var affected = await _helper.CreateAsync(dto, CurrentUser);
        if (affected <= 0)
        {
            return ApiDataInvalid("Order not created.");
        }

        if (affected == (int)OrderResponseEnum.Active)
        {
            return ApiDataInvalid($"Order {dto.Code} still active");
        }

        return ApiCreated();
    }
}