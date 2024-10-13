using System;
using System.Collections.Generic;

namespace FoodOnline.Repository.Entities;

public partial class Order
{
    public long Id { get; set; }

    public string Code { get; set; } = null!;

    public DateTime Date { get; set; }

    public int StatusId { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<OrderPayment> OrderPayments { get; set; } = new List<OrderPayment>();
}
