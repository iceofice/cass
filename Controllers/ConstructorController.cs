using cass.Data;
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using cass.Areas.Identity.Data;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2;

namespace cass.Controllers
{
    public class ConstructorController : Controller
    {
        //declare constant to refer to the bucketname in S3
        const string bucketname = "construction-assistances";

        private readonly UserManager<User> _userManager;

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
        public ConstructorController(CassContext context, UserManager<User> userManager)
        {
            _CassContext = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Constructor")]
        public async Task<IActionResult> Index()
        {
            //passing database

            var projects = from m in _CassContext.Project
                           where m.Status.Equals("Pending")
                           select m;

            return View(await projects.ToListAsync());
        }

        [Authorize(Roles = "Constructor")]
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

        [Authorize(Roles = "Constructor")]
        public async Task<IActionResult> Myproject()
        {
            var user = await _userManager.GetUserAsync(User);

            var project = from m in _CassContext.Project
                          where m.Constructor_Id.Equals(user.Id)
                          select m;

            return View(await project.ToListAsync());
        }

        [Authorize(Roles = "Constructor")]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            return View(user);
        }

        public IActionResult CustomerProfile()
        {
            return View();
        }

        [Authorize(Roles = "Constructor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var project = await _CassContext.Project.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);
            project.Status = "Taken";
            project.Constructor_Name = user.Name;
            project.Constructor_Id = user.Id;
            _CassContext.Update(project);
            await _CassContext.SaveChangesAsync();

            return RedirectToAction(nameof(Myproject));
        }

        public IActionResult Feedback()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Feedback(String projectName)
        {
            List<string> keyList = getCredentialInfo();
            var dynamoDBclient = new AmazonDynamoDBClient(keyList[0], keyList[1], keyList[2], Amazon.RegionEndpoint.USEast1);

            List<Document> returnRecords = new List<Document>(); //for return records in documentmodel type
            List<KeyValuePair<string, string>> singleConvertedRecord = new List<KeyValuePair<string, string>>(); // single converted result
            List<List<KeyValuePair<string, string>>> fullListConvertedRecord = new List<List<KeyValuePair<string, string>>>(); // full list result
            var user = await _userManager.GetUserAsync(User);

            //3. start collecting all the records
            //ScanFilter scanName = new ScanFilter();
            QueryFilter scanName = new QueryFilter("Constructor_Id", QueryOperator.Equal, user.Id);

            scanName.AddCondition("Project_Name", ScanOperator.Contains, projectName); //Convert.ToDouble(Price);

            Table FeedbackTable = Table.LoadTable(dynamoDBclient, "Feedback");
            Search search = FeedbackTable.Query(scanName);

            //step 3: return list from dynamodb, must use the loop to extract record by record 1 by 1
            do
            {
                returnRecords = await search.GetNextSetAsync(); //read content from the search result

                //if found stg
                foreach (var singledocumentrecord in returnRecords)
                {
                    //convert document type record become to the string, string type - for frontend display usage
                    foreach (var attributeKey in singledocumentrecord.GetAttributeNames())
                    {
                        if (attributeKey != "Constructor_Id")
                        {
                            string attributeValue = "";
                            var value = singledocumentrecord[attributeKey];

                            if (value is DynamoDBBool) //change the type from document type to real string type
                                attributeValue = value.AsBoolean().ToString();
                            else if (value is Primitive)
                                attributeValue = value.AsPrimitive().Value.ToString();
                            else if (value is PrimitiveList)
                                attributeValue = string.Join(",", (from primitive
                                                in value.AsPrimitiveList().Entries
                                                                   select primitive.Value).ToArray());

                            singleConvertedRecord.Add(new KeyValuePair<string, string>(attributeKey, attributeValue));
                        }
                        
                    }
                    fullListConvertedRecord.Add(singleConvertedRecord);
                    singleConvertedRecord = new List<KeyValuePair<string, string>>();
                }
            }
            while (!search.IsDone);
            return View(fullListConvertedRecord);

        }
    }
}
