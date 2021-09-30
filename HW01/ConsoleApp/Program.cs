using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using MenuSystem;

namespace ConsoleApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();


            var mainMenu = new Menu("Calculator Main", EMenuLevel.Root);
            mainMenu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("A", "Binary operations", SubmenuBinary),
                new MenuItem("S", "Unary operations", SubmenuUnary),
            });

            mainMenu.Run();
        }

        public static string SubmenuBinary()
        {
            var menu = new Menu("Binary", EMenuLevel.First);
            menu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("+", "+", Operations.Add),
                new MenuItem("-", "-", Operations.Subtraction),
                new MenuItem("/", "/", Operations.Division),
                new MenuItem("*", "*", Operations.Multiplication),
                new MenuItem("^", "^", Operations.PowerOf)
            });
            var res = menu.Run();
            return res;
        }
        
        public static string SubmenuUnary()
        {
            var menu = new Menu("Unary", EMenuLevel.First);
            menu.AddMenuItems(new List<MenuItem>()
            {
                new MenuItem("Negate", "Negate", Operations.Negate),
                new MenuItem("Sqrt", "Sqrt", Operations.Sqrt),
                new MenuItem("Root", "Root", Operations.Root),
                new MenuItem("Abs", "Abs value", Operations.Abs),
            });
            var res = menu.Run();
            return res;
        }
        
    }
}