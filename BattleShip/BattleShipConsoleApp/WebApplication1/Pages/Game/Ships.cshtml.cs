using BattleShipBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.DAL;
using WebApplication1.WebLogic;

namespace WebApplication1.Pages.Game;

public class Ships : PageModel
{
    public string? LoadedConfigId { get; set; }
    public string? LoadedSaveId { get; set; }
    public string? PlayerSide { get; set; }
    [BindProperty(SupportsGet = true)] 
    public string? ShipToUseInfo { get; set; }
    public string? ShipToUseFromCookie { get; set; }
    public string? UsedShipsFromCookie { get; set; }
    public EPlayer PlayerSideCheck { get; set; } = EPlayer.NotDefined;
    public string? PlayerSideToOutput { get; set; } = "NotDefined!";
    public BsBrain? Brain { get; set; }
    public GameConfig? Config { get; set; }

    public void OnGet(string? moveType, int? x, int? y)
    {
        LoadedConfigId = HttpContext.Request.Cookies["ConfigId"];
        LoadedSaveId = HttpContext.Request.Cookies["SaveId"];
        PlayerSide = HttpContext.Request.Cookies["PlayerSide"];
        ShipToUseFromCookie = Request.Cookies["ShipToUseInfo"];
        UsedShipsFromCookie = Request.Cookies["UsedShips"];
        Brain = AccessData.RestoreSaveFromJson(LoadedConfigId, LoadedSaveId);
        Config = AccessData.RestoreConfigFromJson(LoadedConfigId!);
        if (ShipToUseInfo == null)
        {
            ShipToUseInfo = ShipToUseFromCookie;
        }
        else
        {
            ShipConfig tempShip = new ShipConfig();
            foreach (ShipConfig shipConfig in Config!.ShipConfigs)
            {
                if (shipConfig.Name!.Equals(ShipToUseInfo)) tempShip = shipConfig;
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

        if (LoadedConfigId != null && LoadedSaveId != null && PlayerSide != null)
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
                int.Parse(ShipToUseFromCookie.FromLegacyCookieString()["shipQuantity"]) != 0)
            {
                if (PlayerSide!.Equals("playerSideA"))
                {
                    var cord = new Coordinate();
                    cord.X = (int) x;
                    cord.Y = (int) y;
                    if (Brain!.PutShip(EPlayer.PlayerA,
                            new Ship(ShipToUseFromCookie.FromLegacyCookieString()["shipName"], cord,
                                int.Parse(ShipToUseFromCookie.FromLegacyCookieString()["shipSizeX"]),
                                int.Parse(ShipToUseFromCookie.FromLegacyCookieString()["shipSizeY"]))))
                    {
                        var activeShip = new Dictionary<string, string>()
                        {
                            {"shipName", ShipToUseFromCookie.FromLegacyCookieString()["shipName"]},
                            {"shipQuantity", (int.Parse(ShipToUseFromCookie.FromLegacyCookieString()["shipQuantity"]) - 1).ToString()},
                            {"shipSizeX", ShipToUseFromCookie.FromLegacyCookieString()["shipSizeX"]},
                            {"shipSizeY", ShipToUseFromCookie.FromLegacyCookieString()["shipSizeY"]},
                            {"shipStatus", "Old"}
                        };
                        Response.Cookies.Append("ShipToUseInfo", activeShip.ToLegacyCookieString());
                    }
                }

                if (PlayerSide!.Equals("playerSideB"))
                {
                    Brain!.PutBomb(x ?? default(int), y ?? default(int), EPlayer.PlayerA);
                }

                AccessData.UpdateSave(Brain!, int.Parse(LoadedSaveId), LoadedConfigId);
            }
            ShipToUseFromCookie = Request.Cookies["ShipToUseInfo"];
            if (ShipToUseFromCookie != null &&
                int.Parse(ShipToUseFromCookie.FromLegacyCookieString()["shipQuantity"]) == 0)
            {
                UsedShipsFromCookie = Request.Cookies["UsedShips"];
                IDictionary<string,string> usedShips = new Dictionary<string, string>();
                if (UsedShipsFromCookie != null)
                {
                    usedShips = UsedShipsFromCookie.FromLegacyCookieString();
                    if (!usedShips.Values.Any(value =>
                            ShipToUseFromCookie.FromLegacyCookieString()["shipName"].Equals(value)))
                    {
                        usedShips.Add("shipName"+usedShips.Keys.Count, ShipToUseFromCookie.FromLegacyCookieString()["shipName"]);
                    }
                }
                else
                {
                    usedShips = new Dictionary<string, string>();
                    usedShips.Add("shipName"+usedShips.Keys.Count, ShipToUseFromCookie.FromLegacyCookieString()["shipName"]);
                }
                Response.Cookies.Append("UsedShips", usedShips.ToLegacyCookieString());
                Page();
            }
        }
    }
}