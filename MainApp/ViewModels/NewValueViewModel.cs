using System.ComponentModel;
using MainApp.Annotations;

namespace MainApp.ViewModels
{
    public class NewValueViewModel : INotifyPropertyChanged
    {
        private long value;

        public long Value
        {
            get => value;
            set
            {
                this.value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}