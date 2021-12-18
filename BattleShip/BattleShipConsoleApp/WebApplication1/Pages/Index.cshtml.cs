using System.Net;
using BattleShipBrain;
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
    public static string? LoadedSaveId { get; set; }
    public List<GameConfigSaved>? ListOfConfigs { get; set; }
    public List<GameStateSaved>? ListOfSaves { get; set; }
    public BsBrain? Brain { get; set; }
    public EPlayer Player { get; set; } = EPlayer.NotDefined;
    public string CurrentPlayer { get; set; } = "NotDefined!";
    public string PlayerSideToOutput { get; set; } = "NotDefined!";
    public string PlayerWinnerToOutput { get; set; } = "NotDefined!";
    
    public async Task<IActionResult> OnGet()
    {
        PlayerSide = HttpContext.Request.Cookies["PlayerSide"];
        LoadedConfigId = HttpContext.Request.Cookies["ConfigId"];
        LoadedSaveId = HttpContext.Request.Cookies["SaveId"];
        if (PlayerSide != null && LoadedConfigId != null && LoadedSaveId != null)
        {
            ListOfConfigs = AccessData.GetConfigsFromDb(LoadedConfigId);
            ListOfSaves = AccessData.GetSavesFromDb(LoadedSaveId);
            Brain = AccessData.RestoreSaveFromJson(LoadedConfigId, LoadedSaveId);
        
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