using KeyboardHookGuard;
using MouseHookGuard;
using System;
using System.IO;
using System.Windows.Forms;
using KeysBinding;

namespace FuckRedSpider {
    public partial class Form1 {
        private readonly GlobalKeyboardHookGuard _keyboardGuard;
        private readonly GlobalMouseHookGuard _mouseGuard;
        private bool keyboardGuardRunning = false;
        KeyGridBinding keyGridBinding;

        void hotKeyInit() {
            keyGridBinding = new KeyGridBinding(dataGridView1);
            keyGridBinding.startListen();
            this.FormClosed += (s, e) => {
                keyGridBinding.stopListen();
            };
            keyGridBinding.bind("置顶", 
                () => { 
                    this.topest_with_timer.Checked = !this.topest_with_timer.Checked;
                },
                Keys.Control, Keys.U
            );
            keyGridBinding.bind("自动关闭目标窗体",
                () => { 
                    this.auto_kill.Checked = !this.auto_kill.Checked;
                },
                Keys.Control, Keys.I
            );
            keyGridBinding.bind("自动隐藏目标窗体", 
                () => { 
                    this.auto_hide.Checked = !this.auto_hide.Checked;
                },
                Keys.Control, Keys.O
            );
            keyGridBinding.bind("劫持目标窗体",
                () => { 
                    this.attached_target.Checked = !this.attached_target.Checked;
                },
                Keys.Control, Keys.P
            );
            keyGridBinding.bind("捕获当前窗体",
                () => { 
                    if (!isCapturing) {
                        // 模拟点击捕获按钮以保持逻辑一致
                        capture_drag_area_MouseDown(this, null);
                    } else {
                        capture_drag_area_MouseUp(this, null);
                        insert_captured_Click(this, null);
                    }
                },
                Keys.Control, Keys.OemOpenBrackets
            );
            keyGridBinding.bind("沉浸窗口开关",
                () => {
                    this.immersive_mode.Checked = !this.immersive_mode.Checked;
                    immersive_mode_CheckedChanged(this, null);
                },
                Keys.Control, Keys.OemCloseBrackets
            );
            keyGridBinding.bind("教师监控防护",
                () => {
                    if (screenHookCheckBox != null)
                        screenHookCheckBox.Checked = !screenHookCheckBox.Checked;
                },
                Keys.Control, Keys.H
            );
            keyGridBinding.initGridData();
        }

        void KeyboardGuard_GuardStarted(object sender, EventArgs e) {
            keyboardGuardRunning = true;
        }
        void KeyboardGuard_GuardStopped(object sender, EventArgs e) {
            keyboardGuardRunning = false;
        }
        void KeyboardGuard_ErrorOccurred(object sender, ErrorEventArgs e) {
            log.add("键盘守护出错: " + e.GetException().Message);
        }

        void MouseGuard_GuardStarted(object sender, EventArgs e) {
            log.add("鼠标守护启动");
        }
        void MouseGuard_GuardStopped(object sender, EventArgs e) {
            log.add("鼠标守护停止");
        }
        void MouseGuard_ErrorOccurred(object sender, ErrorEventArgs e) {
            log.add("鼠标守护出错: " + e.GetException().Message);
        }
        void MouseGuard_MouseCorrected(object sender, MouseCorrectionEventArgs e) {
            log.add($"鼠标纠正: ({e.BlockedX},{e.BlockedY}) -> ({e.CorrectedX},{e.CorrectedY})");
        }

        void startKeyboardGuard() {
            if (keyboardGuardRunning) return;
            _keyboardGuard.Start();
            _mouseGuard.Start();
            keyboardGuardRunning= true;
            log.add("键盘守护启动");
        }

        void stopKeyboardGuard() {
            if (!keyboardGuardRunning) return;
            _keyboardGuard.Stop();
            _mouseGuard.Stop();
            keyboardGuardRunning = false;
            log.add("键盘守护停止");
        }

        void restartKeyboardGuard() {
            if (keyboardGuardRunning) {
                _keyboardGuard.Stop();
                _mouseGuard.Stop();
            }
            _keyboardGuard.Start();
            _mouseGuard.Start();
        }
    }
}
