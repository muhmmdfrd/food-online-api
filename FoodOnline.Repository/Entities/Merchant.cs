using System;
using System.Collections.Generic;

namespace FoodOnline.Repository.Entities;

public partial class Merchant
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public long? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int DataStatusId { get; set; }

    public virtual ICollection<Menu> Menus { get; set; } = new List<Menu>();
}
