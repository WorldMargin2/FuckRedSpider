using System;
using System.Windows.Forms;

namespace FuckRedSpider {
    static class Program {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);
            //判断当前登录用户是否为管理员
            if (!principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator)) {
                RunAsAdmin(args);
                return;
            } else {
                Application.Run(new Form1(args));
            }
        }

        static void RunAsAdmin(string[] args) {
            System.Diagnostics.ProcessStartInfo proc = new System.Diagnostics.ProcessStartInfo();
            proc.UseShellExecute = true;
            proc.WorkingDirectory = Environment.CurrentDirectory;
            proc.FileName = Application.ExecutablePath;
            proc.Verb = "runas";
            proc.Arguments = string.Join(" ", args);
            try {
                System.Diagnostics.Process.Start(proc);
            } catch {
            }
        }
    }
}
