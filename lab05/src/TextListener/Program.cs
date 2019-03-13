using System;
using StackExchange.Redis;

namespace TextListener
{
    class Program
    {
        private static ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
        private static IDatabase db = redis.GetDatabase();
        private static ISubscriber sub = redis.GetSubscriber();

        static void Main(string[] args)
        {
            sub.Subscribe("events", (channel, message) => 
            {
                string id = message;
                string data = db.StringGet(id);

                Console.WriteLine("DATA: " + data);
                Console.WriteLine("ID: " + id);
                Console.WriteLine("----------------------------------------");
            });                 
            Console.ReadLine();
        }
    }
}