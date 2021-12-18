using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages.LoadData;

public class LoadDb : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? ConfigId { get; set; }
    public IActionResult OnGet()
    {
        var configIdFromCookie = Request.Cookies["ConfigId"];
        if (ConfigId == null)
        {
            ConfigId = configIdFromCookie;
        }
        else
        {
            CookieOptions option = new CookieOptions();
            Response.Cookies.Append("ConfigId", ConfigId, option);
            return RedirectToPage("LoadSave");
        }

        return Page();
    }
    
    public async Task<IActionResult> OnPostAsync()
    {
        return RedirectToPage("LoadSave", null);
    }
}