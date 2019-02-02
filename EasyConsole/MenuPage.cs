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

        private Func<GetOptionsDelegate> GetOptions { get; set; }

        public MenuPage(string title, Program program, Func<GetOptionsDelegate> options = null)
            : this(title, program, options?.Invoke()?.Invoke()?.ToArray())

        {
            Instance = this;

            Menu = new Menu(options);
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

        internal void UpdateOptions()
        {
            if (GetOptions != null)
                Menu.UpdateOptions(Program);
        }

        private void SetOptions(IEnumerable<Option> options)
        {
            if (options == null)
            {
                IsEmpty = true;
                return;
            }

            Menu.AddRange(options);
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
            Menu.Display(MultipleChoices, caption);
        }
    }
}