using JDog202201.Entity;
using JDog202201.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JDog202201.Services
{
    public class TaskService
    {
        public static List<TaskReward> getTaskRewards(String cookie)
        {
            JToken res = BaseService.base_post("tigernian_getTaskDetail ", cookie, "{}", "");
            String code = res["code"].ToString();
            if (StrHelper.checkEquals(code, "0"))
            {
                if (StrHelper.checkEquals(res["data"]["bizCode"].ToString(), "0"))
                {

                    List<TaskReward> reward = new List<TaskReward>();
                    for (int i = 0; i < res["data"]["result"]["lotteryTaskVos"][0]["badgeAwardVos"].ToArray().Count(); i++)
                    {
                        JToken data = res["data"]["result"]["lotteryTaskVos"][0]["badgeAwardVos"][i];
                        reward.Add(new TaskReward()
                        {
                            awardId = StrHelper.formatInt(data["awardId"]),
                            awardName = StrHelper.formatStr(data["awardName"]),
                            awardToken = StrHelper.formatStr(data["awardToken"]),
                            requireIndex = StrHelper.formatInt(data["requireIndex"]),
                            status = StrHelper.formatInt(data["status"]),
                        });
                    }
                    return reward;
                }
                else
                {
                    return null;
                }
            }
            return null;
        }
        public static List<TaskEntity> getTaskDetail(String cookie, string find = "-1",bool is_wx = false)
        {
            JToken res = null;
           
            if (!is_wx)
            {
                res = BaseService.base_post("tigernian_getTaskDetail ", cookie, "{}", "");
            }
            else
            {
                res = BaseService.base_post("tigernian_getTaskDetail ", cookie, "{\"taskId\":\"\",\"appSign\":\"2\"}&loginWQBiz=businesst1", "");
            }
            if (StrHelper.checkEquals(res["code"].ToString(), "0"))
            {
                if (StrHelper.checkEquals(res["data"]["bizCode"].ToString(), "0"))
                {

                    List<TaskEntity> tasks = new List<TaskEntity>();
                    for (int i = 0; i < res["data"]["result"]["taskVos"].ToArray().Count(); i++)
                    {
                        int type = 0;
                        JToken data = res["data"]["result"]["taskVos"][i];
                        List<TaskVoItemModel> items = new List<TaskVoItemModel>();
                        if (find != "-1")
                        {
                            string[] find_ids = find.Split('-');
                            if (!StrHelper.checkEquals(data["taskType"].ToString(), find_ids[0]))
                            {
                                if(find_ids.Count() > 1)
                                {
                                    if(find_ids[1] != data["taskId"].ToString())
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                                
                            }

                        }

                        switch (data["taskType"].ToString())
                        {
                            //14 助力
                            case "21"://入会
                                type = 4;
                                foreach (JToken j in data["brandMemberVos"])
                                {
                                    items.Add(new TaskVoItemModel()
                                    {
                                        id = j["itemId"].ToString(),
                                        status = Convert.ToInt32(j["status"].ToString()),
                                        token = j["taskToken"].ToString(),
                                        title = j["title"].ToString(),
                                        shopId = j["ext"]["shopId"].ToString(),
                                        url = j["memberUrl"].ToString(),
                                        vendor = j["vendorIds"].ToString(),
                                    });
                                }
                                break;
                            case "3"://参与活动
                                type = 3;
                                foreach (JToken j in data["shoppingActivityVos"])
                                {
                                    items.Add(new TaskVoItemModel()
                                    {
                                        id = j["itemId"].ToString(),
                                        status = Convert.ToInt32(j["status"].ToString()),
                                        token = j["taskToken"].ToString(),
                                        title = j["title"].ToString(),
                                        shopId = "",
                                    });
                                }
                                break;
                            case "26"://参与活动
                                type = 3;
                                foreach (JToken j in data["shoppingActivityVos"])
                                {
                                    items.Add(new TaskVoItemModel()
                                    {
                                        id = j["itemId"].ToString(),
                                        status = Convert.ToInt32(j["status"].ToString()),
                                        token = j["taskToken"].ToString(),
                                        title = j["title"].ToString(),
                                        shopId = "",
                                    });
                                }
                                break;
                            case "9"://浏览8秒
                                foreach (JToken j in data["shoppingActivityVos"])
                                {
                                    items.Add(new TaskVoItemModel()
                                    {
                                        id = j["itemId"].ToString(),
                                        status = Convert.ToInt32(j["status"].ToString()),
                                        token = j["taskToken"].ToString(),
                                        title = j["title"].ToString(),
                                        shopId = "",
                                    });
                                }
                                type = 1;
                                break;
                            case "2"://加购购物车
                                type = 2;
                                JToken productInfoVos = data["productInfoVos"];
                                if (productInfoVos != null)
                                {
                                    foreach (JToken j in productInfoVos)
                                    {
                                        items.Add(new TaskVoItemModel()
                                        {
                                            id = j["itemId"].ToString(),
                                            status = Convert.ToInt32(j["status"].ToString()),
                                            token = j["taskToken"].ToString(),
                                            title = j["skuName"].ToString(),
                                            shopId = j["skuId"].ToString(),
                                        });
                                    }
                                    type = 3;
                                }
                                break;
                            case "7"://浏览8秒
                                type = 1;
                                foreach (JToken j in data["browseShopVo"])
                                {
                                    items.Add(new TaskVoItemModel()
                                    {
                                        id = j["itemId"].ToString(),
                                        status = Convert.ToInt32(j["status"].ToString()),
                                        token = j["taskToken"].ToString(),
                                        title = j["shopName"].ToString(),
                                        shopId = j["shopId"].ToString(),
                                    });
                                }
                                break;
                            case "5"://进店浏览
                                
                                if(data["browseShopVo"] != null)
                                {
                                    foreach (JToken j in data["browseShopVo"])
                                    {
                                        items.Add(new TaskVoItemModel()
                                        {
                                            id = j["itemId"].ToString(),
                                            status = Convert.ToInt32(j["status"].ToString()),
                                            token = j["taskToken"].ToString(),
                                            title = j["shopName"].ToString(),
                                            shopId = j["shopId"].ToString(),
                                        });
                                    }
                                    type = 3;
                                }
                                else
                                {
                                    type = 5;
                                }
                                break;
                            case "0"://进入活动
                                
                                items.Add(new TaskVoItemModel()
                                {
                                    id = data["simpleRecordInfoVo"]["itemId"].ToString(),
                                    status = 1,
                                    token = data["simpleRecordInfoVo"]["taskToken"].ToString(),
                                    title = data["taskName"].ToString(),
                                    shopId = "",
                                });
                                switch (data["taskId"].ToString())
                                {
                                    case "31"://组队
                                        type = 7;
                                        break;
                                    case "5"://品牌墙
                                        type = 6;
                                        break;
                                    default:
                                        type = 3;
                                        break;
                                }
                                break;
                            default:
                                break;
                        }
                        tasks.Add(new TaskEntity()
                        {
                            name = data["taskName"].ToString(),
                            score = data["score"].ToString(),
                            title = data["subTitleName"].ToString(),
                            taskType = data["taskType"].ToString(),
                            status = data["status"].ToString(),
                            taskId = data["taskId"].ToString(),
                            maxTimes = Convert.ToInt32(data["maxTimes"].ToString()),
                            times = Convert.ToInt32(data["times"].ToString()),
                            data = data,
                            Items = items,
                            groupId = data["taskType"].ToString() == "2" ? data["groupId"].ToString() : "",
                            type = type
                        });
                    }
                    return tasks;
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        public static JToken joinMember(String cookie, String url, String refer, string ua)
        {
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem();

            item.URL = url;
            item.Referer = refer;
            item.Method = "GET";
            item.Cookie = cookie;
            item.Host = "api.m.jd.com";
            item.Accept = "*/*";
            if (ua != "")
            {
                item.UserAgent = ua;
            }
            HttpResult res1 = http.GetHtml(item);
            return JsonHelper.ExtractAll(res1.Html);
        }

        public static List<TaskEntity> getShopTaskDetail(String cookie,string appId)
        {
            JToken res = null;
            string body = "{\"taskToken\":\"\",\"appId\":\"" + appId + "\",\"channelId\":1}";
            res = BaseService.base_post("template_mongo_getHomeData ", cookie, body, "");
            MessageBox.Show(res.ToString());
            if (StrHelper.checkEquals(res["code"].ToString(), "0"))
            {
                if (StrHelper.checkEquals(res["data"]["bizCode"].ToString(), "0"))
                {

                    List<TaskEntity> tasks = new List<TaskEntity>();
                    for (int i = 0; i < res["data"]["result"]["taskVos"].ToArray().Count(); i++)
                    {
                        int type = 0;
                        JToken data = res["data"]["result"]["taskVos"][i];
                        List<TaskVoItemModel> items = new List<TaskVoItemModel>();
                        switch (data["taskType"].ToString())
                        {
                            case "12"://签到
                                items.Add(new TaskVoItemModel()
                                {
                                    id = data["simpleRecordInfoVo"]["itemId"].ToString(),
                                    status = 1,
                                    token = data["simpleRecordInfoVo"]["taskToken"].ToString(),
                                    title = data["taskName"].ToString(),
                                    shopId = "",
                                });
                                type = 0;
                                break;
                            case "1"://参与活动
                                type = 1;
                                foreach (JToken j in data["followShopVo"])
                                {
                                    items.Add(new TaskVoItemModel()
                                    {
                                        id = j["itemId"].ToString(),
                                        status = Convert.ToInt32(j["status"].ToString()),
                                        token = j["taskToken"].ToString(),
                                        title = j["shopName"].ToString(),
                                        shopId = j["shopId"].ToString(),
                                    });
                                }
                                break;
                            case "21"://逛店铺
                                type = 2;
                                foreach (JToken j in data["brandMemberVos"])
                                {
                                    items.Add(new TaskVoItemModel()
                                    {
                                        id = j["itemId"].ToString(),
                                        status = Convert.ToInt32(j["status"].ToString()),
                                        token = j["taskToken"].ToString(),
                                        title = j["title"].ToString(),
                                        shopId = "",
                                    });
                                }
                                break;
                            case "28"://邀请
                                type = 3;
                                
                                break;
                            case "3"://浏览
                                foreach (JToken j in data["shoppingActivityVos"])
                                {
                                    items.Add(new TaskVoItemModel()
                                    {
                                        id = j["itemId"].ToString(),
                                        status = Convert.ToInt32(j["status"].ToString()),
                                        token = j["taskToken"].ToString(),
                                        title = j["title"].ToString(),
                                        shopId = "",
                                    });
                                }
                                type = 2;
                                break;
                            case "15"://加购购物车
                                type = 4;
                                JToken productInfoVos = data["productInfoVos"];
                                if (productInfoVos != null)
                                {
                                    foreach (JToken j in productInfoVos)
                                    {
                                        items.Add(new TaskVoItemModel()
                                        {
                                            id = j["itemId"].ToString(),
                                            status = Convert.ToInt32(j["status"].ToString()),
                                            token = j["taskToken"].ToString(),
                                            title = j["skuName"].ToString(),
                                            shopId = j["skuId"].ToString(),
                                        });
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        tasks.Add(new TaskEntity()
                        {
                            name = data["taskName"].ToString(),
                            score = data["score"].ToString(),
                            title = data["subTitleName"].ToString(),
                            taskType = data["taskType"].ToString(),
                            status = data["status"].ToString(),
                            taskId = data["taskId"].ToString(),
                            maxTimes = Convert.ToInt32(data["maxTimes"].ToString()),
                            times = Convert.ToInt32(data["times"].ToString()),
                            data = data,
                            Items = items,
                            groupId = data["taskType"].ToString() == "2" ? data["groupId"].ToString() : "",
                            type = type
                        });
                    }
                    return tasks;
                }
                else
                {
                    return null;
                }
            }
            return null;
        }
    }
}
