using BattleShipBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.DAL;

namespace WebApplication1.Pages.Game;

public class GameMove : PageModel
{
    public string? LoadedConfigId { get; set; }
    public string? LoadedSaveId { get; set; }
    public string? PlayerSide { get; set; }
    public string? PlayerSideToOutput { get; set; } = "NotDefined!";
    public BsBrain? Brain { get; set; }

    public void OnGet(string? moveType, int? x, int? y)
    {
        LoadedConfigId = HttpContext.Request.Cookies["ConfigId"];
        LoadedSaveId = HttpContext.Request.Cookies["SaveId"];
        PlayerSide = HttpContext.Request.Cookies["PlayerSide"];
        if (LoadedConfigId != null && LoadedSaveId != null && PlayerSide != null)
        {
            if (PlayerSide is "playerSideA") PlayerSideToOutput = "Player A";
            if (PlayerSide is "playerSideB") PlayerSideToOutput = "Player B";
            Brain = AccessData.RestoreSaveFromJson(LoadedConfigId, LoadedSaveId);
            if (moveType is "mine" && x != null && y != null)
            {
                if (PlayerSide!.Equals("playerSideA"))
                {
                    Brain!.PutBomb(x ?? default(int), y ?? default(int), EPlayer.PlayerB);
                    Brain.DidPlayerWon(EPlayer.PlayerA);
                }
                
                if (PlayerSide!.Equals("playerSideB"))
                {
                    Brain!.PutBomb(x ?? default(int), y ?? default(int), EPlayer.PlayerA);
                    Brain.DidPlayerWon(EPlayer.PlayerB);
                }

                AccessData.UpdateSave(Brain!, int.Parse(LoadedSaveId), LoadedConfigId);
            }
        }
    }
}