using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace FuckRedSpider {
    public class OverlayForm : Form {
        private Rectangle highlight = Rectangle.Empty;
        private float penWidth = 6f;
        private Pen pen;

        public OverlayForm() {
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;
            this.Bounds = System.Windows.Forms.SystemInformation.VirtualScreen;
            this.AllowTransparency = true;
            this.BackColor = Color.Gray;
            this.TransparencyKey = Color.White;
            this.Opacity = 0.5;
            this.DoubleBuffered = true;
            pen = new Pen(Color.Aqua, penWidth);
            pen.LineJoin = LineJoin.Round;
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
        }
        protected override CreateParams CreateParams {
            get {
                var cp = base.CreateParams;
                const int WS_EX_TRANSPARENT = 0x20;
                const int WS_EX_NOACTIVATE = 0x08000000;
                cp.ExStyle |= WS_EX_TRANSPARENT | WS_EX_NOACTIVATE;
                return cp;
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowThreadProcessId(System.IntPtr hWnd, out int lpdwProcessId);

        public void HighlightRect(Rectangle r) {
            highlight = r;
            if (!this.Visible) {
                Show();
            }
            this.Invalidate();
        }

        public void HighlightRect(Rectangle r, System.IntPtr hwnd) {
            try {
                if (hwnd != System.IntPtr.Zero) {
                    int pid;
                    GetWindowThreadProcessId(hwnd, out pid);
                    if (pid == System.Diagnostics.Process.GetCurrentProcess().Id) {
                        Clear();
                        return;
                    }
                }
            } catch { }

            highlight = r;
            if (!this.Visible) {
                Show();
            }
            this.Invalidate();
        }

        public void Clear() {
            highlight = Rectangle.Empty;
            try {
                this.Invalidate();
            } catch { }
            try {
                if (this.Visible) this.Hide();
            } catch { }
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            // Fill the whole form with a semi-transparent black to dim the screen
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            // use a semi-transparent gray to dim the desktop
            using (var dim = new SolidBrush(Color.FromArgb(160, 128, 128, 128))) {
                e.Graphics.FillRectangle(dim, this.ClientRectangle);
            }

            if (!highlight.IsEmpty) {
                var rect = highlight;
                using (var clear = new SolidBrush(this.TransparencyKey)) {
                    e.Graphics.FillRectangle(clear, rect);
                }

                int inflate = (int)Math.Ceiling(penWidth / 2.0);
                var borderRect = rect;
                borderRect.Inflate(inflate, inflate);
                e.Graphics.DrawRectangle(pen, borderRect);
            }
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                try { pen?.Dispose(); } catch { }
            }
            base.Dispose(disposing);
        }
    }
}
