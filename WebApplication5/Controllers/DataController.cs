using Microsoft.AspNetCore.Mvc;
using WebApplication5.Models;

namespace DHX.Gantt.Controllers
{
    [Produces("application/json")]
    [Route("api/data")]
    public class DataController : Controller
    {
        private readonly AppDbContext _context;
        public DataController(AppDbContext context)
        {
            _context = context;
        }

        // GET api/data
        [HttpGet]
        public object Get()
        {
            var curUser = WebApplication5.Models.User.GetUser(_context, HttpContext);
            var taskSet = TaskComp.GetAllTasksForMyDepartmentCurMonthJson(curUser, _context);
            return taskSet;
        }

    }
}