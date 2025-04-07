using System.Runtime.InteropServices;

namespace TLOU2R_DevMenu;

public class MemoryEditor
{
    // Variables
    public IntPtr ProcessHandle { get; }
    
    // Constructor
    public MemoryEditor(IntPtr processHandle)
    {
        ProcessHandle = processHandle;
    }
    
    // Functions
    public byte[] ReadBytes(IntPtr address, int size)
    {
        byte[] buffer = new byte[size];
        Win32Api.ReadProcessMemory(ProcessHandle, address, buffer, size, out int bytesRead);
        return buffer;
    }
    
    public bool WriteBytes(IntPtr address, byte[] bytes)
    {
        // Set write permissions
        Win32Api.VirtualProtectEx(ProcessHandle, address, new IntPtr(bytes.Length), Constants.PAGE_EXECUTE_READWRITE, out uint oldProtect);

        bool result = Win32Api.WriteProcessMemory(ProcessHandle, address, bytes, bytes.Length, out int bytesWritten);
        if (!result || bytesWritten != bytes.Length)
        {
            Console.WriteLine($"Error: Failed to write memory at 0x{address.ToInt64():X}. Error code: {Marshal.GetLastWin32Error()}");
            return false;
        }

        // Restore old protection
        Win32Api.VirtualProtectEx(ProcessHandle, address, new IntPtr(bytes.Length), oldProtect, out uint _);
        return true;
    }
}