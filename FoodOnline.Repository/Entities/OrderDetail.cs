using System;
using System.Collections.Generic;

namespace FoodOnline.Repository.Entities;

public partial class OrderDetail
{
    public long Id { get; set; }

    public long OrderId { get; set; }

    public long UserId { get; set; }

    public string UserName { get; set; } = null!;

    public long MenuId { get; set; }

    public string MenuName { get; set; } = null!;

    public int Price { get; set; }

    public int Qty { get; set; }

    public int Total { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }
}
