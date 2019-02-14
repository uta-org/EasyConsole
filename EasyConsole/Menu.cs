using System;
using System.Collections.Generic;
using System.Linq;

using Console = Colorful.Console;

namespace EasyConsole
{
    public class Menu
    {
        private const string GoBackCaption = "Go back",
                             WrongInput = "(select another option)";

        private Func<GetOptionsDelegate> GetOptions { get; set; }

        private IList<Option> Options { get; set; }

        private int? SelectedOption { get; set; }

        public int Count => Options.Count;

        public Menu()
        {
            Options = new List<Option>();
        }

        public Menu(Func<GetOptionsDelegate> getOptions)
        {
            Options = getOptions?.Invoke()?.Invoke().ToList();
            GetOptions = getOptions;
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

        public void DisplayOptions()
        {
            for (int i = 0; i < Options.Count; i++)
                Console.WriteLine("{0}. {1}", i + 1, Options[i].Name);
        }

        public void DisplayCaption(bool multipleChoices, string caption = "Choose an option:")
        {
            int[] choices;
            bool keepWhile;

            do
            {
                Input.AddWrongInput(WrongInput);

                choices = SelectedOption.HasValue ? new[] { SelectedOption.Value } :
                    (multipleChoices ? Input.ReadInts(caption, min: 1, max: Options.Count) :
                                new[] { Input.ReadInt(caption, min: 1, max: Options.Count) });

                keepWhile = choices.Length == 1 && Options[choices[0] - 1].Callback == null || choices.Length > 1 && choices.Any(choice => Options[choice - 1].Callback == null);

                if (keepWhile)
                    Input.WrongInputCaption = WrongInput;
            }
            while (keepWhile);

            Input.ResetWrongCaption();

            foreach (int choice in choices)
                Options[choice - 1].Callback?.Invoke();
        }

        public void Display(bool multipleChoices, string caption = "Choose an option:")
        {
            DisplayOptions();
            DisplayCaption(multipleChoices, caption);
        }

        public Menu Insert(int index, string option, Action callback)
        {
            return Insert(index, new Option(option, callback));
        }

        public Menu Insert(int index, Option option)
        {
            Options.Insert(index, option);
            return this;
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

        public Menu AddRange(params Option[] options)
        {
            return AddRange(options.AsEnumerable());
        }

        public Menu AddRange(IEnumerable<Option> options)
        {
            foreach (var option in options)
                Options.Add(option);

            return this;
        }

        public void UpdateOptions(Program program)
        {
            if (GetOptions == null)
                throw new Exception("You must set 'GetOptions' calling its ctor.");

            UpdateOptions(GetOptions?.Invoke()?.Invoke(), program);
        }

        private void UpdateOptions(IEnumerable<Option> options, Program program)
        {
            var opts = options.ToList();

            if (program.NavigationEnabled && !Contains(opts, GoBackCaption))
                opts.Add(BackOption(program));

            Options = opts.ToList();
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