using System;
using System.Collections.Generic;
using ImageProcessor.Common;
using ImageProcessor.Common.Models;
using Xamarin.Forms;
using ImageProcessor.Common.Interfaces;
using Plugin.Media;
using Acr.UserDialogs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO.Compression;

namespace ImageProcessor.Mobile.Forms.Views
{
    public partial class ImagesListPage : ContentPage
    {

        public ImageListViewModel ViewModel = new ImageListViewModel();

        const string originalImagesBlobName = "original-images";
        const string blobConnString = "DefaultEndpointsProtocol=https;AccountName=imageprocessormiwats;AccountKey=6u6ot9nziVhRUPhArwa7JNau/6wanvUq73Ku9drbMYnVhTjwpP7B1iQOfoxR8OEwq/KquiZ4Vuh4WfNRk8aKXA==;EndpointSuffix=core.windows.net";

        static CloudBlobClient blobClient = CloudStorageAccount.Parse(blobConnString).CreateCloudBlobClient();

        CloudBlobContainer originalImagesContainer;

        public ImagesListPage()
        {
            InitializeComponent();

            BindingContext = ViewModel;

            originalImagesContainer = blobClient.GetContainerReference(originalImagesBlobName);
        }

        protected override async void OnAppearing() => await ViewModel.GetAllImages();

        void Handle_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as ProcessedImage;
            if (item == null) return;

            Navigation.PushAsync(new ImageDetailsPage(item));
        }

        async void UploadPhoto(object sender, EventArgs e)
        {
            if (CrossMedia.Current.IsTakePhotoSupported)
            {
                try
                {
                    var results = await UserDialogs.Instance.PromptAsync(new PromptConfig
                    {
                        InputType = InputType.Name,
                        OkText = "Ok",
                        IsCancellable = true,
                        CancelText = "Cancel",
                        Message = "Please name the photo"
                    });

                    if (!results.Ok)
                        return;

                    var name = $"{results.Value}.png";
                    var registered = ServiceContainer.Resolve<INotification>().RegisterTag(name);

                    using (var takenPhoto = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                    {
                        SaveMetaData = true,
                        SaveToAlbum = false
                    }))
                    {
                        if (takenPhoto is null)
                        {
                            return;
                        }

                        //Upload byte[] from photo to Blob storage bucket
                        var blob = originalImagesContainer.GetBlockBlobReference(name);
                        using (var photoStream = takenPhoto.GetStream())
                        {
                            blob.Properties.ContentType = "image/jpg";
                            await blob.UploadFromStreamAsync(photoStream);

                            await blob.SetPropertiesAsync();
                        }
                    }
                }
                catch (Exception exc)
                {
                    System.Diagnostics.Debug.WriteLine(exc.Message);
                }
            }
        }

        async void Handle_Refreshing(object sender, System.EventArgs e) => await ViewModel.GetAllImages();
    }
}