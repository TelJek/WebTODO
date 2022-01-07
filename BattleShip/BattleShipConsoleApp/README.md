
### Scaffolding

~~~
dotnet ef migrations --project DAL --startup-project BattleShipConsoleApp add Initial
dotnet ef migrations --project DAL --startup-project BattleShipConsoleApp bundle
dotnet ef migrations --project DAL --startup-project BattleShipConsoleApp drop
~~~

~~~
dotnet aspnet-codegenerator razorpage -n GameConfigSaved -outDir Pages/GameConfigSaves -dc AppDbContext -udl --referenceScriptLibraries -f
dotnet aspnet-codegenerator razorpage -n GameStateSaved -outDir Pages/GameStateSaves -dc AppDbContext -udl --referenceScriptLibraries -f
~~~