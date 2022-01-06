using System.Transactions;
using BattleShipBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.DAL;
using WebApplication1.WebLogic;

namespace WebApplication1.Pages.Game;

public class Ships : PageModel
{
    // public string? LoadedConfigId { get; set; }
    // public string? LoadedSaveId { get; set; }
    public string? PlayerSide { get; set; }
    [BindProperty(SupportsGet = true)] public string? ShipToUseInfo { get; set; }
    public string? ShipToUseFromCookie { get; set; }
    public string? UsedShipsFromCookie { get; set; }
    public EPlayer PlayerSideCheck { get; set; } = EPlayer.NotDefined;
    public string? PlayerSideToOutput { get; set; } = "NotDefined!";
    public BsBrain? Brain { get; set; }
    public List<StartedGame>? Game { get; set; }
    public string? ShareCode { get; set; }

    public bool ShipsDetect { get; set; } = false;

    public void OnGet(string? moveType, int? x, int? y)
    {
        // LoadedConfigId = HttpContext.Request.Cookies["ConfigId"];
        // LoadedSaveId = HttpContext.Request.Cookies["SaveId"];
        PlayerSide = HttpContext.Request.Cookies["PlayerSide"];
        ShipToUseFromCookie = Request.Cookies["ShipToUseInfo"];
        UsedShipsFromCookie = Request.Cookies["UsedShips"];
        ShareCode = HttpContext.Request.Cookies["ShareCode"];
        Game = AccessData.GetAllGamesFromDb(ShareCode);
        Brain = AccessData.RestoreSaveFromJson(Game[0].SavedGameStateJsnString, Game[0].GameConfigJsnString);
        if (ShipToUseInfo == null)
        {
            ShipToUseInfo = ShipToUseFromCookie;
        }
        else
        {
            ShipConfig tempShip = new ShipConfig();
            if (Brain!.GetGameConfig() != null)
            {
                foreach (ShipConfig shipConfig in Brain!.GetGameConfig()!.ShipConfigs)
                {
                    if (shipConfig.Name!.Equals(ShipToUseInfo)) tempShip = shipConfig;
                }
            }
            
            var a = new Dictionary<string, string>()
            {
                {"shipName", tempShip.Name},
                {"shipQuantity", (tempShip.Quantity).ToString()},
                {"shipSizeX", tempShip.ShipSizeX.ToString()},
                {"shipSizeY", tempShip.ShipSizeY.ToString()},
                {"shipStatus", "Old"}
            };
            Response.Cookies.Append("ShipToUseInfo", a.ToLegacyCookieString());
        }

        if (Game != null)
        {
            if (PlayerSide is "playerSideA")
            {
                PlayerSideToOutput = "Player A";
                PlayerSideCheck = EPlayer.PlayerA;
            }

            if (PlayerSide is "playerSideB")
            {
                PlayerSideToOutput = "Player B";
                PlayerSideCheck = EPlayer.PlayerB;
            }


            if (moveType is "ship" && x != null && y != null && ShipToUseFromCookie != null &&
                Brain!.getPlayerLeftShips(PlayerSideCheck).Count > 0)
            {
                var tempPlayerSideEnum = EPlayer.NotDefined;
                if (PlayerSide!.Equals("playerSideA")) tempPlayerSideEnum = EPlayer.PlayerA;
                if (PlayerSide!.Equals("playerSideB")) tempPlayerSideEnum = EPlayer.PlayerB;

                var cord = new Coordinate();
                cord.X = (int) x;
                cord.Y = (int) y;
                if (Brain!.PutShip(tempPlayerSideEnum,
                        new Ship(ShipToUseFromCookie.FromLegacyCookieString()["shipName"], cord,
                            int.Parse(ShipToUseFromCookie.FromLegacyCookieString()["shipSizeX"]),
                            int.Parse(ShipToUseFromCookie.FromLegacyCookieString()["shipSizeY"]))))
                {
                    var activeShip = new Dictionary<string, string>()
                    {
                        {"shipName", ShipToUseFromCookie.FromLegacyCookieString()["shipName"]},
                        {
                            "shipQuantity",
                            (int.Parse(ShipToUseFromCookie.FromLegacyCookieString()["shipQuantity"]) - 1).ToString()
                        },
                        {"shipSizeX", ShipToUseFromCookie.FromLegacyCookieString()["shipSizeX"]},
                        {"shipSizeY", ShipToUseFromCookie.FromLegacyCookieString()["shipSizeY"]},
                        {"shipStatus", "Old"}
                    };
                    Response.Cookies.Append("ShipToUseInfo", activeShip.ToLegacyCookieString());
                }
            }
            if (Brain!.getPlayerLeftShips(PlayerSideCheck).Count == 0)
            {
                Brain!.PlayerPlacedShips(PlayerSideCheck);
            }
            AccessData.UpdateSave(Brain!, Game[0].ConnectCode);
            // ShipToUseFromCookie = Request.Cookies["ShipToUseInfo"];
            // if (ShipToUseFromCookie != null &&
            //     int.Parse(ShipToUseFromCookie.FromLegacyCookieString()["shipQuantity"]) == 0)
            // {
            //     UsedShipsFromCookie = Request.Cookies["UsedShips"];
            //     IDictionary<string, string> usedShips = new Dictionary<string, string>();
            //     if (UsedShipsFromCookie != null)
            //     {
            //         usedShips = UsedShipsFromCookie.FromLegacyCookieString();
            //         if (!usedShips.Values.Any(value =>
            //                 ShipToUseFromCookie.FromLegacyCookieString()["shipName"].Equals(value)))
            //         {
            //             usedShips.Add("shipName" + usedShips.Keys.Count,
            //                 ShipToUseFromCookie.FromLegacyCookieString()["shipName"]);
            //         }
            //     }
            //     else
            //     {
            //         usedShips = new Dictionary<string, string>();
            //         usedShips.Add("shipName" + usedShips.Keys.Count,
            //             ShipToUseFromCookie.FromLegacyCookieString()["shipName"]);
            //     }
            //
            //     Response.Cookies.Append("UsedShips", usedShips.ToLegacyCookieString());
            //     Page();
            // }
            Page();
        }
    }

    public bool CheckIfShipWithNameIsUsed(string? shipName)
    {
        // UsedShipsFromCookie = Request.Cookies["UsedShips"];
        if (Brain!.getPlayerLeftShips(PlayerSideCheck).Count > 0)
        {
            foreach (ShipConfig playerLeftShip in Brain!.getPlayerLeftShips(PlayerSideCheck))
            {
                if (playerLeftShip.Name == shipName) return false;
            }
        }

        return true;
    }

    public bool CheckIsShipActive()
    {
        if (ShipToUseFromCookie == null && ShipToUseInfo != null) return true;
        if (ShipToUseFromCookie != null)
        {
            return int.Parse(ShipToUseFromCookie.FromLegacyCookieString()["shipQuantity"]) != 0;
        }

        return false;
    }

    // public bool CheckIsShipsDone()
    // {
    //     if (Config != null && UsedShipsFromCookie != null)
    //     {
    //         if (UsedShipsFromCookie.FromLegacyCookieString().Values.Count == Config.ShipConfigs.Count)
    //         {
    //             if (PlayerSide is "playerSideA") Brain!.PlayerPlacedShips(EPlayer.PlayerA);
    //             if (PlayerSide is "playerSideB") Brain!.PlayerPlacedShips(EPlayer.PlayerB);
    //             AccessData.UpdateSave(Brain!, int.Parse(LoadedSaveId), LoadedConfigId);
    //             Response.Cookies.Delete("ShipToUseInfo");
    //             return true;
    //         }
    //     }
    //
    //     return false;
    // }
}