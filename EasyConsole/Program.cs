using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using uzLib.Lite.Core;

namespace EasyConsole
{
    public abstract class Program : Singleton<Program>
    {
        public bool IsExiting { get; private set; }

        protected string Title { get; set; }

        public bool BreadcrumbHeader { get; private set; }

        protected Page CurrentPage
        {
            get
            {
                return (History.Any()) ? History.Peek() : null;
            }
        }

        private Dictionary<Type, Page> Pages { get; set; }

        public Stack<Page> History { get; private set; }

        public bool NavigationEnabled { get { return History.Count > 1; } }

        protected Program(string title, bool breadcrumbHeader)
        {
            Title = title;
            Pages = new Dictionary<Type, Page>();
            History = new Stack<Page>();
            BreadcrumbHeader = breadcrumbHeader;
        }

        public virtual Program Run()
        {
            try
            {
                Console.Title = Title;

                CurrentPage.Display();
            }
            catch (Exception e)
            {
                Output.WriteLine(ConsoleColor.Red, e.ToString());
            }
            finally
            {
                if (Debugger.IsAttached)
                {
                    IsExiting = true;
                    Input.ReadString("Press [Enter] to exit");
                }
            }

            return this;
        }

        public void AddPage(Page page)
        {
            Type pageType = page.GetType();

            if (Pages.ContainsKey(pageType))
                Pages[pageType] = page;
            else
                Pages.Add(pageType, page);
        }

        public T AddPage<T>(Page page)
            where T : Page
        {
            Type pageType = page.GetType();

            if (Pages.ContainsKey(pageType))
                Pages[pageType] = page;
            else
                Pages.Add(pageType, page);

            return (T)Pages[pageType];
        }

        public void NavigateHome()
        {
            while (History.Count > 1)
                History.Pop();

            Console.Clear();
            CurrentPage.Display();
        }

        public T SetPage<T>() where T : Page
        {
            Type pageType = typeof(T);

            // This causes a bug... (If we try to display twice the same pageType, this will not update to the new one)
            //if (CurrentPage != null && CurrentPage.GetType() == pageType)
            //    return CurrentPage as T;

            // leave the current page

            // select the new page
            Page nextPage;
            if (!Pages.TryGetValue(pageType, out nextPage))
                throw new KeyNotFoundException($"The given page \"{pageType.Name}\" was not present in the program");

            // enter the new page
            History.Push(nextPage);

            return CurrentPage as T;
        }

        public T NavigateTo<T>() where T : Page
        {
            SetPage<T>();

            Console.Clear();
            CurrentPage.Display();

            return CurrentPage as T;
        }

        public Page NavigateBack(bool forceUpdate, PopAction popAction = PopAction.PopBefore)
        {
            if (forceUpdate)
            {
                if (popAction == PopAction.PopBefore)
                {
                    History.Pop();
                    (CurrentPage as MenuPage).UpdateOptions();
                }
                else if (popAction == PopAction.PopAfter)
                {
                    (CurrentPage as MenuPage).UpdateOptions();
                    History.Pop();
                }
                else
                    (CurrentPage as MenuPage).UpdateOptions();

                Console.Clear();
                CurrentPage.Display();

                return CurrentPage;
            }

            return NavigateBack(null);
        }

        public Page NavigateBack()
        {
            return NavigateBack(null);
        }

        public Page NavigateBack(int option)
        {
            return NavigateBack((int?)option);
        }

        private Page NavigateBack(int? option = null)
        {
            History.Pop();

            Console.Clear();

            CurrentPage.SelectedOption = option;
            CurrentPage.Display();

            return CurrentPage;
        }
    }
}