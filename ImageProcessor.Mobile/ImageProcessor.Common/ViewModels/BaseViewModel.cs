using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ImageProcessor.Common.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        protected void RaiseAndUpdate<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return;

            field = value;
            OnPropertyChanged(propertyName);
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
