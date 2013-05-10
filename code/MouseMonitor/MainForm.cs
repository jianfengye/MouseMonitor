using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace MouseMonitor
{
    public partial class MainForm : Form
    {
        private int hMouseHook;
        private MouseMonitor mouseMonitor;

        private MouseState state;
        private Thread saveThread;

        public MainForm()
        {
            InitializeComponent();

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
    }
}
