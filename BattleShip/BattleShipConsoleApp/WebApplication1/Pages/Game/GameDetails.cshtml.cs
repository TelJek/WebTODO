using BattleShipBrain;
using BattleShipBrain.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.DAL;

namespace WebApplication1.Pages.Game;

public class GameDetails : PageModel
{
    // public string? LoadedConfigId { get; set; }
    // public string? LoadedSaveId { get; set; }
    public string? PlayerSide { get; set; }
    public string? PlayerSideToOutput { get; set; } = "NotDefined!";
    public string? currentPlayer { get; set; } = "NotDefined!";
    // public List<GameConfigSaved>? ListOfConfigs { get; set; }
    // public GameConfig? Config { get; set; }
    public List<GameStateSaved>? ListOfSaves { get; set; }
    public BsBrain? Brain { get; set; }
    public string? ShareCode { get; set; }
    public List<StartedGame>? Game { get; set; }
    
    public IActionResult OnGet()
    {
        // LoadedConfigId = HttpContext.Request.Cookies["ConfigId"];
        // LoadedSaveId = HttpContext.Request.Cookies["SaveId"];
        PlayerSide = HttpContext.Request.Cookies["PlayerSide"];
        ShareCode = HttpContext.Request.Cookies["ShareCode"];
        if (ShareCode != null)
        {
            if (PlayerSide != null && PlayerSide.Equals("playerSideA")) PlayerSideToOutput = "Player A";
            if (PlayerSide != null && PlayerSide.Equals("playerSideB")) PlayerSideToOutput = "Player B";
            
            Game = AccessData.GetAllGamesFromDb(ShareCode);
            Brain = AccessData.RestoreSaveFromJson(Game[0].SavedGameStateJsnString, Game[0].GameConfigJsnString);
            
            // ListOfConfigs = AccessData.GetConfigsFromDb(LoadedConfigId);
            // Config = AccessData.RestoreConfigFromJson(LoadedConfigId!);
            // ListOfSaves = AccessData.GetSavesFromDb(LoadedSaveId);
            // Brain = AccessData.RestoreSaveFromJson(LoadedConfigId, LoadedSaveId);
            if (Brain!.GetPlayer() == EPlayer.PlayerA) currentPlayer = "Player A";
            if (Brain.GetPlayer() == EPlayer.PlayerB) currentPlayer = "Player B";  
        }
        return Page();
    }
}