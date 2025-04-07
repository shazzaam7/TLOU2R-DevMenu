namespace TLOU2R_DevMenu;

public class ModToggleHandler
{
    private readonly MemoryEditor _memEditor;
    private readonly IntPtr _basePointer;
    private readonly int _dereferenceOffset;
    private readonly int _valueOffset;

    public bool IsEnabled { get; private set; }

    /// <summary>
    /// Initializes a new mod toggle handler.
    /// </summary>
    /// <param name="memEditor">Memory editor to use</param>
    /// <param name="basePointer">Base pointer obtained from pattern scanning</param>
    /// <param name="dereferenceOffset">Offset from base pointer to dereference (0x80 for mod1, 0x78 for mod2)</param>
    /// <param name="valueOffset">Final offset to write to (e.g. 0xC0)</param>
    public ModToggleHandler(MemoryEditor memEditor, IntPtr basePointer, int dereferenceOffset, int valueOffset = 0xC0)
    {
        _memEditor = memEditor;
        _basePointer = basePointer;
        _dereferenceOffset = dereferenceOffset;
        _valueOffset = valueOffset;
    }
    
    
    public void Toggle()
    {
        IsEnabled = !IsEnabled;
        /*
        if (ApplyToggle())
        {
            Console.WriteLine($"Mod at offset 0x{_dereferenceOffset:X} -> +0x{_valueOffset:X} {(IsEnabled ? "ENABLED" : "DISABLED")}");
        }*/
        ApplyToggle();
    }

    private bool ApplyToggle()
    {
        // First, dereference pointer at base + dereferenceOffset.
        byte[] buffer = _memEditor.ReadBytes(_basePointer + _dereferenceOffset, 8);
        long dereferencedValue = BitConverter.ToInt64(buffer, 0);
        //Console.WriteLine($"Dereference [Base+0x{_dereferenceOffset:X}]: 0x{dereferencedValue:X}");

        // Final address: dereferenced pointer + valueOffset
        IntPtr finalAddress = new IntPtr(dereferencedValue + _valueOffset);
        //Console.WriteLine($"Final address: 0x{finalAddress.ToInt64():X}");

        // Read current value for debugging.
        byte currentValue = _memEditor.ReadBytes(finalAddress, 1)[0];
        //Console.WriteLine($"Current value: {currentValue}");

        // Write the new value.
        byte newValue = IsEnabled ? (byte)1 : (byte)0;
        if (!_memEditor.WriteBytes(finalAddress, new byte[] { newValue }))
        {
            return false;
        }

        // Verify write.
        currentValue = _memEditor.ReadBytes(finalAddress, 1)[0];
        //Console.WriteLine($"New value: {currentValue}");
        
        return true;
    }
}