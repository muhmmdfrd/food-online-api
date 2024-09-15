using System.Text.Json.Serialization;
using Flozacode.Models.Paginations;

namespace FoodOnline.Core.Dtos;

public class UserDto
{
    public string Name { get; set; } = null!;
    public string Username { get; set; } = null!;
    public int DataStatusId { get; set; }
    public long RoleId { get; set; }
    public long? PositionId { get; set; }
}

public class UserViewDto : UserDto
{
    public long Id { get; set; }
    public string RoleName { get; set; } = null!;
    public string? PositionName { get; set; }
    public string DataStatusName { get; set; } = null!;
}

public class UserAddDto : UserDto
{
    public string Password { get; set; } = null!;
    
    [JsonIgnore]
    public long? CreatedBy { get; set; }
    
    [JsonIgnore]
    public DateTime? CreatedAt { get; set; }
    
    [JsonIgnore]
    public long? ModifiedBy { get; set; }
    
    [JsonIgnore]
    public DateTime? ModifiedAt { get; set; }
}

public class UserUpdDto : UserDto
{
    public long Id { get; set; }
    
    [JsonIgnore]
    public long? ModifiedBy { get; set; }
    
    [JsonIgnore]
    public DateTime? ModifiedAt { get; set; }
}

public class UserFilter : TableFilter
{
}