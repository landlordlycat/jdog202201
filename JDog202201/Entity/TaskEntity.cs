using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDog202201.Entity
{
    public class TaskEntity
    {
        public string name { set; get; }
        public string groupId { set; get; }
        public string score { set; get; }
        public string title { set; get; }
        public string taskType { set; get; }
        public string status { set; get; }
        public string taskId { set; get; }
        public List<TaskVoItemModel> Items { set; get; }
        public int maxTimes { set; get; }
        public int times { set; get; }
        public JToken data { set; get; }
        public int type { set; get; }


    }
    public class TaskVoItemModel
    {
        public int status { set; get; }
        public string id { set; get; }
        public string token { set; get; }
        public string title { set; get; }
        public string shopId { set; get; }
        public string url { set; get; }
        public string vendor { set; get; }
    }

    public class TaskReward{
        public int awardId { set; get; }
        public string awardName { set; get; }
        public string awardToken{ set; get; }
        public int requireIndex { set; get; }
        public int status{ set; get; }
    }

}
