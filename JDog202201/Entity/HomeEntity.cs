using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDog202201.Entity
{
    public class HomeEntity
    {
        public class homeMainInfo
        {
            public int curCity{ set; get; }
            public bool dropHongbaoDisplay { set; get; }
            public int jdCipher{ set; get; }
            public int pkJdCipher { set; get; }
            public string secretp{ set; get; }
            public int shareMiniprogramSwitch{ set; get; }
            public int taskSwitch{ set; get; }
            public int todaySignStatus{ set; get; }
            public int totalCity{ set; get; }
            public int userType { set; get; }
        }

        public class userMainInfo
        {
            public int now_score { set; get; }
            public int next_score { set; get; }
            public int bonus { set; get; }
            public int now_city { set; get; }
            public int all_city { set; get; }
        }

        public homeMainInfo homeMain { set; get; }
        public userMainInfo userMain { set; get; }
        public string secretp { set; get; }

    }
}
