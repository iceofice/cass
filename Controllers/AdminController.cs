using CASS___Construction_Assistance.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace CASS___Construction_Assistance.Controllers
{
   
    public class AdminController : Controller
    {
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> IndexAsync()
        {
            List<int> resultList;
            List<List<Project>> rawProject = new List<List<Project>>();
            List<List<APIusers>> rawUsers = new List<List<APIusers>>();
            List<APIusers> userList = new List<APIusers>();
            List<APIusers> CustomerList = new List<APIusers>();
            List<APIusers> ConstructorList = new List<APIusers>();
            List<Project> ProjectList = new List<Project>();
            List<Project> finishedProject = new List<Project>();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://px39op01o5.execute-api.us-east-1.amazonaws.com/userList"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    rawUsers = JsonConvert.DeserializeObject<List<List<APIusers>>>(apiResponse);

                    userList = rawUsers.First();
                    CustomerList = userList.FindAll(x => x.Role == "Customer");
                    ConstructorList = userList.FindAll(x => x.Role == "Constructor");
                }
                using (var response = await httpClient.GetAsync("https://4f4j3o96c4.execute-api.us-east-1.amazonaws.com/projectList"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    rawProject = JsonConvert.DeserializeObject<List<List<Project>>>(apiResponse);

                    ProjectList = rawProject.First();
                }
                resultList = new List<int>(){ CustomerList.Count, ConstructorList.Count, ProjectList.Count };
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
            List<APIusers> CustomerList = new List<APIusers>();
            List<Project> projectList = new List<Project>();
            List<Project> finishedProject = new List<Project>();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://px39op01o5.execute-api.us-east-1.amazonaws.com/userList"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    rawUsers = JsonConvert.DeserializeObject<List<List<APIusers>>>(apiResponse);

                    userList = rawUsers.First();
                    CustomerList = userList.FindAll(x => x.Role == "Customer");
                }
                using (var response = await httpClient.GetAsync("https://4f4j3o96c4.execute-api.us-east-1.amazonaws.com/projectList"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    rawProject = JsonConvert.DeserializeObject<List<List<Project>>>(apiResponse);

                    projectList = rawProject.First();
                    finishedProject = projectList.FindAll(x => x.Status == "Completed");
                }
                resultList = new APIresult { users = CustomerList, projects = projectList, finishedProjects = projectList };
                return View(resultList);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Constructor()
        {
            List<List<APIusers>> resultList;
            List<List<APIusers>> rawProject = new List<List<APIusers>>();
            List<List<APIusers>> rawUsers = new List<List<APIusers>>();
            List<APIusers> userList = new List<APIusers>();
            List<APIusers> ConstructorList = new List<APIusers>();
            List<APIusers> projectList = new List<APIusers>();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://px39op01o5.execute-api.us-east-1.amazonaws.com/userList"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    rawUsers = JsonConvert.DeserializeObject<List<List<APIusers>>>(apiResponse);

                    userList = rawUsers.First();
                    ConstructorList = userList.FindAll(x => x.Role == "Constructor");
                }
                using (var response = await httpClient.GetAsync("https://4f4j3o96c4.execute-api.us-east-1.amazonaws.com/projectList"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    rawProject = JsonConvert.DeserializeObject<List<List<APIusers>>>(apiResponse);

                    projectList = rawProject.First();
                }
                resultList = new List<List<APIusers>> {
                    ConstructorList,
                    projectList,
                };
                return View(resultList);
            }
        }
    }
}
