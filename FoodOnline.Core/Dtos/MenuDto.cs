﻿using Flozacode.Models.Paginations;

namespace FoodOnline.Core.Dtos;

public class MenuDto
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
}

public class MenuViewDto : MenuDto{}

public class MenuAddDto : MenuDto{}

public class MenuUpdDto : MenuDto{}

public class MenuFilter : TableFilter{}