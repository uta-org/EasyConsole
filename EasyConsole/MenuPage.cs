using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyConsole
{
    public abstract class MenuPage : Page
    {
        protected Menu Menu { get; set; }

        protected static Program CurrentProgram { get; private set; }

        private bool IsEmpty { get; set; }

        public Action EmptyAction { get; protected set; }

        private Func<IEnumerable<Option>> GetOptions { get; set; }

        public MenuPage(string title, Program program, Func<IEnumerable<Option>> options = null)
            : this(title, program, options?.Invoke().ToArray())

        {
            GetOptions = options;
        }

        public MenuPage(string title, Program program, params Option[] options)
            : base(title, program)
        {
            Menu = new Menu();
            CurrentProgram = program;

            SetOptions(options);
        }

        internal void UpdateOptions(Menu menu = null)
        {
            if (GetOptions != null)
                Menu.UpdateOptions(GetOptions(), menu);
        }

        private void SetOptions(IEnumerable<Option> options)
        {
            if (options == null)
            {
                IsEmpty = true;
                return;
            }

            foreach (var option in options)
                Menu.Add(option);
        }

        public override void Display(string caption = "Choose an option: ")
        {
            if (IsEmpty)
            {
                EmptyAction?.Invoke();
                return;
            }

            base.Display();

            if (Program.NavigationEnabled && !Menu.Contains("Go back"))
                Menu.Add("Go back", () => { Program.NavigateBack(); });

            Menu.SetSelectOption(SelectedOption);
            Menu.Display(caption);
        }
    }
}