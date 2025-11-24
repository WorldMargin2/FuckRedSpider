using KeyboardHookGuard;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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

        //========================setParent================================================
        [DllImport("user32.dll ", EntryPoint = "SetParent")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, long dwNewLong);


        [DllImport("user32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
        public static extern long GetWindowLong(IntPtr hWnd, int nIndex);

        public static void _SetParent(IntPtr child, IntPtr hWndNewParent) {
            ShowWindow(child, 0);                 //先将窗体隐藏，防止出现闪烁
            SetParent(child, hWndNewParent);      //将第三方窗体嵌入父容器      
            ShowWindow(child, 3);                 //让第三方窗体在容器中最大化显示
            RemoveWindowTitle(child);             // 去除窗体标题
        }


        /// <summary>
        /// 去除窗体标题
        /// </summary>
        /// <param name="vHandle">窗口句柄</param>
        public static void RemoveWindowTitle(IntPtr vHandle) {
            long style = GetWindowLong(vHandle, -16);
            style &= ~0x00C00000;
            SetWindowLong(vHandle, -16, style);

        }


        //===========================================================
        private const UInt32 WM_CLOSE = 0x0010;
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);


        private const string default_RMPN = "REDAgent";
        private const string default_RFWCN = "DIBFullViewClass";
        private const string default_RNWCN = "RedEagle.Monitor";

        private readonly string this_process_name = Process.GetCurrentProcess().ProcessName;

        private readonly GlobalKeyboardHookGuard _keyboardGuard;
        private bool keyboardGuardRunning = false;

        void KeyboardGuard_GuardStarted(object sender, EventArgs e) {
            log.add("键盘守护启动");
            keyboardGuardRunning = true;
        }
        void KeyboardGuard_GuardStopped(object sender, EventArgs e) {
            log.add("键盘守护停止");
            keyboardGuardRunning = false;
        }
        void KeyboardGuard_ErrorOccurred(object sender, ErrorEventArgs e) {
            log.add("键盘守护出错: " + e.GetException().Message);
        }
        //======================日志相关==============================

        public struct Log {
            TextBox textbox_log;
            int ignore_times;
            public Log(TextBox textBox, int ignore_times = 0) {
                this.textbox_log = textBox;
                this.ignore_times = 0;
            }
            public void clear() {
                //清空日志
                textbox_log.Text = "";
            }
            public void add(string s) {
                //添加日志
                if (ignore_times > 0) {
                    ignore_times--;
                    return;
                } else if (ignore_times < 0) {
                    return;
                }
                textbox_log.Text += DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + s;
                textbox_log.Text += Environment.NewLine;
            }
            public void ignore(int i_t = 1) {
                ignore_times = i_t;
            }
            public void unignore() {
                ignore_times = 0;
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
            _keyboardGuard = new GlobalKeyboardHookGuard();

            // 订阅事件
            _keyboardGuard.GuardStarted += KeyboardGuard_GuardStarted;
            _keyboardGuard.GuardStopped += KeyboardGuard_GuardStopped;
            _keyboardGuard.ErrorOccurred += KeyboardGuard_ErrorOccurred;
            config_TabControl.BringToFront();
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

        //====================校验父进程============================
        bool validProcess(IntPtr w) {
            int pid = getProcessIdByHandle(w);
            Process p = Process.GetProcessById(pid);
            if (p.ProcessName == this_process_name) {
                return false;
            }
            return true;
        }

        //====================后台监听===============================

        private void Listen(object sender, EventArgs e) {

            if (topest_with_timer.Checked) {
                this.TopMost = true;//防止被覆盖
            }

            Process[] p = Process.GetProcessesByName(process_name.Text.Trim());
            if (p.Length > 0) {
                List<string> lines = new List<string> { };
                call_show_info(ref p, ref lines);

                //操作
                if (auto_kill.Checked) {
                    call_kill_proc(ref p);
                } else if (auto_hide.Checked) {
                    call_hide_window(ref p, ref lines);
                } else if (attached_target.Checked) {
                    call_attach_window(ref p, ref lines);
                }
                return;
            } else {
                //无进程信息，更改显示
                label_running_stat.Text = "否";
                label_running_stat.ForeColor = Color.Red;
                label_process_pid.Text = "None";
            }
        }


        private void call_show_info(ref Process[] p, ref List<string> lines) {
            //信息显示
            label_running_stat.Text = "是";
            label_running_stat.ForeColor = Color.Green;

            bool equals = true;

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
        }
        private void call_kill_proc(ref Process[] p) {
            foreach (var _ in p) {
                _.Kill();   //杀死目标进程
            }
        }
        private void call_hide_window(ref Process[] p, ref List<string> lines) {
            IntPtr h;
            try {
                //全屏控屏窗口
                h = FindWindow(full_window_class.Text.Trim(), null);
                if (h != IntPtr.Zero && validProcess(h) && lines.Contains(getProcessIdByHandle(h).ToString())) {
                    log.add("尝试隐藏全屏控屏窗口: " + getHex(h));
                    closeHandle(h);
                    return;
                }
                //普通控屏窗口
                h = FindWindow(full_window_class.Text.Trim(), null);
                if (h != IntPtr.Zero && validProcess(h) && lines.Contains(getProcessIdByHandle(h).ToString())) {
                    log.add("尝试隐藏普通控屏窗口: " + getHex(h));
                    closeHandle(h);
                    return;
                }
            } catch { }
            foreach (var process in p) {
                if (!IsIconic(process.MainWindowHandle)) {
                    closeHandle(process.MainWindowHandle);
                }
            }
        }
        private void call_attach_window(ref Process[] p, ref List<string> lines) {
            IntPtr h;
            try {
                //获取目标窗口(全屏)
                h = FindWindow(full_window_class.Text.Trim(), null);
                if (h != IntPtr.Zero &&
                    validProcess(h) &&
                    lines.Contains(getProcessIdByHandle(h).ToString()) &&
                    h != target_panel.Handle    //确认父窗口是否为target_panel
                ) {
                    _SetParent(h, target_panel.Handle);
                    //强制缩放为target_panel的大小
                    SetWindowPos(h, IntPtr.Zero, 0, 0, target_panel.Width, target_panel.Height, 0x0040);
                    log.add("嵌入全屏窗口: " + getHex(h));
                    log.ignore(2);
                    _keyboardGuard.Stop();
                    _keyboardGuard.Start();
                    return;
                } else {
                    //获取目标窗口(普通)
                    h = FindWindow(full_window_class.Text.Trim(), null);
                    if (h != IntPtr.Zero &&
                        validProcess(h) &&
                        lines.Contains(getProcessIdByHandle(h).ToString()) &&
                        h != target_panel.Handle    //确认父窗口是否为target_panel
                    ) {
                        _SetParent(h, target_panel.Handle);
                        //强制缩放为target_panel的大小
                        SetWindowPos(h, IntPtr.Zero, 0, 0, target_panel.Width, target_panel.Height, 0x0040);
                        log.add("嵌入普通窗口: " + getHex(h));
                        return;
                    }
                }
            } catch { }
        }


        //======================对目标采取的操作==============================
        private void auto_kill_CheckedChanged(object sender, EventArgs e) {
            //逻辑上关闭和隐藏不需要同时发生
            if (auto_kill.Checked) {
                auto_hide.Checked = false;
                attached_target.Checked = false;
            }
        }

        private void auto_hide_CheckedChanged(object sender, EventArgs e) {
            if (auto_hide.Checked) {
                auto_kill.Checked = false;
                attached_target.Checked = false;
            }
        }
        //将目标窗口嵌入到target_panel
        private void attached_target_CheckedChanged(object sender, EventArgs e) {
            if (attached_target.Checked) {
                auto_hide.Checked = false;
                auto_kill.Checked = false;
                _keyboardGuard.Start();
            } else {
                _keyboardGuard.Stop();
            }
        }


        //===================更改目标进程名字以适应目标名称变化===================
        private void process_name_TextChanged(object sender, EventArgs e) {
            if (process_name.Text.Trim() == this.this_process_name) {
                //防止误杀自己
                auto_hide.Checked = false;
                auto_kill.Checked = false;
                attached_target.Checked = false;
                auto_hide.Enabled = false;
                auto_kill.Enabled = false;
                attached_target.Enabled = false;
                name_conflict_err.Visible = true;
            } else {
                name_conflict_err.Visible = false;
                auto_hide.Enabled = true;
                auto_kill.Enabled = true;
                attached_target.Enabled = true;
            }
        }

        private void label2_DoubleClick(object sender, EventArgs e) {
            process_name.Text = default_RMPN;//重置名称
            process_name_TextChanged(sender, e);
        }


        private void f_w_c_l_DoubleClick(object sender, EventArgs e) {
            this.full_window_class.Text = default_RFWCN;//重置名称
        }



        private void n_w_c_l_DoubleClick(object sender, EventArgs e) {
            this.normal_window_class.Text = default_RNWCN;//重置名称
        }
        //================================================================

        private void button1_Click(object sender, EventArgs e) {
            //打开“关于”界面
            new FuckRedSpider.Form2().ShowDialog();
            System.GC.Collect();
        }

        private void main_tabcontrol_SelectedIndexChanged(object sender, EventArgs e) {
            if (config_TabControl.SelectedIndex == 0 && main_tabcontrol.SelectedIndex == 0) {
                this.MinimumSize = new System.Drawing.Size(355, 325);
            } else {
                this.MinimumSize = new System.Drawing.Size(0, 0);
            }
        }
    }
}
