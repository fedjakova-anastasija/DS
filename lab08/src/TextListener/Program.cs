using System;
using StackExchange.Redis;

namespace TextListener
{
    class Program
    {
        static void Main(string[] args)
        {
            Redis redis = new Redis();
            ISubscriber sub = redis.Sub();            
            IDatabase getDB = redis.GetDB(0);

            sub.Subscribe("events", (channel, message) => {
                string id = message;
                if (id.Contains("TextRankCalc_"))
                {
                    string valueFromMainDB = redis.GetStrFromDB(0, id);               
                    string valueFromRegionDB = redis.GetStrFromDB( GetDatabaseId(valueFromMainDB), id);
                    ShowProcess(id, valueFromMainDB, valueFromRegionDB);
                }
            });
            
            Console.ReadLine();
        }
        
        private static int GetDatabaseId(string key)
        {
            return Convert.ToInt32(key);
        }

        public static void ShowProcess(string id, string region, string data)
        {   
            Console.WriteLine("ID: " + id);
            Console.WriteLine("REGION: " + region);
            Console.WriteLine("DATA: " + data);
            Console.WriteLine("----------------------------------------");
        }
    }
}