using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace FuckRedSpider
{
    /// <summary>
    /// 屏幕 Hook 管理器 (C++ DLL 版本)
    /// 使用 CreateRemoteThread 注入 C++ Hook DLL
    /// DLL 内嵌 ScreenGuard 图片，无需外部文件依赖
    /// </summary>
    public class ScreenHookManager : IDisposable
    {
        #region P/Invoke 声明

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint dwFreeType);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out uint lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandleW([MarshalAs(UnmanagedType.LPWStr)] string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint GetLastError();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadLibraryExW([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, IntPtr hFile, uint dwFlags);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetExitCodeThread(IntPtr hThread, out uint lpExitCode);

        // 进程访问权限
        private const int PROCESS_CREATE_THREAD = 0x0002;
        private const int PROCESS_QUERY_INFORMATION = 0x0400;
        private const int PROCESS_VM_OPERATION = 0x0008;
        private const int PROCESS_VM_WRITE = 0x0020;
        private const int PROCESS_VM_READ = 0x0010;
        private const int PROCESS_ALL_ACCESS = 0x001F0FFF;

        // 内存分配类型
        private const uint MEM_COMMIT = 0x1000;
        private const uint MEM_RESERVE = 0x2000;
        private const uint MEM_RELEASE = 0x8000;

        // 内存保护常量
        private const uint PAGE_READWRITE = 0x04;
        private const uint PAGE_EXECUTE_READWRITE = 0x40;

        // 等待常量
        private const uint WAIT_OBJECT_0 = 0x00000000;
        private const uint WAIT_TIMEOUT = 0x00000102;
        private const uint INFINITE = 0xFFFFFFFF;

        #endregion

        #region 私有字段

        private int _targetPid = -1;
        private string _targetProcessName;
        private IntPtr _hProcess = IntPtr.Zero;
        private IntPtr _remoteDllBase = IntPtr.Zero;
        private bool _isInjected = false;
        private string _dllPath;
        private bool _hookEnabled = false;
        private Action<string> _logCallback;
        private Thread _monitorThread;
        private bool _running = false;
        private IntPtr _hRemoteDll = IntPtr.Zero;
        private SynchronizationContext _syncContext;
        private int _setHookEnabledRva = 0;
        private IntPtr _setHookEnabledRemoteAddr = IntPtr.Zero;

        /// <summary>
        /// Hook 自动卸载事件（进程退出或目标进程名改变时触发）
        /// </summary>
        public event EventHandler OnHookAutoUnload;

        #endregion

        #region 构造函数

        public ScreenHookManager(Action<string> logCallback)
        {
            _syncContext = SynchronizationContext.Current ?? new SynchronizationContext();
            _logCallback = logCallback;
            ExtractDllFromResource();
            CacheExportRvaFromFile();
        }

        /// <summary>
        /// 从 PE 文件解析导出表获取 SetHookEnabled RVA（不加载 DLL，零副作用）
        /// </summary>
        private void CacheExportRvaFromFile()
        {
            try
            {
                byte[] data = File.ReadAllBytes(_dllPath);
                if (data.Length < 0x100) return;

                // DOS header → PE offset
                int peOffset = BitConverter.ToInt32(data, 0x3C);
                if (peOffset + 0x18 > data.Length) return;

                // PE signature
                if (BitConverter.ToInt32(data, peOffset) != 0x4550) return; // "PE\0\0"

                int optHeaderOffset = peOffset + 24;
                int magic = BitConverter.ToInt16(data, optHeaderOffset);
                int peOptionalSize = (magic == 0x10B) ? 224 : (magic == 0x20B) ? 240 : 0;
                if (peOptionalSize == 0) return;

                // Number of RVA and sizes (at offset 92 in optional header)
                int numRvaAndSizes = BitConverter.ToInt32(data, optHeaderOffset + 92);
                if (numRvaAndSizes > 16) numRvaAndSizes = 16;
                if (numRvaAndSizes == 0) return;

                // Data directories start at offset 96 from optional header
                int dataDirOffset = optHeaderOffset + 96;

                // Export directory is DataDirectory[0]
                int exportRva = BitConverter.ToInt32(data, dataDirOffset);
                int exportSize = BitConverter.ToInt32(data, dataDirOffset + 4);
                if (exportRva == 0 || exportSize == 0) return;

                // Section headers start right after all data directories
                int numSections = BitConverter.ToInt16(data, peOffset + 6);
                int sectionOffset = dataDirOffset + (numRvaAndSizes * 8);

                // RVA → file offset
                int RvaToOffset(int rva)
                {
                    for (int i = 0; i < numSections; i++)
                    {
                        int sec = sectionOffset + i * 40;
                        int secVirtAddr = BitConverter.ToInt32(data, sec + 12);
                        int secRawSize = BitConverter.ToInt32(data, sec + 8);
                        int secRawOffset = BitConverter.ToInt32(data, sec + 20);
                        if (rva >= secVirtAddr && rva < secVirtAddr + secRawSize)
                            return rva - secVirtAddr + secRawOffset;
                    }
                    return -1;
                }

                int exportFileOffset = RvaToOffset(exportRva);
                if (exportFileOffset < 0) return;

                // Parse export directory
                int numNames = BitConverter.ToInt32(data, exportFileOffset + 24);
                int addrOfFuncs = RvaToOffset(BitConverter.ToInt32(data, exportFileOffset + 28));
                int addrOfNames = RvaToOffset(BitConverter.ToInt32(data, exportFileOffset + 32));
                int addrOfOrdinals = RvaToOffset(BitConverter.ToInt32(data, exportFileOffset + 36));
                if (addrOfFuncs < 0 || addrOfNames < 0 || addrOfOrdinals < 0) return;

                for (int i = 0; i < numNames; i++)
                {
                    int nameRva = BitConverter.ToInt32(data, addrOfNames + i * 4);
                    int nameOffset = RvaToOffset(nameRva);
                    if (nameOffset < 0) continue;

                    // Read null-terminated string
                    int end = nameOffset;
                    while (end < data.Length && data[end] != 0) end++;
                    string name = System.Text.Encoding.ASCII.GetString(data, nameOffset, end - nameOffset);

                    // Match exact name (32-bit cdecl exports as "SetHookEnabled" in PE)
                    if (name == "SetHookEnabled")
                    {
                        int ordinal = BitConverter.ToInt16(data, addrOfOrdinals + i * 2);
                        int funcRva = BitConverter.ToInt32(data, addrOfFuncs + ordinal * 4);
                        _setHookEnabledRva = funcRva;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                SafeLog($"解析 DLL 失败: {ex.Message}");
            }
        }

        private void SafeLog(string msg)
        {
            if (_logCallback == null) return;
            try
            {
                if (SynchronizationContext.Current == _syncContext)
                    _logCallback(msg);
                else
                    _syncContext.Post(_ => _logCallback(msg), null);
            }
            catch
            {
                // 忽略关闭时的日志异常
            }
        }

        /// <summary>
        /// 从嵌入资源提取 DLL 到临时目录
        /// </summary>
        private void ExtractDllFromResource()
        {
            string tempDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FuckRedSpider");
            Directory.CreateDirectory(tempDir);

            _dllPath = Path.Combine(tempDir, "ScreenHook.dll");

            // 如果文件已存在且被锁定，尝试删除
            if (File.Exists(_dllPath))
            {
                try
                {
                    // 尝试删除旧文件
                    File.Delete(_dllPath);
                }
                catch
                {
                    // 文件被锁定，使用新文件名
                    _dllPath = Path.Combine(tempDir, $"ScreenHook_{DateTime.Now.Ticks}.dll");
                }
            }

            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("FuckRedSpider.Resources.ScreenHook.dll"))
            {
                if (stream == null)
                {
                    SafeLog("错误: 未找到嵌入资源 ScreenHook.dll");
                    return;
                }

                using (var fileStream = File.Create(_dllPath))
                {
                    stream.CopyTo(fileStream);
                }
            }
            SafeLog($"DLL 已提取到: {_dllPath}");
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 注入 Hook 到目标进程
        /// </summary>
        /// <param name="processName">目标进程名</param>
        /// <returns>是否成功注入</returns>
        public bool Inject(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length == 0)
            {
                SafeLog($"未找到目标进程: {processName}");
                return false;
            }

            _targetPid = processes[0].Id;
            _targetProcessName = processName;
            SafeLog($"找到目标进程: {processName}");

            if (!File.Exists(_dllPath))
            {
                SafeLog("DLL 文件不存在");
                return false;
            }

            _hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, _targetPid);
            if (_hProcess == IntPtr.Zero)
            {
                SafeLog($"无法打开目标进程");
                return false;
            }

            // 注入
            if (!InjectDll())
            {
                CloseHandle(_hProcess);
                _hProcess = IntPtr.Zero;
                return false;
            }

            _isInjected = true;
            _hookEnabled = true;
            SafeLog($"注入成功");

            // 启动进程监控线程
            StartMonitorThread();

            return true;
        }

        /// <summary>
        /// 启动进程监控线程
        /// </summary>
        private void StartMonitorThread()
        {
            _running = true;
            _monitorThread = new Thread(MonitorProcess);
            _monitorThread.IsBackground = true;
            _monitorThread.Start();
        }

        /// <summary>
        /// 监控目标进程状态
        /// </summary>
        private void MonitorProcess()
        {
            while (_running && _isInjected)
            {
                Thread.Sleep(1000);

                if (!_running || !_isInjected)
                    break;

                // 检查目标进程是否还在运行
                try
                {
                    Process process = Process.GetProcessById(_targetPid);
                    if (process.HasExited)
                    {
                        SafeLog($"目标进程已退出，自动卸载 Hook");
                        AutoUnload();
                        break;
                    }
                }
                catch
                {
                    SafeLog($"目标进程已退出，自动卸载 Hook");
                    AutoUnload();
                    break;
                }
            }
        }

        /// <summary>
        /// 自动卸载（触发事件通知外部关闭选项）
        /// </summary>
        private void AutoUnload()
        {
            _isInjected = false;
            _hookEnabled = false;
            Cleanup();
            OnHookAutoUnload?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 手动卸载 Hook
        /// </summary>
        public bool Unload()
        {
            if (!_isInjected)
                return true;

            _running = false;
            
            if (_monitorThread != null && _monitorThread.IsAlive)
            {
                _monitorThread.Join(1000);
            }

            if (!UnloadDll())
            {
                SafeLog("卸载失败");
                return false;
            }

            _isInjected = false;
            _hookEnabled = false;
            Cleanup();
            SafeLog("已卸载");
            return true;
        }

        /// <summary>
        /// 注入 DLL 到目标进程（若已加载则直接获取句柄）
        /// </summary>
        private bool InjectDll()
        {
            IntPtr hKernel32 = GetModuleHandleW("kernel32.dll");
            if (hKernel32 == IntPtr.Zero)
            {
                SafeLog("无法连接目标进程");
                return false;
            }

            // 第一步：检查 DLL 是否已在目标进程中
            IntPtr pGetModuleHandleW = GetProcAddress(hKernel32, "GetModuleHandleW");
            if (pGetModuleHandleW != IntPtr.Zero)
            {
                byte[] dllNameBytes = Encoding.Unicode.GetBytes("ScreenHook.dll\0");
                uint nameSize = (uint)dllNameBytes.Length;
                IntPtr pRemoteName = VirtualAllocEx(_hProcess, IntPtr.Zero, nameSize, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
                if (pRemoteName != IntPtr.Zero)
                {
                    uint bw;
                    WriteProcessMemory(_hProcess, pRemoteName, dllNameBytes, nameSize, out bw);
                    IntPtr hCheck = CreateRemoteThread(_hProcess, IntPtr.Zero, 0, pGetModuleHandleW, pRemoteName, 0, IntPtr.Zero);
                    if (hCheck != IntPtr.Zero)
                    {
                        uint checkWait = WaitForSingleObject(hCheck, 5000);
                        uint checkResult = 0;
                        if (checkWait == WAIT_OBJECT_0)
                            GetExitCodeThread(hCheck, out checkResult);
                        CloseHandle(hCheck);
                        if (checkResult != 0 && checkResult != 259)
                        {
                            _hRemoteDll = new IntPtr(checkResult);
                            VirtualFreeEx(_hProcess, pRemoteName, 0, MEM_RELEASE);
                            SafeLog("ScreenHook.dll 已存在");
                            CacheRemoteFuncAddress();
                            return true;
                        }
                    }
                    VirtualFreeEx(_hProcess, pRemoteName, 0, MEM_RELEASE);
                }
            }

            // 第二步：DLL 未加载，使用 LoadLibraryW 注入
            IntPtr pLoadLibraryW = GetProcAddress(hKernel32, "LoadLibraryW");
            if (pLoadLibraryW == IntPtr.Zero)
            {
                SafeLog("注入失败");
                return false;
            }

            byte[] dllPathBytes = Encoding.Unicode.GetBytes(_dllPath + "\0");
            uint pathSize = (uint)dllPathBytes.Length;

            IntPtr pRemotePath = VirtualAllocEx(_hProcess, IntPtr.Zero, pathSize, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
            if (pRemotePath == IntPtr.Zero)
            {
                SafeLog("注入失败");
                return false;
            }

            uint bytesWritten;
            if (!WriteProcessMemory(_hProcess, pRemotePath, dllPathBytes, pathSize, out bytesWritten))
            {
                SafeLog("注入失败");
                VirtualFreeEx(_hProcess, pRemotePath, 0, MEM_RELEASE);
                return false;
            }

            IntPtr hThread = CreateRemoteThread(_hProcess, IntPtr.Zero, 0, pLoadLibraryW, pRemotePath, 0, IntPtr.Zero);
            if (hThread == IntPtr.Zero)
            {
                SafeLog("注入失败");
                VirtualFreeEx(_hProcess, pRemotePath, 0, MEM_RELEASE);
                return false;
            }

            uint waitResult = WaitForSingleObject(hThread, 5000);
            if (waitResult != WAIT_OBJECT_0)
            {
                SafeLog("注入超时");
                CloseHandle(hThread);
                VirtualFreeEx(_hProcess, pRemotePath, 0, MEM_RELEASE);
                return false;
            }

            uint exitCode;
            GetExitCodeThread(hThread, out exitCode);
            _hRemoteDll = new IntPtr(exitCode);

            CloseHandle(hThread);
            VirtualFreeEx(_hProcess, pRemotePath, 0, MEM_RELEASE);

            if (_hRemoteDll == IntPtr.Zero)
            {
                return true;
            }

            CacheRemoteFuncAddress();
            return true;
        }

        /// <summary>
        /// 在目标进程中获取 SetHookEnabled 函数地址并缓存
        /// </summary>
        private void CacheRemoteFuncAddress()
        {
            if (_hProcess == IntPtr.Zero || _hRemoteDll == IntPtr.Zero) return;

            if (_setHookEnabledRva == 0)
                CacheExportRvaFromFile();

            if (_setHookEnabledRva != 0)
            {
                _setHookEnabledRemoteAddr = new IntPtr(_hRemoteDll.ToInt64() + _setHookEnabledRva);
            }

            if (_setHookEnabledRemoteAddr == IntPtr.Zero)
                SafeLog("获取函数地址失败");
        }

        /// <summary>
        /// 卸载远程 DLL
        /// </summary>
        private bool UnloadDll()
        {
            if (_hProcess == IntPtr.Zero || _hRemoteDll == IntPtr.Zero)
                return true;

            IntPtr hKernel32 = GetModuleHandleW("kernel32.dll");
            if (hKernel32 == IntPtr.Zero)
                return false;

            IntPtr pFreeLibrary = GetProcAddress(hKernel32, "FreeLibrary");
            if (pFreeLibrary == IntPtr.Zero)
                return false;

            IntPtr hThread = CreateRemoteThread(_hProcess, IntPtr.Zero, 0, pFreeLibrary, _hRemoteDll, 0, IntPtr.Zero);
            if (hThread == IntPtr.Zero)
                return false;

            WaitForSingleObject(hThread, 3000);
            CloseHandle(hThread);

            _hRemoteDll = IntPtr.Zero;
            return true;
        }

        /// <summary>
        /// 启用/禁用 Hook（通过远程线程调用 DLL 导出函数，使用缓存地址）
        /// </summary>
        public void SetHookEnabled(bool enabled)
        {
            _hookEnabled = enabled;

            if (_hProcess != IntPtr.Zero && _setHookEnabledRemoteAddr != IntPtr.Zero)
            {
                try
                {
                    IntPtr pRemoteArg = VirtualAllocEx(_hProcess, IntPtr.Zero, 4, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
                    if (pRemoteArg != IntPtr.Zero)
                    {
                        byte[] argBytes = BitConverter.GetBytes(enabled ? 1 : 0);
                        uint bytesWritten;
                        WriteProcessMemory(_hProcess, pRemoteArg, argBytes, 4, out bytesWritten);

                        IntPtr hThread = CreateRemoteThread(_hProcess, IntPtr.Zero, 0, _setHookEnabledRemoteAddr, pRemoteArg, 0, IntPtr.Zero);
                        if (hThread != IntPtr.Zero)
                        {
                            WaitForSingleObject(hThread, 3000);
                            CloseHandle(hThread);
                        }
                        else
                        {
                            SafeLog("调用失败");
                        }
                        VirtualFreeEx(_hProcess, pRemoteArg, 0, MEM_RELEASE);
                    }
                }
                catch (Exception ex)
                {
                    SafeLog($"调用异常: {ex.Message}");
                }
            }

            SafeLog($"Hook {(enabled ? "已启用" : "已禁用")}");
        }

        /// <summary>
        /// 检查是否已注入
        /// </summary>
        public bool IsInjected => _isInjected;

        /// <summary>
        /// 获取目标进程 PID
        /// </summary>
        public int TargetPid => _targetPid;

        /// <summary>
        /// 获取目标进程名
        /// </summary>
        public string TargetProcessName => _targetProcessName;

        #endregion

        #region 清理

        private void Cleanup()
        {
            if (_hProcess != IntPtr.Zero)
            {
                CloseHandle(_hProcess);
                _hProcess = IntPtr.Zero;
            }
            _targetPid = -1;
            _targetProcessName = null;
        }

        public void Dispose()
        {
            _running = false;
            
            if (_monitorThread != null && _monitorThread.IsAlive)
            {
                _monitorThread.Join(1000);
            }

            Unload();
            Cleanup();
        }

        #endregion
    }
}