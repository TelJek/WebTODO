using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.DAL;

namespace WebApplication1.Pages.LoadData;

public class LoadSave : PageModel
{
    [BindProperty(SupportsGet = true)] public string? SaveId { get; set; }

    [BindProperty(SupportsGet = true)] public string? PlayerSide { get; set; }
    
    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var inputSaveName = Request.Form["inputNewSaveName"];
        var playerSideFromCookie = Request.Cookies["PlayerSide"];
        var saveIdFromCookie = Request.Cookies["SaveId"];
        if (PlayerSide == null)
        {
            PlayerSide = playerSideFromCookie;
        }
        else
        {
            CookieOptions option = new CookieOptions();
            Response.Cookies.Append("PlayerSide", PlayerSide, option);
        }

        if (inputSaveName[0].Length > 3)
        {
            AccessData.CreateNewSave(inputSaveName, Request.Cookies["ConfigId"]);
        }
        else
        {
            if (SaveId == null)
            {
                SaveId = saveIdFromCookie;
            }
            else
            {
                CookieOptions option = new CookieOptions();
                Response.Cookies.Append("SaveId", SaveId, option);
                var shareCode = new Random().Next(10000, 100000);
                Response.Cookies.Append("ShareCode", shareCode.ToString(), option);
                AccessData.CreateNewGame(SaveId, Request.Cookies["ConfigId"], shareCode);
                return RedirectToPage("/Index");
            }
        }

        if (inputSaveName.Count == 0 && inputSaveName[0].Length < 3 && Request.Cookies["SaveId"] is not null)
        {
            return RedirectToPage("/LoadSave?error=inputError");
        }
        
        return Page();
    }
}