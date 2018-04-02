using System;
using System.Collections.Generic;

using Xamarin.Forms;
using ImageProcessor.Common.Models;
using ImageProcessor.Common.ViewModels;
using FFImageLoading.Forms;

namespace ImageProcessor.Mobile.Forms.Views
{
    public partial class ImageDetailsPage : ContentPage
    {
        public ImageDetailViewModel ViewModel;

        public ImageDetailsPage(ProcessedImage image)
        {
            InitializeComponent();

            ViewModel = new ImageDetailViewModel(image);
            BindingContext = ViewModel;

            imageCarousel.WidthRequest = App.ScreenWidth;
            imageCarousel.HeightRequest = App.ScreenHeight * 0.8;

            var viewList = new List<View>();
            viewList.Add(new CachedImage
            {
                WidthRequest = App.ScreenWidth,
                HeightRequest = App.ScreenHeight * 0.8,
                DownsampleToViewSize = true,
                ErrorPlaceholder = "Camera.png",
                Source = image.OriginalImageUrl
            });
            viewList.Add(new CachedImage
            {
                WidthRequest = App.ScreenWidth,
                HeightRequest = App.ScreenHeight * 0.8,
                DownsampleToViewSize = true,
                ErrorPlaceholder = "Camera.png",
                Source = image.CogsReadyImageUrl
            });
            if (!string.IsNullOrEmpty(image.OcrDrawImageUrl))
            {
                viewList.Add(new CachedImage
                {
                    WidthRequest = App.ScreenWidth,
                    HeightRequest = App.ScreenHeight * 0.8,
                    DownsampleToViewSize = true,
                    ErrorPlaceholder = "Camera.png",
                    Source = image.OcrDrawImageUrl
                });
            }

            imageCarousel.ItemsSource = viewList;
        }
    }
}