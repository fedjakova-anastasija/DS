using System;
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
    public class ValuesController : Controller
    {
        static readonly ConcurrentDictionary<string, string> _data = new ConcurrentDictionary<string, string>();
        static private ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
        static private IDatabase db = redis.GetDatabase();
        static private ISubscriber sub = redis.GetSubscriber();
        
        // GET api/values/<id>
        [HttpGet("{id}")]
        public string Get(string id)
        {
            string value = null;
            for (int i = 0; i < 5; i++) {
                value = db.StringGet("TextRankCalc_" + id);
                if (value == null)
				{
					Thread.Sleep(200);
				}
            }
            return value;
        }

        // POST api/values
        [HttpPost]
        public string Post([FromBody]string value)
        {
            var id = Guid.NewGuid().ToString();
            _data[id] = value;
            db.StringSet(id, value);
            sub = redis.GetSubscriber();
            sub.Publish("events", id);
            
            return id;
        }
    }
}
