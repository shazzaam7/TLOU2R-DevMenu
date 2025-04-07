using System.Diagnostics;

namespace TLOU2R_DevMenu;

class Program
{
    private static Process GetGameProcess()
    {
        Process gameProcess = null;
        while (gameProcess == null)
        {
            foreach (string GAME_PROCESS_NAME in Constants.GAME_PROCESS_NAMES)
            {
                Process[] processes = Process.GetProcessesByName(GAME_PROCESS_NAME);
                if (processes.Length > 0)
                {
                    gameProcess = processes[0];
                    break;
                }
            }
        }
        return gameProcess;
    }
    
    static void Main(string[] args)
    {
        // Grabbing game process
        Console.WriteLine("Grabbing game process");
        Process gameProcess = GetGameProcess();
        if (gameProcess == null)
        {
            Console.WriteLine("Error: Game process not found");
            Console.ReadKey();
            return;
        }
        Console.WriteLine($"Found game process: {gameProcess.ProcessName} ({gameProcess.Id})");
        Thread.Sleep(2000);
        
        // Open Process
        IntPtr processHandle = Win32Api.OpenProcess(Constants.PROCESS_ALL_ACCESS, false, gameProcess.Id);
        if (processHandle == IntPtr.Zero)
        {
            Console.WriteLine("Error: Failed to open process!");
            Console.ReadKey();
            return;
        }
        Console.WriteLine($"Process found! PID: {gameProcess.Id}");
        Thread.Sleep(500);
        
        // Create MemoryEditor instance
        MemoryEditor memoryEditor = new MemoryEditor(processHandle);
        
        // Find the menu pointer using pattern scanning
        IntPtr menuPointer = PatternScanner.FindMenuPointer(memoryEditor, gameProcess);
        if (menuPointer == IntPtr.Zero)
        {
            Console.WriteLine("Error: Failed to find memory pattern! Make sure you reach main menu before opening this.");
            Console.ReadKey();
            return;
        }
        Console.WriteLine($"Menu pointer found at: 0x{menuPointer.ToInt64():X}");
        
    }
}