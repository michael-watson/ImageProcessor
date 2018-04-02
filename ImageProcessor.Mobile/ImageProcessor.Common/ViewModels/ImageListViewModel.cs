using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using ImageProcessor.Common.Interfaces;
using ImageProcessor.Common.Models;
using ImageProcessor.Common.ViewModels;
using System;
using System.Diagnostics;

namespace ImageProcessor.Common
{
    public class ImageListViewModel : BaseViewModel
    {
        bool isBusy;

        public ObservableCollection<ProcessedImage> Images { get; set; } = new ObservableCollection<ProcessedImage>();

        public bool IsBusy
        {
            get => isBusy;
            set => RaiseAndUpdate(ref isBusy, value);
        }

        public ICosmosDbService CosmosDbService => ServiceContainer.Resolve<ICosmosDbService>();

        public async Task GetAllImages()
        {
            IsBusy = true;

            try
            {
                Images.Clear();
                var images = await CosmosDbService.GetAllProcessedImagesAsync();
                foreach (var image in images)
                    Images.Add(image);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{nameof(ImageListViewModel)}:GetAllImages-{e.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}