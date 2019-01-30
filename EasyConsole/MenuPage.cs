using System;

namespace EasyConsole
{
    public abstract class MenuPage : Page
    {
        protected Menu Menu { get; set; }

        protected static Program CurrentProgram { get; private set; }

        public MenuPage(string title, Program program, params Option[] options)
            : base(title, program)
        {
            Menu = new Menu();
            CurrentProgram = program;

            SetOptions(options);
        }

        public void SetOptions(params Option[] options)
        {
            if (options == null)
                throw new ArgumentNullException("options");

            foreach (var option in options)
                Menu.Add(option);
        }

        public override void Display(string caption = "Choose an option: ")
        {
            base.Display();

            if (Program.NavigationEnabled && !Menu.Contains("Go back"))
                Menu.Add("Go back", () => { Program.NavigateBack(); });

            Menu.SetSelectOption(SelectedOption);
            Menu.Display(caption);
        }
    }
}