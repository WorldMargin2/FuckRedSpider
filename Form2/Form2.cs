using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace FuckRedSpider {
    public partial class Form2 : Form {
        public Form2() {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e) {

        }

        private void github_link_click(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start("https://github.com/WorldMargin2/FuckRedSpider");
        }

        private void official_link_click(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start("http://worldmargin.top");
        }


        private void bilibili_link_click(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start("https://space.bilibili.com/3546734785464807");
        }

    }
}
