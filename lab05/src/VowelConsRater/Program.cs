using System;
using StackExchange.Redis;

namespace VowelConsRater
{
    class Program
    {
        private static ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
        private static IDatabase db = redis.GetDatabase();
        private static ISubscriber sub = redis.GetSubscriber();

        const string RATE_QUEUE_NAME  = "vowel-cons-rater-jobs";
        const string RATE_HINTS_CHANNEL  = "rate-hints";
        static void Main(string[] args)
        {
            redis.GetSubscriber().Subscribe(RATE_HINTS_CHANNEL, delegate
            {
                string msg = redis.GetDatabase().ListRightPop(RATE_QUEUE_NAME);
                while (msg != null)
                {
                    string ratio= "";
                    string id = ParseData( msg, 0 );
                    string vowels = ParseData(msg, 1);
                    string consonants = ParseData(msg, 2);
                    DoJob( id );
                    ratio = vowels + "/" + consonants;
                    db.StringSet("TextRankCalc_" + id, ratio);
                    msg = redis.GetDatabase().ListRightPop(RATE_QUEUE_NAME);
                }
            }); 
            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
        }

        private static void DoJob( string jobData )
        {
            Console.WriteLine( $"Job data: {jobData}" );
            System.Threading.Thread.Sleep(1500); // emulate loading
        }

        private static string ParseData( string msg, int index )
        {
            return msg.Split( ':' )[index];
        }
    }
}