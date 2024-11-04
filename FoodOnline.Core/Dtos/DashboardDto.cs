namespace FoodOnline.Core.Dtos;

public class DashboardDto
{
    public required int MerchantCount { get; set; }
    public required int OrderCount { get; set; }
    public required int MenuCount { get; set; }
    public required int TotalPayment { get; set; }
}