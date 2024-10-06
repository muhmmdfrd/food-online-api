namespace FoodOnline.Core.Utils;

public class OrderUtils
{
    private const string FORMAT = "yyyyMMdd";

    public static string GenerateCode()
    {
        var now = DateTime.UtcNow;
        return now.ToString(FORMAT);
    }

    public static string GenerateCode(DateTime datetime)
    {
        return datetime.ToString(FORMAT);
    }
}