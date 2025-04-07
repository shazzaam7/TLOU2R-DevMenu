using System.Diagnostics;

namespace TLOU2R_DevMenu;

public static class PatternScanner
{
    // Variables
    // Pattern to search
    private static readonly byte?[] _menuPtrPattern = new byte?[]
    {
        0x48, 0x8B, 0x05, null, null, null, null, 0xC5, null, null, null, null, null, null, null,
        0xC5, null, null, null, null, null, null, null, 0xC5, null, null, null, null, null, null, null, 0x4C
    };
    
    // Functions
    private static int? FindPattern(byte[] data, byte?[] pattern)
    {
        for (int i = 0; i <= data.Length - pattern.Length; i++)
        {
            bool found = true;
            for (int j = 0; j < pattern.Length; j++)
            {
                if (!pattern[j].HasValue)
                    continue;
                if (data[i + j] != pattern[j].Value)
                {
                    found = false;
                    break;
                }
            }
            if (found)
                return i;
        }
        return null;
    }
    
    public static IntPtr FindMenuPointer(MemoryEditor memEditor, Process process)
    {
        Console.WriteLine("Scanning memory for pattern...");
        
        IntPtr moduleBase = process.MainModule.BaseAddress;
        int moduleSize = process.MainModule.ModuleMemorySize;
        byte[] moduleBytes = memEditor.ReadBytes(moduleBase, moduleSize);
        
        int? patternOffset = FindPattern(moduleBytes, _menuPtrPattern);
        if (!patternOffset.HasValue)
        {
            Console.WriteLine("Error: Pattern not found!");
            return IntPtr.Zero;
        }
        Console.WriteLine($"Pattern found at offset: 0x{patternOffset.Value:X}");
        IntPtr patternAddress = moduleBase + patternOffset.Value;
        Console.WriteLine($"Pattern address: 0x{patternAddress.ToInt64():X}");

        // Read the 4-byte offset from pattern + 3
        byte[] offsetBytes = memEditor.ReadBytes(patternAddress + 3, 4);
        int offset = BitConverter.ToInt32(offsetBytes, 0);
        Console.WriteLine($"Offset value: 0x{offset:X}");

        // Calculate final address ([addr + offset + 7])
        IntPtr finalAddress = patternAddress + offset + 7;
        Console.WriteLine($"Final base address: 0x{finalAddress.ToInt64():X}");

        // Read the pointer at the final address (64-bit pointer)
        byte[] pointerBytes = memEditor.ReadBytes(finalAddress, 8);
        long pointerValue = BitConverter.ToInt64(pointerBytes, 0);

        return new IntPtr(pointerValue);
    }
}