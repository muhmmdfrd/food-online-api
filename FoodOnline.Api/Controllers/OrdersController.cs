using FirebaseAdmin.Messaging;
using Flozacode.Models.Paginations;
using FoodOnline.Api.Commons;
using FoodOnline.Api.Models;
using FoodOnline.Core.Dtos;
using FoodOnline.Core.Enums;
using FoodOnline.Core.Fcm;
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
    private readonly FirebaseHelper _firebaseHelper;

    public OrdersController(OrderHelper helper, FirebaseHelper firebaseHelper)
    {
        _helper = helper;
        _firebaseHelper = firebaseHelper;
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
    [ProducesResponseType(typeof(ApiResponse<List<OrderViewHistory>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status500InternalServerError)]
    public IActionResult GetMyOrder([FromRoute] long userId)
    {
        var result = _helper.GetMyOrder(userId);
        return ApiOK(result);
    }
    
    [HttpGet("user/{userId:long}/detail/{orderId:long}")]
    [ProducesResponseType(typeof(ApiResponse<List<OrderViewDetailHistory>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status500InternalServerError)]
    public IActionResult GetMyOrderDetail([FromRoute] long userId, long orderId)
    {
        var result = _helper.GetOrderViewDetailHistory(userId, orderId);
        if (result == null)
        {
            return ApiDataInvalid("Order not found");
        }
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

    [HttpPost("firebase")]
    [AllowAnonymous]
    public async Task<IActionResult> SendFirebase([FromBody] UserUpdTokenRequest request)
    {
        try
        {
            var notification = new Notification
            {
                Body = "Ini contoh aja.",
                Title = "Contoh"
            };
        
            var result = await _firebaseHelper.SendMessageAsync(notification, request.Token);
            return ApiOK();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ApiDataInvalid(e.Message);
        }
       
    }
}