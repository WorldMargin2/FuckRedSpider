using KeyboardHookGuard;
using MouseHookGuard;
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
        private const string default_RMPN = "REDAgent";
        private const string default_RFWCN = "DIBFullViewClass";
        private const string default_RNWCN = "RedEagle.Monitor";

        private readonly string this_process_name = Process.GetCurrentProcess().ProcessName;
        IntPtr target_window;
        FormWindowState last_state = FormWindowState.Normal;
        string origin_exeutable_path = "";

        protected override void OnFormClosing(FormClosingEventArgs e) {
            // 关闭窗口时同步卸载Hook，不走后台线程
            if (_screenHookManager != null && _screenHookManager.IsInjected) {
                _screenHookManager.Dispose();
            }
            base.OnFormClosing(e);
        }

        public Form1(string[] args) {
            InitializeComponent();

            listener_timer.Start();
            this.bindTip();
            log = new Log(this.textbox_log);
            log.add("程序启动");
            _keyboardGuard = new GlobalKeyboardHookGuard();
            _mouseGuard = new GlobalMouseHookGuard();

            // 订阅事件
            _keyboardGuard.GuardStarted += KeyboardGuard_GuardStarted;
            _keyboardGuard.GuardStopped += KeyboardGuard_GuardStopped;
            _keyboardGuard.ErrorOccurred += KeyboardGuard_ErrorOccurred;
            _mouseGuard.GuardStarted += MouseGuard_GuardStarted;
            _mouseGuard.GuardStopped += MouseGuard_GuardStopped;
            _mouseGuard.ErrorOccurred += MouseGuard_ErrorOccurred;
            _mouseGuard.MouseCorrected += MouseGuard_MouseCorrected;
            config_TabControl.BringToFront();
            hotKeyInit();

            // ==================== 初始化屏幕 Hook ====================
            _screenHookManager = new ScreenHookManager((msg) => log.add(msg));
            _screenHookManager.OnHookAutoUnload += (s, e) => {
                // 确保在 UI 线程执行，避免跨线程访问 UI 控件导致崩溃
                if (this.InvokeRequired) {
                    this.BeginInvoke(new Action(() => {
                        screenHookCheckBox.Checked = false;
                        screenHookEnabled = false;
                        screenHookStatusLabel.Text = "目标进程已退出";
                        screenHookStatusLabel.ForeColor = Color.Gray;
                        log.add("屏幕 Hook 已自动卸载");
                    }));
                } else {
                    screenHookCheckBox.Checked = false;
                    screenHookEnabled = false;
                    screenHookStatusLabel.Text = "目标进程已退出";
                    screenHookStatusLabel.ForeColor = Color.Gray;
                    log.add("屏幕 Hook 已自动卸载");
                }
            };
        }

        #region --提示--
        private void bindTip() {
            //绑定提示
            //状态监听窗口
            toolTip1.SetToolTip(this.label_running_stat, "当前是否检测到目标进程");
            toolTip1.SetToolTip(this.label_process_pid, "目标进程的PID(可能有多个)");
            //日志窗口
            toolTip1.SetToolTip(this.textbox_log, "日志记录");
            //设置窗口
            //  主选项
            toolTip1.SetToolTip(this.topest_with_timer, "每次检测时都置顶窗口");
            toolTip1.SetToolTip(this.auto_kill, "检测到进程时自动结束进程");
            toolTip1.SetToolTip(this.auto_hide, "检测到进程时自动隐藏窗口");
            toolTip1.SetToolTip(this.attached_target, "检测到进程时尝试劫持窗口");
            toolTip1.SetToolTip(this.keyboardGuardInTime, "通过不断重启键盘守护来防止键盘被劫持");
            toolTip1.SetToolTip(screenHookCheckBox, "Hook 目标进程的屏幕捕获，替换为内置图片");

            //  自定义
            toolTip1.SetToolTip(this.process_name, "要监控的进程名，默认REDAgent");
            toolTip1.SetToolTip(this.label2, "双击重置");
            toolTip1.SetToolTip(this.full_window_class, "全屏控屏窗口类名，默认DIBFullViewClass");
            toolTip1.SetToolTip(this.f_w_c_l, "双击重置");
            toolTip1.SetToolTip(this.normal_window_class, "普通控屏窗口类名，默认RedEagle.Monitor");
            toolTip1.SetToolTip(this.n_w_c_l, "双击重置");
            //  增强功能
            toolTip1.SetToolTip(this.capture_drag_area, "拖拽捕获窗口");
            toolTip1.SetToolTip(this.captured_executable, "捕获到的窗口的exe路径");
            toolTip1.SetToolTip(this.captured_window, "捕获到的窗口的类名");
            toolTip1.SetToolTip(this.open_captured_directory, "双击打开捕获到的窗口的进程所在目录");
            toolTip1.SetToolTip(this.insert_captured, "点击将捕获到的窗口应用到自定义页");

            //  热键
            //关于按钮
            toolTip1.SetToolTip(this.button1, "关于作者");
        }
        #endregion

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

            if(keyboardGuardInTime.Checked) {
                restartKeyboardGuard();
            }

            // ==================== 屏幕 Hook 自动注入 ====================
            if (screenHookEnabled && !_screenHookManager.IsInjected) {
                TryInjectScreenHook();
            }

            Process[] p = Process.GetProcessesByName(process_name.Text.Trim());

            if(origin_exeutable_path == "") {
                if (p.Length > 0) {
                    try {
                        // Use safer method that works across 32/64-bit boundaries
                        origin_exeutable_path = GetProcessImagePath(p[0].Id) ?? "";
                    } catch (Exception ex) {
                        log.add("获取进程路径失败：" + ex.Message);
                    }
                }
            } else {
                if (auto_kill.Checked) {
                    // 避免重复操作：仅当 .bak 不存在时才尝试重命名
                    if (File.Exists(origin_exeutable_path) && !File.Exists(origin_exeutable_path + ".bak")) {
                        File.Move(origin_exeutable_path, origin_exeutable_path + ".bak");
                    }
                } else {
                    if (File.Exists(origin_exeutable_path + ".bak")) {
                        File.Move(origin_exeutable_path + ".bak", origin_exeutable_path);
                    }
                }
            }
            
            if (p.Length > 0) {
                List<string> lines = new List<string> { };
                call_show_info(ref p, ref lines);
                //操作
                if (auto_kill.Checked) {
                    call_kill_proc(ref p);
                } else {
                    if (auto_hide.Checked) {
                        call_hide_window(ref p, ref lines);
                    } else if (attached_target.Checked) {
                        call_attach_window(ref p, ref lines);
                    }
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
                h = FindWindow(normal_window_class.Text.Trim(), null);
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
            try {
                if (
                    target_window != IntPtr.Zero &&
                    GetParent(target_window) != IntPtr.Zero
                ) return;

                #region --获取目标窗口(全屏)--
                target_window = FindWindow(full_window_class.Text.Trim(), null);
                if (target_window != IntPtr.Zero &&
                    validProcess(target_window) &&
                    lines.Contains(getProcessIdByHandle(target_window).ToString())
                ) {
                    _SetParent(target_window, target_panel.Handle);
                    //强制缩放为target_panel的大小
                    SetWindowPos(target_window, target_panel.Handle, 0, 0, target_panel.Width, target_panel.Height, 0x0040);
                    log.add("嵌入全屏窗口: " + getHex(target_window));
                    log.ignore(2);
                    restartKeyboardGuard();
                    return;
                }
                #endregion
                #region --获取目标窗口(普通)--
                target_window = FindWindow(normal_window_class.Text.Trim(), null);
                if (target_window != IntPtr.Zero &&
                    validProcess(target_window) &&
                    lines.Contains(getProcessIdByHandle(target_window).ToString())
                ) {
                    _SetParent(target_window, target_panel.Handle);
                    //强制缩放为target_panel的大小
                    SetWindowPos(target_window, target_panel.Handle, 0, 0, target_panel.Width, target_panel.Height, 0x0040);
                    log.add("嵌入普通窗口: " + getHex(target_window));
                    return;
                }
                #endregion
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
                keepRatio.Enabled = true;
                startKeyboardGuard();
            } else {
                stopKeyboardGuard();
                // 恢复所有已被嵌入的窗口
                RestoreAllHijackedWindows();
                keepRatio.Enabled = false;
                target_window = IntPtr.Zero;
            }
            ratioWidth.Enabled = keepRatio.Checked;
            ratioHeight.Enabled = keepRatio.Checked;
        }

        public struct Box {
            public int width;
            public int height;
        }

        private Box KeepRatio(int width, int height) {
            if (this.WindowState == FormWindowState.Minimized) {
                return new Box() {
                    width = 0,
                    height = 0
                };
            }
            if (!(keepRatio.Checked && keepRatio.Enabled)) {
                return new Box() {
                    width = width,
                    height = height
                };
            }
            float ratio = (float)ratioWidth.Value / (float)ratioHeight.Value;
            if (width / height > ratio) {
                return new Box() {
                    width = (int)(height * ratio),
                    height = height
                };
            } else {
                return new Box() {
                    width = width,
                    height = (int)(width / ratio)
                };
            }
        }

        private void keepRatio_CheckedChanged(object sender, EventArgs e) {
            if (keepRatio.Checked) {
                ratioWidth.Enabled = true;
                ratioHeight.Enabled = true;
            } else {
                ratioWidth.Enabled = false;
                ratioHeight.Enabled = false;
            }
             Form1_ResizeEnd(sender, e);
        }

        //===================更改目标进程名字以适应目标名称变化===================
        private void process_name_TextChanged(object sender, EventArgs e) {
            //恢复被改名的原文件，避免用户在输入其它名称时重命名文件未恢复
            if(File.Exists(origin_exeutable_path + ".bak")) {
                File.Move(origin_exeutable_path + ".bak", origin_exeutable_path);
                origin_exeutable_path = "";
            }
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

        private void button1_Click(object sender, EventArgs e) {
            //打开"关于"界面
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

        private void Form1_ResizeEnd(object sender, EventArgs e) {
            if (attached_target.Checked) {
                if (target_window != IntPtr.Zero) {
                    var box = KeepRatio(target_panel.Width, target_panel.Height);
                    SetWindowPos(target_window, IntPtr.Zero, 0, 0, box.width, box.height, 0x0040);
                }
            }
        }

        private void Form1_Resize(object sender, EventArgs e) {
            if(this.WindowState != last_state) {
                last_state = this.WindowState;
                Form1_ResizeEnd(sender, e);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e) {
            // 退出前恢复所有被嵌入的窗口
            RestoreAllHijackedWindows();
            //恢复被改名的原文件
            if (File.Exists(origin_exeutable_path + ".bak")) {
                File.Move(origin_exeutable_path + ".bak", origin_exeutable_path);
            }
        }


        private void to_smallest_btn_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
