using BattleShipBrain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.DAL;

namespace WebApplication1.Pages.Game;

public class GameBoards : PageModel
{
    public string? LoadedConfigId { get; set; }
    public string? LoadedSaveId { get; set; }
    public string? PlayerSide { get; set; }
    public string? PlayerSideToOutput { get; set; } = "NotDefined!";
    public BsBrain? Brain { get; set; }

    public void OnGet()
    {
        LoadedConfigId = HttpContext.Request.Cookies["ConfigId"];
        LoadedSaveId = HttpContext.Request.Cookies["SaveId"];
        PlayerSide = HttpContext.Request.Cookies["PlayerSide"];
        if (LoadedConfigId != null && LoadedSaveId != null && PlayerSide != null)
        {
            if (PlayerSide != null && PlayerSide.Equals("playerSideA")) PlayerSideToOutput = "Player A";
            if (PlayerSide != null && PlayerSide.Equals("playerSideB")) PlayerSideToOutput = "Player B";
            Brain = AccessData.RestoreSaveFromJson(LoadedConfigId, LoadedSaveId); 
        }
    }
}