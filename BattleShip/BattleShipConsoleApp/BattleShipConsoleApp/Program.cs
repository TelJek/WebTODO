using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using BattleShipBrain;
using BattleShipConsoleUI;
using DAL;

namespace BattleShipConsoleApp
{
    class Program
    {
        private static string? _basePath;
        private static GameConfig conf = new GameConfig();

        static void Main(string[] args)
        {
            Console.WriteLine("Hello Battleship!");
            _basePath = args.Length == 1 ? args[0] : System.IO.Directory.GetCurrentDirectory();
            Console.WriteLine($"Base path: {_basePath}");

            SaveConfig("default", conf);
            
            DirectoryInfo di = new DirectoryInfo(@$"{_basePath + System.IO.Path.DirectorySeparatorChar + "Configs"}");
            FileInfo[] files = di.GetFiles("*.json");
            string str = "";
            
            var inputInMethod = "";
            var confType = "";
            Console.Clear();
            Console.WriteLine(_basePath + System.IO.Path.DirectorySeparatorChar + "Configs");
            Console.WriteLine("=========| Load Configuration |=========\n");
            Console.WriteLine("Available: \n");
            Console.WriteLine("=========| Load Local Configuration |=========");
            Console.WriteLine();
            foreach (FileInfo file in files)
            {
                Console.WriteLine(file.Name.Split(".")[0]);
            }
            Console.WriteLine();
            Console.WriteLine("=========| Load DB Configuration |=========");
            Console.WriteLine();
            using var db = new ApplicationDbContext();
            foreach (var dbConfig in db.GameConfigSaves)
            {
                Console.WriteLine(dbConfig.ConfigName);
            }
            Console.WriteLine();
            Console.WriteLine("=============================================");
            while (confType == "" || inputInMethod == "")
            {
                Console.Write("Db or Local: ");
                confType = Console.ReadLine()?.Trim().ToUpper();
                Console.Write("Your choice(R to Load Default): ");
                inputInMethod = Console.ReadLine()?.Trim();
                Console.WriteLine();
            }
            StartProgram(inputInMethod, confType);
        }

        public static string GetFileNameConfig(string configName)
        {
            var fileNameConfig = _basePath + System.IO.Path.DirectorySeparatorChar + "Configs" + System.IO.Path.DirectorySeparatorChar + $"{configName}.json";
            return fileNameConfig;
        }

        private static string GetConfJsonStr(GameConfig config)
        {
            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            
            var confJsonStr = JsonSerializer.Serialize(config, jsonOptions);

            return confJsonStr;
        }

        public static void SaveConfig(string configName, GameConfig config)
        {
            var fileNameStandardConfig = GetFileNameConfig(configName);

            var confJsonStr = GetConfJsonStr(config);

            Console.WriteLine($"{configName} conf is in: " + fileNameStandardConfig);
            if (!System.IO.File.Exists(fileNameStandardConfig))
            {
                Console.WriteLine($"Saving {configName} config!");
                System.IO.File.WriteAllText(fileNameStandardConfig, confJsonStr);
            }
        }

        public static GameConfig LoadNewConfig(string configName, string loadFrom)
        {
            GameConfig config = new GameConfig();
            var fileNameStandardConfig = GetFileNameConfig(configName);
            if (loadFrom == "LOCAL")
            {
                if (System.IO.File.Exists(fileNameStandardConfig))
                {
                    Console.WriteLine("Loading config...");
                    var confText = System.IO.File.ReadAllText(fileNameStandardConfig);
                    config = JsonSerializer.Deserialize<GameConfig>(confText) ?? throw new InvalidOperationException();
                }
            }
            if (loadFrom == "DB")
            {
                using var db = new ApplicationDbContext();
                var confText = db.GameConfigSaves.FirstOrDefault(c => c.ConfigName == configName);;
                if (confText != null)
                {
                    config = JsonSerializer.Deserialize<GameConfig>(confText.GameConfigJsnString) ?? throw new InvalidOperationException();
                }
            }
            
            return config;
        }
        
        static void StartProgram(string config, string type)
        {
            BSBrain brain = new BSBrain(LoadNewConfig(config, type), _basePath); 
            BsConsoleUi console = new BsConsoleUi(brain, config, type);
            console.DrawUi("main");
        }
    }
}