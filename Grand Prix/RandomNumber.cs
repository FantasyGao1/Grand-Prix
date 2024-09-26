using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Grand_Prix
{
    public class RandomNumber
    {
        static readonly Random rd = new Random();
        public static int BreakingAndCornering()
        {
            
            int time = rd.Next(1, 9);//左闭右开
            return time;
        }
        public static int Overtaking(int laps)
        {
            if (laps % 3 == 0)
            {
                Random rd = new Random();
                int time = rd.Next(10, 21);
                return time;
            }
            return 0;
        }
        public static int mechanicalFailure()
        {
            int adds=rd.Next(1, 101);
            if (adds == 1)
            {
                return int.MaxValue;
            }
            else if (adds > 1 && adds < 4)
            {
                return 120;
            }
            else if (adds > 4 && adds < 9)
            {
                return 20;
            }
            else
            {
                return 0;
            }
        }
        public static double isRain()
        {
            double adds = rd.Next(1, 101);
            return adds;
        }
        public static int changeTire()
        {
            int adds = rd.Next(1, 3);
            return adds;
        }
    }
}
