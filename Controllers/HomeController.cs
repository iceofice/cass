using cass.Areas.Identity.Data;
using cass.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace cass.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {

                return Redirect("~/Identity/Account/Login");
            }
            if (user.Role == "Admin")
            {
                return Redirect("~/Admin");
            }
            if (user.Role == "Customer")
            {
                return Redirect("~/Customer");
            }
            return Redirect("~/Constructor");

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
