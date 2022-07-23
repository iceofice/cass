using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

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
        public async Task<IActionResult> IndexAsync()
        {
            List<List<users>> reservationList = new List<List<users>>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://0sarh7yv36.execute-api.us-east-1.amazonaws.com/prod"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    reservationList = JsonConvert.DeserializeObject<List<List<users>>>(apiResponse);

                    return BadRequest(reservationList);
                }
            }
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
