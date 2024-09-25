using System.Text.Json.Serialization;
using Flozacode.Models.Paginations;

namespace FoodOnline.Core.Dtos;

public class OrderDto
{
    public string Code { get; set; } = null!;

    public DateTime Date { get; set; }

    public int StatusId { get; set; }
}

public class OrderViewDto : OrderDto
{
    public long Id { get; set; }
    public string StatusName { get; set; } = null!;
}

public class OrderAddDto : OrderDto
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

public class OrderUpdDto : OrderDto
{
    [JsonIgnore]
    public long? ModifiedBy { get; set; }

    [JsonIgnore]
    public DateTime? ModifiedAt { get; set; }
}

public class OrderFilter : TableFilter{}