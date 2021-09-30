using System;

namespace MenuSystem
{
    public class MenuItem 
    {
        public MenuItem(string shortCut, string title, Func<string> runMethod)
        {
            if (string.IsNullOrEmpty(shortCut))
            {
                throw new ArgumentException("shortCut cannot be empty!");
            }
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentException("title cannot be empty!");
            }
            
            ShortCut = shortCut.Trim();
            Title = title.Trim();
            RunMethod = runMethod;
        }

        public string ShortCut { get; private set; }
        public string Title { get; private set; }

        public Func<string> RunMethod { get; private set; }
        
        public override string ToString()
        {
            return ShortCut + ") " + Title;
        }
    }
}