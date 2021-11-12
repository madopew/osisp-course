using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Core.Util
{
    internal static class WinApiUtil
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);

        public static class ProcessAccess
        {
            public const uint QueryInformation = 0x00000400;
            public const uint ProcessRead = 0x00000010;
            public const uint ProcessWrite = 0x00000020;
        }

        [DllImport("kernel32.dll")]
        public static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MemoryBasicInformation lpBuffer,
            uint dwLength);

        [StructLayout(LayoutKind.Sequential)]
        public struct MemoryBasicInformation
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        public static class MemoryInformationStateType
        {
            public const uint MemCommit = 0x00001000;
        }

        public static class MemoryPageProtectType
        {
            public const uint PageNoAccess = 0x00000001;
            public const uint PageReadWrite = 0x00000004;
            public const uint PageWriteCopy = 0x00000008;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void GetSystemInfo(out SystemInfo info);

        [StructLayout(LayoutKind.Sequential)]
        public struct SystemInfo
        {
            public ushort wProcessorArchitecture;
            public ushort wReserved;
            public uint dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public IntPtr dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public ushort wProcessorLevel;
            public ushort wProcessorRevision;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer,
            int dwSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint GetLastError();

        public static bool SequenceEquals(this byte[] first, byte[] second, long offset)
        {
            if (second.Length - offset < first.Length)
                throw new ArgumentException("Compared values are outside of the range of array", nameof(second));

            for (int i = 0; i < first.Length; i++)
            {
                if (first[i] != second[offset + i]) return false;
            }

            return true;
        }

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize,
            out IntPtr lpNumberOfBytesWritten);

        [DllImport("Psapi.dll", SetLastError = true)]
        public static extern bool EnumProcesses(
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U4)] [In] [Out] uint[] processIds,
            uint arraySizeBytes, [MarshalAs(UnmanagedType.U4)] out uint bytesCopied);

        [DllImport("psapi.dll", SetLastError = true)]
        public static extern bool EnumProcessModules(IntPtr hProcess,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U4)] out IntPtr lphModule, uint cb,
            [MarshalAs(UnmanagedType.U4)] out uint lpcbNeeded);
        
        [DllImport("psapi.dll")]
        public static extern uint GetModuleBaseNameA(IntPtr hProcess, IntPtr hModule, byte[] lpBaseName, uint nSize);
    }
}