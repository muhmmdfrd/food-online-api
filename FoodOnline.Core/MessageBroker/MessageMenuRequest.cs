using Microsoft.AspNetCore.Http;

namespace FoodOnline.Core.MessageBroker;

public class MessageMenuRequest
{
    public required string File { get; set; }
    public required string Note { get; set; } = null!;
    public required int UploadType { get; set; }
    public required long ReferenceId { get; set; }
    public required string UniqueId { get; set; } = null!;
}