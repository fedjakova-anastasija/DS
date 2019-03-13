using System;
using StackExchange.Redis;

namespace TextRankCalc
{
    class Program
    {
        private static ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
        private static IDatabase db = redis.GetDatabase();
        private static ISubscriber sub = redis.GetSubscriber();

        const string COUNTER_QUEUE_NAME  = "vowel-cons-counter-jobs";
        const string COUNTER_HINTS_CHANNEL  = "counter-hints";

        static void Main(string[] args)
        {
            sub.Subscribe("events", (channel, message) =>
            {
                string id = message;
                string data = db.StringGet(id);
                SendMessage($"{id}:{data}", redis);
                Console.WriteLine("Message sent => " + id + ": " + data);
            });
            Console.ReadKey();
        }

        private static void SendMessage(string message, IConnectionMultiplexer redis )
        {
            // put message to queue
            redis.GetDatabase().ListLeftPush( COUNTER_QUEUE_NAME, message, flags: CommandFlags.FireAndForget );
            // and notify consumers
            redis.GetSubscriber().Publish( COUNTER_HINTS_CHANNEL, "" );
        }
    }
}