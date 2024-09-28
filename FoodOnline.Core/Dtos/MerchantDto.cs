using System.Text.Json.Serialization;
using Flozacode.Models.Paginations;

namespace FoodOnline.Core.Dtos;

public class MerchantDto
{
    public string Name { get; set; } = null!;
    public int DataStatusId { get; set; }
}

public class MerchantViewDto : MerchantDto
{
    public long Id { get; set; }
    public string DataStatusName { get; set; } = null!;
}

public class MerchantAddDto : MerchantDto
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

public class MerchantUpdDto : MerchantDto
{
    public long Id { get; set; }
    
    [JsonIgnore]
    public long? ModifiedBy { get; set; }

    [JsonIgnore]
    public DateTime? ModifiedAt { get; set; }
}

public class MerchantFilter : TableFilter
{
}