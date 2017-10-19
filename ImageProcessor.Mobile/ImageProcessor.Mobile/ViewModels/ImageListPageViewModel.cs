using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace ImageProcessor.Mobile
{
	public class ImageListPageViewModel : INotifyPropertyChanged
	{
		public ObservableCollection<ProcessedImage> Images { get; set; } = new ObservableCollection<ProcessedImage>();

		public async Task GetAllImages()
		{
			var images = await CosmosDbService.GetAllProcessedImagesAsync();
			foreach (var image in images)
				Images.Add(image);
		}

		#region INotifyPropertyChanged Implementation
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
}