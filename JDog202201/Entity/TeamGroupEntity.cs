using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDog202201.Entity
{
    public class TeamGroupEntity
    {
        public string join_code { set; get; }
        public string login_code { set; get; }
        public int num { set; get; }
        public string name { set; get; }
        public string secretp { set; get; }

        public int my_score { set; get; }
        public int all_score { set; get; }

        public TeamGroupQuestion question { set; get; }

    }

    public class TeamGroupQuestion {
        /**
         * "groupNumA": "63542",
				"groupNumB": "54674",
				"groupPercentA": "53.8",
				"groupPercentB": "46.2",
				"inflateValue": "1.50",
				"optionA": "https://m.360buyimg.com/babel/jfs/t1/169985/16/21594/15156/61600b82E4f2b35dd/70834886a504c98e.png",
				"optionAText": "全糖",
				"optionB": "https://m.360buyimg.com/babel/jfs/t1/170113/23/28495/14860/61600b73Eb5411939/9aeff598421fcb17.png",
				"optionBText": "0卡糖",
				"packageA": "400000",
				"packageB": "400000",
				"question": "喝奶茶是为了开心，所以你给同事选___",
				"securityCode": "xPJ6LB_BAZDvUqriDaVN0u0xcQ",
				"status": 5,
				"value": "0.40",
				"votFor": "A"
         * 
         */
        public string question { set; get; }
        public string A { set; get; }
        public string B { set; get; }

        public int status { set; get; }
        public string result { set; get; }
    }
}
