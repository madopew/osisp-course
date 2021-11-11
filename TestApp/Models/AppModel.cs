using System.ComponentModel;
using TestApp.Annotations;

namespace TestApp.Models
{
    public class AppModel : INotifyPropertyChanged
    {
        public const int BreadCost = 1;

        public const int CarCost = 100000;

        private int balance = 10;

        public int Balance
        {
            get => balance;
            set
            {
                balance = value;
                OnPropertyChanged(nameof(Balance));
            }
        }

        private int breadAmount = 0;

        public int BreadAmount
        {
            get => breadAmount;
            set
            {
                breadAmount = value;
                OnPropertyChanged(nameof(BreadAmount));
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