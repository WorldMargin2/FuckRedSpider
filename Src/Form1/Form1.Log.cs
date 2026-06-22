using System;
using System.Windows.Forms;

namespace FuckRedSpider {
    public partial class Form1 {
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
    }
}
