using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace Circular.Framework.Notifications
{
    public class FirebaseNotification
    {
        public int Send(NotificationPayload obj)
        {
            try
            {

                string path = obj.NotificationPath ?? "";
                FirebaseApp? app = null;
                try
                {
                    app = FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile(path)
                    }, "CircularConnectDev");
                }
                catch (Exception ex)
                {
                    app = FirebaseApp.GetInstance("CircularConnectDev");

                }
                var fcm = FirebaseMessaging.GetMessaging(app);
                Message message = new Message()
                {
                    Notification = new Notification
                    {
                        Title = obj.NotificationTitle,
                        Body = obj.NotificationBody
                    },
                    Data = new Dictionary<string, string>()
                    {
                        { "NotificationTypeId", obj.NotificationTypeId.ToString() },
                        { "SenderId", obj.NotificationSenderId.ToString() },
                        { "NotificationId", obj.NotificationId.ToString() },
                        { "AdditionalData", obj.NotificationReferenceId.ToString()},
                    },
                    Topic = obj.NotificationTopic.ToString(),
                    Apns = new ApnsConfig() { Aps = new Aps() { Sound = "default"} }
                };
                try
                {
                    fcm.SendAsync(message);
                }
                catch (Exception ex)
                {
                    return 0;
                }
                return 1;


            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
