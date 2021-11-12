using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Core.Util;

namespace Core
{
    internal static class WinApiFacade
    {
        public static List<long> ScanProcess(IntPtr handle, byte[] value)
        {
            var alignment = CalculateAlignment(value);

            var result = new List<long>();
            WinApiUtil.GetSystemInfo(out var systemInfo);
            long address = 0;

            while (address < (long)systemInfo.lpMaximumApplicationAddress)
            {
                if (WinApiUtil.VirtualQueryEx(handle, (IntPtr)address, out var memoryInfo,
                        (uint)Marshal.SizeOf(typeof(WinApiUtil.MemoryBasicInformation))) !=
                    (uint)Marshal.SizeOf(typeof(WinApiUtil.MemoryBasicInformation))) continue;
                address = (long)memoryInfo.BaseAddress;

                if (memoryInfo.State == WinApiUtil.MemoryInformationStateType.MemCommit &&
                    (memoryInfo.Protect & WinApiUtil.MemoryPageProtectType.PageReadWrite) != 0 &&
                    (memoryInfo.Protect & WinApiUtil.MemoryPageProtectType.PageWriteCopy) == 0)
                {
                    var buff = new byte[(long)memoryInfo.RegionSize];
                    if (WinApiUtil.ReadProcessMemory(handle, (IntPtr)address, buff, (int)memoryInfo.RegionSize,
                        out var bytesRead))
                    {
                        for (long i = 0; i < (long)bytesRead - value.Length; i += alignment)
                        {
                            if (value.SequenceEquals(buff, i))
                            {
                                result.Add(address + i);
                            }
                        }
                    }
                }

                address += (long)memoryInfo.RegionSize;
            }

            return result;
        }

        public static byte[] ScanAddress(IntPtr handle, long address, int size)
        {
            var result = new byte[size];
            return WinApiUtil.ReadProcessMemory(handle, (IntPtr)address, result, size, out _) ? result : null;
        }

        public static bool WriteAddress(IntPtr handle, long address, byte[] value)
        {
            WinApiUtil.WriteProcessMemory(handle, (IntPtr)address, value, value.Length, out var bytesWritten);
            return (long)bytesWritten == value.Length;
        }

        public static Dictionary<int, string> GetProcesses()
        {
            var pids = new uint[1024];
            WinApiUtil.EnumProcesses(pids, (uint)pids.Length, out var bytes);
            var actualLength = (int)bytes / 4;
            return pids.Take(actualLength).Select(pid => (int)pid).ToDictionary(pid => pid, pid =>
            {
                var handle = WinApiUtil.OpenProcess(
                    WinApiUtil.ProcessAccess.QueryInformation | WinApiUtil.ProcessAccess.ProcessRead, false, pid);

                if (handle != IntPtr.Zero)
                {
                    var result = new byte[1024];
                    WinApiUtil.GetModuleBaseNameA(handle, IntPtr.Zero, result, (uint) result.Length);
                    var name = Encoding.ASCII.GetString(result);
                    var i = name.IndexOf('\0');
                    return name.Substring(0, i);
                }

                return string.Empty;
            });
        }

        private static long CalculateAlignment(byte[] value)
        {
            switch (value.Length)
            {
                case 2:
                    return 2;
                case 4:
                case 8:
                    return 4;
                default:
                    return 1;
            }
        }
    }
}