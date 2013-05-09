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

            this.hMouseHook = mouseMonitor.MouseHookStart(onMouseProc);
            // Action的Start置灰，Action的Stop开启
            this.stopToolStripMenuItem.Enabled = true;
            this.startToolStripMenuItem.Enabled = false;


            // 启动另外一个记录线程，每分钟记录一次
            this.saveThread = new Thread(new ThreadStart(BeginSave));
            this.saveThread.IsBackground = true;
            this.saveThread.Start();
        }

        public int onMouseProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            this.state.recordAction(wParam.ToInt32());

            return WinApi.CallNextHookEx(this.hMouseHook, nCode, wParam, lParam);
        }

        // 每分钟记录一次
        private void BeginSave()
        {
            while (true)
            {
                Thread.Sleep(60 * 1000);

                this.state.saveAction(DateTime.Today);
            }
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.hMouseHook != 0)
            {
                WinApi.UnhookWindowsHookEx(this.hMouseHook);
                this.BeginSave();
                this.saveThread.Suspend();

                // 页面变化
                this.startToolStripMenuItem.Enabled = true;
                this.stopToolStripMenuItem.Enabled = false;
            }
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.hMouseHook = mouseMonitor.MouseHookStart(onMouseProc);
            this.saveThread.Resume();

            // Action的Start置灰，Action的Stop开启
            this.stopToolStripMenuItem.Enabled = false;
        }
    }
}
