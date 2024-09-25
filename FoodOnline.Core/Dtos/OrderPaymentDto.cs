using System.Text.Json.Serialization;
using Flozacode.Models.Paginations;

namespace FoodOnline.Core.Dtos;

public class OrderPaymentDto
{
    public long OrderId { get; set; }
    public long UserId { get; set; }
    public int GrandTotal { get; set; }
    public int TotalPayment { get; set; }
    public int Cashback { get; set; }
    public int PaymentStatusId { get; set; }
}

public class OrderPaymentViewDto : OrderPaymentDto
{ 
    public long Id { get; set; } 
    public string PaymentStatusName { get; set; } = null!;
}

public class OrderPaymentAddDto : OrderPaymentDto
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

public class OrderPaymentUpdDto : OrderPaymentDto
{
    public long Id { get; set; }
    
    [JsonIgnore]
    public long? ModifiedBy { get; set; }

    [JsonIgnore]
    public DateTime? ModifiedAt { get; set; }
}

public class OrderPaymentFilter : TableFilter{}