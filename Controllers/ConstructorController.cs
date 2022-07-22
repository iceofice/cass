using CASS___Construction_Assistance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace CASS___Construction_Assistance.Controllers
{
    public class ConstructorController : Controller
    {
        private readonly CASSContext _CassContext;
        public ConstructorController(CASSContext context)
        {
            _CassContext = context;
        }

        public async Task<IActionResult> Index()
        {
            //passing database
           
            var projects = from m in _CassContext.Project
                          where m.Status.Equals("Pending")
                          select m;

            return View(await projects.ToListAsync());
        }

        public async Task<IActionResult> Shop(int? id)
        {
            //passing database
            if (id == null)
            {
                return NotFound();
            }

            var project = await _CassContext.Project
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        public async  Task<IActionResult> Myproject()
        {
            var project = from m in _CassContext.Project
                        where m.Constructor_Id.Equals("2")
                        select m;

            return View(await project.FirstAsync());
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult CustomerProfile()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var project = await _CassContext.Project.FindAsync(id);

            project.Status = "Taken";
            project.Constructor_Id = "2";
            _CassContext.Update(project);
            await _CassContext.SaveChangesAsync();

            return RedirectToAction(nameof(Myproject));
        }
    }
}
