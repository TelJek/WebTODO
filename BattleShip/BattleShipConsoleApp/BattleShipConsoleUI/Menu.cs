using System;
using System.Collections.Generic;
using System.Linq;
using BattleShipBrain;
using MenuSystem;

namespace BattleShipConsoleUI
{
    public class Menu
    {
        private readonly EMenuLevel _menuLevel;

        private readonly List<BattleShipConsoleUI.MenuItem> _menuItems = new List<BattleShipConsoleUI.MenuItem>();
        private readonly BattleShipConsoleUI.MenuItem _menuItemExit = new BattleShipConsoleUI.MenuItem("E", "Exit", null);
        private readonly BattleShipConsoleUI.MenuItem _menuItemReturn = new BattleShipConsoleUI.MenuItem("R", "Return", null);
        private readonly BattleShipConsoleUI.MenuItem _menuItemMain = new BattleShipConsoleUI.MenuItem("M", "Main", null);

        private readonly HashSet<string> _menuShortCuts = new HashSet<string>();
        private readonly HashSet<string> _menuSpecialShortCuts = new HashSet<string>();

        private readonly string _title;

        private readonly Func<List<string>> _getNamesForHeaderInfoString;
        private readonly Func<List<EDataLocationType>> _getDataTypesForHeaderInfoString;

        public Menu(Func<List<string>> getNamesForHeaderInfoString, Func<List<EDataLocationType>> getDataTypesForHeaderInfoString, string title, EMenuLevel menuLevel)
        {
            _getNamesForHeaderInfoString = getNamesForHeaderInfoString;
            _title = title;
            _menuLevel = menuLevel;
            _getDataTypesForHeaderInfoString = getDataTypesForHeaderInfoString;

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

        public void AddMenuItem(BattleShipConsoleUI.MenuItem item, int position = -1)
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

        public void AddMenuItems(List<BattleShipConsoleUI.MenuItem> items)
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
                Console.Clear();
                OutputMenu();
                Console.Write("Your choice: ");
                input = Console.ReadLine()?.Trim().ToUpper();
                Console.WriteLine();
                var isInputValid = _menuShortCuts.Contains(input);
                if (isInputValid)
                {
                    var item = _menuItems.FirstOrDefault(t => t.ShortCut.ToUpper() == input);
                    input = item?.RunMethod == null ? input : item.RunMethod();
                }

                runDone = _menuSpecialShortCuts.Contains(input);

                if (!runDone && !isInputValid)
                {
                    Console.WriteLine($"Unknown shortcut '{input}'!");
                }
            } while (!runDone);

            if (input == _menuItemReturn.ShortCut.ToUpper()) return "";

            return input;
        }

        private void OutputMenu()
        {
            Console.WriteLine("====> " + _title + " <====");
            if (_getNamesForHeaderInfoString != null)
            {
                var headerInfoCurrentPlayer = _getNamesForHeaderInfoString()[0];
                var headerInfoLoadedConfigName = _getNamesForHeaderInfoString()[1];
                var headerInfoLoadedConfigType = _getDataTypesForHeaderInfoString()[0];
                var headerInfoLoadedSaveName = _getNamesForHeaderInfoString()[2];
                var headerInfoLoadedSaveType = _getDataTypesForHeaderInfoString()[1];
                if (headerInfoCurrentPlayer != null)
                {
                    Console.WriteLine($"Current player to move: {headerInfoCurrentPlayer}");
                    Console.WriteLine($"Current loaded configuration name: {headerInfoLoadedConfigName}");
                    Console.WriteLine($"Current loaded configuration type: {headerInfoLoadedConfigType}");
                    Console.WriteLine($"Current loaded save name: {headerInfoLoadedSaveName}");
                    Console.WriteLine($"Current loaded save type: {headerInfoLoadedSaveType}");
                }
            }


            Console.WriteLine("-------------------");

            foreach (var t in _menuItems)
            {
                Console.WriteLine(t);
            }

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
        }
    }
}