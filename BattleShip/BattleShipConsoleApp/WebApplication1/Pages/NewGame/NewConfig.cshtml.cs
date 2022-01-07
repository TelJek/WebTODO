using System.Globalization;
using BattleShipBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using WebApplication1.DAL;
using WebApplication1.WebLogic;

namespace WebApplication1.Pages.NewGame;

public class NewConfig : PageModel
{
    public TempConfigHolder? TempConfigHolder { get; set; }

    public void OnGet(string? passedObject)
    {
        if (passedObject is not null)
        {
            TempConfigHolder = JsonConvert.DeserializeObject<TempConfigHolder>(passedObject);
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        TempConfigHolder? _tempConfig = null;
        
        var inputConfigName = Request.Form["inputConfigName"];
        var inputBoardSizeX = Request.Form["inputBoardSizeX"];
        var inputBoardSizeY = Request.Form["inputBoardSizeY"];
        var inputTouchRule = Request.Form["inputTouchRule"];
        var inputShipQuantity = Request.Form["inputShipQuantity"];
        var checkForBb = Request.Form["checkForBb"];
        var checkForLocal = Request.Form["checkForLocal"];

        var tempTouchRule = EShipTouchRule.NotDefined;
        if (inputTouchRule.Equals("NoTouch")) tempTouchRule = EShipTouchRule.NoTouch;
        if (inputTouchRule.Equals("CornetTouch")) tempTouchRule = EShipTouchRule.CornerTouch;
        if (inputTouchRule.Equals("SideTouch")) tempTouchRule = EShipTouchRule.SideTouch;

        var tempSaveLocation = "NotDefined!";
        if (checkForBb.Equals("on")) tempSaveLocation = "db";
        if (checkForLocal.Equals("on")) tempSaveLocation = "local";

        _tempConfig = new TempConfigHolder()
        {
            EShipTouchRule = tempTouchRule, ShipConfigs = null,
            ConfigName = inputConfigName, WhereToSave = tempSaveLocation
        };

        if (int.TryParse(inputBoardSizeX, out int numberBoardX))
        {
            _tempConfig.BoardSizeX = numberBoardX;
        }
        if (int.TryParse(inputBoardSizeY, out int numberBoardY))
        {
            _tempConfig.BoardSizeX = numberBoardY;
        }
        if (int.TryParse(inputShipQuantity, out int shipQuantity))
        {
            _tempConfig.ShipsToCreate = shipQuantity;
        }
        
        TempConfigHolder = _tempConfig;

        if (Request.Form["inputShipName1"].Count > 0 && inputConfigName.Count > 0 && inputBoardSizeX.Count > 0 && inputBoardSizeY.Count > 0 && inputShipQuantity.Count > 0)
        {
            var shipsConfig = new List<ShipConfig>();
            var i = 1;
            while (Request.Form[$"inputShipName{i}"].Count > 0)
            {
                shipsConfig.Add(new ShipConfig()
                {
                    Name = Request.Form[$"inputShipName{i}"],
                    Quantity = int.Parse(Request.Form[$"inputShipQuantity{i}"]),
                    ShipSizeY = int.Parse(Request.Form[$"inputShipSizeY{i}"]),
                    ShipSizeX = int.Parse(Request.Form[$"inputShipSizeX{i}"])
                });
                i++;
            }

            AccessData.SaveConfigInDb(inputConfigName, int.Parse(inputBoardSizeX), int.Parse(inputBoardSizeY),
                tempTouchRule, shipsConfig);
            return RedirectToPage("/LoadData/LoadDb", null);
        }
        _tempConfig.Error = "notEnoughInputs";
        TempConfigHolder = _tempConfig;
        
        Dictionary<string, string> tempConfig =
            new Dictionary<string, string> {{"passedObject", JsonConvert.SerializeObject(_tempConfig)}};
        return await Task.FromResult<IActionResult>(RedirectToPage("newConfig", tempConfig));
    }
}