using System.Net;
using BattleShipBrain;
using BattleShipBrain.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.DAL;

namespace WebApplication1.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public string? PlayerSide { get; set; }
    public static string? LoadedConfigId { get; set; }
    public string? LoadedSaveId { get; set; }
    public string? ShareCode { get; set; }
    public List<StartedGame>? Game { get; set; }
    // public List<GameConfigSaved>? ListOfConfigs { get; set; }
    // public List<GameStateSaved>? ListOfSaves { get; set; }
    public BsBrain? Brain { get; set; }
    public EPlayer Player { get; set; } = EPlayer.NotDefined;
    public string CurrentPlayer { get; set; } = "NotDefined!";
    public string PlayerSideToOutput { get; set; } = "NotDefined!";
    public string PlayerWinnerToOutput { get; set; } = "NotDefined!";
    
    public async Task<IActionResult> OnGet(string? newGame)
    {
        PlayerSide = HttpContext.Request.Cookies["PlayerSide"];
        if (newGame is "yes")
        {
            PlayerSide = null;
            Response.Cookies.Delete("PlayerSide");
            Response.Cookies.Delete("SaveId");
            Response.Cookies.Delete("ConfigId");
            Response.Cookies.Delete("ShareCode");
            Response.Cookies.Delete("ShipToUseInfo");
            return RedirectToPage("/Index");
        }
        LoadedConfigId = HttpContext.Request.Cookies["ConfigId"];
        LoadedSaveId = HttpContext.Request.Cookies["SaveId"];
        ShareCode = HttpContext.Request.Cookies["ShareCode"];
        if (ShareCode != null)
        {
            Game = AccessData.GetAllGamesFromDb(ShareCode);
            Brain = AccessData.RestoreSaveFromJson(Game[0].SavedGameStateJsnString, Game[0].GameConfigJsnString);
            
            // ListOfConfigs = AccessData.GetConfigsFromDb(LoadedConfigId);
            // ListOfSaves = AccessData.GetSavesFromDb(LoadedSaveId);
        
            if (Brain!.GetPlayer() == EPlayer.PlayerA) CurrentPlayer = "Player A";
            if (Brain.GetPlayer() == EPlayer.PlayerB) CurrentPlayer = "Player B";
            if (PlayerSide is "playerSideA")
            {
                PlayerSideToOutput = "Player A";
                Player = EPlayer.PlayerA;
            }

            if (PlayerSide is "playerSideB")
            {
                PlayerSideToOutput = "Player B";
                Player = EPlayer.PlayerB;
            }

            if (PlayerSide != null && Brain.GetWinner() == EPlayer.PlayerA) PlayerWinnerToOutput = "Player A";
            if (PlayerSide != null && Brain.GetWinner() == EPlayer.PlayerB) PlayerWinnerToOutput = "Player B";
            if (PlayerSide != null && Brain.GetWinner() == EPlayer.NotDefined)
                PlayerWinnerToOutput = "No one won, try better!";
        }
        return Page();
    }
}