using KeyboardHookGuard;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Process = System.Diagnostics.Process;
using System.Text;

namespace FuckRedSpider {


    public partial class Form1 : Form {
        // 存储被劫持窗口的原始信息，以便恢复
        private struct HijackInfo {
            public IntPtr OriginalParent;
            public long OriginalStyle;
            public RECT OriginalRect;
        }

        private readonly Dictionary<IntPtr, HijackInfo> _hijacked = new Dictionary<IntPtr, HijackInfo>();

        // 将第三方窗口嵌入到本窗体的容器中；在嵌入前保存原始父句柄与样式，便于恢复
        private void _SetParent(IntPtr child, IntPtr hWndNewParent) {
            try {
                // 保存原始信息（仅第一次）
                if (child != IntPtr.Zero && !_hijacked.ContainsKey(child)) {
                    HijackInfo info = new HijackInfo();
                    try { info.OriginalParent = GetParent(child); } catch { info.OriginalParent = IntPtr.Zero; }
                    try { info.OriginalStyle = GetWindowLong(child, -16); } catch { info.OriginalStyle = 0; }
                    try { RECT r; if (GetWindowRect(child, out r)) info.OriginalRect = r; } catch { info.OriginalRect = new RECT(); }
                    _hijacked[child] = info;
                }

                ShowWindow(child, 0);                 //先将窗体隐藏，防止出现闪烁
                SetParent(child, hWndNewParent);      //将第三方窗体嵌入父容器      
                ShowWindow(child, 3);                 //让第三方窗体在容器中最大化显示
                RemoveWindowTitle(child);             // 去除窗体标题
            } catch { }
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

        private const UInt32 WM_CLOSE = 0x0010;
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);


        //===========================================================


        private const string default_RMPN = "REDAgent";
        private const string default_RFWCN = "DIBFullViewClass";
        private const string default_RNWCN = "RedEagle.Monitor";

        private readonly string this_process_name = Process.GetCurrentProcess().ProcessName;

        private readonly GlobalKeyboardHookGuard _keyboardGuard;
        private bool keyboardGuardRunning = false;
        IntPtr target_window;
        FormWindowState last_state = FormWindowState.Normal;

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
        #region --日志--
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
        #endregion
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

        // 恢复单个已嵌入窗口的父句柄和样式
        private void RestoreHijackedWindow(IntPtr h) {
            try {
                if (h == IntPtr.Zero) return;
                if (!_hijacked.ContainsKey(h)) return;
                var info = _hijacked[h];
                // 先复原父句柄
                SetParent(h, info.OriginalParent);
                // 恢复样式
                SetWindowLong(h, -16, info.OriginalStyle);
                // 尝试恢复位置和大小
                try {
                    var r = info.OriginalRect;
                    int w = r.Right - r.Left;
                    int ht = r.Bottom - r.Top;
                    SetWindowPos(h, IntPtr.Zero, r.Left, r.Top, w, ht, 0);
                } catch { }
                // 显示为普通窗口
                ShowWindow(h, 1);
                _hijacked.Remove(h);
            } catch { }
        }

        // 恢复所有已嵌入的窗口
        private void RestoreAllHijackedWindows() {
            try {
                var keys = _hijacked.Keys.ToArray();
                foreach (var k in keys) {
                    RestoreHijackedWindow(k);
                }
            } catch { }
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

        string origin_exeutable_path = "";
        private void Listen(object sender, EventArgs e) {

            if (topest_with_timer.Checked) {
                this.TopMost = true;//防止被覆盖
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
                    if (File.Exists(origin_exeutable_path)) {
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
                    _keyboardGuard.Stop();
                    _keyboardGuard.Start();
                    return;
                }
                #endregion
                #region --获取目标窗口(普通)--
                target_window = FindWindow(full_window_class.Text.Trim(), null);
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
                _keyboardGuard.Start();
            } else {
                _keyboardGuard.Stop();
                // 恢复所有已被嵌入的窗口
                RestoreAllHijackedWindows();
                target_window = IntPtr.Zero;
            }
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


        private void Form1_ResizeEnd(object sender, EventArgs e) {
            if (attached_target.Checked) {
                if (target_window != IntPtr.Zero) {
                    SetWindowPos(target_window, IntPtr.Zero, 0, 0, target_panel.Width, target_panel.Height, 0x0040);
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

        void capture_drag_area_MouseDown(object sender, MouseEventArgs e) {
            // 开始捕获
            isCapturing = true;
            if (overlay == null) {
                overlay = new OverlayForm();
            }
            overlay.Show();
            // 立刻更新一次位置
            UpdateCaptureAtCursor();
        }
        void capture_drag_area_MouseUp(object sender, MouseEventArgs e) {
            // 结束捕获：关闭并清理 overlay
            isCapturing = false;
            try {
                if (overlay != null) {
                    overlay.Clear();
                    overlay = null;
                }
            } catch { }
            lastCapturedWindow = IntPtr.Zero;
        }
        void capture_drag_area_MouseMove(object sender, MouseEventArgs e) {
            if (!isCapturing) return;
            UpdateCaptureAtCursor();
        }

        // --- 捕获相关字段和方法 ---
        private OverlayForm overlay;
        private bool isCapturing = false;
        private IntPtr lastCapturedWindow = IntPtr.Zero;

        private void UpdateCaptureAtCursor() {
            try {
                var p = Cursor.Position;
                IntPtr desktop = GetDesktopWindow();
                IntPtr h = ChildWindowFromPointEx(desktop, p, CWP_SKIPTRANSPARENT | CWP_SKIPINVISIBLE | CWP_SKIPDISABLED);
                if (h == IntPtr.Zero) {
                    // 回退到 WindowFromPoint
                    h = WindowFromPoint(p);
                }
                if (h == IntPtr.Zero) return;

                // 获取顶层窗口：使用 GA_ROOTOWNER 优先获取 owner chain 的顶层（比 GA_ROOT 在被 SetParent 情况下更可靠）
                IntPtr top = GetAncestor(h, GA_ROOTOWNER);
                if (top == IntPtr.Zero) top = GetAncestor(h, GA_ROOT);
                if (top == IntPtr.Zero) top = h;
                int currentPid = Process.GetCurrentProcess().Id;
                Func<IntPtr, bool> isRealWindow = (wnd) => {
                    try {
                        if (wnd == IntPtr.Zero) return false;
                        if (!IsWindowVisible(wnd)) return false;
                        if (wnd == this.Handle) return false;
                        if (overlay != null && wnd == overlay.Handle) return false;
                        // 如果该窗口或其任何祖先属于当前进程，则视为自身窗口，排除
                        if (IsWindowOrAncestorOwnedByProcess(wnd, currentPid)) return false;
                        // 标题长度大于0 优先
                        int len = GetWindowTextLength(wnd);
                        if (len > 0) return true;
                        // 否则根据类名排除桌面/壳窗口
                        var cls = new StringBuilder(256);
                        GetClassName(wnd, cls, cls.Capacity);
                        string c = cls.ToString();
                        if (!string.IsNullOrEmpty(c)) {
                            // 排除一些常见的壳/缩略图窗口
                            string[] excludePrefixes = new string[] { "WorkerW", "Progman", "Shell_TrayWnd", "ThumbnailDeviceHelperWnd", "TaskbandThumbnailFlyoutWindow", "TaskbarThumbnailWnd" };
                            foreach (var ex in excludePrefixes) {
                                if (c.StartsWith(ex, StringComparison.OrdinalIgnoreCase)) return false;
                            }
                            if (c.IndexOf("Thumbnail", StringComparison.OrdinalIgnoreCase) >= 0) return false;
                            return true;
                        }
                    } catch { }
                    return false;
                };

                // 优先检查当前 top
                if (!isRealWindow(top)) {
                    // 在 z-order 上下邻近搜索
                    IntPtr cand = top;
                    bool found = false;
                    for (int i = 0; i < 200 && cand != IntPtr.Zero; i++) {
                        // try previous (higher) windows first
                        IntPtr prev = GetWindow(cand, GW_HWNDPREV);
                        if (prev != IntPtr.Zero && isRealWindow(prev)) { top = prev; found = true; break; }
                        IntPtr next = GetWindow(cand, GW_HWNDNEXT);
                        if (next != IntPtr.Zero && isRealWindow(next)) { top = next; found = true; break; }
                        // 向上或向下继续寻找
                        if (prev != IntPtr.Zero) cand = prev;
                        else cand = next;
                    }
                    // if not found, keep original top
                }
                if (top == IntPtr.Zero) {
                    // fallback to original
                    top = GetAncestor(h, GA_ROOT);
                    if (top == IntPtr.Zero) top = h;
                }

                // 确定最终选择的句柄并统一使用
                IntPtr selected = top;

                // 如果选中的窗口属于当前进程，则完全忽略——不绘制、高亮或显示路径/类名
                try {
                    int selPid;
                    GetWindowThreadProcessId(selected, out selPid);
                    if (selPid == currentPid) {
                        try {
                            if (overlay != null) overlay.Clear();
                        } catch { }
                        lastCapturedWindow = IntPtr.Zero;
                        captured_window.Text = "";
                        captured_executable.Text = "";
                        return;
                    }
                } catch { }

                RECT r;
                if (GetWindowRect(selected, out r)) {
                    Rectangle rect = new Rectangle(r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top);
                    if (overlay == null) overlay = new OverlayForm();
                    overlay.HighlightRect(rect, selected);
                }

                // 记录并显示选中窗口的信息（确保与绘制一致）
                try {
                    var sb = new StringBuilder(256);
                    GetClassName(selected, sb, sb.Capacity);
                    var className = sb.ToString();
                    captured_window.Text = className;

                    int selPid;
                    GetWindowThreadProcessId(selected, out selPid);
                    string path = null;
                    try {
                        path = GetProcessImagePath(selPid);
                    } catch (Exception ex) {
                        log.add("GetMainModule error: " + ex.Message);
                    }
                    captured_executable.Text = path ?? "";

                    // 更新 lastCapturedWindow
                    lastCapturedWindow = selected;

                } catch (Exception ex) {
                    log.add("UpdateCaptureAtCursor info error: " + ex.Message);
                }
            } catch { }
        }

        private string GetProcessImagePath(int pid) {
            try {
                // Try QueryFullProcessImageName via OpenProcess
                IntPtr h = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, false, (uint)pid);
                if (h != IntPtr.Zero) {
                    try {
                        var sb = new StringBuilder(1024);
                        int size = sb.Capacity;
                        if (QueryFullProcessImageName(h, 0, sb, ref size)) {
                            return sb.ToString();
                        }
                    } finally {
                        CloseHandle(h);
                    }
                }
            } catch (Exception ex) {
                log.add("QueryFullProcessImageName failed: " + ex.Message);
            }

            // Fallback to Process.MainModule (may throw on some processes)
            try {
                var p = Process.GetProcessById(pid);
                return p.MainModule.FileName;
            } catch (Exception ex) {
                log.add("Process.MainModule failed: " + ex.Message);
                return null;
            }
        }

        // 检查窗口或其任一父窗口是否属于指定进程
        private bool IsWindowOrAncestorOwnedByProcess(IntPtr wnd, int pid) {
            try {
                IntPtr cur = wnd;
                while (cur != IntPtr.Zero) {
                    int p;
                    GetWindowThreadProcessId(cur, out p);
                    if (p == pid) return true;
                    cur = GetParent(cur);
                }
            } catch { }
            return false;
        }

        private void insert_captured_Click(object sender, EventArgs e) {
            // 优先使用最后捕获的句柄，确保高亮与应用结果一致
            try {
                int currentPid = Process.GetCurrentProcess().Id;
                string currentExe = null;
                currentExe = Process.GetCurrentProcess().MainModule.FileName;

                if (lastCapturedWindow != IntPtr.Zero) {
                    int pid;
                    GetWindowThreadProcessId(lastCapturedWindow, out pid);
                    if (pid == currentPid) {
                        log.add("捕获的窗口属于自身，已忽略");
                        return;
                    }

                    var sb = new StringBuilder(256);
                    GetClassName(lastCapturedWindow, sb, sb.Capacity);
                    var cls = sb.ToString();
                    string appliedClass = null;
                    string appliedExe = null;
                    string appliedName = null;
                    if (!string.IsNullOrWhiteSpace(cls)) {
                        full_window_class.Text = cls;
                        appliedClass = cls;
                    }

                    if (pid != 0) {
                        string path = null;
                        try {
                            path = GetProcessImagePath(pid);
                        } catch (Exception ex) {
                            log.add("获取捕获进程路径失败: " + ex.Message);
                        }
                        if (!string.IsNullOrWhiteSpace(path)) {
                            // 避免将自身的可执行路径写入
                            if (!string.IsNullOrWhiteSpace(currentExe) && string.Equals(path, currentExe, StringComparison.OrdinalIgnoreCase)) {
                                // ignore executable
                            } else {
                                captured_executable.Text = path;
                                appliedExe = path;
                                string fileName = null;
                                try {
                                    if (File.Exists(path)) fileName = Path.GetFileNameWithoutExtension(path);
                                    else fileName = Path.GetFileNameWithoutExtension(Path.GetFileName(path));
                                } catch { }
                                if (!string.IsNullOrWhiteSpace(fileName)) {
                                    process_name.Text = fileName;
                                    process_name_TextChanged(this, EventArgs.Empty);
                                    appliedName = fileName;
                                }
                            }
                        }
                    }

                    // single combined log
                    var parts = new List<string>();
                    if (!string.IsNullOrWhiteSpace(appliedClass)) parts.Add($"类:{appliedClass}");
                    if (!string.IsNullOrWhiteSpace(appliedExe)) parts.Add($"可执行:{appliedExe}");
                    if (!string.IsNullOrWhiteSpace(appliedName)) parts.Add($"进程名:{appliedName}");
                    if (parts.Count > 0) log.add("已应用捕获: " + string.Join(" | ", parts));
                    return;
                }

                // 回退到文本框内容（若没有 lastCapturedWindow）
                string appliedClass2 = null;
                string appliedExe2 = null;
                string appliedName2 = null;
                if (!string.IsNullOrWhiteSpace(captured_window.Text)) {
                    full_window_class.Text = captured_window.Text;
                    appliedClass2 = captured_window.Text;
                }
                var path2 = captured_executable.Text?.Trim();
                if (!string.IsNullOrWhiteSpace(path2)) {
                    // 避免将自身的可执行名写入
                    if (!string.IsNullOrWhiteSpace(currentExe) && string.Equals(path2, currentExe, StringComparison.OrdinalIgnoreCase)) {
                        // ignore
                    } else {
                        string fileName2 = null;
                        if (File.Exists(path2)) fileName2 = Path.GetFileNameWithoutExtension(path2);
                        else {
                            try { var name = Path.GetFileName(path2); if (!string.IsNullOrEmpty(name)) fileName2 = Path.GetFileNameWithoutExtension(name); } catch { }
                        }
                        if (!string.IsNullOrWhiteSpace(fileName2)) {
                            process_name.Text = fileName2;
                            process_name_TextChanged(this, EventArgs.Empty);
                            appliedName2 = fileName2;
                            appliedExe2 = path2;
                        }
                    }
                }

                var parts2 = new List<string>();
                if (!string.IsNullOrWhiteSpace(appliedClass2)) parts2.Add($"类:{appliedClass2}");
                if (!string.IsNullOrWhiteSpace(appliedExe2)) parts2.Add($"可执行:{appliedExe2}");
                if (!string.IsNullOrWhiteSpace(appliedName2)) parts2.Add($"进程名:{appliedName2}");
                if (parts2.Count > 0) log.add("已应用捕获: " + string.Join(" | ", parts2));
            } catch (Exception ex) {
                log.add("应用捕获失败: " + ex.Message);
            }
        }

        private void open_captured_directory_DoubleClick(object sender, EventArgs e) {
            try {
                var raw = captured_executable.Text?.Trim();
                if (string.IsNullOrEmpty(raw)) {
                    log.add("没有可用的可执行文件路径");
                    return;
                }
                // 如果是文件路径，打开资源管理器并选中；如果是目录则打开目录；否则尝试根据字符串处理
                string path = raw;
                if (File.Exists(path)) {
                    Process.Start("explorer.exe", "/select,\"" + path + "\"");
                    return;
                }
                if (Directory.Exists(path)) {
                    Process.Start("explorer.exe", "\"" + path + "\"");
                    return;
                }
                // 可能 captured_executable 只是包含文件路径的不完整字符串，尝试取目录部分
                try {
                    var dir = Path.GetDirectoryName(path);
                    if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir)) {
                        Process.Start("explorer.exe", "\"" + dir + "\"");
                        return;
                    }
                } catch { }

                // 最后退回：尝试从运行时路径打开
                log.add("路径不存在或无法识别: " + raw);
            } catch (Exception ex) {
                log.add("打开目录失败: " + ex.Message);
            }
        }
    }
}
