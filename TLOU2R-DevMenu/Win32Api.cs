using System.Runtime.InteropServices;

namespace TLOU2R_DevMenu;

public static class Win32Api
{
    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
}