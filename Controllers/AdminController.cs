using CASS___Construction_Assistance.Models;
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
using Microsoft.AspNetCore.Identity;
using CASS___Construction_Assistance.Areas.Identity.Data;

namespace CASS___Construction_Assistance.Controllers
{
   
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
        private readonly Data.CassContext _cassContext;

        private readonly UserManager<User> _userManager;
        public AdminController(Data.CassContext context, UserManager<User> userManager)
        {
            _cassContext = context;
            _userManager = userManager;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> IndexAsync()
        {
            List<int> resultList;
            List<List<Project>> rawProject = new List<List<Project>>();
            List<List<APIusers>> rawUsers = new List<List<APIusers>>();
            List<APIusers> userList = new List<APIusers>();
            List<APIusers> customerList = new List<APIusers>();
            List<APIusers> constructorList = new List<APIusers>();
            List<Project> projectList = new List<Project>();
            List<Project> finishedProject = new List<Project>();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://px39op01o5.execute-api.us-east-1.amazonaws.com/userList"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    rawUsers = JsonConvert.DeserializeObject<List<List<APIusers>>>(apiResponse);

                    userList = rawUsers.First();
                    customerList = userList.FindAll(x => x.Role == "Customer");
                    constructorList = userList.FindAll(x => x.Role == "Constructor");
                }
                using (var response = await httpClient.GetAsync("https://4f4j3o96c4.execute-api.us-east-1.amazonaws.com/projectList"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    rawProject = JsonConvert.DeserializeObject<List<List<Project>>>(apiResponse);

                    projectList = rawProject.First().FindAll(x => x.Status != "Completed");
                }
                resultList = new List<int>(){ customerList.Count, constructorList.Count, projectList.Count };
                return View(resultList);
            }
            
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Customer()
        {
            APIresult resultList;
            List<List<Project>> rawProject = new List<List<Project>>();
            List<List<APIusers>> rawUsers = new List<List<APIusers>>();
            List<APIusers> userList = new List<APIusers>();
            List<APIusers> customerList = new List<APIusers>();
            List<Project> projectList = new List<Project>();
            List<Project> finishedProject = new List<Project>();
            List<Project> registeredProject = new List<Project>();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://px39op01o5.execute-api.us-east-1.amazonaws.com/userList"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    rawUsers = JsonConvert.DeserializeObject<List<List<APIusers>>>(apiResponse);

                    userList = rawUsers.First();
                    customerList = userList.FindAll(x => x.Role == "Customer");
                }
                using (var response = await httpClient.GetAsync("https://4f4j3o96c4.execute-api.us-east-1.amazonaws.com/projectList"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    rawProject = JsonConvert.DeserializeObject<List<List<Project>>>(apiResponse);

                    projectList = rawProject.First();
                    registeredProject = projectList.FindAll(x => x.Status != "Completed");
                    finishedProject = projectList.FindAll(x => x.Status == "Completed");
                }
                resultList = new APIresult { users = customerList, filteredProjects1 = registeredProject, filteredProjects2 = finishedProject , unregisteredUser = null};
                return View(resultList);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Constructor()
        {
            APIresult resultList;
            List<List<Project>> rawProject = new List<List<Project>>();
            List<List<APIusers>> rawUsers = new List<List<APIusers>>();
            List<APIusers> userList = new List<APIusers>();
            List<APIusers> constructorRList = new List<APIusers>();
            List<APIusers> constructorUnList = new List<APIusers>();
            List<Project> projectList = new List<Project>();
            List<Project> finishedProject = new List<Project>();
            List<Project> registeredProject = new List<Project>();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://px39op01o5.execute-api.us-east-1.amazonaws.com/userList"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    rawUsers = JsonConvert.DeserializeObject<List<List<APIusers>>>(apiResponse);

                    userList = rawUsers.First();
                    constructorRList = userList.FindAll(x => x.Role == "Constructor" && x.EmailConfirmed == true);
                    constructorUnList = userList.FindAll(x => x.Role == "Constructor" && x.EmailConfirmed == false);
                }
                using (var response = await httpClient.GetAsync("https://4f4j3o96c4.execute-api.us-east-1.amazonaws.com/projectList"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    rawProject = JsonConvert.DeserializeObject<List<List<Project>>>(apiResponse);

                    projectList = rawProject.First();
                    registeredProject = projectList.FindAll(x => x.Status == "Taken");
                    finishedProject = projectList.FindAll(x => x.Status == "Completed");
                }
                resultList = new APIresult { users = constructorRList, filteredProjects1 = registeredProject, filteredProjects2 = finishedProject , unregisteredUser = constructorUnList};
                return View(resultList);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> UpdateConstructorStatus(String id)
        {
            var user = await _userManager.FindByIdAsync(id);
            user.EmailConfirmed = true;
            var result = await _userManager.UpdateAsync(user);
            return RedirectToAction(nameof(Constructor));
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
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DeleteCustomer(String id)
        {
            var user = await _userManager.FindByIdAsync(id);

            var result = await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Customer));
        }
        public async Task<IActionResult> DeleteConstructor(String id)
        {
            var user = await _userManager.FindByIdAsync(id);

            var result = await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Constructor));
        }
    }
}
