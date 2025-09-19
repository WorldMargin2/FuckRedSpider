using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Process = System.Diagnostics.Process;

namespace FuckRedSpider {

    public partial class Form1 : Form {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        //===========================================================
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsIconic(IntPtr hWnd);
        //===========================================================
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        //===========================================================
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        //===========================================================
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        //========================GetWindowThreadProcessId===================================
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);


        //===========================================================
        private const UInt32 WM_CLOSE = 0x0010;
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);


        private const string default_RMPN = "REDAgent";
        private string R_MainProcessName = default_RMPN;
        private const string default_RFWCN = "DIBFullViewClass";
        private string R_full_window_class_name = default_RFWCN;
        private const string default_RNWCN = "RedEagle.Monitor";
        private string R_normal_window_class_name = default_RNWCN;

        private readonly string this_process_name = Process.GetCurrentProcess().ProcessName;
        //======================日志相关==============================

        public struct Log {
            TextBox textbox_log;
            public Log(TextBox textBox) {
                this.textbox_log = textBox;
            }
            public void clear() {
                //清空日志
                textbox_log.Text = "";
            }
            public void add(string s) {
                //添加日志
                textbox_log.Text += DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + s;
                textbox_log.Text += Environment.NewLine;
            }
        }
        private Log log;
        //===========================================================
        public Form1(string[] args) {
            InitializeComponent();
            listener_timer.Start();
            this.bindTip();
            log = new Log(this.textbox_log);
            log.add("程序启动");
        }

        private void bindTip() {
            //绑定提示
            //状态监听窗口
            toolTip1.SetToolTip(this.label_running_stat, "当前是否检测到目标进程");
            toolTip1.SetToolTip(this.label_process_pid, "目标进程的PID(可能有多个)");
            //日志窗口
            toolTip1.SetToolTip(this.textbox_log, "日志记录");
            //设置窗口
            //  page1
            toolTip1.SetToolTip(this.topest_with_timer, "每次检测时都置顶窗口");
            toolTip1.SetToolTip(this.auto_kill, "检测到进程时自动结束进程");
            toolTip1.SetToolTip(this.auto_hide, "检测到进程时自动隐藏窗口");
            //  page2
            toolTip1.SetToolTip(this.process_name, "要监控的进程名，默认REDAgent");
            toolTip1.SetToolTip(this.label2, "双击重置");
            toolTip1.SetToolTip(this.full_window_class, "全屏控屏窗口类名，默认DIBFullViewClass");
            toolTip1.SetToolTip(this.f_w_c_l, "双击重置");
            toolTip1.SetToolTip(this.normal_window_class, "普通控屏窗口类名，默认RedEagle.Monitor");
            toolTip1.SetToolTip(this.n_w_c_l, "双击重置");
            //关于按钮
            toolTip1.SetToolTip(this.button1, "关于作者");

        }

        private void closeHandle(IntPtr h) {
            SetWindowPos(h, HWND_NOTOPMOST, 0, 0, 0, 0, 0x0001 | 0x0002); //找到窗口并置于底层

            if (!IsIconic(h)) {
                ShowWindow(h, 2); //找到窗口并最小化
                ShowWindow(h, 0); //找到窗口并隐藏
            }
            //HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE SWP_NOSIZE
            SendMessage(h, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);   //找到窗口并发送关闭消息
        }

        //===============获取十六进制handle（预留3个重载）================
        private string getHex(int i) {
            return "0x" + i.ToString("X");
        }

        private string getHex(IntPtr i) {
            return "0x" + i.ToString("X");
        }

        private string getHex(string s) {
            return "0x" + Convert.ToInt32(s).ToString("X");
        }
        //===========================================================
        int getProcessIdByHandle(IntPtr h) {
            int pid;
            GetWindowThreadProcessId(h, out pid);
            return pid;
        }


        
        //====================后台监听===============================

        private void Listen(object sender, EventArgs e) {

            if (topest_with_timer.Checked) {
                this.TopMost = true;//防止被覆盖
            }

            Process[] p = Process.GetProcessesByName(this.R_MainProcessName);
            if (p.Length > 0) {
                //信息显示
                label_running_stat.Text = "是";
                label_running_stat.ForeColor = Color.Green;

                bool equals = true;
                List<string> lines = new List<string> { };

                if (p.Count() != label_process_pid.Lines.Count()) {
                    equals = false;//数量不等，结果不等
                }
                foreach (var _ in p) {
                    string s = _.Id.ToString();
                    lines.Add(s);
                    if (equals && (!label_process_pid.Lines.Contains(s))) {    //equals在&&前面，防止多余的比较；s不在Lines中时代表结果不相等
                        equals = false;
                    }
                }

                if (!equals) {
                    label_process_pid.Lines = lines.ToArray();//更新PID显示
                }

                //操作
                if (auto_kill.Checked) {
                    foreach (var _ in p) {
                        _.Kill();   //杀死目标进程
                    }
                    return;
                }
                if (auto_hide.Checked) {
                    //DIBFullViewClass


                    IntPtr h;
                    try {
                        h = FindWindow(this.R_full_window_class_name, null);
                        if (h != IntPtr.Zero && lines.Contains( getProcessIdByHandle(h).ToString()) ) {
                            log.add("尝试隐藏全屏控屏窗口: " + getHex(h));
                            closeHandle(h);
                            return;
                        }
                        //普通控屏窗口
                        h = FindWindow(this.R_normal_window_class_name, null);
                        if (h != IntPtr.Zero && lines.Contains(getProcessIdByHandle(h).ToString()) ) {
                            log.add("尝试隐藏普通控屏窗口: " + getHex(h));
                            closeHandle(h);
                            return;
                        }
                    } catch {
                    }

                    foreach (var process in p) {
                        if (!IsIconic(process.MainWindowHandle)) {
                            closeHandle(process.MainWindowHandle);
                        }
                    }
                }
            } else {
                //无进程信息，更改显示
                label_running_stat.Text = "否";
                label_running_stat.ForeColor = Color.Red;
                label_process_pid.Text = "None";
            }
        }
        //======================对目标采取的操作==============================
        private void auto_kill_CheckedChanged(object sender, EventArgs e) {
            //逻辑上关闭和隐藏不需要同时发生
            if (auto_kill.Checked) {
                auto_hide.Checked = false;
            }
        }

        private void auto_hide_CheckedChanged(object sender, EventArgs e) {
            if (auto_hide.Checked) {
                auto_kill.Checked = false;
            }
        }


        //===================更改目标进程名字以适应目标名称变化===================
        private void process_name_TextChanged(object sender, EventArgs e) {
            this.R_MainProcessName = process_name.Text.Trim();//更改目标进程名称
            if (this.R_MainProcessName == this.this_process_name) {
                //防止误杀自己
                auto_hide.Checked = false;
                auto_kill.Checked = false;
                auto_hide.Enabled = false;
                auto_kill.Enabled = false;
                name_conflict_err.Visible = true;
            } else {
                name_conflict_err.Visible = false;
                auto_hide.Enabled = true;
                auto_kill.Enabled = true;
            }
        }

        private void label2_DoubleClick(object sender, EventArgs e) {
            process_name.Text = default_RMPN;//重置名称
            process_name_TextChanged(sender, e);
        }
        private void full_window_class_TextChanged(object sender, EventArgs e) {
            this.R_full_window_class_name = full_window_class.Text.Trim();
        }

        private void f_w_c_l_DoubleClick(object sender, EventArgs e) {
            this.full_window_class.Text = default_RFWCN;//重置名称
            full_window_class_TextChanged(sender, e);
        }

        
        private void normal_window_class_TextChanged(object sender, EventArgs e) {
            this.R_normal_window_class_name = normal_window_class.Text.Trim();
        }
        private void n_w_c_l_DoubleClick(object sender, EventArgs e) {
            this.normal_window_class.Text = default_RNWCN;//重置名称
            normal_window_class_TextChanged(sender, e);
        }
        //================================================================

        private void button1_Click(object sender, EventArgs e) {
            //打开“关于”界面
            new FuckRedSpider.Form2().ShowDialog();
            System.GC.Collect();
        }


    }
}
