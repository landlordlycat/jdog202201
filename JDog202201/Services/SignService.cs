using JDog202201.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JDog202201.Services
{
    public class SignService
    {

        public static string getAppBody(string cookie,string secretp,string head = "extraData")
        {
            string boom_url = IniHelper.Instance.ReadString("Boom", "url", "http://jd.abug.cc");
            string ua = IniHelper.Instance.ReadString("Setting", "ua", "");
            int boom_type = IniHelper.Instance.ReadInteger("Boom", "index", 0);
            //{"ss":"{\"extraData\":{\"log\":\"\",\"sceneid\":\"ZNShPageh5\"},\"secretp\":\"iW1FlxIsAxZBoRW5bA\",\"random\":\"35012299\"}"}
            string log = "-1";
            string random = "12345678";

            if (boom_type == 1)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("cookie", cookie);
                dic.Add("ua", ua);
                dic.Add("type", "1");
                String url = boom_url + "/type"+ boom_type + "/getBody.php";
                JToken res = JsonHelper.ExtractAll(CurlHelper.Post(url, dic));
                if (res != null)
                {
                    log = res["log"].ToString();
                    random = res["random"].ToString();
                    MessageBox.Show(res["random"].ToString());
                }
                else
                {
                    MessageBox.Show("接口未启用");
                }
            }
            //return "{\\\"extraData\\\":{\\\"log\\\":\\\"-1\\\",\\\"sceneid\\\":\\\"ZNShPageh5\\\"},\\\"secretp\\\":\\\"iW1FlxIsAxZBoRW5bA\\\",\\\"random\\\":\\\"35012299\\\"}";
            if(head == "extraData")
            {
                return "{\\\"" + head + "\\\":{\\\"log\\\":\\\"" + log + "\\\",\\\"sceneid\\\":\\\"ZNShPageh5\\\"},\\\"secretp\\\":\\\"" + secretp + "\\\",\\\"random\\\":\\\"" + random + "\\\"}";
            }
            if(head == "safeStr")
            {
                return "\"safeStr\":\"{\\\"random\\\":\\\""+ random + "\\\",\\\"sceneid\\\":\\\"ZNShPageh5\\\",\\\"log\\\":\\\"" + log + "\\\"}\"";
            }
            return "{}";
            
        }


    }
}
