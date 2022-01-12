using System.Globalization;
using WebAppTODO.Domain;

namespace WebAppTODO.DAL;

public class AccessData
{
    private AppDbContext _context;
    
    public AccessData(AppDbContext context)
    {
        _context = context;
    }

    public List<TodoDb> SortAllTodos(string? searchString, string? searchSelect, string? sortSelect, string sessionId)
    {
        List<TodoDb> resultList = new List<TodoDb>();
        List<TodoDb> listToSort = GetAllTodos(sessionId);
        if (!string.IsNullOrEmpty(searchString))
        {
            foreach (var todoDb in listToSort)
            {
                if (searchSelect == "searchHeadline")
                {
                    if (todoDb.Headline.ToUpper().Contains(searchString.ToUpper()))
                    {
                        resultList.Add(todoDb);
                    }
                }
                if (searchSelect == "searchDescription")
                {
                    if (todoDb.Description.ToUpper().Contains(searchString.ToUpper()))
                    {
                        resultList.Add(todoDb);
                    }
                }
                if (searchSelect == "searchCategory")
                {
                    var categoryId = GetAllCategories()
                        .FindAll(x => x.CategoryName.ToUpper().Contains(searchString.ToUpper()));
                    foreach (CategoryDb categoryDb in categoryId)
                    {
                        if (todoDb.CategoryDbId == categoryDb.Id)
                        {
                            resultList.Add(todoDb);
                        }
                    }
                }
                if (searchSelect == "searchPriority")
                {
                    int priorityToSave = 0;
                    if (searchString is "high" or "1") priorityToSave = 1;
                    if (searchString is "medium" or "2") priorityToSave = 2;
                    if (searchString is "low" or "3") priorityToSave = 3;
                    if (todoDb.Priority == priorityToSave)
                    {
                        resultList.Add(todoDb);
                    }
                }
            }
        }
        else if (string.IsNullOrEmpty(searchString))
        {
            resultList = GetAllTodos(sessionId);
        }

        if (!string.IsNullOrEmpty(sortSelect))
        {
            // sortPriority
            // sortCreation
            // sortDueDate
            if (sortSelect == "sortPriority")
            {
                resultList.Sort((x,y) => x.Priority.CompareTo(y.Priority));
            } else if (sortSelect == "sortCreation")
            {
                resultList.Sort((x,y) => x.CreationDate.CompareTo(y.CreationDate));
            } else if (sortSelect == "sortDueDate")
            {
                resultList.Sort((x,y) => x.DueDate.CompareTo(y.DueDate));
            }
        }
        
        return resultList;
    }

    public List<TodoDb> GetAllTodos(string sessionId)
    {
        List<TodoDb> resultList = new List<TodoDb>();
        foreach (var todoDb in _context.TodoDbs)
        {
            if (todoDb.SessionID == sessionId)
            {
                resultList.Add(todoDb);
            }
        }

        return resultList;
    }

    public List<CategoryDb> GetAllCategories()
    {
        List<CategoryDb> resultList = new List<CategoryDb>();
        foreach (var categoryDb in _context.CategoryDbs)
        {
            resultList.Add(categoryDb);
        }

        return resultList;
    }
    
    public CategoryDb GetCategoryById(int id)
    {
        return _context.CategoryDbs.FirstOrDefault(c => c.Id == id)!;
    }

    public void AddTodoToDb(string sessionId, string headline, string priority, string description, int? categoryId,
        string? categoryName, string dueDate)
    {
        int priorityToSave = 0;
        if (priority == "high") priorityToSave = 1;
        if (priority == "medium") priorityToSave = 2;
        if (priority == "low") priorityToSave = 3;
        if (priority.Length == 1 && priority != "") priorityToSave = int.Parse(priority);

        var cultureInfo = new CultureInfo("de-DE");
        string dateString = dueDate;

        var dateTime = new DateTime();

        if (DateTime.TryParse(dateString, cultureInfo,
                DateTimeStyles.NoCurrentDateDefault, out dateTime))
        {
            int categoryIdNew = 0;
            if (categoryId != null) categoryIdNew = (int) categoryId;
            if (categoryId == null && categoryName != null)
            {
                AddCategoryInDb(categoryName);
                categoryIdNew = _context.CategoryDbs.FirstOrDefault(c => c.CategoryName == categoryName)!.Id;
            }

            var todo = new TodoDb
            {
                SessionID = sessionId,
                Headline = headline,
                Priority = priorityToSave,
                Description = description,
                Done = false,
                CreationDate = DateTime.Now,
                DueDate = dateTime,
                CategoryDbId = categoryIdNew
            };
            _context.TodoDbs.Add(todo);
            _context.SaveChanges();
        }
    }

    public void UpdateTodo(int id)
    {
        var todoToUpdate = _context.TodoDbs.SingleOrDefault(t => t.Id == id);
        if (todoToUpdate != null) todoToUpdate.Done = !todoToUpdate.Done;
        _context.SaveChanges();
    }

    public void DeleteTodo(int id)
    {
        var todoToUpdate = _context.TodoDbs.SingleOrDefault(t => t.Id == id);
        if (todoToUpdate != null) _context.TodoDbs.Remove(todoToUpdate);
        _context.SaveChanges();
    }

    public void AddCategoryInDb(string name)
    {
        var category = new CategoryDb
        {
            CategoryName = name
        };
        var duplicate = _context.CategoryDbs.FirstOrDefault(c => c.CategoryName == name);
        if (duplicate != null && duplicate.CategoryName == name)
        {
            return;
        }
        _context.CategoryDbs.Add(category);
        _context.SaveChanges();
    }
}