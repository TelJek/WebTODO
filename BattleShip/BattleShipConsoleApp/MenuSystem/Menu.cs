using System;
using System.Collections.Generic;
using System.Linq;

namespace MenuSystem
{
    public class Menu
    {
        private readonly List<MenuItem> _menuItems = new List<MenuItem>();
        private readonly string _title;
        private readonly HashSet<string> _menuShortCuts = new HashSet<string>();
        private readonly string _playerTurn = "A";
        
        public Menu(string title)
        {
            _title = title;
        }

        public void AddMenuItem(MenuItem item, int position = -1)
        {
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

        public string? Run()
        {
            var runDone = false;
            var input = "";
            do
            {
                OutputMenu();
                Console.Write("Your choice:");
                input = Console.ReadLine()?.Trim().ToUpper();
                var isInputValid = _menuShortCuts.Contains(input);
                if (isInputValid)
                {
                    var item = _menuItems.FirstOrDefault(t => t.ShortCut.ToUpper() == input);
                    input = item?.Method;
                    if (input == "playerAMenu")
                    {
                        OutputPlayerAMenu();
                    }
                    else if (input == "playerBMenu")
                    {
                        OutputPlayerBMenu();
                    }
                }
                if (input == "A")
                {
                     runDone = true;
                }

            } while (!runDone);

            return input;
        }

        private void OutputPlayerAMenu()
        {
            var mainMenu = new Menu("Player A Menu");
            mainMenu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("1", "Board with ships", "playerAMenu"),
                new MenuItem("2", "Board with mines", "playerBMenu")
            });
            var res = mainMenu.Run();
        }
        
        private void OutputPlayerBMenu()
        {
            var mainMenu = new Menu("Player B Menu");
            mainMenu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("1", "Board with ships", "playerAMenu"),
                new MenuItem("2", "Board with mines", "playerBMenu")
            });
            var res = mainMenu.Run();
        }

        public void OutputMenu()
        {
            
            Console.WriteLine("\n====> " + _title + " <====");
            Console.WriteLine($"Player {_playerTurn} turn.");
            Console.WriteLine("-------------------");
            Console.ResetColor();
            

            foreach (var t in _menuItems)
            {
                Console.WriteLine(t);
            }

            Console.WriteLine("-------------------");
            Console.WriteLine("Enter A to return");
            Console.WriteLine("=====================");
        }
    }
}