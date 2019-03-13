﻿using System;
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

        [HttpGet]
        public async Task<IActionResult> TextDetails(string id)
        {
            string textDetails = null; 
            string url = "http://localhost:5000/api/values/" + id;
            HttpClient client = new HttpClient();

            HttpResponseMessage request = await client.GetAsync(url);
            textDetails = await request.Content.ReadAsStringAsync();
            ViewData["TextDetails"] = textDetails;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(string data)
        {
            string id = null; 

            if (data != null)
            {
                string url = "http://127.0.0.1:5000/api/values";
                HttpClient client = new HttpClient();

                HttpResponseMessage request = await client.PostAsJsonAsync(url, data);
                id = await request.Content.ReadAsStringAsync();
            }         

            return Redirect("TextDetails/" + id);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
