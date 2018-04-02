using System;
using Foundation;
using ImageProcessor.Common.Interfaces;
using ImageProcessor.Mobile.Forms;
using WindowsAzure.Messaging;

namespace ImageProcessor.Mobile.iOS.ServiceImplementations
{
    public class Notification_iOS : INotification
    {
        public static SBNotificationHub Hub;

        public static NSData DeviceToken;

        public Notification_iOS()
        {
            Hub = new SBNotificationHub(Constants.ConnectionString, Constants.NotificationHubPath);
        }

        public bool RegisterTag(string tag)
        {
            if (DeviceToken is null) return false;

            NSSet tags = new NSSet(tag); // create tags if you want
            Hub.RegisterNativeAsync(DeviceToken, tags, (errorCallback) =>
            {
                if (errorCallback != null)
                {
                    Console.WriteLine("RegisterNativeAsync error: " + errorCallback.ToString());
                }
            });

            return true;
        }
    }
}