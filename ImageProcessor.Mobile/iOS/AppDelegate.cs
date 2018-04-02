using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using ImageProcessor.Mobile.Forms;
using UIKit;
using ImageProcessor.Common;
using ImageProcessor.Common.Interfaces;
using ImageProcessor.Mobile.iOS.ServiceImplementations;
using CarouselView.FormsPlugin.iOS;
using FFImageLoading.Forms.Touch;
using WindowsAzure.Messaging;
using ImageProcessor.Mobile.iOS.Delegates;
using ImageProcessor.Mobile.Forms.Views;
using Newtonsoft.Json;
using ImageProcessor.Common.Models;
using System.Threading.Tasks;

namespace ImageProcessor.Mobile.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public NSData DeviceToken = null;

        SBNotificationHub Hub;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            CachedImageRenderer.Init();
            CarouselViewRenderer.Init();
            // Code for starting up the Xamarin Test Cloud Agent
#if DEBUG
            Xamarin.Calabash.Start();
#endif

            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
                       UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                       new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            }
            else
            {
                UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
                UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
            }

            ServiceContainer.Register<INotification>(new Notification_iOS());
            ServiceContainer.Register<ICosmosDbService>(new CosmosDbService());

            App.ScreenWidth = (double)UIScreen.MainScreen.Bounds.Width;
            App.ScreenHeight = (double)UIScreen.MainScreen.Bounds.Height;

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            Notification_iOS.Hub.UnregisterAllAsync(deviceToken, (error) =>
            {
                if (error != null)
                {
                    Console.WriteLine("Error calling Unregister: {0}", error.ToString());
                    return;
                }

                Notification_iOS.DeviceToken = deviceToken;
            });
        }

        public override async void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            Console.WriteLine("Received Remote Notification");

            foreach (var item in userInfo)
            {
                Console.WriteLine($"\tKey: {item.Key}");
                Console.WriteLine($"\tValue: {item.Value}");
            }
            await ProcessNotification(userInfo, false);
        }

        async Task ProcessNotification(NSDictionary options, bool fromFinishedLaunching)
        {
            string processedImageJson = string.Empty;

            try
            {
                // Check to see if the dictionary has the aps key.  This is the notification payload you would have sent
                if (null != options && options.ContainsKey(new NSString("aps")))
                {
                    //Get the aps dictionary
                    NSDictionary aps = options.ObjectForKey(new NSString("aps")) as NSDictionary;

                    //Extract the alert text
                    // NOTE: If you're using the simple alert by just specifying
                    // "  aps:{alert:"alert msg here"}  ", this will work fine.
                    // But if you're using a complex alert with Localization keys, etc.,
                    // your "alert" object from the aps dictionary will be another NSDictionary.
                    // Basically the JSON gets dumped right into a NSDictionary,
                    // so keep that in mind.

                    if (aps.ContainsKey(new NSString("acme2")))
                        processedImageJson = (aps[new NSString("acme2")] as NSString).ToString();

                    UIAlertView avAlert = new UIAlertView("Image Processed", $"Completed processing image {processedImageJson}", null, "Ok");
                    avAlert.Show();

                    if (App.Current.MainPage.Navigation.NavigationStack.Count == 1)
                    {//Currently only showing the List of images
                        var listPage = App.Current.MainPage.Navigation.NavigationStack[0] as ImagesListPage;
                        await listPage.ViewModel.GetAllImages();
                    }
                    else
                    {//Currently showing the image details page
                        await App.Current.MainPage.Navigation.PopAsync();
                        var listPage = App.Current.MainPage.Navigation.NavigationStack[0] as ImagesListPage;
                        await listPage.ViewModel.GetAllImages();
                    }


                    //var processedImage = JsonConvert.DeserializeObject<ProcessedImage>(processedImageJson);
                    //if (processedImage is default(ProcessedImage)) return;

                    ////If this came from the ReceivedRemoteNotification while the app was running,
                    //// we of course need to manually process things like the sound, badge, and alert.
                    //if (!fromFinishedLaunching)
                    //{
                    //    //Manually show an alert
                    //    if (!string.IsNullOrEmpty(processedImageJson))
                    //    {
                    //        //Need to add delegate to show page
                    //        UIAlertView avAlert = new UIAlertView("Image Processed", $"Completed processing image {processedImage.id}", new ImageProcessedAlertDelegate(processedImage), "Ok", "View Image");
                    //        avAlert.Show();
                    //    }
                    //}
                    //else
                    //{
                    //    //Just handle 
                    //    if (App.Current.MainPage.Navigation.NavigationStack.Count == 1)
                    //    {//Currently only showing the List of images
                    //        App.Current.MainPage.Navigation.PushAsync(new ImageDetailsPage(processedImage));
                    //    }
                    //    else
                    //    {//Currently showing the image details page
                    //        var detailsPage = App.Current.MainPage.Navigation.NavigationStack[1] as ImageDetailsPage;
                    //        if (detailsPage is null) return;

                    //        detailsPage.ViewModel.ImageData = processedImage;
                    //    }
                    //}
                }
            }
            catch (JsonException je)
            {
                Console.WriteLine($"Push Notification payload improperly formatted: {processedImageJson}");
            }
        }
    }
}