using JDog202201.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JDog202201.Frm.Plugin
{
    public partial class FrmPluginWskey : Form
    {
        public FrmPluginWskey()
        {
            InitializeComponent();
        }

        private string gentoken = "https://api.m.jd.com/client.action?functionId=genToken&clientVersion=9.5.4&client=android&uuid=60439a1c4e78bca9&st=1621846685250&sign=bdbfdac2873edc7ae66a71134ff28688&sv=121";
        private string postbody = "body=%7B%22action%22%3A%22to%22%2C%22to%22%3A%22https%253A%252F%252Fdivide.jd.com%252Fuser_routing%253FskuId%253D100012043978%22%7D";
        private string useragent = "Mozilla/5.0 (Linux; Android 9.0; Z832 Build/MMB29M) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.87 Mobile Safari/537.36";

        private void FrmPluginWskey_Load(object sender, EventArgs e)
        {

        }

        public static string GetCookie(string name, string cookies)
        {
            try
            {
                foreach (string s in cookies.Split(';'))
                {
                    if (s.Contains("="))
                    {
                        int k = s.IndexOf('=');
                        string nam = s.Substring(0, k).Trim();
                        string val = s.Substring(k + 1);
                        if (nam == name)
                        {
                            return val;
                        }
                    }
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem();
            item.URL = gentoken;
            item.Method = "POST";
            item.Cookie = this.textBox1.Text;
            item.ContentType = "application/x-www-form-urlencoded";
            item.Postdata = postbody;
            item.UserAgent = useragent;
            item.Accept = "*/*";
            HttpResult res1 = http.GetHtml(item);
            JToken res = JsonHelper.ExtractAll(res1.Html);

            if (res["code"].ToString() == "0")
            {
                HttpHelper http2 = new HttpHelper();
                item.Method = "GET";
                item.ResultCookieType = ResultCookieType.CookieCollection;
                item.URL = string.Format("{0}?tokenKey={1}&to=https%3A%2F%2Fhome.m.jd.com%2FmyJd%2Fnewhome.action", res["url"].ToString(), res["tokenKey"].ToString());
                HttpResult res2 = http.GetHtml(item);
                StringBuilder cookie_str = new StringBuilder();

                foreach (Cookie co in res2.CookieCollection)
                {
                    cookie_str.Append(co.Name + "=" + co.Value + ";");

                }

                this.textBox1.Text = cookie_str.ToString();
                MessageBox.Show(string.Format("提示：{0}", "CK转换成功！"));

            }
            else
            {
                MessageBox.Show(string.Format("提示：{0}(返回值：{1})", res["echo"].ToString(), res["code"].ToString()));
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
