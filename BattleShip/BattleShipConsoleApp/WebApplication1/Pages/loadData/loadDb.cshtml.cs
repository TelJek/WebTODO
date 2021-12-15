using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages.loadData;

public class loadDb : PageModel
{
    public void OnGet()
    {
        
    }
    
    public async Task<IActionResult> OnPostAsync()
    {
        var inputDbConfigName = Request.Form["inputDbConfigName"];
        return RedirectToPage("/Index", null);
    }
}