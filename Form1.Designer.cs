
namespace FuckRedSpider {
    partial class Form1 {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.listener_timer = new System.Windows.Forms.Timer(this.components);
            this.group_running_stat = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label_process_pid = new System.Windows.Forms.TextBox();
            this.label_running_stat = new System.Windows.Forms.Label();
            this.label_1 = new System.Windows.Forms.Label();
            this.name_conflict_err = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.process_name = new System.Windows.Forms.TextBox();
            this.auto_hide = new System.Windows.Forms.CheckBox();
            this.auto_kill = new System.Windows.Forms.CheckBox();
            this.topest_with_timer = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.group_log = new System.Windows.Forms.GroupBox();
            this.textbox_log = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.normal_window_class = new System.Windows.Forms.TextBox();
            this.full_window_class = new System.Windows.Forms.TextBox();
            this.n_w_c_l = new System.Windows.Forms.Label();
            this.f_w_c_l = new System.Windows.Forms.Label();
            this.group_running_stat.SuspendLayout();
            this.group_log.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // listener_timer
            // 
            this.listener_timer.Tick += new System.EventHandler(this.Listen);
            // 
            // group_running_stat
            // 
            this.group_running_stat.Controls.Add(this.label1);
            this.group_running_stat.Controls.Add(this.label_process_pid);
            this.group_running_stat.Controls.Add(this.label_running_stat);
            this.group_running_stat.Controls.Add(this.label_1);
            this.group_running_stat.Font = new System.Drawing.Font("黑体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.group_running_stat.Location = new System.Drawing.Point(43, 35);
            this.group_running_stat.Name = "group_running_stat";
            this.group_running_stat.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.group_running_stat.Size = new System.Drawing.Size(228, 162);
            this.group_running_stat.TabIndex = 0;
            this.group_running_stat.TabStop = false;
            this.group_running_stat.Text = "运行状态";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 98);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "进程pid：";
            // 
            // label_process_pid
            // 
            this.label_process_pid.AcceptsReturn = true;
            this.label_process_pid.Location = new System.Drawing.Point(91, 65);
            this.label_process_pid.Multiline = true;
            this.label_process_pid.Name = "label_process_pid";
            this.label_process_pid.ReadOnly = true;
            this.label_process_pid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.label_process_pid.Size = new System.Drawing.Size(131, 91);
            this.label_process_pid.TabIndex = 4;
            this.label_process_pid.Text = "None";
            // 
            // label_running_stat
            // 
            this.label_running_stat.AutoSize = true;
            this.label_running_stat.Location = new System.Drawing.Point(100, 30);
            this.label_running_stat.Name = "label_running_stat";
            this.label_running_stat.Size = new System.Drawing.Size(23, 15);
            this.label_running_stat.TabIndex = 2;
            this.label_running_stat.Text = "否";
            // 
            // label_1
            // 
            this.label_1.AutoSize = true;
            this.label_1.Location = new System.Drawing.Point(7, 30);
            this.label_1.Name = "label_1";
            this.label_1.Size = new System.Drawing.Size(87, 15);
            this.label_1.TabIndex = 0;
            this.label_1.Text = "是否运行：";
            // 
            // name_conflict_err
            // 
            this.name_conflict_err.AutoSize = true;
            this.name_conflict_err.Font = new System.Drawing.Font("黑体", 6.6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.name_conflict_err.ForeColor = System.Drawing.Color.Red;
            this.name_conflict_err.Location = new System.Drawing.Point(69, 47);
            this.name_conflict_err.Name = "name_conflict_err";
            this.name_conflict_err.Size = new System.Drawing.Size(221, 11);
            this.name_conflict_err.TabIndex = 5;
            this.name_conflict_err.Text = "目标进程与自身名称相同，禁用危险操作";
            this.name_conflict_err.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("黑体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(3, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "目标进程名";
            this.label2.DoubleClick += new System.EventHandler(this.label2_DoubleClick);
            // 
            // process_name
            // 
            this.process_name.Font = new System.Drawing.Font("黑体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.process_name.Location = new System.Drawing.Point(107, 14);
            this.process_name.Name = "process_name";
            this.process_name.Size = new System.Drawing.Size(183, 25);
            this.process_name.TabIndex = 3;
            this.process_name.Text = "REDAgent";
            this.process_name.TextChanged += new System.EventHandler(this.process_name_TextChanged);
            // 
            // auto_hide
            // 
            this.auto_hide.AutoSize = true;
            this.auto_hide.Font = new System.Drawing.Font("黑体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.auto_hide.Location = new System.Drawing.Point(6, 83);
            this.auto_hide.Name = "auto_hide";
            this.auto_hide.Size = new System.Drawing.Size(141, 19);
            this.auto_hide.TabIndex = 2;
            this.auto_hide.Text = "自动隐藏红蜘蛛";
            this.auto_hide.UseVisualStyleBackColor = true;
            this.auto_hide.CheckedChanged += new System.EventHandler(this.auto_hide_CheckedChanged);
            // 
            // auto_kill
            // 
            this.auto_kill.AutoSize = true;
            this.auto_kill.Font = new System.Drawing.Font("黑体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.auto_kill.Location = new System.Drawing.Point(6, 44);
            this.auto_kill.Name = "auto_kill";
            this.auto_kill.Size = new System.Drawing.Size(141, 19);
            this.auto_kill.TabIndex = 1;
            this.auto_kill.Text = "自动关闭红蜘蛛";
            this.auto_kill.UseVisualStyleBackColor = true;
            this.auto_kill.CheckedChanged += new System.EventHandler(this.auto_kill_CheckedChanged);
            // 
            // topest_with_timer
            // 
            this.topest_with_timer.AutoSize = true;
            this.topest_with_timer.Checked = true;
            this.topest_with_timer.CheckState = System.Windows.Forms.CheckState.Checked;
            this.topest_with_timer.Font = new System.Drawing.Font("黑体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.topest_with_timer.Location = new System.Drawing.Point(6, 6);
            this.topest_with_timer.Name = "topest_with_timer";
            this.topest_with_timer.Size = new System.Drawing.Size(125, 19);
            this.topest_with_timer.TabIndex = 0;
            this.topest_with_timer.Text = "应用强制置顶";
            this.topest_with_timer.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("黑体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(753, 527);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "关于";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // group_log
            // 
            this.group_log.Controls.Add(this.textbox_log);
            this.group_log.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.group_log.Location = new System.Drawing.Point(43, 203);
            this.group_log.Name = "group_log";
            this.group_log.Size = new System.Drawing.Size(477, 314);
            this.group_log.TabIndex = 3;
            this.group_log.TabStop = false;
            this.group_log.Text = "日志";
            // 
            // textbox_log
            // 
            this.textbox_log.Location = new System.Drawing.Point(7, 19);
            this.textbox_log.Multiline = true;
            this.textbox_log.Name = "textbox_log";
            this.textbox_log.ReadOnly = true;
            this.textbox_log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textbox_log.Size = new System.Drawing.Size(466, 289);
            this.textbox_log.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Font = new System.Drawing.Font("黑体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabControl1.Location = new System.Drawing.Point(522, 35);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(306, 486);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.topest_with_timer);
            this.tabPage1.Controls.Add(this.auto_kill);
            this.tabPage1.Controls.Add(this.auto_hide);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(296, 348);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "主选项";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.normal_window_class);
            this.tabPage2.Controls.Add(this.full_window_class);
            this.tabPage2.Controls.Add(this.n_w_c_l);
            this.tabPage2.Controls.Add(this.f_w_c_l);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.name_conflict_err);
            this.tabPage2.Controls.Add(this.process_name);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(298, 457);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "自定义";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // normal_window_class
            // 
            this.normal_window_class.Font = new System.Drawing.Font("黑体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.normal_window_class.Location = new System.Drawing.Point(107, 103);
            this.normal_window_class.Name = "normal_window_class";
            this.normal_window_class.Size = new System.Drawing.Size(183, 25);
            this.normal_window_class.TabIndex = 9;
            this.normal_window_class.Text = "RedEagle.Monitor";
            this.normal_window_class.TextChanged += new System.EventHandler(this.normal_window_class_TextChanged);
            // 
            // full_window_class
            // 
            this.full_window_class.Font = new System.Drawing.Font("黑体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.full_window_class.Location = new System.Drawing.Point(107, 62);
            this.full_window_class.Name = "full_window_class";
            this.full_window_class.Size = new System.Drawing.Size(183, 25);
            this.full_window_class.TabIndex = 8;
            this.full_window_class.Text = "DIBFullViewClass";
            this.full_window_class.TextChanged += new System.EventHandler(this.full_window_class_TextChanged);
            // 
            // n_w_c_l
            // 
            this.n_w_c_l.AutoSize = true;
            this.n_w_c_l.Font = new System.Drawing.Font("黑体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.n_w_c_l.Location = new System.Drawing.Point(3, 108);
            this.n_w_c_l.Name = "n_w_c_l";
            this.n_w_c_l.Size = new System.Drawing.Size(103, 15);
            this.n_w_c_l.TabIndex = 7;
            this.n_w_c_l.Text = "普通窗口类名";
            this.n_w_c_l.DoubleClick += new System.EventHandler(this.n_w_c_l_DoubleClick);
            // 
            // f_w_c_l
            // 
            this.f_w_c_l.AutoSize = true;
            this.f_w_c_l.Font = new System.Drawing.Font("黑体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.f_w_c_l.Location = new System.Drawing.Point(3, 67);
            this.f_w_c_l.Name = "f_w_c_l";
            this.f_w_c_l.Size = new System.Drawing.Size(103, 15);
            this.f_w_c_l.TabIndex = 6;
            this.f_w_c_l.Text = "全屏窗口类名";
            this.f_w_c_l.DoubleClick += new System.EventHandler(this.f_w_c_l_DoubleClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(838, 562);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.group_log);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.group_running_stat);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "红蜘蛛终结者";
            this.group_running_stat.ResumeLayout(false);
            this.group_running_stat.PerformLayout();
            this.group_log.ResumeLayout(false);
            this.group_log.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer listener_timer;
        private System.Windows.Forms.GroupBox group_running_stat;
        private System.Windows.Forms.Label label_1;
        private System.Windows.Forms.Label label_running_stat;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox topest_with_timer;
        private System.Windows.Forms.CheckBox auto_kill;
        private System.Windows.Forms.CheckBox auto_hide;
        private System.Windows.Forms.TextBox process_name;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox label_process_pid;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label name_conflict_err;
        private System.Windows.Forms.GroupBox group_log;
        private System.Windows.Forms.TextBox textbox_log;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label n_w_c_l;
        private System.Windows.Forms.Label f_w_c_l;
        private System.Windows.Forms.TextBox normal_window_class;
        private System.Windows.Forms.TextBox full_window_class;
    }
}

