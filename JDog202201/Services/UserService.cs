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
    internal class UserService
    {
        public static TeamGroupEntity getTeamGroup(string cookie)
        {
            TeamGroupEntity team = new TeamGroupEntity();
            JToken res = BaseService.base_post("tigernian_pk_getHomeData", cookie);
            if (StrHelper.checkEquals(res["code"].ToString(), "0"))
            {
                if (StrHelper.checkEquals(res["data"]["bizCode"].ToString(), "0"))
                {
                    JToken result = res["data"]["result"];
                    if(result == null)
                    {
                        return null;
                    }
                    team.join_code = "";
                    team.login_code = "";
                    team.num = 0;
                    team.name = "未加入任何队伍";
                    team.my_score = 0;
                    team.all_score = 0;
                    team.secretp = StrHelper.formatStr(result["secretp"]);
                    team.question = new TeamGroupQuestion()
                    {
                        question = "",
                        A = "",
                        B = "",
                        status = 0,
                        result = "A",
                    };

                    if (result["groupInfo"] != null)
                    {
                        team.join_code = StrHelper.formatStr(result["groupInfo"]["groupJoinInviteId"]);
                        team.login_code = StrHelper.formatStr(result["groupInfo"]["groupLoginInviteId"]);
                        team.num = StrHelper.formatInt(result["groupInfo"]["groupNum"]);
                        team.name = StrHelper.formatStr(result["groupInfo"]["groupName"]);
                    }

                    if (result["autoProduceInfo"] != null)
                    {
                        team.my_score = StrHelper.formatInt(result["autoProduceInfo"]["produceScore"]);
                        team.all_score = StrHelper.formatInt(result["autoProduceInfo"]["teamProduceScore"]);
                    }
                    team.question.question = StrHelper.formatStr(result["votInfo"]["question"]);
                    team.question.A = StrHelper.formatStr(result["votInfo"]["optionAText"]);
                    team.question.B = StrHelper.formatStr(result["votInfo"]["optionBText"]);
                    team.question.status = StrHelper.formatInt(result["votInfo"]["status"]);
                    team.question.result = StrHelper.formatStr(result["votInfo"]["votFor"]);
                    return team;
                }
            }
            return null;
        }

        public static JToken getUserInfo(string cookie)
        {
            String url = "https://me-api.jd.com/user_new/info/GetJDUserInfoUnion?orgFlag=JD_PinGou_New&callSource=mainorder&channel=4&isHomewhite=0&sceneval=2&sceneval=2&g_login_type=1&callback=GetJDUserInfoUnion&g_ty=ls";
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem();
            item.UserAgent = "jdapp;android;10.0.2;11;3643931626635363-1393666303034626;network/wifi;model/MI CC9 Pro Premium Edition;addressid/1473398996;aid/c49ab65619f600db;oaid/a3e1df0f1a3d6452;osVer/30;appBuild/88569;partner/xiaomi001;eufv/1;jdSupportDarkMode/0;Mozilla/5.0 (Linux; Android 11; MI CC9 Pro Premium Edition Build/RKQ1.200826.002; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/77.0.3865.120 MQQBrowser/6.2 TBS/045511 Mobile Safari/537.36";
            item.URL = url;
            item.Method = "GET";
            item.Cookie = cookie;
            item.Timeout = 30000;
            item.Encoding = Encoding.UTF8;
            HttpResult res = http.GetHtml(item);
            return JsonHelper.ExtractAll((res.Html + "#########").Replace("GetJDUserInfoUnion(","").Replace(")#########",""));
        }

        public static JToken getLevel(string cookie)
        {
            String url = "https://vip.m.jd.com/scoreDetail/current";
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem();
            string ua = IniHelper.Instance.ReadString("Setting", "ua", "");
            item.URL = url;
            item.Referer = "https://vip.m.jd.com";
            item.Method = "GET";
            item.Cookie = cookie;
            item.Encoding = Encoding.UTF8;
            HttpResult res = http.GetHtml(item);
            return JsonHelper.ExtractAll(res.Html);
        }

        public static HomeEntity getHome(String cookie)
        {
            JToken res = BaseService.base_post("tigernian_getHomeData", cookie);

            HomeEntity home = new HomeEntity();
            //homeMainInfo
            HomeEntity.homeMainInfo homeMainInfo = new HomeEntity.homeMainInfo();
            if (res == null)
            {
                return null;
            }
            if (res["code"] == null)
            {
                return null;
            }
            if (!StrHelper.checkEquals(res["code"].ToString(), "0"))
            {
                return null;
            }
            if (res["data"] == null)
            {
                return null;
            }
            if (!StrHelper.checkEquals(res["data"]["bizCode"].ToString(), "0"))
            {
                
                return null;
            }
            homeMainInfo.curCity = StrHelper.formatInt(res["data"]["result"]["homeMainInfo"]["curCity"].ToString());
            homeMainInfo.dropHongbaoDisplay = StrHelper.formatStr(res["data"]["result"]["homeMainInfo"]["dropHongbaoDisplay"].ToString()) != "False";
            homeMainInfo.jdCipher = StrHelper.formatInt(res["data"]["result"]["homeMainInfo"]["jdCipher"].ToString());
            homeMainInfo.pkJdCipher = StrHelper.formatInt(res["data"]["result"]["homeMainInfo"]["pkJdCipher"].ToString());
            homeMainInfo.secretp = StrHelper.formatStr(res["data"]["result"]["homeMainInfo"]["secretp"].ToString());
            homeMainInfo.shareMiniprogramSwitch = StrHelper.formatInt(res["data"]["result"]["homeMainInfo"]["shareMiniprogramSwitch"].ToString());
            homeMainInfo.taskSwitch = StrHelper.formatInt(res["data"]["result"]["homeMainInfo"]["taskSwitch"].ToString());
            homeMainInfo.todaySignStatus = StrHelper.formatInt(res["data"]["result"]["homeMainInfo"]["todaySignStatus"].ToString());
            homeMainInfo.totalCity = StrHelper.formatInt(res["data"]["result"]["homeMainInfo"]["totalCity"].ToString());
            homeMainInfo.userType = StrHelper.formatInt(res["data"]["result"]["homeMainInfo"]["userType"].ToString());

            home.homeMain = homeMainInfo;
            //userMainInfo
            HomeEntity.userMainInfo userMainInfo = new HomeEntity.userMainInfo();
            userMainInfo.now_score = StrHelper.formatInt(res["data"]["result"]["homeMainInfo"]["raiseInfo"]["remainScore"].ToString());
            userMainInfo.next_score = StrHelper.formatInt(res["data"]["result"]["homeMainInfo"]["raiseInfo"]["nextLevelScore"].ToString()) - StrHelper.formatInt(res["data"]["result"]["homeMainInfo"]["raiseInfo"]["curLevelStartScore"].ToString());
            userMainInfo.bonus = StrHelper.formatInt(res["data"]["result"]["homeMainInfo"]["raiseInfo"]["userBonusCount"].ToString());

            userMainInfo.now_city = 0;
            userMainInfo.all_city = 0;

            if (res["data"]["result"]["homeMainInfo"]["raiseInfo"]["cityConfig"] != null)
            {
                if (res["data"]["result"]["homeMainInfo"]["raiseInfo"]["cityConfig"]["points"] != null)
                {
                    JToken cityConfig = res["data"]["result"]["homeMainInfo"]["raiseInfo"]["cityConfig"]["points"];
                    foreach(JToken city in cityConfig)
                    {
                        userMainInfo.all_city++;
                        if (city["status"].ToString() == "2")
                        {
                            userMainInfo.now_city++;
                        }
                    }
                }
            }
            



            home.userMain = userMainInfo;

            home.secretp = StrHelper.formatStr(res["data"]["result"]["homeMainInfo"]["secretp"].ToString());
            return home;
        }
    }
}
