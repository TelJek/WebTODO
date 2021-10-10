using System;
using System.Text.Json;
using BattleShipBrain;
using BattleShipConsoleUI;

namespace BattleShipConsoleApp
{
    class Program
    {
        private static string? _basePath;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello Battleship!");
            _basePath = args.Length == 1 ? args[0] : System.IO.Directory.GetCurrentDirectory();
            Console.WriteLine($"Base path: {_basePath}");

            var conf = new GameConfig();

            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            var fileNameStandardConfig = _basePath + System.IO.Path.DirectorySeparatorChar + "Configs" + System.IO.Path.DirectorySeparatorChar + "standard.json";

            var confJsonStr = JsonSerializer.Serialize(conf, jsonOptions);

            Console.WriteLine("Standard conf is in: " + fileNameStandardConfig);
            if (!System.IO.File.Exists(fileNameStandardConfig))
            {
                Console.WriteLine("Saving default config!");
                System.IO.File.WriteAllText(fileNameStandardConfig, confJsonStr);
            }

            if (System.IO.File.Exists(fileNameStandardConfig))
            {
                Console.WriteLine("Loading config...");
                var confText = System.IO.File.ReadAllText(fileNameStandardConfig);
                conf = JsonSerializer.Deserialize<GameConfig>(confText);
            }
            
            var fileNameSave = _basePath + System.IO.Path.DirectorySeparatorChar + "SaveGames" + System.IO.Path.DirectorySeparatorChar + "game.json";

            BSBrain brain = new BSBrain(conf);
            BSConsoleUI console = new BSConsoleUI(brain);
            console.DrawUi();
        }
    }
}