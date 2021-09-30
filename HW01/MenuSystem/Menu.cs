using System;
using System.Collections.Generic;
using System.Linq;

namespace MenuSystem
{
    public class Menu
    {
        
        private readonly EMenuLevel _menuLevel;

        private readonly List<MenuItem> _menuItems = new List<MenuItem>();
        private readonly MenuItem _menuItemExit = new MenuItem("E", "Exit", null);
        private readonly MenuItem _menuItemReturn = new MenuItem("R", "Return", null);
        private readonly MenuItem _menuItemMain = new MenuItem("M", "Main", null);

        private readonly HashSet<string> _menuShortCuts = new HashSet<string>();
        private readonly HashSet<string> _menuSpecialShortCuts = new HashSet<string>();

        private readonly string _title;
        public static double CurrentValue;

        public Menu(string title, EMenuLevel menuLevel)
        {
            _title = title;
            _menuLevel = menuLevel;
            
            switch (_menuLevel)
            {
                case EMenuLevel.Root:
                    _menuSpecialShortCuts.Add(_menuItemExit.ShortCut.ToUpper());
                    break;
                case EMenuLevel.First:
                    _menuSpecialShortCuts.Add(_menuItemReturn.ShortCut.ToUpper());
                    _menuSpecialShortCuts.Add(_menuItemMain.ShortCut.ToUpper());
                    _menuSpecialShortCuts.Add(_menuItemExit.ShortCut.ToUpper());
                    break;
                case EMenuLevel.SecondOrMore:
                    _menuSpecialShortCuts.Add(_menuItemReturn.ShortCut.ToUpper());
                    _menuSpecialShortCuts.Add(_menuItemMain.ShortCut.ToUpper());
                    _menuSpecialShortCuts.Add(_menuItemExit.ShortCut.ToUpper());
                    break;
            }
        }

        public void UpdateCurrentValue(double value)
        {
            CurrentValue = value;
        }

        public void AddMenuItem(MenuItem item, int position = -1)
        {
            if (_menuSpecialShortCuts.Add(item.ShortCut.ToUpper()) == false)
            {
                throw new ApplicationException($"Conflicting menu shortcut {item.ShortCut.ToUpper()}");
            }
            if (_menuShortCuts.Add(item.ShortCut.ToUpper()) == false)
            {
                throw new ApplicationException($"Conflicting menu shortcut {item.ShortCut.ToUpper()}");
            }


            if (position == -1)
            {
                _menuItems.Add(item);
            }
            else
            {
                _menuItems.Insert(position, item);
            }
        }

        public void DeleteMenuItem(int position = 0)
        {
            _menuItems.RemoveAt(position);
        }

        public void AddMenuItems(List<MenuItem> items)
        {
            foreach (var menuItem in items)
            {
                AddMenuItem(menuItem);
            }
        }

        public string Run()
        {
            var runDone = false;
            var input = "";
            do
            {
                OutputMenu();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Current value: " + CurrentValue);
                Console.Write("Your choice:");
                input = Console.ReadLine()?.Trim().ToUpper();
                var isInputValid = _menuShortCuts.Contains(input);
                if (isInputValid)
                {
                    var item = _menuItems.FirstOrDefault(t => t.ShortCut.ToUpper() == input);
                    input = item?.RunMethod==null ? input : item.RunMethod();
                   
                }
                
                runDone = _menuSpecialShortCuts.Contains(input);
                
                if (!runDone && !isInputValid)
                {
                    Console.WriteLine($"Unknown shortcut '{input}'!");
                }

            } while (!runDone);

            if (input == _menuItemReturn.ShortCut.ToUpper()) return "";
            
            Console.ResetColor();
            
            return input;
        }

        private void OutputMenu()
        {
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n====> " + _title + " <====");
            Console.WriteLine("-------------------");
            Console.ResetColor();
            
            Console.ForegroundColor = ConsoleColor.Cyan;
            foreach (var t in _menuItems)
            {
                Console.WriteLine(t);
            }
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("-------------------");

            switch (_menuLevel)
            {
                case EMenuLevel.Root:
                    Console.WriteLine(_menuItemExit);
                    break;
                case EMenuLevel.First:
                    Console.WriteLine(_menuItemReturn);
                    Console.WriteLine(_menuItemExit);
                    break;
                case EMenuLevel.SecondOrMore:
                    Console.WriteLine(_menuItemReturn);
                    Console.WriteLine(_menuItemMain);
                    Console.WriteLine(_menuItemExit);
                    break;
            }

            Console.WriteLine("=====================");
            Console.ResetColor();
        }
    }
}