using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CASS___Construction_Assistance.Controllers
{
    public class users
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string ImageUrl { get; set; }
        public string Role { get; set; }
    }

    public class AdminController : Controller
    {
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> IndexAsync()

        {
            List<List<users>> reservationList = new List<List<users>>();
            List<users> userList = new List<users>();
            List<users> CustomerList = new List<users>();
            List<users> ConstructorList = new List<users>();
            List<users> AdminList = new List<users>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://0sarh7yv36.execute-api.us-east-1.amazonaws.com/prod"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    reservationList = JsonConvert.DeserializeObject<List<List<users>>>(apiResponse);

                    userList = reservationList.First();
                    CustomerList = userList.FindAll(x => x.Role == "Customer");
                    ConstructorList = userList.FindAll(x => x.Role == "Constructor");
                    AdminList = userList.FindAll(x => x.Role == "Admin");

                    // return BadRequest(AdminList);
                }
            }

            return View();

        }

        [Authorize(Roles = "Admin")]
        public IActionResult Customer()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Constructor()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SendPromo(string promoMessage)
        {

            
            List<string> KeyList = getCredentialInfo();
            var client = new AmazonSimpleNotificationServiceClient(KeyList[0], KeyList[1], KeyList[2], Amazon.RegionEndpoint.USEast1);
            var request = new ListTopicsRequest();
            var response = new ListTopicsResponse();
            var topics = "arn:aws:sns:us-east-1:668220914140:EmailSubs";
            

            await client.PublishAsync(new PublishRequest
            {
                Subject = "CASS --- PROMO",
                Message = promoMessage, 
                TopicArn = topics
            });

            return RedirectToAction(nameof(Customer));


        }
    }
}
