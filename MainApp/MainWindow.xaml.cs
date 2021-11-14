using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Core;
using MainApp.ViewModels;
using MainApp.Views;

namespace MainApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddressesListBox_OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var newValueViewModel = new NewValueViewModel();
            var newValueWindow = new NewValueView { DataContext = newValueViewModel };
            if (newValueWindow.ShowDialog() == true)
            {
                ((AppViewModel)DataContext).OnAddressSelect((long)((ListBoxItem)sender).Content,
                    newValueViewModel.Value);
            }
        }

        private void ProcessSelect_OnClick(object sender, RoutedEventArgs e)
        {
            var processSelectViewModel = new ProcessSelectViewModel(ProgramObject.GetPrograms());
            var processSelectWindow = new ProcessSelectView { DataContext = processSelectViewModel };
            if (processSelectWindow.ShowDialog() == true)
            {
                ((AppViewModel)DataContext).OnProcessSelect(processSelectViewModel.SelectedProcess);
            }
        }
    }
}