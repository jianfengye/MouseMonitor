using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.IO;

namespace MouseMonitor
{
    public partial class MainForm : Form
    {
        private int hMouseHook;
        private MouseMonitor mouseMonitor;

        private MouseState state;
        private Thread saveThread;
        private const string CurrentVersion = "1.0.1";

        public MainForm()
        {
            InitializeComponent();

            // 创建文件夹  System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\mouseMonitor\\
            string logFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\mouseMonitor";
            if (!System.IO.Directory.Exists(logFolder))
            {
                System.IO.Directory.CreateDirectory(logFolder);
            }

            // 启动监控服务
            mouseMonitor = new MouseMonitor();
            state = new MouseState();


            this.formStartHook();

            // 启动另外一个记录线程，每分钟记录一次
            this.saveThread = new Thread(new ThreadStart(BeginSave));
            this.saveThread.IsBackground = true;
            this.saveThread.Start();
            
            // 需要读取今天的点击次数
            this.state.loadClick(DateTime.Today);


            // 获取最新版本信息
            string url = "http://mousemonitor.funaio.com/upgrade/info";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream receiveStream = response.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            byte[] data = new byte[100];
            receiveStream.Read(data, 0, 100);
            receiveStream.Close();

            string newestVersion = Encoding.UTF8.GetString(data);
            newestVersion = newestVersion.Trim('\0');
            if (newestVersion != CurrentVersion)
            {
                string message = "请到 http://mousemonitor.funaio.com/ 下载最新版本" + newestVersion;
                MessageBox.Show(message);
            }
        }

        private void formStartHook()
        {
            this.hMouseHook = mouseMonitor.MouseHookStart(onMouseProc);
            // Action的Start置灰，Action的Stop开启
            this.stopToolStripMenuItem.Enabled = true;
            this.startToolStripMenuItem.Enabled = false;
            this.label_status.Text = "统计中...";
        }

        private void formStopHook()
        {
            if (this.hMouseHook != 0)
            {
                WinApi.UnhookWindowsHookEx(this.hMouseHook);
                this.state.saveAction(DateTime.Today);

                // 页面变化
                this.startToolStripMenuItem.Enabled = true;
                this.stopToolStripMenuItem.Enabled = false;
                this.label_status.Text = "暂停...";
            }
        }

        public int onMouseProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            this.state.recordAction(wParam.ToInt32());
            this.leftClick.Text = this.state.leftClickCount.ToString();
            this.rightClick.Text = this.state.rightClickCount.ToString();

            return WinApi.CallNextHookEx(this.hMouseHook, nCode, wParam, lParam);
        }


        // 每分钟记录一次
        private void BeginSave()
        {
            while (true)
            {
                Thread.Sleep(20 * 1000);

                this.state.saveAction(DateTime.Today);
            }
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.saveThread.Suspend();
            this.formStopHook();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.saveThread.Resume();
            this.formStartHook();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                this.Hide();
                this.notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
            this.notifyIcon1.Visible = false;
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string text;
            text = "主页：http://mousemonitor.funaio.com/ \n";
            text += "作者：jianfengye110@gmail.com";
            MessageBox.Show(text);
        }

        private void showToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.state.saveAction(DateTime.Today);
            ShowForm showForm = new ShowForm();
            showForm.Visible = true;
        }
    }
}
