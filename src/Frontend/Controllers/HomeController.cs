using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Frontend.Models;
using System.Net.Http;

namespace Frontend.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(string data)
        {
            string id = null; 

            if (data != null)
            {
                string url = "http://127.0.0.1:5000/api/values";
                HttpClient client = new HttpClient();
            
                FormUrlEncodedContent str = new FormUrlEncodedContent(new[]{new KeyValuePair<string, string>("value", data)});
                var request = client.PostAsync(url, str);
                id = request.Result.Content.ReadAsStringAsync().Result;
            }

            return Ok(id);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
