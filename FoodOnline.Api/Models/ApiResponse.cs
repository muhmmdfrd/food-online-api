using FoodOnline.Api.Constants;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FoodOnline.Api.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; } = true;
    public string? Code { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; } = default;

    public ApiResponse()
    {

    }

    public ApiResponse(bool success, T? data, string code, string message)
    {
        Success = success;
        Code = code;
        Message = message;
        Data = data;
    }

    public ApiResponse<T> SuccessResponse(T data)
    {
        return new ApiResponse<T>(true, data, ResponseConstant.SUCCESS_CODE, "Success.");
    }

    public ApiResponse<T> SuccessResponse(T data, string message)
    {
        return new ApiResponse<T>(true, data, ResponseConstant.SUCCESS_CODE, message);
    }

    public ApiResponse<T> Fail(string message)
    {
        return new ApiResponse<T>(false, default, ResponseConstant.INTERNAL_SERVER_ERROR_CODE, message);
    }

    public ApiResponse<T> Fail(string message, string code)
    {
        return new ApiResponse<T>(false, default, code, message);
    }

    public ApiResponse<T> Unauthorized()
    {
        return new ApiResponse<T>(false, default, ResponseConstant.UNAUTHORIZED_CODE, ResponseConstant.UNAUTHORIZED_MESSAGE);
    }

    public ApiResponse<T> Forbidden()
    {
        return new ApiResponse<T>(false, default, ResponseConstant.FORBIDDEN_CODE, ResponseConstant.FORBIDDEN_MESSAGE);
    }
    
    public override string ToString()
    {
        DefaultContractResolver contractResolver = new()
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };

        return JsonConvert.SerializeObject(this, new JsonSerializerSettings
        {
            ContractResolver = contractResolver,
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore,
        });
    }
}