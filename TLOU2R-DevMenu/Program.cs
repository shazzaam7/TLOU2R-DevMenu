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
        Thread.Sleep(500);
        
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
        Console.Clear();
        
        Console.WriteLine("The Last of Us Part 2 Remastered - Dev Menu");
        Console.WriteLine("Press F1 to toggle Dev Menu");
        Console.WriteLine("Press F2 to toggle Quick Menu");
        Console.WriteLine("Press ESC to exit\n");
        // Create mod toggle handlers for each mod
        // 1: Toggle Dev Menu
        ModToggleHandler devMenu = new ModToggleHandler(memoryEditor, menuPointer, 0x80);
        // 2: Toggle Quick Menu
        ModToggleHandler quickDevMenu = new ModToggleHandler(memoryEditor, menuPointer, 0x78);
        
        bool running = true;
        while (running)
        {
            // Revert changes and stop this program from running
            if ((Win32Api.GetAsyncKeyState(Constants.VK_ESC) & 0x8000) != 0)
            {
                if (devMenu.IsEnabled)
                {
                    devMenu.Toggle();
                }
                if (quickDevMenu.IsEnabled)
                {
                    quickDevMenu.Toggle();
                }
                running = false;
                continue;
            }

            // Enable/Disable Dev Menu
            if ((Win32Api.GetAsyncKeyState(Constants.VK_F1) & 0x8000) != 0)
            {
                devMenu.Toggle();
                string status = devMenu.IsEnabled ? "ENABLED" : "DISABLED";
                Console.WriteLine($"Dev Menu: {status}");
                Thread.Sleep(Constants.KEY_DELAY_MS);
            }
            
            // Enable/Disable Quick Dev Menu
            if ((Win32Api.GetAsyncKeyState(Constants.VK_F2) & 0x8000) != 0)
            {
                quickDevMenu.Toggle();
                string status = quickDevMenu.IsEnabled ? "ENABLED" : "DISABLED";
                Console.WriteLine($"Quick Menu: {status}");
                Thread.Sleep(Constants.KEY_DELAY_MS);
            }
            Thread.Sleep(10);
        }
    }
}