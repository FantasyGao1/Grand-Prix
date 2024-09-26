using ConsoleTables;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Grand_Prix
{
    internal class Championship
    {
        static void Main(string[] args)
        {
            //读取场地信息
            var pathvenue = "venues.txt";
            ArrayList venues = ReadVenues(pathvenue);
            //读取驾驶员信息
            var pathDriver = "driver.txt";
            var pathLog = "log.txt";
            File.WriteAllText(pathLog, string.Empty);//清空文件
            List<Driver> drivers = ReadDrivers(pathDriver);
            //准备结束开始比赛
            Console.WriteLine("请选择你要参加的比赛数目（至少参加三场，至多参加五场）：");
            //使用 while (true) 无限循环，直到用户输入有效的场数（3 到 5 之间）。如果输入无效，提示用户重新输入。
            while (true)
            {
                int n;
                try
                {
                    n = int.Parse(Console.ReadLine());
                    if (n < 3 || n > 5)
                    {
                        Console.WriteLine("输入错误，请重新输入！");
                        continue;
                    }
                    //显示可选场地信息，并要求用户选择一个场地。使用 ConsoleTable 类显示场地列表。用户输入的场地名称将与列表中的名称匹配，匹配成功后退出循环。
                    for (int i = 0; i < n; i++)
                    {
                        Console.WriteLine("请选择比赛场地：");
                        var table = new ConsoleTable("venueName", "number of turns", "averageLapTime", "chanceOfRain");
                        //填充表格
                        foreach (Venue item in venues)
                        {
                            table.AddRow(item.venueName, item.noOfLaps, item.averageLapTime, item.chanceOfRain);
                        }
                        //输出表格
                        table.Write(Format.Alternative);
                        Venue thisVenue = new Venue();
                        int lapNum;
                        double rainProbability;
                        int x = 0;
                        File.AppendAllText(pathLog, DateTime.Now.ToString() + "champion\r\n");//添加进文件
                        while (true)
                        {
                            Console.WriteLine("你的选择：");
                            string thisVenueName = Console.ReadLine();//获取比赛场地
                            foreach (Venue item in venues)
                            {
                                if (thisVenueName == item.venueName)
                                {
                                    thisVenue = item;
                                    x = 1;
                                    File.AppendAllText(pathLog, item.venueName + "\r\n");//添加进文件
                                }
                            }
                            if (x == 1) break;
                            Console.WriteLine("输入错误，请重新输入");
                        }
                        Console.WriteLine();
                        Console.WriteLine("--比赛开始了--");
                        lapNum = thisVenue.noOfLaps;//获取场地圈数
                        rainProbability = thisVenue.chanceOfRain * 100;//下雨概率
                        foreach (Driver item in drivers)//起跑时间需加上排名罚时
                        {
                            if (item.ranking == 1)
                            {
                                item.accumulateTime += 0;
                            }
                            else if (item.ranking == 2)
                            {
                                item.accumulateTime += 3;
                            }
                            else if (item.ranking == 3)
                            {
                                item.accumulateTime += 5;
                            }
                            else if (item.ranking == 4)
                            {
                                item.accumulateTime += 7;
                            }
                            else
                            {
                                item.accumulateTime += 10;
                            }
                            File.AppendAllText(pathLog, item.name + "罚时是" + item.accumulateTime + "秒\r\n");
                        }
                        File.AppendAllText(pathLog, "\r\n");
                        double adds = RandomNumber.isRain();
                        for (int j = 1; j < lapNum+1; j++)//开始跑圈
                        {
                            File.AppendAllText(pathLog, j + "st lap");
                            int time = 0;
                            foreach (Driver item in drivers)
                            {
                                if (item.eligible)
                                {
                                    time = RandomNumber.mechanicalFailure();//获取机械故障概率
                                    if (time == int.MaxValue)
                                    {
                                        File.AppendAllText(pathLog, item.name + "无法修复的机械故障，失去比赛资格!\r\n");
                                        item.eligible = false;
                                        item.accumulateTime = int.MaxValue;
                                        continue;
                                    }
                                    else if (time == 20)
                                    {
                                        File.AppendAllText(pathLog, item.name + "轻微机械故障,增加20秒!\r\n");
                                        item.Default = "a minor mechanical fault";
                                    }
                                    else if (time == 120)
                                    {
                                        File.AppendAllText(pathLog, item.name + "重大机械故障,增加120秒!\r\n");
                                        item.Default = "a major mechanical fault";
                                    }
                                    else
                                    {
                                        File.AppendAllText(pathLog, item.name + "没有机械故障，增加0秒\r\n");
                                        item.Default = "no mechanical fault";
                                    }
                                    item.accumulateTime += time;//加上机械故障时间
                                    if (item.specialskill.Equals("breaking") || item.specialskill.Equals("cornering"))
                                    {
                                        int skilltime = RandomNumber.BreakingAndCornering();
                                        item.accumulateTime -= skilltime;
                                        File.AppendAllText(pathLog, item.name + "使用" + item.specialskill + "减去" + skilltime + "秒\r\n");
                                        item.isskill = true;
                                    }
                                    else
                                    {
                                        int time1 = RandomNumber.Overtaking(j);
                                        item.accumulateTime -= time1;
                                        if (time1 != 0)
                                        {
                                            item.isskill = true;
                                            File.AppendAllText(pathLog, item.name + "使用" + item.specialskill + "减去" + time1 + "秒\r\n");
                                        }
                                        else
                                        {
                                            File.AppendAllText(pathLog, item.name + "不能使用" + item.specialskill + "减去" + time1 + "秒\r\n");
                                        }
                                    }
                                    if (j == 2)
                                    {
                                        if (adds <= rainProbability)
                                        {
                                            File.AppendAllText(pathLog, "下雨了！");
                                            if (RandomNumber.changeTire() == 2)
                                            {
                                                item.accumulateTime += 10;
                                                File.AppendAllText(pathLog, item.name + "换轮胎了，加10秒\r\n");
                                            }
                                            else
                                            {
                                                File.AppendAllText(pathLog, item.name + $"没换轮胎，每圈加5秒,共加{5 * (lapNum - 1)}秒\r\n");
                                                item.accumulateTime += 5 * (lapNum - 1);
                                            }
                                        }
                                        else
                                        {
                                            File.AppendAllText(pathLog, "没下雨" + item.name + "没有罚时" + "\r\n");
                                        }
                                    }
                                    item.accumulateTime += thisVenue.averageLapTime;
                                    File.AppendAllText(pathLog, "\r\n");
                                }
                            }
                            drivers.Sort();//跑完本圈之后按时间进行排序
                            int k = 1;
                            foreach (var driver in drivers)
                            {
                                driver.ranking = k++;
                            }
                            
                            Console.WriteLine($"第{j}圈结果如下：");
                            PrintDriverLap(drivers);
                            File.AppendAllText(pathLog, "\r\n\r\n");
                        }
                        foreach (Venue venue in venues)  //删除本场场地，下场不可再用
                        {

                            if (venue.venueName.Equals(thisVenue.venueName))
                            {
                                venues.Remove(venue);
                                break;
                            }
                        }
                        //赋积分
                        AssignPointsAndRanking(drivers);
                        Console.WriteLine("--这场比赛结束了--");
                        Console.WriteLine();
                        Console.WriteLine("比赛结果如下:");
                        //输出本场比赛结果
                        PrintDriverInfo(drivers);
                        Console.WriteLine();
                    }
                    //对积分进行排序
                    ArrayList arrayList = new ArrayList();
                    arrayList.AddRange(drivers);
                    CalculateChampionshipResults(arrayList, pathDriver);
                }

                catch
                {
                    Console.WriteLine("输入错误，请重新输入!!!");
                }
                Console.ReadLine();
            }
        }
        //给赛车赋分
        public static void AssignPointsAndRanking(List<Driver> drivers)//跑完之后赋分
        {
            int count = 1;
            foreach (Driver driver in drivers)
            {
                if (count == 1)
                {
                    driver.accumulateScore += 8;
                }
                else if (count == 2)
                {
                    driver.accumulateScore += 5;
                }
                else if (count == 3)
                {
                    driver.accumulateScore += 3;
                }
                else if (count == 4)
                {
                    driver.accumulateScore += 1;
                }
                else
                {
                    driver.accumulateScore += 0;
                }
                driver.ranking = count++;
            }

        }
        ////积分排名
        public static void CalculateChampionshipResults(ArrayList drivers, string pathDriver)
        {
            drivers.Sort(new ScoreCompare());
            Console.WriteLine("比赛的最终结果：");
            int rank1 = 1;
            File.WriteAllText(pathDriver, string.Empty);//清空文件
            var table = new ConsoleTable("ranking", "name", "Score");
            foreach (Driver driver in drivers)
            {
                driver.ranking = rank1++;
                table.AddRow(driver.ranking, driver.name, driver.accumulateScore);//向表格中添加一行数据
                string str = driver.name + ", " + driver.ranking + ", " + driver.specialskill + ", " + driver.eligible + ", " + 0 + "\r\n";//文件操作，创建一个字符串 str，包含驾驶员的姓名、排名、特殊技能、是否有资格参加比赛等信息
                File.AppendAllText(pathDriver, str);  //添加进文件
            }
            table.Write(Format.Alternative);
        }

        //读取车手信息
        public static List<Driver> ReadDrivers(string pathDriver)
        {
            string[] lines = File.ReadAllLines(pathDriver);
            string[] Str;

            List<Driver> drivers = new List<Driver>();

            foreach (string line in lines)
            {
                Driver driver = new Driver();
                Str = line.Split(',');
                driver.name = Str[0];
                driver.ranking = int.Parse(Str[1]);
                driver.specialskill = Str[2].Trim();

                if (Str[3].Trim().ToLower().Equals("true"))
                {
                    driver.eligible = true;
                }
                else if (Str[3].Trim().ToLower().Equals("false"))
                {
                    driver.eligible = false;
                }
                else
                {
                    Console.WriteLine("Error!");
                    return null;
                }

                driver.accumulateTime = int.Parse(Str[4]);
                driver.accumulateScore = 0;
                driver.isskill = false;
                drivers.Add(driver);
            }
            return drivers;
        }
        //读取场地信息
        public static ArrayList ReadVenues(string pathvenue)
        {
            ArrayList venues = new ArrayList();
            string[] lines = File.ReadAllLines(pathvenue);
            string[] Str;
            foreach (string line in lines)
            {
                Venue venue = new Venue();
                Str = line.Split(',');
                venue.venueName = Str[0];
                venue.noOfLaps = int.Parse(Str[1]);
                venue.averageLapTime = int.Parse(Str[2]);
                venue.chanceOfRain = double.Parse(Str[3]);
                venues.Add(venue);
            }
            return venues;
        }
        //输出比赛结果
        public static void PrintDriverInfo(List<Driver> drivers)
        {
            var table = new ConsoleTable("ranking", "name", "accumulateTime", "accumulateScore");
            foreach (Driver driver in drivers)
            {
                if (driver.eligible)
                {
                    table.AddRow(driver.ranking, driver.name, driver.accumulateTime, driver.accumulateScore);
                }
                else
                {
                    table.AddRow(driver.ranking, driver.name, "   lose qualification   ", driver.accumulateScore);
                }
                driver.eligible = true; //在下场比赛前使得所有人具备资格
                driver.accumulateTime = 0;//时间清零
            }
            table.Write(Format.Alternative);
            Console.WriteLine();
        }
        public static void PrintDriverLap(List<Driver> drivers)
        {
            var table = new ConsoleTable("ranking", "name", "accumulateTime", "accumulateScore", "skill", "isskill", "mechanical failure");
            foreach (Driver driver in drivers)
            {
                if (driver.eligible)
                {
                    table.AddRow(driver.ranking, driver.name, driver.accumulateTime, driver.accumulateScore, driver.specialskill, driver.isskill, driver.Default);
                }
                else
                {
                    table.AddRow(driver.ranking, driver.name, "     ", driver.accumulateScore, "Unrecoverable mechanical fault!lose qualification!", "     ", "  ");
                }
            }
            table.Write(Format.Alternative);
            Console.WriteLine();
        }

        class ScoreCompare : IComparer
        {
            public int Compare(object x, object y)
            {
                Driver player1 = (Driver)x;
                Driver player2 = (Driver)y;
                return player2.accumulateScore.CompareTo(player1.accumulateScore);
            }
        }
    }
}


