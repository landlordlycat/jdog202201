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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JDog202201.Frm
{
    public partial class FrmHelp : Form
    {
        private string base_cookie = "";
        private string base_ua = "";
        private string base_secretp = "";
        public FrmHelp(string cookie, string base_secretp, string ua)
        {
            InitializeComponent();
            this.base_cookie = cookie;
            this.base_ua = ua;
            this.base_secretp = base_secretp;
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            JToken res = BaseService.base_post("tigernian_getTaskDetail ", this.base_cookie, "{}", "");
            if (StrHelper.checkEquals(res["code"].ToString(), "0"))
            {
                if (StrHelper.checkEquals(res["data"]["bizCode"].ToString(), "0"))
                {
                    string code = StrHelper.formatStr(res["data"]["result"]["inviteId"]);
                    if (code != "")
                    {
                        textBox1.Text = code;
                        addText("获取成功");
                        return;
                    }
                }
            }
            //MessageBox.Show("获取失败，可能原因：\r\n1、cookie失效\r\n2、活动火爆\r\n3、助力已满");
            
        }

        private void addText(String str = "")
        {
            listBox1.Items.Add(str);
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            List<string> cookies = new List<string>();
            for (int i = 0; i < textBox2.Lines.Length; i++)
            {
                if (!StrHelper.checkEquals(textBox2.Lines[i].ToString()))
                {
                    cookies.Add(textBox2.Lines[i].ToString());
                }
                
            }
            addText("已获取到 " + cookies.Count + " 条数据");
            int j = 0;
            foreach (string cookie in cookies)
            {
                j++;
                addText("正在执行第【 " + j +" 】条》》》");
                if(!doHelp(cookie, textBox1.Text))
                {
                    break; 
                }
                Thread.Sleep(5000);
            }
            addText("处理完毕");
        }

        private bool doHelp(string cookie,string code="")
        {
            //HomeEntity home = UserService.getHome(cookie);
            string body = "{\"ss\":\"" + SignService.getAppBody(this.base_cookie,this.base_secretp) + "\",\"inviteId\": \""+ cookie + "\"}";
            JToken res = BaseService.base_post("tigernian_collectScore", this.base_cookie,body,this.base_ua);
            if (res == null)
            {
                addText("异常账号跳过");
                return true;
            }
            if(res["code"] == null)
            {
                addText("异常账号跳过");
                return true;
            }
            if (res["code"].ToString()=="0")
            {
                /**
                if (StrHelper.checkEquals(res["data"]["bizCode"].ToString(), "-201"))
                {
                    addText("助力已满，跳出！");
                    return false;
                }**/
                addText("【"+res["data"]["bizCode"].ToString()+ "】"  + res["data"]["bizMsg"].ToString());
                
            }
            return true;
        }

        private void FrmHelp_Load(object sender, EventArgs e)
        {
            //textBox2.Text = this.base_cookie + "\r\n";
            JToken res = BaseService.base_post("tigernian_getTaskDetail ", this.base_cookie, "{}", "");
            if (StrHelper.checkEquals(res["code"].ToString(), "0"))
            {
                if (StrHelper.checkEquals(res["data"]["bizCode"].ToString(), "0"))
                {
                    string code = StrHelper.formatStr(res["data"]["result"]["inviteId"]);
                    if (code != "")
                    {
                        textBox1.Text = code;
                        addText("获取成功");
                        return;
                    }
                }
            }
            
            addText("获取当前助力码失败，可能原因：");
            addText("1、cookie失效");
            addText("2、活动火爆");
            addText("3、助力已满");
            addText("===============================");
            addText();
        }
    }
}
