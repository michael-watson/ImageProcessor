using Xamarin.Forms;

namespace ImageProcessor.Mobile
{
	public partial class ImageListPage : ContentPage
	{
		ImageListPageViewModel viewModel = new ImageListPageViewModel();

		public ImageListPage()
		{
			InitializeComponent();

			BindingContext = viewModel;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			await viewModel.GetAllImages();
		}

		void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
		{
			var item = e.SelectedItem as ProcessedImage;
			if (item == null) return;
		}
	}
}