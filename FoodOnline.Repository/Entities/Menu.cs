using System;
using System.Collections.Generic;

namespace FoodOnline.Repository.Entities;

public partial class Menu
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public int DataStatusId { get; set; }

    public int Price { get; set; }

    public long MerchantId { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual Merchant Merchant { get; set; } = null!;
}
