﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using StackExchange.Redis;
using System.Threading;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        Redis redis = new Redis();
        static readonly ConcurrentDictionary<string, string> _data = new ConcurrentDictionary<string, string>();
        
        // GET api/statistics/<TextRankCalculated>
        [HttpGet("{TextRankCalculated}")]
        public IActionResult Get()
        {
            string valueFromMainDB = null;
            string valueFromRegionDB = null;
            string text = "TextRankCalculated";
            
            for (int i = 0; i < 5; i++)
            {
                valueFromMainDB = redis.GetStrFromDB(0, text);

                if (valueFromRegionDB == null)
				{
					Thread.Sleep(200);
				}
            }

            return Ok(valueFromMainDB);
        }

        // POST api/statistics
        [HttpPost]
        public string Post([FromBody] string value)
        {
            return null;
        }
    }
}