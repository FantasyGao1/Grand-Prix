using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grand_Prix
{
    class Driver : IComparable <Driver>
    {
        public string name { get; set; }
        public int rank { get; set; }
        public string specialskill { get; set; }
        public bool eligible { get; set; }
        public int accumulateScore { get; set; }
        public int accumulateTime { get; set; }
        public int ranking { get; set; }
        public string Default {  get; set; }
        public bool isskill {  get; set; }
        public Driver (string name,int accumulateTime)
        {
            this.name = name;
            this.accumulateTime = accumulateTime;
            this.specialskill = specialskill;
            this.rank = 0;
        }
        public Driver()
        {

        }
        public int CompareTo(Driver other)
        {
            int timecomparion = accumulateTime.CompareTo(other.accumulateTime);
            if (timecomparion == 0)
            {
                return new Random().Next(-1, 2);
            }
            return timecomparion;
        }
    }
}
