using BattleShipBrain;
using BattleShipBrain.Data;
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
    public List<StartedGame>? Game { get; set; }
    public string? ShareCode { get; set; }
    public List<int> IndexForBoards { get; set; } = new() {0,2};

    public void OnGet()
    {
        LoadedConfigId = HttpContext.Request.Cookies["ConfigId"];
        LoadedSaveId = HttpContext.Request.Cookies["SaveId"];
        PlayerSide = HttpContext.Request.Cookies["PlayerSide"];
        ShareCode = HttpContext.Request.Cookies["ShareCode"];
        if (PlayerSide!.Equals("playerSideB")) IndexForBoards = new List<int>() {2,0};
        if (ShareCode != null)
        {
            if (PlayerSide is "playerSideA")
            {
                PlayerSideToOutput = "Player A";
            }

            if (PlayerSide is "playerSideB")
            {
                PlayerSideToOutput = "Player B";
            }
            Game = AccessData.GetAllGamesFromDb(ShareCode);
            Brain = AccessData.RestoreSaveFromJson(Game[0].SavedGameStateJsnString, Game[0].GameConfigJsnString);
        }
    }
}