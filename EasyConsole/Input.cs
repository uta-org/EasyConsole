using System;
using System.Collections.Generic;
using System.Linq;
using uzLib.Lite.Core;
using uzLib.Lite.Extensions;
using System.Drawing;

namespace EasyConsole
{
    public static class Input
    {
        public static int ReadInt(string prompt, int min, int max, bool displayPrompt = true)
        {
            if (displayPrompt)
                Output.DisplayPrompt(prompt);

            return ReadInt(min, max);
        }

        public static int[] ReadInts(string prompt, int min, int max, bool displayPrompt = true)
        {
            if (displayPrompt)
                Output.DisplayPrompt(prompt);

            return ReadInts(min, max);
        }

        public static int ReadInt(int min, int max)
        {
            int value = ReadInt();

            while (value < min || value > max)
            {
                Output.DisplayPrompt("Please enter an integer between {0} and {1} (inclusive)", Color.Red, min, max);
                value = ReadInt();
            }

            return value;
        }

        public static int[] ReadInts(int min, int max)
        {
            int[] values = ReadInts().ToArray();

            while (values.Any(value => value < min || value > max))
            {
                Output.DisplayPrompt("Please enter an integer/s between {0} and {1} (inclusive)", Color.Red, min, max);
                values = ReadInts().ToArray();
            }

            return values;
        }

        public static IEnumerable<int> ReadInts()
        {
            var output = ConsoleOutput.ReadLineOrKey();

            string input;
            bool isMultiple;

            if (output.IsExitKey())
            {
                Program.Instance.NavigateBack();
                yield break;
            }
            else
            {
                input = output.GetValue();
                isMultiple = input.CheckCommaNumbers();
            }

            int value;
            bool firstParse = int.TryParse(input, out value);

            if (!firstParse)
                do
                {
                    Output.DisplayPrompt("Please enter an integer", Color.Yellow);
                    output = ConsoleOutput.ReadLineOrKey();

                    if (output.IsExitKey())
                    {
                        Program.Instance.NavigateBack();
                        yield break;
                    }
                    else
                    {
                        input = output.GetValue();
                        isMultiple = input.CheckCommaNumbers();
                    }
                }
                while (!int.TryParse(input, out value) || isMultiple);

            if (!isMultiple)
                yield return value;
            else
            {
                foreach (string val in input.Split(','))
                    yield return int.Parse(val.Trim(' '));
            }
        }

        public static int ReadInt()
        {
            var output = ConsoleOutput.ReadLineOrKey();
            string input;

            if (output.IsExitKey())
            {
                Program.Instance.NavigateBack();
                return 0;
            }
            else
                input = output.GetValue();

            int value;

            while (!int.TryParse(input, out value))
            {
                Output.DisplayPrompt("Please enter an integer", Color.Yellow);
                output = ConsoleOutput.ReadLineOrKey();

                if (output.IsExitKey())
                {
                    Program.Instance.NavigateBack();
                    return 0;
                }
                else
                    input = output.GetValue();
            }

            return value;
        }

        public static string ReadString(string prompt)
        {
            Output.DisplayPrompt(prompt);

            var output = ConsoleOutput.ReadLineOrKey();

            if (output.IsExitKey())
            {
                Program.Instance.NavigateBack();
                return null;
            }
            else
                return output.GetValue();
        }

        public static TEnum ReadEnum<TEnum>(string prompt) where TEnum : struct, IConvertible, IComparable, IFormattable
        {
            Type type = typeof(TEnum);

            if (!type.IsEnum)
                throw new ArgumentException("TEnum must be an enumerated type");

            Output.WriteLine(prompt);
            Menu menu = new Menu();

            TEnum choice = default(TEnum);
            foreach (var value in Enum.GetValues(type))
                menu.Add(Enum.GetName(type, value), () => { choice = (TEnum)value; });

            menu.Display(false);

            return choice;
        }
    }
}