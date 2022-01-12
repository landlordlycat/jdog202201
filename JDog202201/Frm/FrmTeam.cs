using JDog202201.Entity;
using JDog202201.Services;
using JDog202201.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JDog202201.Frm
{
    public partial class FrmTeam : Form
    {

        private string base_cookie = "";
        private string base_ua = "";
        private string base_secretp = "";

        public FrmTeam(string cookie, string base_secretp, string ua)
        {
            InitializeComponent();
            this.base_cookie = cookie;
            this.base_ua = ua;
            this.base_secretp = base_secretp;
        }

        private void FrmTeam_Load(object sender, EventArgs e)
        {
            //refresh();
        }

        private void refresh()
        {
            TeamGroupEntity team = UserService.getTeamGroup(this.base_cookie);
            if(team == null)
            {
                addText("系统错误");
                return;
            }
            label8.Text = team.name;
            label2.Text = team.num.ToString();
            label5.Text = team.my_score.ToString();
            label6.Text = team.all_score.ToString();
            label10.Text = team.question.question;
            label11.Text = "A：" + team.question.A;
            label12.Text = "B："+team.question.B;
            if(team.question.result == "A")
            {
                label11.ForeColor = Color.Red;
            }
            if (team.question.result == "B")
            {
                label12.ForeColor = Color.Red;
            }
        }

        private void addText(String str = "")
        {
            listBox1.Items.Add(str);
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
            //MessageBox.Show(str);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            TeamGroupEntity team = UserService.getTeamGroup(this.base_cookie);
            if (team == null)
            {
                addText("系统错误");
                return;
            }
            textBox1.Text = team.join_code;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            TeamGroupEntity team = UserService.getTeamGroup(this.base_cookie);
            if (team == null)
            {
                addText("系统错误");
                return;
            }
            textBox1.Text = team.login_code;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            pk(textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /**
            String url = "http://jd.abug.cc/user.php";
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem();
            item.URL = url;
            HttpResult res1 = http.GetHtml(item);
            JToken res = JsonHelper.ExtractAll(res1.Html);
            if (res != null)
            {
                if (!StrHelper.checkEquals(res["team_code"].ToString()))
                {
                    //{"inviteId":"E7unasWZDJzSo6jCMoz5lyW9AXRW-ooenr7FyOPJOw","confirmFlag":"1","ss":"{\"extraData\":{\"log\":\"1634746977281~1RKnM0sr6unMDJRUnhHRjAxMQ==.YGRLc3FlZEFycWNkSDkpIWMQFBYhGC4DOGB+Tmt+fWAGdThgLB4jLx1mTXQkFCc+cys/NFM/aT0FLx0hbG8G.59ef146f~8,2~AED26242633D38A1651213FAAB7114C854A56AA406941C5725F08084CFABAC4A~1l83k7t~C~TxFBWRQOb2kbQEFZWkEMbhcHVBQEfR9wZhpyYnIbUxsHB1MaQRdPQ10HGQd8GHNmGHJuOxkGGVMGBRkXQxUSUAIYAHkYcWAbJQVtGxYaQRc+TRtXRl0XDgcYFkdEQA8VBlUFAQJXVwAFAQYGBwMFBQMVThdAUgcUDxcXFU1EQFVAUhQYFkNSAxcNFQVQQUEXFUxRFh8XRFJaFg5sWxkGAU8EGQZPUBUFaR8XXlwWDgUbQFZEFVkUUAEBWF0GB1IAVlJWDAJRAAIEBQcPVgZXVQAIVVAMDAAWGBZZEhcNFS9fW0AbQVhRRlBdAgIWGBZDQA8GAVACAgFVWA4HBgsEGBReXxYNQBgaVFcPVFdVVA8BDVUDBAZQVhYbQFNHVUEMF1MHCHUEAQBXcUNyAFlZBBVPGA9hYm8EXAQSGBFbQhQOFnNYDVJbUkN/W1ZNQxUSWlJDFgwWDQQOUgUVG0FFVkdBW2IIAQIZBw8HaRgVEFoVDTgUeHxDInoJFGNHWxZkRlFaC0BaFyZSXEEKDlcSGBFUWlJGW11TQBkVBlIUGRdSURcCGgMXGBQNBA0HUhcbFVAAAQZaUggGBgUDBwMHAgcaUwYOBFUAAwZTUgkDAAUHAhQYFgUVPxkVXgxXFw9BB19WUlVTQEIWGBZWCBcNFRYUGRcACBsKFkQGGgQaABYbQFZRaBUUDxdTUBscFlFRFgwWRlVZBlpaClQPDABTVw8DFh8XWVwWDm8GTgUbBz4aF1cPDl4SDhEEAgUAAwABWwEAB1QASwQqBE55QnlzUQ5xeXBxLGJwYSpicXQbLF4NCR1TcwNjUnxDV1dbBlRTfWMXOX1pXXUHdUFSBwQGJkJAHC5cQQUrU2tdbWReVlBefgx9BVlPfA1dZWMKAnV+Z0NWcFgAVnNAL1FSc1tFc10QE3ZaAWJ3d11xd012V2VAclt3X1lTJ0NDdXFNR3pWZUd0I3EHXygFXmwoNXBxZERWf3J3TBt5MAFifQpddno3MHN3dVtYd2MBWnZgO3l8XlN0cgciCGkDckR/UwsJGgUOW1ADUQcFS0JPUEdOSnFLYHRQc3JmVmNhcQx5fXMxA2t2cVdtdUJXdXZ9MnNhbjtyXWA7JntmbHVkBG51XHVhJ3N9UgFzf3IgUVpxYVtgVnAEdWJzGmx2dSJ3d3NTWWhlcXJhdV5xcWd2JH1/ciVXZ00ALn9hYXV3Q2Nic0xUI2JPRCpzBEYiJlVpUmVNfVNKBAUAF11YREEaF1gQBhsKFhFI~05779c5\",\"sceneid\":\"HYGJZYh5\"},\"secretp\":\"Ab28a9XXRMSQsLiMfg\",\"random\":\"79950663\"}"}
                    pk(res["team_code"].ToString());
                    //pk(res["pk_code"].ToString());
                }
            }**/
        }

        private void pk(string code)
        {
            if(code == "")
            {
                return;
            }
            string body = "{\"confirmFlag\":\"1\",\"ss\":\"" + SignService.getAppBody(this.base_cookie, this.base_secretp) + "\",\"inviteId\": \"" + code + "\"}";
            JToken res = BaseService.base_post("tigernian_pk_joinGroup", this.base_cookie, body, base_ua);
            if (res == null)
            {
                addText("系统错误");
                return;
            }
            if (res["code"] == null)
            {
                addText("系统错误");
                return;
            }
            if (res["code"].ToString() == "0")
            {
                addText("【" + res["data"]["bizCode"].ToString() + "】" + res["data"]["bizMsg"].ToString());

            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string body = "{\"ss\":\"" + SignService.getAppBody(this.base_cookie, this.base_secretp) + "\"}";
            JToken res = BaseService.base_post("tigernian_pk_getExpandDetail", this.base_cookie, body, base_ua);
            if (res == null)
            {
                addText("系统错误");
                return;
            }
            if (res["code"] == null)
            {
                addText("系统错误");
                return;
            }
            if (res["code"].ToString() != "0")
            {
                addText("【" + res["data"]["bizCode"].ToString() + "】" + res["data"]["bizMsg"].ToString());
                return;
            }
            if (res["data"]["result"] != null)
            {
                textBox1.Text = StrHelper.formatStr(res["data"]["result"]["inviteId"]);
            }

        }

        

        private void button9_Click(object sender, EventArgs e)
        {
            string code = textBox1.Text;
            if (code == "")
            {
                return;
            }
            string body = "{\"ss\":\"" + SignService.getAppBody(this.base_cookie, this.base_secretp) + "\",\"inviteId\": \"" + code + "\"}";
            JToken res = BaseService.base_post("tigernian_pk_collectPkExpandScore", this.base_cookie, body, base_ua);
            if (res == null)
            {
                addText("系统错误");
                return;
            }
            if (res["code"] == null)
            {
                addText("系统错误");
                return;
            }
            addText("【" + res["data"]["bizCode"].ToString() + "】" + res["data"]["bizMsg"].ToString());
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            string body = "{\"ss\":\"" + SignService.getAppBody(this.base_cookie, this.base_secretp) + "\"}";
            JToken res = BaseService.base_post("tigernian_pk_divideScores", this.base_cookie, body, base_ua);
            res = formatResult(res);
            if(res != null)
            {
                addText("开奖成功！获得 " + StrHelper.formatStr(res["produceScore"]) + "狗币");
            }
        }

        private JToken formatResult(JToken res,bool is_zero = false)
        {
            if (res == null)
            {
                addText("系统错误");
                return null;
            }
            if (res["code"] == null)
            {
                addText("系统错误");
                return null;
            }
            if (res["code"].ToString() != "0")
            {
                addText("【" + res["code"].ToString() + "】" + res["msg"].ToString());
            }
            if (res["data"]["bizCode"].ToString() != "0" || is_zero)
            {
                addText("【" + res["data"]["bizCode"].ToString() + "】" + res["data"]["bizMsg"].ToString());
            }

            return res["data"]["result"];
        }
        private void button7_Click(object sender, EventArgs e)
        {
            JToken res = BaseService.base_post("tigernian_pk_getAmountForecast", this.base_cookie, "{}", base_ua);
            res = formatResult(res);
            if (res != null)
            {
                addText("领取成功！");
                addText("可获得 " + StrHelper.formatStr(res["userAward"]) + "元红包");
                addText("可膨胀到 " + StrHelper.formatStr(res["userAwardExpand"]) + "元红包");
                addText("需要 " + StrHelper.formatStr(res["targetNum"]) + "元红包");
                addText("助力膨胀码已加载");
                string body = "{\"ss\":\"" + SignService.getAppBody(this.base_cookie, this.base_secretp) + "\"}";
                res = BaseService.base_post("tigernian_pk_getExpandDetail", this.base_cookie, body, base_ua);
                res = formatResult(res);
                if (res != null)
                {
                    textBox1.Text = StrHelper.formatStr(res["inviteId"]);
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            /**
            String url = "http://jd.abug.cc/user.php";
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem();
            item.URL = url;
            HttpResult res1 = http.GetHtml(item);
            JToken res = JsonHelper.ExtractAll(res1.Html);
            if (res != null)
            {
                if (!StrHelper.checkEquals(res["pk_code"].ToString()))
                {
                    string body = "{\"ss\":\"" + SignService.getAppBody(this.base_cookie, this.base_secretp) + "\",\"inviteId\": \"" + res["pk_code"].ToString() + "\"}";
                    res = BaseService.base_post("tigernian_pk_collectPkExpandScore", this.base_cookie, body, base_ua);
                    formatResult(res,true);
                }
            }**/
        }

        private void button12_Click(object sender, EventArgs e)
        {
            //string body = "{\"ss\":\"" + SignService.getBody(this.base_cookie, this.base_secretp) + "\",\"inviteId\": \"" + res["pk_code"].ToString() + "\"}";
            JToken res = BaseService.base_post("tigernian_pk_receiveAward", this.base_cookie, "{}", base_ua);
            formatResult(res, true);
        }
    }
}
