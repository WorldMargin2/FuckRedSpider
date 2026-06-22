using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace MouseHookGuard
{
    /// <summary>
    /// 提供一个全局鼠标钩子守护，用于监控和纠正异常的鼠标移动。
    /// 实现了 IDisposable 接口，确保钩子可以被正确卸载。
    /// </summary>
    public class GlobalMouseHookGuard : IDisposable
    {
        #region P/Invoke Declarations and Constants

        private const int WH_MOUSE_LL = 14;
        private const int WM_MOUSEMOVE = 0x0200;

        private const uint LLMHF_INJECTED = 0x00000001;

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int X, int Y);

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        #endregion

        #region Private Fields

        private IntPtr _hookID = IntPtr.Zero;
        private LowLevelMouseProc _proc;
        private bool _isDisposed = false;
        private POINT _lastValidPosition;
        private bool _isCorrecting = false;

        #endregion

        #region Public Events

        /// <summary>
        /// 当鼠标守护成功启动时发生。
        /// </summary>
        public event EventHandler GuardStarted;

        /// <summary>
        /// 当鼠标守护停止时发生。
        /// </summary>
        public event EventHandler GuardStopped;

        /// <summary>
        /// 当发生错误时发生（例如，安装钩子失败）。
        /// </summary>
        public event EventHandler<ErrorEventArgs> ErrorOccurred;

        /// <summary>
        /// 当检测到并纠正异常鼠标移动时发生。
        /// </summary>
        public event EventHandler<MouseCorrectionEventArgs> MouseCorrected;

        #endregion

        #region Constructor and Destructor

        public GlobalMouseHookGuard()
        {
            _proc = HookCallback;
        }

        ~GlobalMouseHookGuard()
        {
            Dispose(false);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 启动鼠标守护。
        /// </summary>
        public void Start()
        {
            if (_hookID != IntPtr.Zero)
            {
                return;
            }

            // 初始化最后有效位置
            if (GetCursorPos(out _lastValidPosition))
            {
                // 成功获取初始位置
            }

            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                _hookID = SetWindowsHookEx(WH_MOUSE_LL, _proc, GetModuleHandle(curModule.ModuleName), 0);
            }

            if (_hookID == IntPtr.Zero)
            {
                int errorCode = Marshal.GetLastWin32Error();
                string errorMessage = $"安装鼠标钩子失败，错误代码: {errorCode}。请确保以管理员身份运行。";
                OnErrorOccurred(new ErrorEventArgs(new Exception(errorMessage)));
            }
            else
            {
                OnGuardStarted(EventArgs.Empty);
            }
        }

        /// <summary>
        /// 停止鼠标守护。
        /// </summary>
        public void Stop()
        {
            if (_hookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookID);
                _hookID = IntPtr.Zero;
                OnGuardStopped(EventArgs.Empty);
            }
        }

        #endregion

        #region Private Methods

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
            {
                return CallNextHookEx(_hookID, nCode, wParam, lParam);
            }

            // 如果正在纠正位置，避免无限循环
            if (_isCorrecting)
            {
                return CallNextHookEx(_hookID, nCode, wParam, lParam);
            }

            MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));

            // 检查是否是注入的事件（由其他程序产生）
            bool isInjected = (hookStruct.flags & LLMHF_INJECTED) != 0;

            // 如果是鼠标移动事件
            if (wParam == (IntPtr)WM_MOUSEMOVE)
            {
                // 如果是注入的鼠标移动，纠正回最后有效位置
                if (isInjected)
                {
                    try
                    {
                        _isCorrecting = true;
                        SetCursorPos(_lastValidPosition.X, _lastValidPosition.Y);
                        OnMouseCorrected(new MouseCorrectionEventArgs(hookStruct.pt.X, hookStruct.pt.Y, _lastValidPosition.X, _lastValidPosition.Y));
                    }
                    finally
                    {
                        _isCorrecting = false;
                    }
                    return (IntPtr)1; // 阻止这个消息继续传递
                }
                else
                {
                    // 真实的物理鼠标移动，更新最后有效位置
                    _lastValidPosition = hookStruct.pt;
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        protected virtual void OnGuardStarted(EventArgs e)
        {
            GuardStarted?.Invoke(this, e);
        }

        protected virtual void OnGuardStopped(EventArgs e)
        {
            GuardStopped?.Invoke(this, e);
        }

        protected virtual void OnErrorOccurred(ErrorEventArgs e)
        {
            ErrorOccurred?.Invoke(this, e);
        }

        protected virtual void OnMouseCorrected(MouseCorrectionEventArgs e)
        {
            MouseCorrected?.Invoke(this, e);
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    GuardStarted = null;
                    GuardStopped = null;
                    ErrorOccurred = null;
                    MouseCorrected = null;
                }
                Stop();
                _isDisposed = true;
            }
        }

        #endregion
    }

    /// <summary>
    /// 鼠标纠正事件参数
    /// </summary>
    public class MouseCorrectionEventArgs : EventArgs
    {
        /// <summary>
        /// 被阻止的鼠标目标位置 X
        /// </summary>
        public int BlockedX { get; }

        /// <summary>
        /// 被阻止的鼠标目标位置 Y
        /// </summary>
        public int BlockedY { get; }

        /// <summary>
        /// 纠正到的位置 X
        /// </summary>
        public int CorrectedX { get; }

        /// <summary>
        /// 纠正到的位置 Y
        /// </summary>
        public int CorrectedY { get; }

        public MouseCorrectionEventArgs(int blockedX, int blockedY, int correctedX, int correctedY)
        {
            BlockedX = blockedX;
            BlockedY = blockedY;
            CorrectedX = correctedX;
            CorrectedY = correctedY;
        }
    }
}
