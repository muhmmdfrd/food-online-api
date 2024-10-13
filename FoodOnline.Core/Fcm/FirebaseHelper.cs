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
}