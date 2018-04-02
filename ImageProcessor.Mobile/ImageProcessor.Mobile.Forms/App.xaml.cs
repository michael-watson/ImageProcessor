using System;
using System.Collections.Generic;
using ImageProcessor.Mobile.Forms.Views;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

[assembly: Xamarin.Forms.Xaml.XamlCompilation(Xamarin.Forms.Xaml.XamlCompilationOptions.Compile)]
namespace ImageProcessor.Mobile.Forms
{
    public partial class App : Application
    {
        public static double ScreenWidth;
        public static double ScreenHeight;

        public static string HubConnection = "Endpoint=sb://image-analyzer.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=Lamf3D9JF+hay+rgk0fXd0gCBkn9owKhsb9/udMqfpo=";

        public App()
        {
            var navPage = new Xamarin.Forms.NavigationPage(new ImagesListPage());

            navPage.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetLargeTitleDisplay(LargeTitleDisplayMode.Automatic);

            MainPage = navPage;
        }
    }
}