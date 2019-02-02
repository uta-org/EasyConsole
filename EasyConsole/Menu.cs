using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

using Console = Colorful.Console;

namespace EasyConsole
{
    public class Menu
    {
        private const string GoBackCaption = "Go back",
                             WrongInput = "(select another option)";

        private IList<Option> Options { get; set; }

        private int? SelectedOption { get; set; }

        public Menu()
        {
            Options = new List<Option>();
        }

        public void SetSelectOption(int? option)
        {
            if (!option.HasValue)
                return;

            if (option.Value < 0)
                SelectedOption = Options.Count + option.Value - 1;
            else
                SelectedOption = option.Value;
        }

        public void Display(string caption = "Choose an option:")
        {
            for (int i = 0; i < Options.Count; i++)
                Console.WriteLine("{0}. {1}", i + 1, Options[i].Name);

            int[] choices;
            bool alreadyPrompted = false;
            int lastLeftPad = Console.CursorLeft,
                leftPad = lastLeftPad + caption.Length + WrongInput.Length + 1;

            do
            {
                if (alreadyPrompted)
                {
                    if (caption.Contains(WrongInput))
                    {
                        Console.SetCursorPosition(leftPad, Console.CursorTop - 1);
                        Console.Write(' ');
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    }
                    else
                    {
                        Console.SetCursorPosition(lastLeftPad, Console.CursorTop - 1);

                        for (int i = 0; i < leftPad; i++)
                            Console.Write(' ');

                        caption = caption.Replace(":", $" {WrongInput}:");

                        Console.SetCursorPosition(lastLeftPad, Console.CursorTop);
                        Console.Write(caption, Color.Yellow);
                    }
                }

                choices = SelectedOption.HasValue ? new[] { SelectedOption.Value } : (Input.ReadInts(caption, min: 1, max: Options.Count, displayPrompt: !alreadyPrompted));
                alreadyPrompted = true;
            }
            while (choices.Length == 1 && Options[choices[0] - 1].Callback == null || choices.Length > 1);

            foreach (int choice in choices)
                Options[choice - 1].Callback?.Invoke();
        }

        public Menu Add(string option, Action callback)
        {
            return Add(new Option(option, callback));
        }

        public Menu Add(Option option)
        {
            Options.Add(option);
            return this;
        }

        internal void UpdateOptions(IEnumerable<Option> options, Program program, Menu menu = null)
        {
            var opts = options.ToList();

            if (program.NavigationEnabled && !Contains(opts, GoBackCaption))
                opts.Add(BackOption(program));

            if (menu == null)
                Options = opts.ToList();
            else
                menu.Options = opts.ToList();
        }

        internal void AddBackIfEnabled(Program program)
        {
            if (program.NavigationEnabled && !Contains(GoBackCaption))
                Add(BackOption(program));
        }

        private Option BackOption(Program program)
        {
            return new Option(GoBackCaption, () => { program.NavigateBack(); });
        }

        public static bool Contains(IEnumerable<Option> Options, string option)
        {
            return Options.FirstOrDefault((op) => op.Name.Equals(option)) != null;
        }

        public bool Contains(string option)
        {
            return Options.FirstOrDefault((op) => op.Name.Equals(option)) != null;
        }
    }
}