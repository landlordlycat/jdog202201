using JDog202201.Entity;
using JDog202201.Services;
using JDog202201.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace JDog202201
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
            //randomUA();
            //this.label12.Text = now_version;
            IniHelper.Instance.FileName = "config.ini";
            Control.CheckForIllegalCrossThreadCalls = false;
            String sPath = Path.Combine(Environment.CurrentDirectory, "config.ini");
            if (!File.Exists(sPath))
            {
                IniHelper.Instance.WriteString("User", "Cookie", "");
                IniHelper.Instance.WriteString("User", "Name", "");
                IniHelper.Instance.WriteInteger("Setting", "collect_time", 1200);
                IniHelper.Instance.WriteInteger("Setting", "auto_update", 0);
                IniHelper.Instance.WriteString("Setting", "ua", "");
                IniHelper.Instance.WriteInteger("Boom", "index", 0);
                IniHelper.Instance.WriteString("Boom", "url", "http://jd.abug.cc");
            }
        }

        private String base_cookies = "";//当前的执行cookie
        private String base_secretp = "";

        int base_sign_time = 0;
        Thread dataThread;

        private void FrmMain_Load(object sender, EventArgs e)
        {
            //SignService.getBody2("","");
            textBox1.Text = IniHelper.Instance.ReadString("Setting", "ua", "");
            textBox2.Text = IniHelper.Instance.ReadString("Boom", "url", "http://jd.abug.cc");
            comboBox1.SelectedIndex = IniHelper.Instance.ReadInteger("Boom", "index", 0);
            this.base_cookies = IniHelper.Instance.ReadString("User", "Cookie", "");
            
            init();
            dataThread = new Thread(delegate ()
            {
                doTask();
            });
            checkUpdate(true);
            
        }

        private void 退出软件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 清空记录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void addText(String str = "")
        {
            str = str.Replace("汪汪币", "爆竹");
            listBox1.Items.Add(str);
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
        }

        private void init()
        {
            String cookie = IniHelper.Instance.ReadString("User", "Cookie", "");
            int collect_time = IniHelper.Instance.ReadInteger("Setting", "collect_time", 1200);
            numericUpDown1.Value = collect_time;
            label15.Text = collect_time.ToString();
            checkBox2.Checked = IniHelper.Instance.ReadInteger("Setting", "auto_update", 0) == 1;
            if (!StrHelper.checkEquals(cookie))
            {
                this.base_cookies = cookie;
                this.label2.Text = IniHelper.Instance.ReadString("User", "Name", "未登录");
                notifyIcon1.Text = "【小软件】"+this.label2.Text;
                button1.Text = "重新登录";
                getScore();
            }
        }

        

        private String getUser(String cookie)
        {
            JToken user = UserService.getUserInfo(cookie);
            if (user["data"] == null)
            {
                return null;
            }

            if (user["data"]["userInfo"] == null)
            {
                return null;
            }
            if (user["data"]["userInfo"]["baseInfo"] == null)
            {
                return null;
            }
            return StrHelper.formatStr(user["data"]["userInfo"]["baseInfo"]["nickname"]);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Frm.FrmLoginByInput frm = new Frm.FrmLoginByInput();
            frm.ShowDialog();
            if (frm.status)
            {
                IniHelper.Instance.FileName = "config.ini";
                string cookie = frm.cookie;
                string user = frm.user;
                if (user == "")
                {
                    user = getUser(cookie);
                }
                if (user != null)
                {
                    IniHelper.Instance.WriteString("User", "Cookie", cookie);
                    IniHelper.Instance.WriteString("User", "Name", user);
                    label2.Text = user;
                    button1.Text = "重新登录";
                    addText("【" + user + "】登录成功");
                    notifyIcon1.Text = "【小软件】" + user;
                    this.base_cookies = cookie;
                    getScore();
                }
            }
        }

        private void getScore()
        {
            HomeEntity home = UserService.getHome(this.base_cookies);
            
            if(home != null)
            {
                label6.Text = home.userMain.bonus.ToString();
                label7.Text = home.userMain.now_score.ToString() + "/" + home.userMain.next_score.ToString();
                label8.Text = home.homeMain.curCity.ToString() + "/" + home.homeMain.totalCity.ToString();
                label22.Text = "("+home.userMain.now_city.ToString() + "/" + home.userMain.all_city.ToString()+")";
                this.base_secretp = home.secretp;
                if(home.userMain.now_score > home.userMain.next_score && checkBox2.Checked)
                {

                    unlock();
                    
                }
            }
            else
            {
                addText("登录失效");
            }
            
        }

        

        private void button2_Click(object sender, EventArgs e)
        {
            unlock();
        }

        private bool base_status = false;

        private void button3_Click(object sender, EventArgs e)
        {

            if (base_status)
            {
                base_status = false;

                addText("任务已停止");
                button3.Text = "开始任务";
                if (dataThread.IsAlive)
                    dataThread.Abort();
                //dataThread.Abort();
            }
            else
            {
                base_status = true;
                button3.Text = "结束任务";
                if (!dataThread.IsAlive)
                {
                    dataThread = new Thread(delegate ()
                    {
                        doTask();
                    });
                    dataThread.Start();
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Frm.FrmTeam frm = new Frm.FrmTeam(this.base_cookies, this.base_secretp, getUA());
            frm.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Frm.FrmHelp frm = new Frm.FrmHelp(this.base_cookies, this.base_secretp, getUA());
            frm.Show();
        }

        private void checkUpdate(bool is_load = false)
        {
            /**
            String url = "http://jd.abug.cc/update.php";
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem();
            item.URL = url;
            HttpResult res1 = http.GetHtml(item);
            JToken res = JsonHelper.ExtractAll(res1.Html);
            if (res == null)
            {
                addText("无法连接服务器");
            }
            else
            {
                if (!StrHelper.checkEquals(res["status"].ToString(), "1"))
                {
                    MessageBox.Show("服务已关闭");
                    Application.Exit();
                    return;
                }
                //version_code":1,"version":"10.20.03","url":"https:\/\/a.bai0.cc"}
                if (Convert.ToInt32(res["version_code"].ToString()) > this.version_code)
                {
                    addText("存在新版本【" + res["version"].ToString() + "】，正在打开更新页");
                    System.Diagnostics.Process.Start(res["url"].ToString());
                }
                else
                {
                    if (!is_load)
                    {
                        addText("当前已为最新版本");
                    }
                    
                }
            }
            **/
        }

        private void FrmMain_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.ShowInTaskbar = false;
                this.notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            notifyIcon1.Visible = true;

            this.Show();
            this.Activate();
            this.WindowState = FormWindowState.Normal;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                int time = Convert.ToInt32(numericUpDown1.Value);
                Random rd = new Random();
                IniHelper.Instance.WriteInteger("Setting", "collect_time", time);
                this.base_sign_time = time + rd.Next(10, 100);
                label15.Text = base_sign_time.ToString();
                this.timer1.Start();
                collect_score();
            }
            else
            {
                this.base_sign_time = Convert.ToInt32(numericUpDown1.Value);
                label15.Text = base_sign_time.ToString();
                this.timer1.Stop();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.base_sign_time > 0)
            {
                this.base_sign_time--;
            }
            else
            {
                int time = Convert.ToInt32(numericUpDown1.Value);
                Random rd = new Random();
                this.base_sign_time = time + rd.Next(100, 1000);
                //TODO 执行收集爆竹
                collect_score();

            }
            label15.Text = base_sign_time.ToString();
        }

        private void sleep(int time)
        {
            Random r = new Random();
            time = r.Next(time + 2, time * 2);
            addText(">>>>>>>>>>等待【" + time + "】秒");
            Thread.Sleep(time * 1000);
        }

        private void doTask()
        {
            if (checkBox3.Checked)//签到
                doSign();
            if (checkBox4.Checked)//APP任务
                doAppTask();
            if (checkBox5.Checked)
                doWxTask();//微信任务
            if (checkBox11.Checked)
                doLottery();//抽奖任务
            addText("任务已结束,如发现有未完成任务请多执行几次");
            button3.Text = "开始任务";
            base_status = false;
            if (dataThread.IsAlive)
                dataThread.Abort();
        }



        private void doLottery()
        {
            addText("开始执行抽奖任务");
            String body = "{\"qryParam\":\"[{\\\"type\\\":\\\"advertGroup\\\",\\\"mapTo\\\":\\\"homeMsgs\\\",\\\"id\\\":\\\"05863713\\\"},{\\\"type\\\":\\\"advertGroup\\\",\\\"mapTo\\\":\\\"homeBtnDrawNotFirsts\\\",\\\"id\\\":\\\"06079449\\\"},{\\\"type\\\":\\\"advertGroup\\\",\\\"id\\\":\\\"06079417\\\",\\\"mapTo\\\":\\\"homePullDowner\\\"},{\\\"type\\\":\\\"advertGroup\\\",\\\"id\\\":\\\"06079457\\\",\\\"mapTo\\\":\\\"homeNaming\\\"},{\\\"type\\\":\\\"advertGroup\\\",\\\"id\\\":\\\"05863717\\\",\\\"mapTo\\\":\\\"homeBtnLink\\\"},{\\\"type\\\":\\\"advertGroup\\\",\\\"id\\\":\\\"06079423\\\",\\\"mapTo\\\":\\\"homePopupPrivateDomain\\\"},{\\\"type\\\":\\\"advertGroup\\\",\\\"id\\\":\\\"05863725\\\",\\\"mapTo\\\":\\\"homeBtnBranch\\\"},{\\\"type\\\":\\\"advertGroup\\\",\\\"id\\\":\\\"05863757\\\",\\\"mapTo\\\":\\\"homeBtnMainDivided\\\"},{\\\"type\\\":\\\"advertGroup\\\",\\\"id\\\":\\\"06082301\\\",\\\"mapTo\\\":\\\"homeBtnTaskKoi\\\"},{\\\"type\\\":\\\"advertGroup\\\",\\\"id\\\":\\\"05863748\\\",\\\"mapTo\\\":\\\"homeBtnTaskUnavailable\\\"},{\\\"type\\\":\\\"advertGroup\\\",\\\"id\\\":\\\"06083624\\\",\\\"mapTo\\\":\\\"homeBtnKoi\\\"},{\\\"type\\\":\\\"advertGroup\\\",\\\"id\\\":\\\"06079457\\\",\\\"mapTo\\\":\\\"homePopupFallingRedbag\\\"},{\\\"type\\\":\\\"advertGroup\\\",\\\"mapTo\\\":\\\"babelCountDownFromAdv\\\",\\\"id\\\":\\\"05884370\\\"},{\\\"type\\\":\\\"advertGroup\\\",\\\"mapTo\\\":\\\"taskPanelBanner\\\",\\\"id\\\":\\\"05863785\\\"}]\",\"activityId\":\"41AJZXRUJeTqdBK9bPoPgUJiodcU\",\"pageId\":\"\",\"reqSrc\":\"\",\"applyKey\":\"jd_star\"}";
            JToken res = BaseService.base_post("qryCompositeMaterials", base_cookies, body);
            if (!StrHelper.checkEquals(res["code"].ToString(), "0"))
            {
                addText(res["msg"].ToString());
                return;
            }

            JToken shop_list = res["data"]["feedBottomData7"]["list"];
            addText("获取到" + shop_list.Count() + "个店铺");
            int shop_inde = 0;
            foreach (JToken shop in shop_list)
            {
                shop_inde++;
                JToken shopId = shop["link"];
                if (shopId == null || shopId.ToString() == "")
                {
                    continue;
                }
                addText("执行第" + shop_inde + "个店铺:" + shop["name"].ToString());
                String venderId = StrHelper.formatJToken(shop, "extension.shopInfo.venderId").ToString();
                string encryptActivityId = StrHelper.formatJToken(shop, "extension.venderLink1").ToString().Replace("https://h5.m.jd.com/babelDiy/Zeus/","").Replace("/index.html", ""); ;
                body = "{\"encryptActivityId\":\""+ encryptActivityId + "\",\"channelId\":1}";
                JToken shop_info = BaseService.base_post("factory_getStaticConfig", this.base_cookies, body, getUA());
                if (shop_info == null)
                {
                    continue;
                }
                if (!StrHelper.checkEquals(shop_info["code"].ToString(), "0"))
                {
                    addText(shop_info["msg"].ToString());
                    continue;
                }

                string appId = StrHelper.formatJToken(shop_info, "data.result.appId").ToString();
                List<TaskEntity> tasks = TaskService.getShopTaskDetail(this.base_cookies, appId);
                if(tasks == null || tasks.Count == 0)
                {
                    return;
                }
                foreach (TaskEntity task in tasks)
                {
                    if (task.status != "1")
                    {
                        continue;
                    }
                    
                    switch (task.type)
                    {
                        case 0://店铺签到
                            addText("【执行任务】  -->  " + task.name + "【" + task.title + "】[" + task.times + "/" + task.maxTimes + "]");
                            addText("执行店铺签到");
                            //body = SignService.getBody(this.base_cookies, this.base_secretp);
                            if (task.Items.Count == 0)
                            {
                                continue;
                            }
                            //{"shopId":"762660","venderId":"767122","projectId":7342,"taskId":39429,"token":"101002850000062958945B95C9E4AF3D","opType":2}
                            body = "{\"taskToken\":\"" + task.Items[0].token + "\",\"taskId\":" + task.taskId + ",\"actionType\":0,\"appId\":\"" + appId + "\"," + SignService.getAppBody(this.base_cookies, this.base_secretp, "safeStr") + "}";
                            JToken score = formatResult(BaseService.base_post("template_mongo_collectScore", this.base_cookies, body, getUA()), false);
                            if (score == null)
                            {
                                continue;
                            }
                            addText("签到获得" + score["score"].ToString() + "爆竹");
                            getScore();
                            sleep(3);
                            break;
                        case 1://参与活动
                            
                            addText("【执行任务】  -->  " + task.name + "【" + task.title + "】[" + task.times + "/" + task.maxTimes + "]");
                            foreach (TaskVoItemModel item in task.Items)
                            {
                                if (!StrHelper.checkEquals(item.status.ToString(), "1"))
                                {
                                    continue;
                                }
                                addText("正在执行:" + item.title.ToString() + "");
                                body = "{\"taskToken\":\"" + item.token + "\",\"taskId\":" + task.taskId + ",\"actionType\":0,\"appId\":\"" + appId + "\"," + SignService.getAppBody(this.base_cookies, this.base_secretp, "safeStr") + "}";
                                score = formatResult(BaseService.base_post("template_mongo_collectScore", this.base_cookies, body, getUA()), false);
                                if (score == null)
                                {
                                    continue;
                                }
                                addText("获得" + score["score"].ToString() + "爆竹");
                                getScore();
                                sleep(3);
                                
                            }
                            break;
                        case 2://逛店铺

                            addText("【执行任务】  -->  " + task.name + "【" + task.title + "】[" + task.times + "/" + task.maxTimes + "]");
                            foreach (TaskVoItemModel item in task.Items)
                            {
                                if (!StrHelper.checkEquals(item.status.ToString(), "1"))
                                {
                                    continue;
                                }
                                addText("正在执行:" + item.title.ToString() + "");
                                body = "{\"taskToken\":\"" + item.token + "\",\"taskId\":" + task.taskId + ",\"actionType\":0,\"appId\":\"" + appId + "\"," + SignService.getAppBody(this.base_cookies, this.base_secretp, "safeStr") + "}";
                                score = formatResult(BaseService.base_post("template_mongo_collectScore", this.base_cookies, body, getUA()), false);
                                if (score == null)
                                {
                                    continue;
                                }
                                addText("获得" + score["score"].ToString() + "爆竹");
                                getScore();
                                sleep(3);
                                
                            }
                            break;
                        case 3://邀请
                            break;
                        case 4://加购购物车
                            if (!checkBox7.Checked)
                            {
                                continue;
                            }
                            
                            addText("【执行任务】  -->  " + task.name + "【" + task.title + "】[" + task.times + "/" + task.maxTimes + "]");
                            foreach (TaskVoItemModel item in task.Items)
                            {
                                addText("加购:" + item.title.ToString() + "");
                                body = "{\"taskToken\":\"" + item.token + "\",\"taskId\":" + task.taskId + ",\"actionType\":0,\"appId\":\"" + appId + "\"," + SignService.getAppBody(this.base_cookies, this.base_secretp, "safeStr") + "}";
                                score = formatResult(BaseService.base_post("template_mongo_collectScore", this.base_cookies, body, getUA()), false);
                                if (score == null)
                                {
                                    continue;
                                }
                                addText("获得" + score["score"].ToString() + "爆竹");
                                getScore();
                                sleep(3);
                                break;
                            }
                            break;
                        default:
                            addText("【执行任务】  -->  " + task.name + "【" + task.title + "】[" + task.times + "/" + task.maxTimes + "]");
                            addText("暂不支持该任务");
                            break;
                    }
                    
                    getScore();
                }
                //jm_task_process
                //{"shopId":"694243","venderId":"698434","projectId":7609,"taskId":41315,"token":"26A061018160100026A2615387F210BC","opType":2,"functionIdFixed":"jm_task_process_play"}
                body = "{\"taskToken\":\"\",\"appId\":\"" + appId + "\",\"channelId\":1}";
                JToken shopResult = BaseService.base_post("template_mongo_getHomeData ", this.base_cookies, body, getUA());
                shopResult = formatResult(shopResult,false);
                if (shopResult == null)
                {
                    continue;
                }
                shopResult = shopResult["userInfo"];
                //userLightChance  剩余次数
                //fragmentList  列表Index
                //wholeTaskStatus   白色块数
                int userLightChance = Convert.ToInt32(shopResult["userLightChance"].ToString());
                int wholeTaskStatus = Convert.ToInt32(shopResult["wholeTaskStatus"].ToString());
                if (userLightChance == 0 || wholeTaskStatus == 0)
                {
                    continue;
                }
                addText("开始拆盒,共" + wholeTaskStatus + "个");
                sleep(2);
                for (int i = 1; i < 7; i++)
                {
                    //{"appId":"1GVRRw6s","fragmentId":3}
                    foreach (JToken fragment in shopResult["fragmentList"])
                    {
                        if(StrHelper.formatInt(fragment) == i)
                        {
                            continue;
                        }
                    }
                    if (userLightChance <= 0)
                    {
                        break;
                    }
                    addText("开始拆第" + i + "个拼图");
                    body = "{\"appId\":\"" + appId + "\",\"fragmentId\":"+ i + "}";
                    JToken res_box = formatResult(BaseService.base_post("template_mongo_lottery", this.base_cookies,body,getUA()), false);
                    if (res_box == null)
                    {
                        continue;
                    }

                    res_box = res_box["userAwardDto"];
                    
                    switch (res_box["type"].ToString())
                    {
                        case "5":
                            addText("获得爆竹：" + res_box["scoreVo"]["quantity"].ToString());
                            break;
                        case "1":
                            addText("获得获得优惠券");
                            break;
                        default:
                            addText("抽奖结果未知" + res_box.ToString());
                            break;
                    }
                    userLightChance--;
                }

            }
        }

        private string getUA()
        {
            return textBox1.Text;
        }

        private void collect_score()
        {
            string body = "{\"ss\":\"" + SignService.getAppBody(this.base_cookies,this.base_secretp) +"\"}";
            JToken res = BaseService.base_post("tigernian_collectAutoScore", this.base_cookies, body, getUA());
            if (StrHelper.checkEquals(res["code"].ToString(), "0"))
            {
                if (StrHelper.checkEquals(res["data"]["bizCode"].ToString(), "0"))
                {
                    addText("签到成功，获得 " + StrHelper.formatInt(res["data"]["result"]["produceScore"].ToString()) + " 爆竹");

                }
                else
                {
                    addText(res["data"]["bizMsg"].ToString());
                }
            }
            else
            {
                addText(res["msg"].ToString());
            }
            //MessageBox.Show(res.ToString());
            getScore();

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                IniHelper.Instance.WriteInteger("Setting", "auto_update", 1);
                getScore();
            }
            else
            {
                IniHelper.Instance.WriteInteger("Setting", "auto_update", 0);
            }
        }

        private void unlock()
        {
            string body = "{\"ss\":\"" + SignService.getAppBody(this.base_cookies, this.base_secretp) + "\"}";
            JToken res = BaseService.base_post("tigernian_raise", this.base_cookies,body ,getUA());
            String code = res["code"].ToString();
            if (StrHelper.checkEquals(code, "0"))
            {
                if (StrHelper.checkEquals(res["data"]["bizCode"].ToString(), "0"))
                {
                    addText("升级成功");
                    
                }
                else
                {
                    addText(res["data"]["bizMsg"].ToString());
                    if (res["data"]["bizCode"].ToString() == "-1002")
                    { 
                        return;
                    }
                    
                }
            }
            else
            {
                addText(res["msg"].ToString());
            }
            getScore();
        }

        private void doSign()
        {

            //首次打开领奖
            string body = "{\"channel\":\"1\"}";
            JToken res = BaseService.base_post("tigernian_getMainMsgPopUp", this.base_cookies, body, getUA());
            if (res["data"]["result"] != null)
            {
                switch (res["data"]["result"]["type"].ToString())
                {
                    case "5":
                        addText("获得鞭炮：" + res["data"]["result"]["score"].ToString());
                        break;
                    case "1":
                        addText("获得获得优惠券");
                        break;
                    default:
                        addText("未识别奖励");
                        break;
                }
            }
            //签到
            body = "{\"ss\":\"" + SignService.getAppBody(this.base_cookies, this.base_secretp) + "\"}";
            res = BaseService.base_post("tigernian_sign", this.base_cookies, body,getUA());
            String code = res["code"].ToString();
            if (StrHelper.checkEquals(code, "0"))
            {
                addText(res["data"]["bizMsg"].ToString());
            }
            else
            {
                addText(res["msg"].ToString());
            }
        }
        private JToken formatResult(JToken res, bool show_text = true)
        {
            if (res == null)
            {
                addText("系统异常");
                return null;
            }
            if (res["code"] == null)
            {
                addText(res.ToString());
                return null;
            }
            if (!StrHelper.checkEquals(res["code"].ToString(), "0"))
            {
                if (res["msg"]!=null)
                {
                    addText(res["msg"].ToString());
                }
                else
                {
                    MessageBox.Show(res.ToString());
                }
                
                return null;
            }
            if (res["data"] == null)
            {
                addText(res.ToString());
                return null;
            }
            if (!StrHelper.checkEquals(res["data"]["bizCode"].ToString(), "0"))
            {
                addText(res["data"]["bizMsg"].ToString());
                return null;
            }
            if (show_text)
            {
                if (res["data"]["result"] != null)
                {
                    addText(res["data"]["result"].ToString());
                }
                else
                {
                    addText(res["data"]["bizMsg"].ToString());
                    return null;
                }
            }
            return res["data"]["result"];
        }


        private void doAppTask()
        {
            List<TaskEntity> tasks = TaskService.getTaskDetail(this.base_cookies);
            foreach (TaskEntity task in tasks)
            {
                if (task.times >= task.maxTimes)
                {
                    continue;
                }
                addText("【执行任务】  -->  " + task.name + "【" + task.title + "】[" + task.times + "/" + task.maxTimes + "]");
                switch (task.type)
                {
                    case 4://入会
                        if (!checkBox6.Checked)
                        {
                            addText("未开启入会任务，即将跳过");
                            continue;
                        }
                        foreach (TaskVoItemModel item in task.Items)
                        {
                            if (!StrHelper.checkEquals(item.status.ToString(), "1"))
                            {
                                continue;
                            }

                            addText("              -->  " + item.title);
                            int channel_index = item.url.IndexOf("channel=");
                            string channel = "";
                            if (channel_index >= 0)
                            {
                                channel = item.url.Substring(channel_index).Replace("channel=", "");
                                if (channel.IndexOf("&") >= 0)
                                {
                                    channel = channel.Substring(0, channel.IndexOf("&") + 1);
                                }
                            }

                            String body = "{\"venderId\": \"" + item.vendor + "\",\"shopId\": \"" + item.shopId + "\",\"bindByVerifyCodeFlag\": 1,\"registerExtend\": {},\"writeChildFlag\": 0,\"channel\": \"" + channel + "\"}";
                            string url = "https://api.m.jd.com/client.action?appid=jd_shop_member&functionId=bindWithVender&body=" + HttpUtility.UrlEncode(body) + "&client=H5&clientVersion=9.2.0&uuid=88888";
                            JToken res = TaskService.joinMember(this.base_cookies, url, item.url, getUA());
                            addText(res.ToString());
                            Console.WriteLine("========================");
                            Console.WriteLine(channel);
                            Console.WriteLine("========================");
                            string sign2 = SignService.getAppBody(this.base_cookies, this.base_secretp);
                            body = "{\"taskId\":\"" + task.taskId + "\",\"taskToken\": \"" + item.token + "\", \"ss\": \"" + sign2 + "\", \"shopSign\": \"\", \"actionType\":1, \"showErrorToast\": false }";
                            JToken taskToken = formatResult(BaseService.base_post("tigernian_collectScore", this.base_cookies, body, getUA()), false);
                            if (taskToken != null)
                            {
                                addText("任务完成,获得" + taskToken["score"] + "爆竹");

                            }
                            getScore();
                        }
                        break;
                    case 1://浏览6秒
                        foreach (TaskVoItemModel item in task.Items)
                        {
                            if (!StrHelper.checkEquals(item.status.ToString(), "1"))
                            {
                                continue;
                            }

                            string sign2 = SignService.getAppBody(this.base_cookies, this.base_secretp);
                            String body = "{\"taskId\":\"" + task.taskId + "\",\"taskToken\": \"" + item.token + "\", \"ss\": \"" + sign2 + "\", \"shopSign\": \"\", \"actionType\": 1, \"showErrorToast\": false }";
                            JToken taskToken = formatResult(BaseService.base_post("tigernian_collectScore", this.base_cookies, body, getUA()), false);
                            if (taskToken == null)
                            {
                                continue;
                            }
                            addText("正在执行:" + item.title.ToString() + "");
                            sleep(6);
                            JToken call = BaseService.base_call(this.base_cookies, taskToken["taskToken"].ToString());
                            addText(call["toast"]["subTitle"].ToString());
                            getScore();
                        }
                        break;
                    case 3://进入店铺
                        foreach (TaskVoItemModel item in task.Items)
                        {
                            if (!StrHelper.checkEquals(item.status.ToString(), "1"))
                            {
                                continue;
                            }
                            addText("正在执行:" + item.title.ToString() + "");
                            string sign2 = SignService.getAppBody(this.base_cookies, this.base_secretp);
                            String body = "{\"taskId\":\"" + task.taskId + "\",\"taskToken\": \"" + item.token + "\", \"ss\": \"" + sign2 + "\", \"shopSign\": \"\", \"actionType\": 1, \"showErrorToast\": false }";
                            JToken taskToken = formatResult(BaseService.base_post("tigernian_collectScore", this.base_cookies, body, getUA()), false);
                            sleep(2);
                            if (taskToken != null)
                            {
                                addText("任务完成,获得" + taskToken["score"] + "爆竹");

                            }
                            getScore();
                        }
                        break;
                    case 2://加购购物车
                        if (!checkBox7.Checked)
                        {
                            addText("未开启加购任务，即将跳过");
                            continue;
                        }

                        List<TaskEntity> cars = TaskService.getTaskDetail(this.base_cookies, "2");
                        while (cars.Count > 0)
                        {
                            sleep(2);
                            TaskEntity new_task = cars[0];
                            if (new_task.status == "2")
                            {
                                break;
                            }
                            JToken feedDetailInfo = formatResult(BaseService.base_post("tigernian_getFeedDetail", this.base_cookies, "{\"taskId\":\"" + new_task.taskId + "\"}", getUA()), false);
                            if (feedDetailInfo == null)
                            {
                                continue; ;
                            }
                            feedDetailInfo = feedDetailInfo["addProductVos"][0];

                            for (int i = Convert.ToInt32(feedDetailInfo["times"].ToString()); i < Convert.ToInt32(feedDetailInfo["maxTimes"].ToString()); i++)
                            {
                                JToken productInfoVos = feedDetailInfo["productInfoVos"][i];
                                addText("加购：" + productInfoVos["skuName"].ToString());
                                string sign2 = SignService.getAppBody(this.base_cookies, this.base_secretp);
                                String body = "{\"taskId\": \"" + new_task.taskId + "\",\"taskToken\" : \"" + productInfoVos["taskToken"].ToString() + "\",\"ss\":\"" + sign2 + "\"}";
                                JToken taskToken = formatResult(BaseService.base_post("tigernian_collectScore", this.base_cookies, body, getUA()), false);
                                if (taskToken == null)
                                {
                                    break;
                                }
                                int score = Convert.ToInt32(taskToken["acquiredScore"].ToString());
                                if (score > 0)
                                {

                                    addText("加购成功,获得" + score + "爆竹");
                                    cars = TaskService.getTaskDetail(this.base_cookies, "2");
                                    break;
                                }
                            }
                            getScore();
                        }
                        break;
                    case 5://种草
                        JToken cao = formatResult(BaseService.base_post("tigernian_getFeedDetail", this.base_cookies, "{\"taskId\":\"" + task.taskId + "\"}", getUA()), false);
                        if (cao == null)
                        {
                            continue; ;
                        }
                        cao = cao["taskVos"][0];

                        for (int i = Convert.ToInt32(cao["times"].ToString()); i < Convert.ToInt32(cao["maxTimes"].ToString()); i++)
                        {
                            JToken productInfoVos = cao["browseShopVo"][i];
                            addText("种草：" + productInfoVos["shopName"].ToString());
                            string sign2 = SignService.getAppBody(this.base_cookies, this.base_secretp);
                            String body = "{\"taskId\": \"" + task.taskId + "\",\"taskToken\" : \"" + productInfoVos["taskToken"].ToString() + "\",\"ss\":\"" + sign2 + "\"}";
                            JToken taskToken = formatResult(BaseService.base_post("tigernian_collectScore", this.base_cookies, body, getUA()), false);
                            if (taskToken == null)
                            {
                                break;
                            }
                            int score = Convert.ToInt32(taskToken["acquiredScore"].ToString());
                            addText("种草成功,获得" + score + "爆竹");
                        }
                        break;
                    case 6://品牌墙

                        List<TaskEntity> task6 = TaskService.getTaskDetail(this.base_cookies, "0-5");

                        while (task6.Count > 0)
                        {
                            TaskEntity new_task = task6[0];

                            if (!StrHelper.checkEquals(new_task.status.ToString(), "1"))
                            {
                                break;
                            }

                            addText("正在执行:" + new_task.title.ToString() + "");
                            string sign2 = SignService.getAppBody(this.base_cookies, this.base_secretp);
                            String body = "{\"taskId\":\"" + new_task.taskId + "\",\"taskToken\": \"" + new_task.Items[0].token + "\", \"ss\": \"" + sign2 + "\", \"shopSign\": \"\", \"actionType\": 1, \"showErrorToast\": false }";
                            JToken taskToken = formatResult(BaseService.base_post("tigernian_collectScore", this.base_cookies, body, getUA()), false);
                            sleep(2);
                            if (taskToken != null)
                            {
                                addText("任务完成,获得" + taskToken["score"] + "爆竹");
                                task6 = TaskService.getTaskDetail(this.base_cookies, "0-5");
                            }
                            getScore();
                        }
                        break;
                    case 7:
                        if (task.Items.Count > 0)
                        {
                            if (!StrHelper.checkEquals(task.status, "3") && !StrHelper.checkEquals(task.status, "1"))
                            {
                                break;
                            }

                            addText("正在执行:" + task.title.ToString() + "");
                            string sign2 = SignService.getAppBody(this.base_cookies, this.base_secretp);
                            String body = "{\"taskId\":\"" + task.taskId + "\",\"taskToken\": \"" + task.Items[0].token + "\", \"ss\": \"" + sign2 + "\", \"shopSign\": \"\", \"actionType\": 1, \"showErrorToast\": false }";
                            JToken taskToken = formatResult(BaseService.base_post("tigernian_collectScore", this.base_cookies, body, getUA()), false);

                            if (taskToken != null)
                            {
                                sleep(2);
                                addText("任务完成,获得" + taskToken["score"] + "爆竹");
                            }
                            else
                            {
                                addText("可能是还未加入队伍");
                            }
                            getScore();
                        }
                        break;

                    default:
                        addText("暂不支持该任务");
                        break;
                }
                getScore();
            }
            List<TaskReward> rewards = TaskService.getTaskRewards(this.base_cookies);
            foreach (TaskReward reward in rewards)
            {
                if (reward.status != 3)
                {
                    continue;
                }
                string sign2 = SignService.getAppBody(this.base_cookies, this.base_secretp);
                String body = "{\"awardToken\":\"" + reward.awardToken + "\"}";
                JToken taskToken = formatResult(BaseService.base_post("tigernian_getBadgeAward", this.base_cookies, body, getUA()), false);
                sleep(2);
                if (taskToken != null)
                {
                    addText("任务完成,获得爆竹");

                }
                getScore();
            }

            tasks = TaskService.getTaskDetail(this.base_cookies, "3-26-9-7-5-0");
            if(tasks != null && tasks.Count >0)
            {
                addText("发现还有任务，重做一次");
                doAppTask();
            }
        }

        private void doWxTask()
        {
            List<TaskEntity> tasks = TaskService.getTaskDetail(this.base_cookies,"-1",true);
            foreach (TaskEntity task in tasks)
            {
                if (task.status != "1")
                {
                    continue;
                }
                addText("【执行任务】  -->  " + task.name + "【" + task.title + "】[" + task.times + "/" + task.maxTimes + "]");
                switch (task.type)
                {
                    case 4://入会
                        if (!checkBox6.Checked)
                        {
                            addText("未开启入会任务，即将跳过");
                            continue;
                        }
                        foreach (TaskVoItemModel item in task.Items)
                        {
                            addText("              -->  " + item.title);
                            if (!StrHelper.checkEquals(item.status.ToString(), "1"))
                            {
                                continue;
                            }

                            int channel_index = item.url.IndexOf("channel=");
                            string channel = "";
                            if (channel_index >= 0)
                            {
                                channel = item.url.Substring(channel_index).Replace("channel=", "");
                                if (channel.IndexOf("&") >= 0)
                                {
                                    channel = channel.Substring(0, channel.IndexOf("&") + 1);
                                }
                            }

                            String body = "{\"venderId\": \"" + item.vendor + "\",\"shopId\": \"" + item.shopId + "\",\"bindByVerifyCodeFlag\": 1,\"registerExtend\": {},\"writeChildFlag\": 0,\"channel\": \"" + channel + "\"}";
                            string url = "https://api.m.jd.com/client.action?appid=jd_shop_member&functionId=bindWithVender&body=" + HttpUtility.UrlEncode(body) + "&client=H5&clientVersion=9.2.0&uuid=88888";
                            JToken res = TaskService.joinMember(this.base_cookies, url, item.url, getUA());
                            addText(res.ToString());
                            Console.WriteLine("========================");
                            Console.WriteLine(channel);
                            Console.WriteLine("========================");
                            string sign2 = SignService.getAppBody(this.base_cookies, this.base_secretp);
                            body = "{\"taskId\":\"" + task.taskId + "\",\"taskToken\": \"" + item.token + "\", \"ss\": \"" + sign2 + "\", \"shopSign\": \"\", \"actionType\":1, \"showErrorToast\": false }";
                            JToken taskToken = formatResult(BaseService.base_post("tigernian_collectScore", this.base_cookies, body, getUA()), false);
                            if (taskToken != null)
                            {
                                addText("任务完成,获得" + taskToken["score"] + "爆竹");

                            }
                            getScore();
                        }
                        break;
                    case 1://浏览6秒
                        foreach (TaskVoItemModel item in task.Items)
                        {
                            if (!StrHelper.checkEquals(item.status.ToString(), "1"))
                            {
                                continue;
                            }

                            string sign2 = SignService.getAppBody(this.base_cookies, this.base_secretp);
                            String body = "{\"taskId\":\"" + task.taskId + "\",\"taskToken\": \"" + item.token + "\", \"ss\": \"" + sign2 + "\", \"shopSign\": \"\", \"actionType\": 1, \"showErrorToast\": false }";
                            JToken taskToken = formatResult(BaseService.base_post("tigernian_collectScore", this.base_cookies, body, getUA()), false);
                            if (taskToken == null)
                            {
                                continue;
                            }
                            addText("正在执行:" + item.title.ToString() + "");
                            sleep(6);
                            JToken call = BaseService.base_call(this.base_cookies, taskToken["taskToken"].ToString());
                            addText(call["toast"]["subTitle"].ToString());
                            getScore();
                        }
                        break;
                    case 3://进入店铺
                        foreach (TaskVoItemModel item in task.Items)
                        {
                            if (!StrHelper.checkEquals(item.status.ToString(), "1"))
                            {
                                continue;
                            }

                            addText("正在执行:" + item.title.ToString() + "");
                            string sign2 = SignService.getAppBody(this.base_cookies, this.base_secretp);
                            String body = "{\"taskId\":\"" + task.taskId + "\",\"taskToken\": \"" + item.token + "\", \"ss\": \"" + sign2 + "\", \"shopSign\": \"\", \"actionType\": 1, \"showErrorToast\": false }";
                            JToken taskToken = formatResult(BaseService.base_post("tigernian_collectScore", this.base_cookies, body, getUA()), false);
                            sleep(2);
                            if (taskToken != null)
                            {
                                addText("任务完成,获得" + taskToken["score"] + "爆竹");

                            }
                            getScore();
                        }
                        break;
                    case 2://加购购物车
                        if (!checkBox7.Checked)
                        {
                            addText("未开启加购任务，即将跳过");
                            continue;
                        }
                        List<TaskEntity> cars = TaskService.getTaskDetail(this.base_cookies, "2",true);
                        while (cars.Count > 0)
                        {
                            sleep(2);
                            TaskEntity new_task = cars[0];
                            if (new_task.status == "2")
                            {
                                break;
                            }

                            JToken feedDetailInfo = formatResult(BaseService.base_post("tigernian_getFeedDetail", this.base_cookies, "{\"taskId\":\"" + new_task.taskId + "\"}", getUA()), false);
                            if (feedDetailInfo == null)
                            {
                                continue; ;
                            }
                            feedDetailInfo = feedDetailInfo["addProductVos"][0];

                            for (int i = Convert.ToInt32(feedDetailInfo["times"].ToString()); i < Convert.ToInt32(feedDetailInfo["maxTimes"].ToString()); i++)
                            {
                                JToken productInfoVos = feedDetailInfo["productInfoVos"][i];
                                addText("加购：" + productInfoVos["skuName"].ToString());
                                string sign2 = SignService.getAppBody(this.base_cookies, this.base_secretp);
                                String body = "{\"taskId\": \"" + new_task.taskId + "\",\"taskToken\" : \"" + productInfoVos["taskToken"].ToString() + "\",\"ss\":\"" + sign2 + "\"}";
                                JToken taskToken = formatResult(BaseService.base_post("tigernian_collectScore", this.base_cookies, body, getUA()), false);
                                if (taskToken == null)
                                {
                                    break;
                                }
                                int score = Convert.ToInt32(taskToken["acquiredScore"].ToString());
                                if (score > 0)
                                {

                                    addText("加购成功,获得" + score + "爆竹");
                                    cars = TaskService.getTaskDetail(this.base_cookies, "2", true);
                                    break;
                                }
                            }
                            getScore();
                        }
                        break;
                    case 5://种草
                        JToken cao = formatResult(BaseService.base_post("tigernian_getFeedDetail", this.base_cookies, "{\"taskId\":\"" + task.taskId + "\"}", getUA()), false);
                        if (cao == null)
                        {
                            continue; ;
                        }

                        cao = cao["taskVos"][0];

                        for (int i = Convert.ToInt32(cao["times"].ToString()); i < Convert.ToInt32(cao["maxTimes"].ToString()); i++)
                        {
                            JToken productInfoVos = cao["browseShopVo"][i];
                            addText("种草：" + productInfoVos["shopName"].ToString());
                            string sign2 = SignService.getAppBody(this.base_cookies, this.base_secretp);
                            String body = "{\"taskId\": \"" + task.taskId + "\",\"taskToken\" : \"" + productInfoVos["taskToken"].ToString() + "\",\"ss\":\"" + sign2 + "\"}";
                            JToken taskToken = formatResult(BaseService.base_post("tigernian_collectScore", this.base_cookies, body, getUA()), false);
                            if (taskToken == null)
                            {
                                break;
                            }
                            int score = Convert.ToInt32(taskToken["acquiredScore"].ToString());
                            addText("种草成功,获得" + score + "爆竹");
                        }
                        break;
                    case 6://品牌墙

                        List<TaskEntity> task6 = TaskService.getTaskDetail(this.base_cookies, "0-5");

                        while (task6.Count > 0)
                        {
                            TaskEntity new_task = task6[0];

                            if (!StrHelper.checkEquals(new_task.status.ToString(), "1"))
                            {
                                break;
                            }

                            addText("正在执行:" + new_task.title.ToString() + "");
                            string sign2 = SignService.getAppBody(this.base_cookies, this.base_secretp);
                            String body = "{\"taskId\":\"" + new_task.taskId + "\",\"taskToken\": \"" + new_task.Items[0].token + "\", \"ss\": \"" + sign2 + "\", \"shopSign\": \"\", \"actionType\": 1, \"showErrorToast\": false }";
                            JToken taskToken = formatResult(BaseService.base_post("tigernian_collectScore", this.base_cookies, body, getUA()), false);
                            sleep(2);
                            if (taskToken != null)
                            {
                                addText("任务完成,获得" + taskToken["score"] + "爆竹");
                                task6 = TaskService.getTaskDetail(this.base_cookies, "0-5");
                            }
                            getScore();
                        }
                        break;
                    case 7:
                        if (task.Items.Count > 0)
                        {
                            if (!StrHelper.checkEquals(task.status, "3") && !StrHelper.checkEquals(task.status, "1"))
                            {
                                break;
                            }

                            addText("正在执行:" + task.title.ToString() + "");
                            string sign2 = SignService.getAppBody(this.base_cookies, this.base_secretp);
                            String body = "{\"taskId\":\"" + task.taskId + "\",\"taskToken\": \"" + task.Items[0].token + "\", \"ss\": \"" + sign2 + "\", \"shopSign\": \"\", \"actionType\": 1, \"showErrorToast\": false }";
                            JToken taskToken = formatResult(BaseService.base_post("tigernian_collectScore", this.base_cookies, body, getUA()), false);

                            if (taskToken != null)
                            {
                                sleep(2);
                                addText("任务完成,获得" + taskToken["score"] + "爆竹");
                            }
                            else
                            {
                                addText("可能是还未加入队伍");
                            }
                            getScore();
                        }
                        break;
                    default:
                        addText("暂不支持该任务");
                        break;
                }
                getScore();
            }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            IniHelper.Instance.WriteString("Setting", "ua", textBox1.Text);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
            IniHelper.Instance.WriteString("Boom", "url", textBox2.Text);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            IniHelper.Instance.WriteInteger("Boom", "index", comboBox1.SelectedIndex);
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            randomUA();
        }

        private void randomUA()
        {
            //jdapp;android;10.2.2;10;4336165326366353-7383232683336626;
            //model/PBEM00;addressid/0;aid/4ca5bc65782b83fb;oaid/7A226D623EB44FDFB9A814D8CC5D635Ca495743b50140372aa6273c4bb92e899;osVer/29;appBuild/91077;partner/oppo;eufv/1;jdSupportDarkMode/0;Mozilla/5.0 (Linux; Android 10; PBEM00 Build/QKQ1.190918.001; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/77.0.3865.120 MQQBrowser/6.2 TBS/045713 Mobile Safari/537.36
            string str = "jdapp;android;10.2.2;11;" + StrHelper.GetRandomString(16) + "-" + StrHelper.GetRandomString(16)
                + ";model/xiaomi" + ";"
                + ";network/wifi;" + StrHelper.GetRandomString(4, false, false, true) + "/"
                + StrHelper.GetRandomString(8, true, false, true) + "-"
                + StrHelper.GetRandomString(4, true, false, true) + "-"
                + StrHelper.GetRandomString(4, true, false, true) + "-"
                + StrHelper.GetRandomString(4, true, false, true) + "-"
                + StrHelper.GetRandomString(12, true, false, true) + ";"
                + "addressid/" + StrHelper.GetRandomString(10) + ";"
                + "appBuild/91077;"
                + "jdSupportDarkMode/0;Mozilla/5.0 (Linux; Android 10; PBEM00 Build/QKQ1.190918.001; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/77.0.3865.120 MQQBrowser/6.2 TBS/045713 Mobile Safari/537.36";

            this.textBox1.Text = str;
        }

        private void 关于本程序ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Frm.FrmAbout frm  = new Frm.FrmAbout();
            frm.ShowDialog();
        }

        private void wskey转Cookie工具ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Frm.Plugin.FrmPluginWskey frm = new Frm.Plugin.FrmPluginWskey();
            frm.ShowDialog();
        }

        private void 扫码登录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("测试中，暂未启用");
        }
    }
}
