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

            sub.Subscribe("events", (channel, message) => {
                string id = message;
                string valueFromMainDB = redis.GetStrFromDB(4, id);
                ShowProcess(id, valueFromMainDB);
            });
            
            Console.ReadLine();
        }
        
        private static int GetDatabaseId(string key)
        {
            return Convert.ToInt32(key);
        }

        public static void ShowProcess(string data, string region)
        {   
            Console.WriteLine("ID: " + data);
            Console.WriteLine("REGION: " + region);
            Console.WriteLine("----------------------------------------");
        }
    }
}