using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MainApp.Annotations;

namespace MainApp.ViewModels
{
    public class ProcessSelectViewModel : INotifyPropertyChanged
    {
        private Dictionary<int, string> processes;

        public ProcessSelectViewModel(Dictionary<int, string> processes)
        {
            this.processes = processes;
        }

        public List<string> Processes => processes.Select(pidToName => pidToName.Value).ToList();

        private int selectedProcessIndex;

        public int SelectedProcessIndex
        {
            get => selectedProcessIndex;
            set
            {
                selectedProcessIndex = value;
                OnPropertyChanged(nameof(SelectedProcessIndex));
            }
        }

        public int SelectedProcess => processes.ElementAt(selectedProcessIndex).Key;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}