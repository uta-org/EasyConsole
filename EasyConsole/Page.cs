﻿using System;
using System.Linq;
using uzLib.Lite.Core;

namespace EasyConsole
{
    public abstract class Page : Singleton<Page>
    {
        public string Title { get; private set; }

        public Program Program { get; set; }

        public int? SelectedOption { get; set; }

        public Page(string title, Program program)
        {
            Title = title;
            Program = program;
        }

        public virtual void Display(string caption = "Choose an option: ")
        {
            if (Program.History.Count > 1 && Program.BreadcrumbHeader)
            {
                string breadcrumb = null;

                foreach (var title in Program.History.Select((page) => page.Title).Reverse())
                    breadcrumb += title + " > ";

                breadcrumb = breadcrumb.Remove(breadcrumb.Length - 3);

                Console.WriteLine(breadcrumb);
            }
            else
            {
                Console.WriteLine(Title);
            }

            Console.WriteLine("---");
        }
    }
}