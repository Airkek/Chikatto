using System;

namespace Chikatto.Utils
{
    public static class XConsole
    {
        public static void Log(string message, ConsoleColor fore = ConsoleColor.White, ConsoleColor back = ConsoleColor.Black)
        {
            Console.Write($"[{DateTime.Now:hh:mm:ss}] ");
            
            Console.ForegroundColor = fore;
            Console.BackgroundColor = back;
            
            Console.WriteLine(message);
            
            Console.ResetColor();
        }
    }
}