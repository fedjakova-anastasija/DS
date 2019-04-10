using System;
using StackExchange.Redis;

namespace TextStatistics
{
    class Program
    {
        private static int textNum = 0;
        private static int highRankPart = 0;
        private static double avgRank = 0;
        private static double totalRatioNumber = 0;
        static void Main(string[] args)
        {
            Redis redis = new Redis();
            ISubscriber sub = redis.Sub();

            sub.Subscribe("textRankCalculated", (channel, message) => {
                string msg = message;
                string id = ParseData(msg, 0);
                string text = "TextRankCalculated";                
                string ratio = ParseData(msg, 1);
                double ratioNumber = Convert.ToDouble(ratio);
                string valueFromMainDB = redis.GetStrFromDB(0, id);
                totalRatioNumber += ratioNumber;
                textNum++;                
                avgRank = totalRatioNumber / textNum;
                if (ratioNumber > 0.5) {
                    highRankPart++;
                }
                redis.Add(0, text, $"{textNum}:{highRankPart}:{avgRank}");
                ShowProcess(textNum.ToString(), highRankPart.ToString(), avgRank.ToString());
            });
            
            Console.Title = "TextStatistics";
            Console.ReadLine();
        }
        
        private static int GetDatabaseId(string key)
        {
            return Convert.ToInt32(key);
        }
        private static string ParseData( string msg, int pos )
        {
            return msg.Split( ':' )[pos];
        }

        public static void ShowProcess(string textNum, string highRankPart, string avgRank)
        {   
            Console.WriteLine("TEXTNUM: " + textNum);
            Console.WriteLine("HIGHTRANKRART: " + highRankPart);            
            Console.WriteLine("AVGRANK: " + avgRank);
            Console.WriteLine("----------------------------------------");
        }
    }
}
