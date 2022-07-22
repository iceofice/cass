using CASS___Construction_Assistance.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CASS___Construction_Assistance.Controllers
{
    public class CustomerController : Controller
    {
        private readonly Data.CassContext _cassContext;

        public CustomerController(Data.CassContext context)
        {
            _cassContext = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _cassContext.Project.ToListAsync());

        }public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Location,Price,Description")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.Status = "Pending";
                _cassContext.Add(project);
                await _cassContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _cassContext.Project
                .FirstOrDefaultAsync(m => m.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, [Bind("Id,Status,Name,Location,Price,Description")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                { 
                    _cassContext.Update(project);
                    await _cassContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var project = await _cassContext.Project.FindAsync(id);

            project.Status = "Completed";
            _cassContext.Update(project);
            await _cassContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _cassContext.Project.FindAsync(id);
            _cassContext.Project.Remove(project);
            await _cassContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        private bool ProjectExists(int id)
        {
            return _cassContext.Project.Any(e => e.Id == id);
        }
    }
}
