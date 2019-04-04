using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using StackExchange.Redis;

namespace TextRankCalc
{
    class Program
{
        const string COUNTER_QUEUE_NAME  = "vowel-cons-counter-jobs";
        const string COUNTER_HINTS_CHANNEL  = "counter-hints";

        static void Main(string[] args)
        {
            Redis redis = new Redis();
            ISubscriber sub = redis.Sub();
            IDatabase getDB = redis.GetDB(4);

            sub.Subscribe("events", (channel, message) =>
            {
                string text = message;
                string valueFromMainDB = redis.GetStrFromDB(4, text);
                //valueFromRegionDB = redis.GetStrFromDB( GetDatabaseId(valueFromMainDB), text);
                SendMessage($"{text}:{valueFromMainDB}", getDB, sub);
                ShowProcess(text, valueFromMainDB);
            });
            
            Console.WriteLine("TextRankCalc");
            Console.ReadLine();
        }

        private static int GetDatabaseId(string key)
        {
            return Convert.ToInt32(key);
        }

        private static void SendMessage(string message, IDatabase db, ISubscriber sub )
        {
            // put message to queue
            db.ListLeftPush( COUNTER_QUEUE_NAME, message, flags: CommandFlags.FireAndForget );
            // and notify consumers
            sub.Publish( COUNTER_HINTS_CHANNEL, "" );
        }

        private static string ParseData( string msg, int pos )
        {
            return msg.Split( ':' )[pos];
        }

        public static void ShowProcess(string data, string region)
        {   
            Console.WriteLine("ID: " + data);
            Console.WriteLine("REGION: " + region);
            Console.WriteLine("----------------------------------------");
        }
    }
}