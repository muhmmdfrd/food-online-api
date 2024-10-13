using FirebaseAdmin.Messaging;

namespace FoodOnline.Core.Fcm;

public class FirebaseHelper
{
    public Task<string> SendMessageAsync(Notification notification, string token)
    {
        var message = new Message
        {
            Token = token,
            Notification = notification
        };
        return FirebaseMessaging.DefaultInstance.SendAsync(message);
    }

    public Task SendBroadcastAsync(Notification notification, List<string> tokens)
    {
        foreach (var token in tokens)
        {
            SendMessageAsync(notification, token);
            Thread.Sleep(500);
        }
        return Task.CompletedTask;
    }
}