using System.Text.Json.Serialization;
using Flozacode.Models.Paginations;
using FoodOnline.Repository.Entities;

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

public class OrderViewHistory : OrderViewDto
{
    public int Total { get; set; }
    public List<string> Menus { get; set; } = [];
}

public class OrderViewDetailHistory
{
    public string Code { get; set; } = null!;
    public DateTime Date { get; set; }
    public List<OrderViewDetailItemHistory> OrderDetails { get; set; } = [];
    public List<OrderViewDetailPaymentHistory> OrderPayments { get; set; } = [];
}

public class OrderViewDetailItemHistory
{
    public string Name { get; set; } = null!;
    public int Qty { get; set; }
    public int Price { get; set; }
    public int Total { get; set; }
}

public class OrderViewDetailPaymentHistory
{
    public int TotalPayment { get; set; }
    public int Cashback { get; set; }
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