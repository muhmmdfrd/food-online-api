using Flozacode.Models.Paginations;
using Microsoft.AspNetCore.Http;

namespace FoodOnline.Core.Dtos;

public class MenuDto
{
    public string Name { get; set; } = null!;
    public int DataStatusId { get; set; }
    public int Price { get; set; }
    public long MerchantId { get; set; }
    public string? Description { get; set; }
    public string? Code { get; set; }
}

public class MenuViewDto : MenuDto
{
    public long Id { get; set; }
    public string DataStatusName { get; set; } = null!;
    public string MerchantName { get; set; } = null!;
}

public class MenuAddDto : MenuDto
{
    public string? File { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public long? ModifiedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
}

public class MenuUpdDto : MenuDto
{
    public long Id { get; set; }
    public string? File { get; set; }
    public long? ModifiedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
}

public class MenuFilter : TableFilter
{
    public long? MerchantId { get; set; }
}