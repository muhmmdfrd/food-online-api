namespace FoodOnline.Core.Dtos;

public class AuthRequestDto
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class AuthResponseDto
{
    public string Token { get; set; } = null!;
    public UserViewDto User { get; set; } = null!;
}