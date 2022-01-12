using JDog202201.Utils;
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
    public partial class FrmLoginByInput : Form
    {
        public FrmLoginByInput()
        {
            InitializeComponent();
        }

        private void FrmLoginByInput_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (StrHelper.checkEquals(textBox1.Text))
            {
                MessageBox.Show("请输入Cookie");
                return;
            }
            string[] str = textBox1.Text.Split(';');
            string cookies = "";
            foreach (string key in str)
            {
                string[] str1 = key.Split('=');
                if (str1[0].IndexOf("pt_key") >= 0 || str1[0].IndexOf("pt_pin") >= 0 || str1[0].IndexOf("shshshfpb") >= 0)
                {
                    cookies += key + ";";
                }

            }
            this.user = this.textBox2.Text;
            this.cookie = cookies;
            this.status = true;
            this.Close();
        }

        public bool status = false;
        public string cookie = "";
        public string user = "";

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = "https://docs.qq.com/doc/DR1hsTFF2WGdsUE5M";
            System.Diagnostics.Process.Start(url);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
