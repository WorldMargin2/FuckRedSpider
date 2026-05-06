using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

/// <summary>
/// 提供一个全局键盘钩子守护，用于对抗并恢复被其他钩子禁用的物理键盘输入。
/// 实现了 IDisposable 接口，确保钩子可以被正确卸载。
/// </summary>
namespace KeyboardHookGuard {

    /// <summary>
    /// 提供一个全局键盘钩子守护，用于对抗并恢复被其他钩子禁用的物理键盘输入。
    /// 实现了 IDisposable 接口，确保钩子可以被正确卸载。
    /// </summary>
    public class GlobalKeyboardHookGuard : IDisposable {
        #region P/Invoke Declarations and Constants

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;

        private const uint INPUT_KEYBOARD = 1;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint LLKHF_INJECTED = 0x00000010;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT {
            public uint type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion {
            [FieldOffset(0)] public MOUSEINPUT mi;
            [FieldOffset(0)] public KEYBDINPUT ki;
            [FieldOffset(0)] public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT { public int dx; public int dy; public uint mouseData; public uint dwFlags; public uint time; public IntPtr dwExtraInfo; }

        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT { public uint uMsg; public ushort wParamL; public ushort wParamH; }

        #endregion

        #region Private Fields

        private IntPtr _hookID = IntPtr.Zero;
        private LowLevelKeyboardProc _proc;
        private bool _isDisposed = false;

        #endregion

        #region Public Events

        /// <summary>
        /// 当键盘守护成功启动时发生。
        /// </summary>
        public event EventHandler GuardStarted;

        /// <summary>
        /// 当键盘守护停止时发生。
        /// </summary>
        public event EventHandler GuardStopped;

        /// <summary>
        /// 当发生错误时发生（例如，安装钩子失败）。
        /// </summary>
        public event EventHandler<ErrorEventArgs> ErrorOccurred;

        #endregion

        #region Constructor and Destructor

        public GlobalKeyboardHookGuard() {
            _proc = HookCallback;
        }

        // 析构函数，以防忘记调用 Dispose
        ~GlobalKeyboardHookGuard() {
            Dispose(false);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 启动键盘守护。
        /// </summary>
        public void Start() {
            if (_hookID != IntPtr.Zero) {
                // 钩子已经运行
                return;
            }

            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule) {
                _hookID = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, GetModuleHandle(curModule.ModuleName), 0);
            }

            if (_hookID == IntPtr.Zero) {
                int errorCode = Marshal.GetLastWin32Error();
                string errorMessage = $"安装钩子失败，错误代码: {errorCode}。请确保以管理员身份运行。";
                // *** 修正后的代码行 ***
                OnErrorOccurred(new ErrorEventArgs(new Exception(errorMessage)));
            } else {
                OnGuardStarted(EventArgs.Empty);
            }
        }

        /// <summary>
        /// 停止键盘守护。
        /// </summary>
        public void Stop() {
            if (_hookID != IntPtr.Zero) {
                UnhookWindowsHookEx(_hookID);
                _hookID = IntPtr.Zero;
                OnGuardStopped(EventArgs.Empty);
            }
        }

        #endregion

        #region Private Methods

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
            if (nCode < 0) {
                return CallNextHookEx(_hookID, nCode, wParam, lParam);
            }

            KBDLLHOOKSTRUCT hookStruct = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));

            // 如果是注入的事件（由我们自己的SendInput产生），则放行
            if ((hookStruct.flags & LLKHF_INJECTED) != 0) {
                return CallNextHookEx(_hookID, nCode, wParam, lParam);
            }

            // 如果是真实的物理按键，我们“吃掉”它并用SendInput重新生成
            INPUT input = new INPUT {
                type = INPUT_KEYBOARD,
                u = new InputUnion {
                    ki = new KEYBDINPUT {
                        wVk = (ushort)hookStruct.vkCode,
                        wScan = (ushort)hookStruct.scanCode,
                        dwFlags = (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP) ? KEYEVENTF_KEYUP : 0,
                        dwExtraInfo = hookStruct.dwExtraInfo
                    }
                }
            };

            SendInput(1, new[] { input }, Marshal.SizeOf(typeof(INPUT)));

            // 返回1表示我们已经处理了这个消息，阻止它继续传递给下一个钩子
            return (IntPtr)1;
        }

        // 事件触发方法
        protected virtual void OnGuardStarted(EventArgs e) {
            GuardStarted?.Invoke(this, e);
        }

        protected virtual void OnGuardStopped(EventArgs e) {
            GuardStopped?.Invoke(this, e);
        }

        protected virtual void OnErrorOccurred(ErrorEventArgs e) {
            ErrorOccurred?.Invoke(this, e);
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (!_isDisposed) {
                if (disposing) {
                    // 清理托管资源
                    GuardStarted = null;
                    GuardStopped = null;
                    ErrorOccurred = null;
                }
                // 清理非托管资源（卸载钩子）
                Stop();
                _isDisposed = true;
            }
        }

        #endregion
    }

}