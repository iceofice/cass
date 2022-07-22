using CASS___Construction_Assistance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Amazon.S3.Model;
using Amazon.S3;
using System.IO;
using Amazon;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System;
using Amazon.S3.Transfer;

namespace CASS___Construction_Assistance.Controllers
{
    public class ConstructorController : Controller
    {
        //declare constant to refer to the bucketname in S3
        const string bucketname = "construction-assistance";

        private List<string> getCredentialInfo()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            IConfigurationRoot configure = builder.Build();

            List<string> keyList = new List<string>();
            keyList.Add(configure["AWSCredential:key1"]);
            keyList.Add(configure["AWSCredential:key2"]);
            keyList.Add(configure["AWSCredential:key3"]);

            return keyList;
        }

        private readonly CassContext _CassContext;
        public ConstructorController(CassContext context)
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

        public async Task<IActionResult> Myproject()
        {
            var project = from m in _CassContext.Project
                          where m.Constructor_Id.Equals("2")
                          select m;

            return View(await project.ToListAsync());
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
