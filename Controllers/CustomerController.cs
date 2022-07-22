using CASS___Construction_Assistance.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
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

    public class CustomerController : Controller
    {
        private readonly Data.CassContext _cassContext;
        const string bucketName = "construction-assistance";

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

        public CustomerController(Data.CassContext context)
        {
            _cassContext = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _cassContext.Project.ToListAsync());

        }
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(List<IFormFile> images, [Bind("Id,Name,Location,Price,Description")] Project project)
        {
            if (ModelState.IsValid)
            {
                List<string> KeyList = getCredentialInfo();
                var s3clientobject = new AmazonS3Client(KeyList[0], KeyList[1], KeyList[2], RegionEndpoint.USEast1);

                string ImageUrl = "";

                foreach (var image in images)
                {
                    if (image.Length <= 0)
                    {
                        return BadRequest(image.FileName + " is empty!");
                    }
                    else if (image.Length > 1048576)
                    {
                        return BadRequest(image.FileName + " is too big!");
                    }
                    else if (image.ContentType.ToLower() != "image/png"
                        && image.ContentType.ToLower() != "image/jpeg"
                        && image.ContentType.ToLower() != "image/gif")
                    {
                        return BadRequest(image.FileName + " is invalid!");

                    }
                    try
                    {
                        PutObjectRequest uploadRequest = new PutObjectRequest
                        {
                            InputStream = image.OpenReadStream(),
                            BucketName = bucketName + "/images",
                            Key = image.FileName,
                            CannedACL = S3CannedACL.PublicRead
                        };

                        await s3clientobject.PutObjectAsync(uploadRequest);
                        ImageUrl = "https://" + bucketName + ".s3.amazonaws.com/images/" + image.FileName;
                    }
                    catch (Exception ex)
                    {
                        return BadRequest("Unable to upload, " + ex.Message);
                    }
                }

                project.Status = "Pending";
                project.ImageUrl = ImageUrl;
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
        public async Task<IActionResult> Update(String ImageUrl, List<IFormFile> images, int id, [Bind("Id,Status,Constructor_Id,Name,Location,Price,Description")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                List<string> KeyList = getCredentialInfo();
                var s3clientobject = new AmazonS3Client(KeyList[0], KeyList[1], KeyList[2], RegionEndpoint.USEast1);

                string ProjectImageUrl = "";

                if (images.Count == 0)
                {
                    ProjectImageUrl = ImageUrl;
                }
                else
                {
                    foreach (var image in images)
                    {
                        if (image.Length <= 0)
                        {
                            return BadRequest(image.FileName + " is empty!");
                        }
                        else if (image.Length > 1048576)
                        {
                            return BadRequest(image.FileName + " is too big!");
                        }
                        else if (image.ContentType.ToLower() != "image/png"
                            && image.ContentType.ToLower() != "image/jpeg"
                            && image.ContentType.ToLower() != "image/gif")
                        {
                            return BadRequest(image.FileName + " is invalid!");

                        }
                        try
                        {
                            PutObjectRequest uploadRequest = new PutObjectRequest
                            {
                                InputStream = image.OpenReadStream(),
                                BucketName = bucketName + "/images",
                                Key = image.FileName,
                                CannedACL = S3CannedACL.PublicRead
                            };

                            await s3clientobject.PutObjectAsync(uploadRequest);
                            ProjectImageUrl = "https://" + bucketName + ".s3.amazonaws.com/images/" + image.FileName;
                        }
                        catch (Exception ex)
                        {
                            return BadRequest("Unable to upload, " + ex.Message);
                        }
                    }
                }
                try
                {
                    project.ImageUrl = ProjectImageUrl;
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
