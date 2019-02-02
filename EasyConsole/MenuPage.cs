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

        public delegate IEnumerable<Option> GetOptionsDelegate();

        private Func<Program, GetOptionsDelegate> GetOptions { get; set; }

        public MenuPage(string title, Program program, Func<Program, GetOptionsDelegate> options = null)
            : this(title, program, options?.Invoke(program)?.Invoke()?.ToArray())

        {
            Instance = this;

            CurrentProgram = program;
            GetOptions = options;
        }

        public MenuPage(string title, Program program, params Option[] options)
            : base(title, program)
        {
            Instance = this;

            Menu = new Menu();
            CurrentProgram = program;

            SetOptions(options);
        }

        internal void UpdateOptions(Menu menu = null)
        {
            if (GetOptions != null)
                Menu.UpdateOptions(GetOptions?.Invoke(Program)?.Invoke(), Program, menu);
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

            Menu.AddBackIfEnabled(Program);

            Menu.SetSelectOption(SelectedOption);
            Menu.Display(caption);
        }
    }
}