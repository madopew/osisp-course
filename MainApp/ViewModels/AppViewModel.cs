using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core;
using MainApp.Annotations;
using MainApp.Commands;

namespace MainApp.ViewModels
{
    public class AppViewModel : INotifyPropertyChanged
    {
        private ProgramObject model;

        private ProgramObject Model
        {
            get => model;
            set
            {
                model = value;
                OnPropertyChanged(nameof(Model));
            }
        }

        public string ProcessName => Model?.ProcessName ?? "No process selected";

        private long scanValue;

        public long ScanValue
        {
            get => scanValue;
            set
            {
                scanValue = value;
                OnPropertyChanged(nameof(ScanValue));
            }
        }

        private int size = 4;

        public int Size
        {
            get => size;
            set
            {
                size = value;
                OnPropertyChanged(nameof(Size));
            }
        }

        public int[] Sizes { get; } = { 1, 2, 4, 8 };

        public List<long> Addresses => Model?.Addresses ?? new List<long>();

        private RelayCommand firstScanCommand;

        public RelayCommand FirstScanCommand
        {
            get
            {
                return firstScanCommand ?? (firstScanCommand = new RelayCommand(obj =>
                {
                    var value = BitConverter.GetBytes(scanValue);
                    Model.FirstScan(value.Take(Size).ToArray());
                    OnPropertyChanged(nameof(Addresses));
                }, obj => Model != null));
            }
        }

        private RelayCommand nextScanCommand;

        public RelayCommand NextScanCommand
        {
            get
            {
                return nextScanCommand ?? (nextScanCommand = new RelayCommand(obj =>
                {
                    var value = BitConverter.GetBytes(scanValue);
                    Model.NextScan(value.Take(Size).ToArray());
                    OnPropertyChanged(nameof(Addresses));
                }, obj => Model != null && Model.BeenScanned));
            }
        }

        public void OnAddressSelect(long address, long value)
        {
            var bytes = BitConverter.GetBytes(value);
            Model.InsertValue(address, bytes.Take(size).ToArray());
        }

        public void OnProcessSelect(int pid)
        {
            Model = new ProgramObject(pid);
            OnPropertyChanged(nameof(ProcessName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}