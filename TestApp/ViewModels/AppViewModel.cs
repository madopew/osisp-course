using System.ComponentModel;
using System.Windows;
using TestApp.Annotations;
using TestApp.Commands;
using TestApp.Models;

namespace TestApp.ViewModels
{
    public class AppViewModel : INotifyPropertyChanged
    {
        private readonly AppModel model = new AppModel();

        public int Balance
        {
            get => model.Balance;
            set
            {
                model.Balance = value;
                OnPropertyChanged(nameof(Balance));
            }
        }

        public int BreadAmount
        {
            get => model.BreadAmount;
            set
            {
                model.BreadAmount = value;
                OnPropertyChanged(nameof(BreadAmount));
            }
        }

        public string BuyCarTooltip => $"Buy car! for {AppModel.CarCost}";

        private RelayCommand buyBreadCommand;

        public RelayCommand BuyBreadCommand
        {
            get
            {
                return buyBreadCommand ?? (buyBreadCommand = new RelayCommand(obj =>
                {
                    Balance -= AppModel.BreadCost;
                    BreadAmount++;
                }, obj => Balance >= AppModel.BreadCost));
            }
        }

        private RelayCommand sellBreadCommand;

        public RelayCommand SellBreadCommand
        {
            get
            {
                return sellBreadCommand ?? (sellBreadCommand = new RelayCommand(obj =>
                {
                    Balance += AppModel.BreadCost;
                    BreadAmount--;
                }, obj => BreadAmount > 0));
            }
        }

        private RelayCommand buyCarCommand;

        public RelayCommand BuyCarCommand
        {
            get
            {
                return buyCarCommand ?? (buyCarCommand = new RelayCommand(obj =>
                {
                    Balance -= AppModel.CarCost;
                    MessageBox.Show("You have managed to buy a car! How did you do this?))", "YAY", MessageBoxButton.OK,
                        MessageBoxImage.Asterisk);
                }, obj => Balance >= AppModel.CarCost));
            }
        }

        private RelayCommand infoCommand;

        public RelayCommand InfoCommand
        {
            get
            {
                return infoCommand ?? (infoCommand = new RelayCommand(obj =>
                {
                    MessageBox.Show(
                        $"In this mini game you need to buy your first car! Can you manage this?\nBread cost - {AppModel.BreadCost}\nCar cost - {AppModel.CarCost}",
                        "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }));
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