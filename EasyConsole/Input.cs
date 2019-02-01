using System;
using System.Collections.Generic;
using System.Linq;
using uzLib.Lite.Core;
using uzLib.Lite.Extensions;

namespace EasyConsole
{
    public static class Input
    {
        public static int ReadInt(string prompt, int min, int max)
        {
            Output.DisplayPrompt(prompt);
            return ReadInt(min, max);
        }

        public static int[] ReadInts(string prompt, int min, int max)
        {
            Output.DisplayPrompt(prompt);
            return ReadInts(min, max);
        }

        public static int ReadInt(int min, int max)
        {
            int value = ReadInt();

            while (value < min || value > max)
            {
                Output.DisplayPrompt("Please enter an integer between {0} and {1} (inclusive)", min, max);
                value = ReadInt();
            }

            return value;
        }

        public static int[] ReadInts(int min, int max)
        {
            IEnumerable<int> values = ReadInts();

            while (values.Any(value => value < min || value > max))
            {
                Output.DisplayPrompt("Please enter an integer/s between {0} and {1} (inclusive)", min, max);
                values = ReadInts();
            }

            return values.ToArray();
        }

        public static IEnumerable<int> ReadInts()
        {
            var output = ConsoleOutput.ReadLineOrKey();
            string input;

            if (output.IsExitKey())
            {
                Program.Instance.NavigateBack();
                yield break;
            }
            else
                input = output.GetValue();

            int value;
            bool isMultiple;

            do
            {
                Output.DisplayPrompt("Please enter an integer");
                output = ConsoleOutput.ReadLineOrKey();

                if (output.IsExitKey())
                {
                    Program.Instance.NavigateBack();
                    yield break;
                }
                else
                {
                    input = output.GetValue();
                    isMultiple = input.SeparatedCommaNumbers();
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
                Output.DisplayPrompt("Please enter an integer");
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
            menu.Display();

            return choice;
        }
    }
}