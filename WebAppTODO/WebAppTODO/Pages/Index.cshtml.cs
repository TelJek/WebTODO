using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppTODO.DAL;
using WebAppTODO.Domain;

namespace WebAppTODO.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private AppDbContext _context;
    public string? SessionId { get; set; }
    public List<TodoDb>? TodoDbs { get; set; }
    public List<CategoryDb>? CategoryDbs { get; set; }
    public bool Errors { get; set; }

    public IndexModel(ILogger<IndexModel> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public void OnGet(string doneSwitch, string deleteSwitch, string inputSearch, string sortSelect,
        string searchSelect, string error)
    {
        if (error != null && error != "")
        {
            Errors = true;
        }

        if (doneSwitch != null && doneSwitch != "")
        {
            new AccessData(_context).UpdateTodo(int.Parse(doneSwitch.Split(".")[1]));
        }

        if (deleteSwitch != null && deleteSwitch != "")
        {
            new AccessData(_context).DeleteTodo(int.Parse(deleteSwitch.Split(".")[1]));
        }

        AccessData accessData = new AccessData(_context);
        if (Request.Cookies["SessionId"] != null)
        {
            SessionId = Request.Cookies["SessionId"];
            if (inputSearch != null && inputSearch != "" || sortSelect != null && searchSelect != "" ||
                searchSelect != "")
            {
                TodoDbs = accessData.SortAllTodos(inputSearch, searchSelect, sortSelect, Request.Cookies["SessionId"]!);
            }
            else
            {
                TodoDbs = accessData.GetAllTodos(Request.Cookies["SessionId"]!);
            }
        }

        CategoryDbs = accessData.GetAllCategories();
    }

    public void OnPost()
    {
        var SessionId = Request.Cookies["SessionId"];
        var inputNewList = Request.Form["inputNewList"];
        var inputCode = Request.Form["inputSessionId"];
        var inputTodoName = Request.Form["inputTodoName"];
        var inputPriority = Request.Form["inputTodoPriority"];
        var inputTodoDescription = Request.Form["inputTodoDescription"];
        var inputNewCategory = Request.Form["inputNewCategory"];
        var inputCategoryId = Request.Form["inputCategoryId"];
        var inputDueDate = Request.Form["inputDueDate"];
        CookieOptions option = new CookieOptions();

        if (inputCode.Count > 0 && inputCode[0].Length > 3)
        {
            Response.Cookies.Append("SessionId", inputCode, option);
            Page();
            return;
        }

        if (inputNewList == "yes")
        {
            Random rnd = new Random();
            var newSessionId = rnd.Next(1000, 100000);
            Response.Cookies.Append("SessionId", newSessionId.ToString(), option);
            SessionId = newSessionId.ToString();
            Page();
            return;
        }


        if (inputTodoName[0].Length > 0 && inputTodoDescription[0].Length > 0 && inputNewCategory.Count > 0 &&
            inputPriority[0].Length > 0 && inputDueDate[0].Length > 0)
        {
            if (inputNewCategory[0].Length > 1)
            {
                new AccessData(_context).AddTodoToDb(SessionId, inputTodoName, inputPriority, inputTodoDescription,
                    null, inputNewCategory, inputDueDate);
            }
            else
            {
                new AccessData(_context).AddTodoToDb(SessionId, inputTodoName, inputPriority, inputTodoDescription,
                    int.Parse(inputCategoryId), null, inputDueDate);
            }
        }
        else
        {
            OnGet("", "", "", "", "","error");
        }
    }

    public CategoryDb GetCategoryById(int Id)
    {
        return new AccessData(_context).GetCategoryById(Id);
    }
}