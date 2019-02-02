using System;
using System.Drawing;

using Console = Colorful.Console;

namespace EasyConsole
{
    public static class Output
    {
        public static void WriteLine(ConsoleColor color, string format, params object[] args)
        {
            System.Console.ForegroundColor = color;
            Console.WriteLine(format, args);
            Console.ResetColor();
        }

        public static void WriteLine(ConsoleColor color, string value)
        {
            System.Console.ForegroundColor = color;
            Console.WriteLine(value);
            Console.ResetColor();
        }

        public static void WriteLine(string format, Color color, params object[] args)
        {
            Console.WriteLine(format, color, args);
        }

        public static void DisplayPrompt(string format, Color color, params object[] args)
        {
            format = format.Trim() + " ";
            Console.Write(format, color, args);
        }

        public static void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }

        public static void DisplayPrompt(string format, params object[] args)
        {
            format = format.Trim() + " ";
            Console.Write(format, args);
        }
    }
}