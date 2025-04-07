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
}