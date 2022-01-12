using JDog202201.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace JDog202201.Services
{
    internal class BaseService
    {
        public static JToken base_post(String type, String cookie, String body = "{}", string ua = "")
        {

            String url = "https://api.m.jd.com/client.action?functionId=" + type;

            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem();
            item.URL = url;
            if (ua != "")
            {
                item.UserAgent = ua;
            }
            item.Referer = "https://wbbny.m.jd.com/babelDiy/Zeus/41AJZXRUJeTqdBK9bPoPgUJiodcU/index.html?babelChannel=syfc&from=home&sid=35da990a0620191909e77030ddc1749w&un_area=13_1032_28922_59727";
            item.ContentType = "application/x-www-form-urlencoded";
            item.Accept = "*/*";
            item.Method = "POST";
            item.Cookie = cookie;
            item.Host = "api.m.jd.com";
            item.Postdata = "functionId=" + type + "&body=" + StrHelper.ChEncodeUrl(body) + "&client=wh5&clientVersion=1.0.0";
            HttpResult res1 = http.GetHtml(item);
            try
            {
                JToken res = JsonHelper.ExtractAll(res1.Html);
                if (res == null)
                {
                    return JsonHelper.ExtractAll("{\"code\":\"0\",\"msg\":\"网络异常，请稍后重试\"}");
                }
                else
                {
                    return res;
                }
            }
            catch (Exception e)
            {
                return JsonHelper.ExtractAll("{\"code\":\"0\",\"msg\":\"" + e.Message.ToString() + "\"}");
            }

        }



        public static JToken base_call(String cookie, String taskToken, string ua = "")
        {
            //{"dataSource":"newshortAward","method":"getTaskAward","reqParams":"{\"taskToken\":\"P129qQxH0Ib9VXXFjRWn6u7yx4KDGD52HpNlDOPlRk\"}","sdkVersion":"1.0.0","clientLanguage":"zh","onlyTimeId":1634726417053,"riskParam":{"platform":"3","orgType":"2","openId":"-1","pageClickKey":"Babel_VKCoupon","eid":"eidAf665812230sfhopX+rsxSaqJa3xn342QEoaKWXV+em1XNrW73Cmp0y9nONV7wktl/UZNC6g89in50rm/4CwV8BfuenrZy90DxGTHad/63KdqgjVp","fp":"-1","shshshfp":"317600491624b2a52e5db77c3b610274","shshshfpa":"54906d42-821d-24a5-f861-d7659f72a1a1-1626828347","shshshfpb":"fdiL453bEuF4mnf x/lWWZg==","childActivityUrl":"https%3A%2F%2Fprodev.m.jd.com%2Fmall%2Factive%2F24fcnjmXen4Tj9NPwV57vYwu9Lia%2Findex.html%3FcomponentId%3De623ae1275ae4d1da40329ba4868761f%26taskParam%3D%257B%2522biz%2522%253A%2522babel%2522%252C%2522taskToken%2522%253A%2522P129qQxH0Ib9VXXFjRWn6u7yx4KDGD52HpNlDOPlRk%2522%257D%26tttparams%3D3Yp89geyJnTGF0IjoiMzYuNzA0OTAzIiwiZ0xuZyI6IjExOS4xNDAzODgiLCJncHNfYXJlYSI6IjBfMF8wXzAiLCJsYXQiOjAsImxuZyI6MCwibW9kZWwiOiJNSSBDQzkgUHJvIFByZW1pdW0gRWRpdGlvbiIsInByc3RhdGUiOiIwIiwidW5fYXJlYSI6IjEzXzEwMzJfMjg5MjJfNTk3MjcifQ6%253D%253D%26un_area%3D13_1032_28922_59727","userArea":"-1","client":"","clientVersion":"","uuid":"","osVersion":"","brand":"","model":"","networkType":"","jda":"-1"}}
            string sendInfo = "{\"dataSource\":\"newshortAward\",\"method\":\"getTaskAward\",\"reqParams\":\"{\\\"taskToken\\\":\\\"" + taskToken + "\\\"}\",\"sdkVersion\":\"1.0.0\",\"clientLanguage\":\"zh\"}";
            sendInfo = HttpUtility.UrlEncode(sendInfo);
            String url = "https://api.m.jd.com/?functionId=qryViewkitCallbackResult&client=wh5&clientVersion=1.0.0&body=" + sendInfo + "&_timestamp=" + TimeHelper.GetTimeStamp(false);
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem();
            item.URL = url;
            item.Referer = "https://bunearth.m.jd.com";
            item.ContentType = "application/x-www-form-urlencoded";
            item.Method = "GET";
            item.Cookie = cookie;
            item.Accept = "*/*";
            if (ua != "")
            {
                item.UserAgent = ua;
            }
            HttpResult res1 = http.GetHtml(item);
            try
            {
                JToken res = JsonHelper.ExtractAll(res1.Html);
                if (res == null)
                {
                    return JsonHelper.ExtractAll("{\"code\":\"0\",\"msg\":\"网络异常，请稍后重试\"}");
                }
                else
                {
                    return res;
                }
            }
            catch (Exception e)
            {
                return JsonHelper.ExtractAll("{\"code\":\"0\",\"msg\":\"" + e.Message.ToString() + "\"}");
            }
        }

        public static string getJoyToken(String cookie)
        {
            String url = "https://bh.m.jd.com/gettoken";
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem();
            item.URL = url;
            item.Referer = "https://wbbny.m.jd.com/";
            item.ContentType = "application/x-www-form-urlencoded";
            item.Method = "POST";
            item.Cookie = cookie;
            item.Postdata = "content={\"appname\":\"50090\",\"whwswswws\":\"fdiL453bEuF4mnf x/ lWWZg == \",\"jdkey\":\"--c49ab65619f600db\",\"body\":{\"platform\":\"1\"}}";
            HttpResult res1 = http.GetHtml(item);
            JToken res = JsonHelper.ExtractAll(res1.Html);
            if (res["joyytoken"] == null)
            {
                return "";
            }
            else
            {
                return res["joyytoken"].ToString();
            }
        }
    }
}
