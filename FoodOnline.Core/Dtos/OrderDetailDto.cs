using Flozacode.Models.Paginations;
using Newtonsoft.Json;

namespace FoodOnline.Core.Dtos;

public class OrderDetailDto
{
    public long OrderId { get; set; }
    public long UserId { get; set; }
    public string UserName { get; set; } = null!;
    public long MenuId { get; set; }
    public string MenuName { get; set; } = null!;
    public int Price { get; set; }
    public int Qty { get; set; }
    public int Total { get; set; }
}

public class OrderDetailViewDto : OrderDetailDto
{
    public long Id { get; set; }
}

public class OrderDetailAddDto : OrderDetailDto
{
    [JsonIgnore]
    public long? CreatedBy { get; set; }

    [JsonIgnore]
    public DateTime? CreatedAt { get; set; }

    [JsonIgnore]
    public long? ModifiedBy { get; set; }

    [JsonIgnore]
    public DateTime? ModifiedAt { get; set; }
}

public class OrderDetailUpdDto : OrderDetailDto
{
    public long Id { get; set; }
    
    [JsonIgnore]
    public long? ModifiedBy { get; set; }

    [JsonIgnore]
    public DateTime? ModifiedAt { get; set; }
}

public class OrderDetailFilter : TableFilter
{
    public long OrderId { get; set; }
}

public class OrderDetailAddRequestDto
{
    public List<OrderDetailAddChildDto> Details { get; set; } = null!;
}

public class OrderDetailAddChildDto
{
    public long MenuId { get; set; }
    public int Qty { get; set; }
}

public class OrderDetailCaculateResultDto
{
    public int GrandTotal { get; set; }
    public List<OrderDetailCaculateResultItemDto> Items { get; set; } = [];
}

public class OrderDetailCaculateResultItemDto
{
    public string MenuName { get; set; } = null!;
    public int Qty { get; set; }
    public int Total { get; set; }
}

public class OrderDetailGroupByUser
{
    public string Name { get; set; } = null!;
    public int Total { get; set; }
    public List<OrderDetailGroupByUserItem> Details { get; set; } = [];
}

public class OrderDetailGroupByUserItem
{
    public string MenuName { get; set; } = null!;
    public int Qty { get; set; }
    public int Price { get; set; }
}