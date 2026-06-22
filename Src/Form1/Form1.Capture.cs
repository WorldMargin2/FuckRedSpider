using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Process = System.Diagnostics.Process;

namespace FuckRedSpider {
    public partial class Form1 {
        // --- 捕获相关字段和方法 ---
        private OverlayForm overlay;
        private bool isCapturing = false;
        private IntPtr lastCapturedWindow = IntPtr.Zero;

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
                    for (int i = 0; i < 200 && cand != IntPtr.Zero; i++) {
                        // try previous (higher) windows first
                        IntPtr prev = GetWindow(cand, GW_HWNDPREV);
                        if (prev != IntPtr.Zero && isRealWindow(prev)) { top = prev;break; }
                        IntPtr next = GetWindow(cand, GW_HWNDNEXT);
                        if (next != IntPtr.Zero && isRealWindow(next)) { top = next; break; }
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
                        log.add("捕获的窗口属于自身，已忽略了");
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
