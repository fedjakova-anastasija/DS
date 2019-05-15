using System;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TextProcessingLimiter
{
    class Program
    {
        static void Main(string[] args)
        {
            Redis redis = new Redis();
            ISubscriber sub = redis.Sub();
            IDatabase getDB = redis.GetDB(0);
            int allowedRequestsCount = 0;
            int textLimit = Convert.ToInt32(GetApplicationParams()["TextLimit"]);

            sub.Subscribe("events", (channel, message) =>
            {
                string msg = message;
                string id = ParseData(msg, 0); 
                if (id.Contains("TextRankCalc_") && (message.ToString().Split(":").Length == 1))
                {
                    allowedRequestsCount++;
                    bool result = allowedRequestsCount <= textLimit;
                    redis.Publish(id + ":" + (result).ToString());
                    if (!result)
                    {
                        Task.Run(async () =>
                        {
                            await Task.Delay(60 * 1000);
                            allowedRequestsCount = 0;
                        });
                    }
                    string valueFromMainDB = redis.GetStrFromDB(0, id);
                }
                if (id.Contains("TextStatistics_"))
                {
                    string ratio = ParseData(message, 1);
                    double ratioNumber = Convert.ToDouble(ratio);
                    if (ratioNumber <= 0.5)
                    {
                        allowedRequestsCount--;
                    }
                    ShowProcess(id, ratio);
                }                
            });
            
            Console.ReadLine();
        }
        
        private static string ParseData( string msg, int pos )
        {
            return msg.Split( ':' )[pos];
        }
        public static void ShowProcess(string data, string ratio)
        {   
            Console.WriteLine("ID: " + data);
            Console.WriteLine("RATIO: " + ratio);
            Console.WriteLine("----------------------------------------");
        }
        static Dictionary<string, string> GetApplicationParams()
        {
            var data = new Dictionary<string, string>();
            foreach (var row in File.ReadAllLines(Directory.GetParent(Directory.GetCurrentDirectory()) + "/config/config.txt"))
            data.Add(row.Split(':')[0], string.Join(":",row.Split(':').Skip(1).ToArray()));

            return data;
        }
    }
}
