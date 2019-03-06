using System;
using StackExchange.Redis;
using System.Collections.Generic;

namespace TextRankCalc
{
    class Program
    {
        private static ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
        private static IDatabase db = redis.GetDatabase();
        private static ISubscriber sub = redis.GetSubscriber();

        private static HashSet<char> VOWELS = new HashSet<char>{'a', 'e', 'i', 'o', 'u', 'y'};
		private static HashSet<char> CONSONANTS = new HashSet<char>{'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'z'};

        static void Main(string[] args)
        {
            sub.Subscribe("events", (channel, message) =>
            {
                string id = message;
                string data = db.StringGet(id);
                string ratio= "";
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
                ratio = vowels + "/" + consonants;
                db.StringSet("TextRankCalc_" + id, ratio);
            });
            Console.ReadKey();
        }
    }
}