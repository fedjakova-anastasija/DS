using System;
using StackExchange.Redis;
using System.Collections.Generic;

namespace VowelConsCounter
{
    class Program
    {
        private static ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
        private static IDatabase db = redis.GetDatabase();
        private static ISubscriber sub = redis.GetSubscriber();

        const string COUNTER_QUEUE_NAME  = "vowel-cons-counter-jobs";
        const string COUNTER_HINTS_CHANNEL  = "counter-hints";
        const string RATE_QUEUE_NAME  = "vowel-cons-rater-jobs";
        const string RATE_HINTS_CHANNEL  = "rate-hints";

        private static HashSet<char> VOWELS = new HashSet<char>{'a', 'e', 'i', 'o', 'u', 'y'};
		private static HashSet<char> CONSONANTS = new HashSet<char>{'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'z'};

        static void Main(string[] args)
        {
            redis.GetSubscriber().Subscribe(COUNTER_HINTS_CHANNEL, delegate
           {
               string msg = redis.GetDatabase().ListRightPop(COUNTER_QUEUE_NAME);
               while (msg != null)
               {
                    string id = ParseData(msg, 0);
                    string data = ParseData(msg, 1);

                    int vowels = 0;
                    int consonants = 0;

                    foreach (char ch in data)
                    {
                        char chToLower = Char.ToLower(ch);
                        if (VOWELS.Contains(chToLower))
                        {
                            ++vowels;
                        }
                        else if (CONSONANTS.Contains(chToLower))
                        {
                            ++consonants;
                        }
                    }
                    //ratio = vowels + "/" + consonants;
                    //db.StringSet("TextRankCalc_" + id, ratio);

                    SendMessage($"{id}:{vowels}:{consonants}", redis);
                    msg = redis.GetDatabase().ListRightPop( COUNTER_QUEUE_NAME );
               }
           });
            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
        }
        private static void SendMessage(string message, ConnectionMultiplexer redis )
        {
            // put message to queue
            redis.GetDatabase().ListLeftPush( RATE_QUEUE_NAME, message, flags: CommandFlags.FireAndForget );
            // and notify consumers
            redis.GetSubscriber().Publish( RATE_HINTS_CHANNEL, "" );
        }

        private static string ParseData( string msg, int index )
        {
            return msg.Split( ':' )[index];
        }
    }
}