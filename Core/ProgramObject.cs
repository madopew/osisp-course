using System;
using System.Collections.Generic;
using System.Linq;
using Core.Util;

namespace Core
{
    public class ProgramObject
    {
        private readonly IntPtr handle;

        private byte[] firstValue;

        public ProgramObject(int pid)
        {
            handle = WinApiUtil.OpenProcess(
                WinApiUtil.ProcessAccess.QueryInformation | WinApiUtil.ProcessAccess.ProcessRead |
                WinApiUtil.ProcessAccess.ProcessWrite, false, pid);
            if (handle == IntPtr.Zero) throw new ArgumentException("Cannot open process", nameof(pid));
        }

        ~ProgramObject()
        {
            if (handle != IntPtr.Zero) WinApiUtil.CloseHandle(handle);
        }

        public bool BeenScanned { get; private set; }

        public List<long> Addresses { get; private set; }

        public byte[] FirstValue
        {
            get => firstValue ?? throw new InvalidOperationException("Nothing has been scanned yet");
            private set => firstValue = value;
        }

        public void FirstScan(byte[] value)
        {
            FirstValue = value ?? throw new ArgumentNullException(nameof(value));

            BeenScanned = true;
            Addresses = WinApiFacade.ScanProcess(handle, value);
        }

        public void NextScan(byte[] value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            if (!BeenScanned) throw new InvalidOperationException("Cannot sequential scan new object");
            if (Addresses.Count == 0) throw new InvalidOperationException("Nothing to scan");

            var updated = new List<long>();
            for (int i = 0; i < Addresses.Count; i++)
            {
                var addressValue = WinApiFacade.ScanAddress(handle, Addresses[i], value.Length);
                if (addressValue != null && value.SequenceEqual(addressValue))
                {
                    updated.Add(Addresses[i]);
                }
            }

            Addresses = updated;
        }

        public bool InsertValue(long address, byte[] value)
        {
            if (!Addresses.Contains(address)) throw new InvalidOperationException("No such address");
            if (value is null) throw new ArgumentNullException(nameof(value));
            if (!BeenScanned) throw new InvalidOperationException("Cannot insert on new object");
            if (value.Length != FirstValue.Length)
                throw new ArgumentException("Value length is not the same as the first scan");

            return WinApiFacade.WriteAddress(handle, address, value);
        }

        public static Dictionary<int, string> GetPrograms()
        {
            return WinApiFacade.GetProcesses().Where(pidToName => pidToName.Value != string.Empty)
                .ToDictionary(pn => pn.Key, pn => pn.Value);
        }
    }
}