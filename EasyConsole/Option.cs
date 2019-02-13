using System;

namespace EasyConsole
{
    public class Option
    {
        public string Name { get; private set; }
        public Action Callback { get; private set; }
        //public Action<int> IndexedCallback { get; private set; }

        public Option(string name)
        {
            Name = name;
        }

        public Option(string name, Action callback)
        {
            Name = name;
            Callback = callback;
        }

        //public Option(string name, Action<int> indexedCallback)
        //{
        //    Name = name;
        //    IndexedCallback = indexedCallback;
        //}

        public override string ToString()
        {
            return Name;
        }
    }
}