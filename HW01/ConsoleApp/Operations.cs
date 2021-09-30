using System;
using MenuSystem;

namespace ConsoleApp
{
    public class Operations
    {
        private static double _i;
        public static string Add()
        {
            // i
            Console.WriteLine("\nCurrent value: " + _i);
            Console.WriteLine("plus");
            Console.Write("number: ");
            var n = Console.ReadLine()?.Trim();
            double.TryParse(n, out var converted);

            _i = _i + converted;
            Menu.CurrentValue = _i;
            Console.WriteLine("\nNew value: " + _i);
            
            
            return "";
        }
        
        public static string Subtraction()
        {
            // CalculatorCurrentDisplay
            Console.WriteLine("\nCurrent value: " + _i);
            Console.WriteLine("minus");
            Console.Write("number: ");
            var n = Console.ReadLine()?.Trim();
            double.TryParse(n, out var converted);

            _i = _i - converted;
            Menu.CurrentValue = _i;
            Console.WriteLine("\nNew value: " + _i);

            return "";
        }
        
        public static string Division()
        {
            // CalculatorCurrentDisplay
            Console.WriteLine("\nCurrent value: " + _i);
            Console.WriteLine("divide by");
            Console.Write("number: ");
            var n = Console.ReadLine()?.Trim();
            double.TryParse(n, out var converted);

            _i = _i / converted;
            Menu.CurrentValue = _i;
            Console.WriteLine("\nNew value: " + _i);

            return "";
        }
        
        public static string Multiplication()
        {
            // CalculatorCurrentDisplay
            Console.WriteLine("\nCurrent value: " + _i);
            Console.WriteLine("multiply by");
            Console.Write("number: ");
            var n = Console.ReadLine()?.Trim();
            double.TryParse(n, out var converted);

            _i = _i * converted;
            Menu.CurrentValue = _i;
            Console.WriteLine("\nNew value: " + _i);

            return "";
        }

        public static string PowerOf()
        {
            // CalculatorCurrentDisplay
            Console.WriteLine("\nCurrent value: " + _i);
            Console.WriteLine("power by");
            Console.Write("number: ");
            var n = Console.ReadLine()?.Trim();
            double.TryParse(n, out var converted);

            _i = Math.Pow(_i, converted);
            Menu.CurrentValue = _i;
            Console.WriteLine("\nNew value: " + _i);

            return "";
        }

        public static string Negate()
        {
            // CalculatorCurrentDisplay
            Console.WriteLine("\nCurrent value: " + _i);
            _i = _i - _i * 2;
            Menu.CurrentValue = _i;
            Console.WriteLine("negated: " + _i);
            
            return "";
        }
        
        public static string Sqrt()
        {
            // CalculatorCurrentDisplay
            Console.WriteLine("\nCurrent value: " + _i);
            _i = _i * _i ;
            Menu.CurrentValue = _i;
            Console.WriteLine("sqrted: " + _i);
            
            return "";
        }
        
        public static string Root()
        {
            // CalculatorCurrentDisplay
            Console.WriteLine("\nCurrent value: " + _i);
            _i = Math.Sqrt(_i);
            Menu.CurrentValue = _i;
            Console.WriteLine("root: " + _i);
            
            return "";
        }
        
        public static string Abs()
        {
            // CalculatorCurrentDisplay
            Console.WriteLine("\nCurrent value: " + _i);
            _i = Math.Abs(_i);
            Menu.CurrentValue = _i;
            Console.WriteLine("abs: " + _i);
            
            
            return "";
        }
    }
}