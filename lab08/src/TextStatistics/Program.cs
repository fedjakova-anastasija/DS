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
        private static int rejectedRequestsCount = 0;
        static void Main(string[] args)
        {
            Redis redis = new Redis();
            ISubscriber sub = redis.Sub();

            sub.Subscribe("events", (channel, message) => {
                string text = "TextRankCalculated";   
                string msg = message;
                string id = ParseData(msg, 0); 
                
                if (id.Contains("TextRankCalc_") && (message.ToString().Split(":").Length == 2))
                { 
                    bool isAccessAllowed = Convert.ToBoolean(ParseData(msg, 1)); 
                    if (!isAccessAllowed)
                    {
                        rejectedRequestsCount++;
                    }
                }
                
                if (id.Contains("TextStatistics_"))
                {            
                    string ratio = ParseData(msg, 1);
                    double ratioNumber = Convert.ToDouble(ratio);

                    totalRatioNumber += ratioNumber;
                    textNum++;                
                    avgRank = totalRatioNumber / textNum;
                    if (ratioNumber > 0.5) {
                        highRankPart++;
                    }
                    ShowProcess(textNum.ToString(), highRankPart.ToString(), avgRank.ToString());
                }
                redis.Add(0, text, $"{textNum}:{highRankPart}:{avgRank}:{rejectedRequestsCount}");
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
