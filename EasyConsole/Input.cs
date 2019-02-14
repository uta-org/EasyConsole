using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using uzLib.Lite.Core;
using uzLib.Lite.Extensions;
using Console = Colorful.Console;

namespace EasyConsole
{
    public static class Input
    {
        private const string WrongInput = "(invalid input)";

        public static string WrongInputCaption
        {
            get
            {
                return WrongCaptions[m_lastPrompt];
            }
            set
            {
                m_wrongInputCaption = value;
                m_curPrompt = WrongCaptions.IndexOf(m_wrongInputCaption);

                m_showWarning = true;
            }
        }

        public static List<string> WrongCaptions { get; private set; }

        private static int m_rightPad, m_curPrompt, m_lastPrompt;
        private static bool m_alreadyPrompted, m_showWarning;
        private static string m_wrongInputCaption;

        static Input()
        {
            ResetWrongCaption();
        }

        public static void AddWrongInput(string input)
        {
            if (WrongCaptions.Contains(input))
                return;

            WrongCaptions.Insert(0, input);
            m_lastPrompt = WrongCaptions.Count - 1;

            m_showWarning = false;
        }

        public static void ResetWrongCaption()
        {
            m_wrongInputCaption = WrongInput;
            m_alreadyPrompted = false;

            WrongCaptions = new List<string>();
            WrongCaptions.Add(WrongInput);
        }

        public static int ReadInt(string prompt, int min, int max, bool displayPrompt = true)
        {
            if (displayPrompt && !m_alreadyPrompted)
                Output.DisplayPrompt(prompt);

            return ReadInt(min, max, prompt);
        }

        public static int[] ReadInts(string prompt, int min, int max, bool displayPrompt = true)
        {
            if (displayPrompt)
                Output.DisplayPrompt(prompt);

            return ReadInts(min, max, prompt);
        }

        public static int ReadInt(int min, int max, string prompt = "")
        {
            int value = ReadInt(prompt);

            while (value < min || value > max)
            {
                Output.DisplayPrompt("Please enter an integer between {0} and {1} (inclusive)", Color.Red, min, max);
                value = ReadInt();
            }

            return value;
        }

        public static int[] ReadInts(int min, int max, string prompt = "")
        {
            int[] values = ReadInts(prompt).ToArray();

            while (values.Any(value => value < min || value > max))
            {
                Output.DisplayPrompt("Please enter an integer/s between {0} and {1} (inclusive)", Color.Red, min, max);
                values = ReadInts().ToArray();
            }

            return values;
        }

        public static IEnumerable<int> ReadInts(string prompt = "")
        {
            ConsoleOutput output;
            string inputResult;

            if (!GetOutput(ref prompt, out output, out inputResult))
            {
                //Console.WriteLine("There was problem reading the output.", Color.Red);
                yield break;
            }

            bool isMultiple;

            if (output.IsExitKey())
            {
                Program.Instance.NavigateBack();
                yield break;
            }
            else
            {
                inputResult = output.GetValue();
                isMultiple = inputResult.CheckCommaNumbers();
            }

            int value;
            bool firstParse = int.TryParse(inputResult, out value);

            if (!firstParse && !isMultiple)
                do
                {
                    Output.DisplayPrompt("Please enter an integer:", Color.Yellow);
                    output = ConsoleOutput.ReadLineOrKey();

                    if (output.IsExitKey())
                    {
                        Program.Instance.NavigateBack();
                        yield break;
                    }
                    else
                    {
                        inputResult = output.GetValue();
                        isMultiple = inputResult.CheckCommaNumbers();
                    }
                }
                while (!int.TryParse(inputResult, out value) || isMultiple);

            if (!isMultiple)
                yield return value;
            else
            {
                foreach (string val in inputResult.Split(','))
                    yield return int.Parse(val.Trim(' '));
            }
        }

        public static int ReadInt(string prompt = "")
        {
            ConsoleOutput output;
            string inputResult;

            if (!GetOutput(ref prompt, out output, out inputResult))
            {
                //Console.WriteLine("There was problem reading the output.", Color.Red);
                return 0;
            }

            int value;

            while (!int.TryParse(inputResult, out value))
            {
                Output.DisplayPrompt("Please enter an integer:", Color.Yellow);
                output = ConsoleOutput.ReadLineOrKey();

                if (output.IsExitKey())
                {
                    Program.Instance.NavigateBack();
                    return 0;
                }
                else
                    inputResult = output.GetValue();
            }

            return value;
        }

        private static bool GetOutput(ref string prompt, out ConsoleOutput output, out string inputResult)
        {
            bool keepWhile;

            inputResult = "";

            do
            {
                //WrongInputCaption = WrongCaptions.Last();

                output = ConsoleOutput.ReadLineOrKey();

                if (output.IsExitKey())
                {
                    Program.Instance.NavigateBack();
                    return false;
                }
                else if (!output.IsKey())
                    inputResult = output.GetValue();

                keepWhile = output.IsKey() || string.IsNullOrEmpty(inputResult);

                if (m_showWarning || keepWhile)
                    prompt = ShowWarning(prompt, output.CurrentRightPad);
            }
            while (keepWhile);

            WrongInputCaption = WrongCaptions.First();

            return true;
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

        private static string ShowWarning(string prompt, int defaultRightPad)
        {
            bool containsPoints = prompt.Contains(":");

            if (!m_alreadyPrompted)
            {
                m_rightPad = defaultRightPad + prompt.Length + WrongInput.Length + 1;
                m_alreadyPrompted = true;
            }

            if (m_alreadyPrompted)
            {
                bool firstDelete = !prompt.Contains("{0}");

                // This is buggy
                if (firstDelete || m_lastPrompt != m_curPrompt)
                {
                    if (m_lastPrompt != m_curPrompt)
                        m_rightPad = defaultRightPad + prompt.Length + WrongInput.Length + 1;

                    Console.SetCursorPosition(0, Console.CursorTop - 1);

                    for (int i = 0; i < defaultRightPad; i++)
                        Console.Write(' ');

                    prompt = containsPoints ? prompt.Replace(":", " {0}:") : "{0}:";

                    Console.SetCursorPosition(0, Console.CursorTop);

                    string[] _wrongInput = new[] { WrongInputCaption };
                    Console.WriteFormatted(prompt, Color.Yellow, Color.LightGray, _wrongInput);

                    m_lastPrompt = m_curPrompt;
                }
                else
                {
                    Console.SetCursorPosition(defaultRightPad, Console.CursorTop - 1);
                    Console.Write(' ');
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                }
            }

            return prompt;
        }
    }
}