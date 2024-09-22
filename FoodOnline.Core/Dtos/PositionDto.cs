using Flozacode.Models.Paginations;
using FoodOnline.Core.Enums;
using Newtonsoft.Json;

namespace FoodOnline.Core.Dtos;

public class PositionDto
{
    public string Name { get; set; } = null!;
    
    [JsonIgnore]
    public int DataStatusId { get; set; } = (int)DataStatusEnum.Active;
}

public class PositionViewDto : PositionDto
{
    public long Id { get; set; }
}

public class PositionAddDto : PositionDto
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

public class PositionUpdDto : PositionDto
{
    public long Id { get; set; }
    
    [JsonIgnore]
    public long? ModifiedBy { get; set; }

    [JsonIgnore]
    public DateTime? ModifiedAt { get; set; }
}

public class PositionFilter : TableFilter {}