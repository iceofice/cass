using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CASS___Construction_Assistance.Controllers
{
    public class AdminUser : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
