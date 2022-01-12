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
