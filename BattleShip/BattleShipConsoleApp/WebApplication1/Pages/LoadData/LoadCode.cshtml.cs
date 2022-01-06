using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.DAL;

namespace WebApplication1.Pages.LoadData;

public class LoadCode : PageModel
{
    public string? ShareCode { get; set; }
    public string? PlayerSide { get; set; }
    
    public void OnGet()
    {
        
    }
    
    public async Task<IActionResult> OnPostAsync()
    {
        CookieOptions option = new CookieOptions();

        string sharedCode = Request.Form["inputShareCode"];
        string playerSide = Request.Form["inputPlayerSide"];
        Response.Cookies.Append("PlayerSide", playerSide, option);
        if (sharedCode != null)
        {
            Response.Cookies.Append("ShareCode", sharedCode, option);
            return RedirectToPage("/Index");
        }

        return Page();
    }
}