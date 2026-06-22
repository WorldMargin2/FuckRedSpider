using System;
using System.Drawing;
using System.Windows.Forms;

namespace FuckRedSpider {
    public partial class Form1 {
        private void immersive_mode_CheckedChanged(object sender, EventArgs e) {
            if (immersive_mode.Checked) {
                target_panel.Visible = true;
                main_tabcontrol.Visible = false;
                this.FormBorderStyle = FormBorderStyle.None;
            } else {
                target_panel.Visible = false;
                main_tabcontrol.Visible = true;
                this.FormBorderStyle = FormBorderStyle.Sizable;
                _isResizing = false;  // 中断拖拽
                _isMoving = false;
                this.Cursor = Cursors.Default;  // 重置光标
            }
        }


        #region --沉浸模式拖拽操作--

        // 拖拽状态
        private bool _isResizing = false;
        private bool _isMoving = false;
        private Point _dragStartPos;
        private Rectangle _dragStartBounds;

        // ---- resize_window: 调整窗口大小（右下角拖拽）----

        private void resize_window_MouseDown(object sender, MouseEventArgs e) {
            if (!immersive_mode.Checked || e.Button != MouseButtons.Left) return;
            _isResizing = true;
            _dragStartPos = Control.MousePosition;  // 屏幕坐标
            _dragStartBounds = this.Bounds;
        }

        private void resize_window_MouseMove(object sender, MouseEventArgs e) {
            if (!_isResizing) return;
            int dx = Control.MousePosition.X - _dragStartPos.X;
            int dy = Control.MousePosition.Y - _dragStartPos.Y;
            int newW = Math.Max(100, _dragStartBounds.Width + dx);
            int newH = Math.Max(100, _dragStartBounds.Height + dy);

            // 保持比例
            if (keepRatio.Checked && keepRatio.Enabled) {
                float ratio = (float)ratioWidth.Value / (float)ratioHeight.Value;
                float currentRatio = (float)newW / newH;
                if (currentRatio > ratio)
                    newW = (int)(newH * ratio);
                else
                    newH = (int)(newW / ratio);
            }

            this.Size = new Size(newW, newH);
        }

        private void resize_window_MouseUp(object sender, MouseEventArgs e) {
            _isResizing = false;
            // 同步嵌入窗口
            if (attached_target.Checked && target_window != IntPtr.Zero)
                SetWindowPos(target_window, IntPtr.Zero, 0, 0, target_panel.Width, target_panel.Height, 0x0040);
        }

        // ---- move_window: 移动窗口位置 ----

        private void move_window_MouseDown(object sender, MouseEventArgs e) {
            if (!immersive_mode.Checked || e.Button != MouseButtons.Left) return;
            _isMoving = true;
            _dragStartPos = Control.MousePosition;  // 屏幕坐标
            _dragStartBounds = this.Bounds;
        }

        private void move_window_MouseMove(object sender, MouseEventArgs e) {
            if (!_isMoving) return;
            int dx = Control.MousePosition.X - _dragStartPos.X;
            int dy = Control.MousePosition.Y - _dragStartPos.Y;
            this.Location = new Point(_dragStartBounds.X + dx, _dragStartBounds.Y + dy);
        }

        private void move_window_MouseUp(object sender, MouseEventArgs e) {
            _isMoving = false;
        }
        #endregion
    }
}
