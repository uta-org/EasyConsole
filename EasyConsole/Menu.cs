using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyConsole
{
    public class Menu
    {
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

            int choice = SelectedOption.HasValue ? SelectedOption.Value : (Input.ReadInt(caption, min: 1, max: Options.Count) - 1);

            Options[choice].Callback?.Invoke();
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

        internal void UpdateOptions(IEnumerable<Option> options, Menu menu = null)
        {
            if (menu == null)
                Options = options.ToList();
            else
                menu.Options = options.ToList();
        }

        public bool Contains(string option)
        {
            return Options.FirstOrDefault((op) => op.Name.Equals(option)) != null;
        }
    }
}