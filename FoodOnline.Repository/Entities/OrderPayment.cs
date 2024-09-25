using System;
using System.Collections.Generic;

namespace FoodOnline.Repository.Entities;

public partial class OrderPayment
{
    public long Id { get; set; }

    public long OrderId { get; set; }

    public long UserId { get; set; }

    public int GrandTotal { get; set; }

    public int TotalPayment { get; set; }

    public int Cashback { get; set; }

    public int PaymentStatusId { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }
}
