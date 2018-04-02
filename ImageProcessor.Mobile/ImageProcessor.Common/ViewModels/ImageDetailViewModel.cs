using System;
using ImageProcessor.Common.Models;
namespace ImageProcessor.Common.ViewModels
{
    public class ImageDetailViewModel : BaseViewModel
    {
        int currentIndex;
        public ImageDetailViewModel(ProcessedImage image) => ImageData = image;

        ProcessedImage imageData = new ProcessedImage();

        public string Name { get => $"Image Name: {imageData.id}"; }
        public string Display
        {
            get
            {
                switch (CurrentIndex)
                {
                    case 0:
                        return "Original";
                    case 1:
                        return "Cogs ready";
                    case 2:
                        return "Ocr Draw";
                    default:
                        return string.Empty;
                }
            }
        }

        public int CurrentIndex
        {
            get => currentIndex;
            set => RaiseAndUpdate(ref currentIndex, value);
        }
        public ProcessedImage ImageData
        {
            get => imageData;
            set
            {
                RaiseAndUpdate(ref imageData, value);
                OnPropertyChanged(nameof(Display));
            }
        }
    }
}
