using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Process = System.Diagnostics.Process;

namespace FuckRedSpider {
    public partial class Form1 {
        // ==================== 屏幕 Hook 相关 ====================
        private ScreenHookManager _screenHookManager;
        private bool screenHookEnabled = false;

        // ==================== 屏幕 Hook 事件处理 ====================
        private void ScreenHookCheckBox_CheckedChanged(object sender, EventArgs e) {
            if (screenHookCheckBox.Checked) {
                // 启用屏幕 Hook
                StartScreenHook();
            }
            else {
                // 禁用屏幕 Hook
                StopScreenHook();
            }
        }

        private void StartScreenHook() {
            try {
                string targetProcess = process_name.Text.Trim();

                if (string.IsNullOrEmpty(targetProcess)) {
                    log.add("请先设置目标进程名");
                    screenHookCheckBox.Checked = false;
                    return;
                }

                // 检查目标进程是否运行
                Process[] processes = Process.GetProcessesByName(targetProcess);
                if (processes.Length == 0) {
                    log.add($"目标进程 {targetProcess} 未运行，等待进程启动...");
                    screenHookStatusLabel.Text = "等待目标进程...";
                    screenHookStatusLabel.ForeColor = Color.Orange;
                    screenHookEnabled = true;
                    return;
                }

                // 异步执行注入，避免UI卡顿
                screenHookStatusLabel.Text = "正在注入...";
                screenHookStatusLabel.ForeColor = Color.Orange;
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    bool success = _screenHookManager.Inject(targetProcess);
                    this.BeginInvoke(new Action(() =>
                    {
                        if (success) {
                            screenHookEnabled = true;
                            screenHookStatusLabel.Text = $"已注入 PID: {_screenHookManager.TargetPid}";
                            screenHookStatusLabel.ForeColor = Color.Green;
                            log.add("屏幕 Hook 已成功启动");

                            // 注入后立即启用钩子
                            _screenHookManager.SetHookEnabled(true);
                        }
                        else {
                            screenHookCheckBox.Checked = false;
                            screenHookStatusLabel.Text = "注入失败";
                            screenHookStatusLabel.ForeColor = Color.Red;
                        }
                    }));
                });
            }
            catch (Exception ex) {
                log.add("启动屏幕 Hook 失败: " + ex.Message);
                screenHookCheckBox.Checked = false;
            }
        }

        private void StopScreenHook() {
            try {
                if (_screenHookManager != null && _screenHookManager.IsInjected) {
                    _screenHookManager.SetHookEnabled(false);
                    log.add("屏幕 Hook 已停止（DLL 仍保留在目标进程中）");
                }
                screenHookEnabled = false;
                screenHookStatusLabel.Text = "未激活";
                screenHookStatusLabel.ForeColor = Color.Gray;
            }
            catch (Exception ex) {
                log.add("停止屏幕 Hook 失败: " + ex.Message);
            }
        }

        // 尝试自动注入屏幕 Hook
        private void TryInjectScreenHook() {
            try {
                string targetProcess = process_name.Text.Trim();

                Process[] processes = Process.GetProcessesByName(targetProcess);
                if (processes.Length > 0) {
                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        bool success = _screenHookManager.Inject(targetProcess);
                        this.BeginInvoke(new Action(() =>
                        {
                            if (success) {
                                screenHookStatusLabel.Text = $"已注入 PID: {_screenHookManager.TargetPid}";
                                screenHookStatusLabel.ForeColor = Color.Green;
                                log.add("屏幕 Hook 自动注入成功");
                            }
                            else {
                                screenHookStatusLabel.Text = "注入失败";
                                screenHookStatusLabel.ForeColor = Color.Red;
                                screenHookEnabled = false;
                                screenHookCheckBox.Checked = false;
                            }
                        }));
                    });
                }
            }
            catch (Exception ex) {
                log.add("自动注入屏幕 Hook 失败: " + ex.Message);
            }
        }
    }
}
