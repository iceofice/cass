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
using Microsoft.AspNetCore.Authorization;
using CASS___Construction_Assistance.Data;
using Microsoft.AspNetCore.Identity;
using CASS___Construction_Assistance.Areas.Identity.Data;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace CASS___Construction_Assistance.Controllers
{

    public class CustomerController : Controller
    {
        private readonly Data.CassContext _cassContext;

        private readonly UserManager<User> _userManager;

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

        public CustomerController(Data.CassContext context, UserManager<User> userManager)
        {
            _cassContext = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Index()
        {

            var user = await _userManager.GetUserAsync(User);

            var project = from m in _cassContext.Project
                          where m.Customer_Id.Equals(user.Id)
                          select m;

            return View(await project.ToListAsync());

        }

        [Authorize(Roles = "Customer")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Customer")]
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
                var user = await _userManager.GetUserAsync(User);

                project.Status = "Pending";
                project.ImageUrl = ImageUrl;
                project.Customer_Id = user.Id;
                project.Customer_Name = user.Name;
                _cassContext.Add(project);
                await _cassContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        [Authorize(Roles = "Customer")]
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
            //return BadRequest(project);
            //Console.WriteLine(project.Name);

            return View(project);
        }

        [Authorize(Roles = "Customer")]
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

        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, String msg, String Constructor_Id, String Name)
        {
            List<string> keyList = getCredentialInfo();
            var dynamoDBclient = new AmazonDynamoDBClient(keyList[0], keyList[1], keyList[2], Amazon.RegionEndpoint.USEast1);

            Dictionary<string, AttributeValue> attributes = new Dictionary<string, AttributeValue>();

            //4. starting to add the data to DynamoDB
            try
            {
                attributes["Constructor_Id"] = new AttributeValue { S = Constructor_Id }; //partition key
                attributes["Project_Id"] = new AttributeValue { N = id.ToString() }; //partition key
                attributes["Project_Name"] = new AttributeValue { S = Name }; //partition key
                attributes["Feedback"] = new AttributeValue { S = msg };
                attributes["Created_At"] = new AttributeValue
                {
                    S = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };


                PutItemRequest orderRequest = new PutItemRequest
                {
                    TableName = "Feedback",
                    Item = attributes
                };

                await dynamoDBclient.PutItemAsync(orderRequest);
            }

            catch (AmazonDynamoDBException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var project = await _cassContext.Project.FindAsync(id);

            project.Status = "Completed";
            _cassContext.Update(project);
            await _cassContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        [Authorize(Roles = "Customer")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _cassContext.Project.FindAsync(id);
            _cassContext.Project.Remove(project);
            await _cassContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            return View(user);
        }

        [Authorize(Roles = "Customer")]
        private bool ProjectExists(int id)
        {
            return _cassContext.Project.Any(e => e.Id == id);
        }
    }
}
