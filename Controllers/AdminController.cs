using CASS___Construction_Assistance.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CASS___Construction_Assistance.Controllers
{
/*    public class gg
    {
        public int userId { get; set; }
        public int id { get; set; }
        public string title { get; set; }
        public bool completed { get; set; }
    }*/
    public class AdminController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
/*            List<gg> reservationList = new List<gg>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://0sarh7yv36.execute-api.us-east-1.amazonaws.com/prod"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    *//*reservationList = JsonConvert.DeserializeObject<List<gg>>(apiResponse);*//*
                   
                    return BadRequest(reservationList);
                }
            }*/
            return View();
        
        }

        public IActionResult Customer()
        {
            return View();
        }
        public IActionResult Constructor()
        {
            return View();
        }
    }
}
