using BattleShipBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.DAL;

namespace WebApplication1.Pages.Game;

public class Mines : PageModel
{
    public string? PlayerSide { get; set; }
    public string? PlayerSideToOutput { get; set; } = "NotDefined!";
    public EPlayer PlayerSideToCheck { get; set; } = EPlayer.NotDefined;
    public BsBrain? Brain { get; set; }
    public List<StartedGame>? Game { get; set; }
    public string? ShareCode { get; set; }
    public List<int> IndexForBoards { get; set; } = new() {0,2};

    public IActionResult OnGet(string? moveType, int? x, int? y)
    {
        PlayerSide = HttpContext.Request.Cookies["PlayerSide"];
        ShareCode = HttpContext.Request.Cookies["ShareCode"];
        Game = AccessData.GetAllGamesFromDb(ShareCode);
        Brain = AccessData.RestoreSaveFromJson(Game[0].SavedGameStateJsnString, Game[0].GameConfigJsnString);
        if (PlayerSide!.Equals("playerSideB")) IndexForBoards = new List<int>() {2,0};
        if (Brain != null)
        {
            if (PlayerSide is "playerSideA")
            {
                PlayerSideToOutput = "Player A";
                PlayerSideToCheck = EPlayer.PlayerA;
            }

            if (PlayerSide is "playerSideB")
            {
                PlayerSideToOutput = "Player B";
                PlayerSideToCheck = EPlayer.PlayerB;
            }
            if (moveType is "mine" && x != null && y != null && Brain.GetPlayer() == PlayerSideToCheck)
            {
                if (PlayerSide!.Equals("playerSideA"))
                {
                    Brain!.PutBomb((int) x,(int) y, EPlayer.PlayerB);
                    if (!Brain.DidBombHit((int) x, (int) y, EPlayer.PlayerB))
                    {
                        Brain.ChangePlayerNum();
                    }
                    Brain.DidPlayerWon(EPlayer.PlayerA);
                    AccessData.UpdateSave(Brain!, Game[0].ConnectCode);
                    if (!Brain.DidBombHit((int) x, (int) y, EPlayer.PlayerB) || Brain.DidPlayerWon(EPlayer.PlayerA))
                    {
                        return RedirectToPage("/Index");
                    }
                }
                
                if (PlayerSide!.Equals("playerSideB"))
                {
                    Brain!.PutBomb((int) x,(int) y, EPlayer.PlayerA);
                    if (!Brain.DidBombHit((int) x, (int) y, EPlayer.PlayerA))
                    {
                        Brain.ChangePlayerNum();
                    }

                    Brain.DidPlayerWon(EPlayer.PlayerB);
                    AccessData.UpdateSave(Brain!, Game[0].ConnectCode);
                    if (!Brain.DidBombHit((int) x, (int) y, EPlayer.PlayerA) || Brain.DidPlayerWon(EPlayer.PlayerB))
                    {
                        return RedirectToPage("/Index");
                    }
                }
            }
        }

        return Page();
    }
}