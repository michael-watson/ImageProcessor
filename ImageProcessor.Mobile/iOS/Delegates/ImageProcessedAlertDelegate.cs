using System;
using UIKit;
using Foundation;
using ImageProcessor.Common.Models;
using ImageProcessor.Mobile.Forms;
using ImageProcessor.Mobile.Forms.Views;

namespace ImageProcessor.Mobile.iOS.Delegates
{
    public class ImageProcessedAlertDelegate : NSObject, IUIAlertViewDelegate
    {
        ProcessedImage imageToDisplay;

        public ImageProcessedAlertDelegate(ProcessedImage processedImage)
        {
            imageToDisplay = processedImage;
        }

        [Export("alertView:didDismissWithButtonIndex:")]
        public void Dismissed(UIAlertView alertView, nint buttonIndex)
        {
            if (buttonIndex == 1)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    if (App.Current.MainPage.Navigation.NavigationStack.Count == 1)
                    {//Currently only showing the List of images
                        App.Current.MainPage.Navigation.PushAsync(new ImageDetailsPage(imageToDisplay));
                    }
                    else
                    {//Currently showing the image details page
                        var detailsPage = App.Current.MainPage.Navigation.NavigationStack[1] as ImageDetailsPage;
                        if (detailsPage is null) return;

                        detailsPage.ViewModel.ImageData = imageToDisplay;
                    }
                });
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            imageToDisplay = null;
        }
    }
}
