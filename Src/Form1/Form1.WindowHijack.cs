using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FuckRedSpider {
    public partial class Form1 {
        // 存储被劫持窗口的原始信息，以便恢复
        private struct HijackInfo {
            public IntPtr OriginalParent;
            public long OriginalStyle;
            public RECT OriginalRect;
        }

        private readonly Dictionary<IntPtr, HijackInfo> _hijacked = new Dictionary<IntPtr, HijackInfo>();

        private const UInt32 WM_CLOSE = 0x0010;
        private const int WM_SIZING = 0x0214;
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

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
    }
}
