
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
            this.config_TabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.attached_target = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.normal_window_class = new System.Windows.Forms.TextBox();
            this.full_window_class = new System.Windows.Forms.TextBox();
            this.n_w_c_l = new System.Windows.Forms.Label();
            this.f_w_c_l = new System.Windows.Forms.Label();
            this.target_panel = new System.Windows.Forms.Panel();
            this.main_tabcontrol = new System.Windows.Forms.TabControl();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.group_log = new System.Windows.Forms.GroupBox();
            this.textbox_log = new System.Windows.Forms.TextBox();
            this.attach_page = new System.Windows.Forms.TabPage();
            this.group_running_stat.SuspendLayout();
            this.config_TabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.main_tabcontrol.SuspendLayout();
            this.tabPage7.SuspendLayout();
            this.group_log.SuspendLayout();
            this.attach_page.SuspendLayout();
            this.SuspendLayout();
            // 
            // listener_timer
            // 
            this.listener_timer.Tick += new System.EventHandler(this.Listen);
            // 
            // group_running_stat
            // 
            this.group_running_stat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.group_running_stat.Controls.Add(this.label1);
            this.group_running_stat.Controls.Add(this.label_process_pid);
            this.group_running_stat.Controls.Add(this.label_running_stat);
            this.group_running_stat.Controls.Add(this.label_1);
            this.group_running_stat.Font = new System.Drawing.Font("阿里妈妈数黑体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.group_running_stat.Location = new System.Drawing.Point(7, 7);
            this.group_running_stat.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.group_running_stat.MinimumSize = new System.Drawing.Size(287, 152);
            this.group_running_stat.Name = "group_running_stat";
            this.group_running_stat.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.group_running_stat.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.group_running_stat.Size = new System.Drawing.Size(552, 152);
            this.group_running_stat.TabIndex = 0;
            this.group_running_stat.TabStop = false;
            this.group_running_stat.Text = "运行状态";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("阿里妈妈数黑体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(303, 84);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 18);
            this.label1.TabIndex = 1;
            this.label1.Text = "进程pid：";
            // 
            // label_process_pid
            // 
            this.label_process_pid.AcceptsReturn = true;
            this.label_process_pid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label_process_pid.Font = new System.Drawing.Font("阿里妈妈数黑体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_process_pid.Location = new System.Drawing.Point(398, 31);
            this.label_process_pid.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.label_process_pid.Multiline = true;
            this.label_process_pid.Name = "label_process_pid";
            this.label_process_pid.ReadOnly = true;
            this.label_process_pid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.label_process_pid.Size = new System.Drawing.Size(147, 108);
            this.label_process_pid.TabIndex = 4;
            this.label_process_pid.Text = "None";
            // 
            // label_running_stat
            // 
            this.label_running_stat.AutoSize = true;
            this.label_running_stat.Font = new System.Drawing.Font("阿里妈妈数黑体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_running_stat.ForeColor = System.Drawing.Color.Red;
            this.label_running_stat.Location = new System.Drawing.Point(94, 59);
            this.label_running_stat.Name = "label_running_stat";
            this.label_running_stat.Size = new System.Drawing.Size(23, 18);
            this.label_running_stat.TabIndex = 2;
            this.label_running_stat.Text = "否";
            // 
            // label_1
            // 
            this.label_1.AutoSize = true;
            this.label_1.Font = new System.Drawing.Font("阿里妈妈数黑体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_1.Location = new System.Drawing.Point(7, 59);
            this.label_1.Name = "label_1";
            this.label_1.Size = new System.Drawing.Size(83, 18);
            this.label_1.TabIndex = 0;
            this.label_1.Text = "是否运行：";
            // 
            // name_conflict_err
            // 
            this.name_conflict_err.AutoSize = true;
            this.name_conflict_err.Font = new System.Drawing.Font("得意黑", 6.6F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.name_conflict_err.ForeColor = System.Drawing.Color.Red;
            this.name_conflict_err.Location = new System.Drawing.Point(78, 56);
            this.name_conflict_err.Name = "name_conflict_err";
            this.name_conflict_err.Size = new System.Drawing.Size(169, 14);
            this.name_conflict_err.TabIndex = 5;
            this.name_conflict_err.Text = "目标进程与自身名称相同，禁用危险操作";
            this.name_conflict_err.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("阿里妈妈数黑体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(3, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 18);
            this.label2.TabIndex = 4;
            this.label2.Text = "目标进程名";
            this.label2.DoubleClick += new System.EventHandler(this.label2_DoubleClick);
            // 
            // process_name
            // 
            this.process_name.Font = new System.Drawing.Font("阿里妈妈数黑体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.process_name.Location = new System.Drawing.Point(120, 17);
            this.process_name.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.process_name.Name = "process_name";
            this.process_name.Size = new System.Drawing.Size(205, 26);
            this.process_name.TabIndex = 3;
            this.process_name.Text = "REDAgent";
            this.process_name.TextChanged += new System.EventHandler(this.process_name_TextChanged);
            // 
            // auto_hide
            // 
            this.auto_hide.AutoSize = true;
            this.auto_hide.Font = new System.Drawing.Font("得意黑", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.auto_hide.Location = new System.Drawing.Point(88, 124);
            this.auto_hide.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.auto_hide.Name = "auto_hide";
            this.auto_hide.Size = new System.Drawing.Size(114, 22);
            this.auto_hide.TabIndex = 2;
            this.auto_hide.Text = "自动隐藏红蜘蛛";
            this.auto_hide.UseVisualStyleBackColor = true;
            this.auto_hide.CheckedChanged += new System.EventHandler(this.auto_hide_CheckedChanged);
            // 
            // auto_kill
            // 
            this.auto_kill.AutoSize = true;
            this.auto_kill.Font = new System.Drawing.Font("得意黑", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.auto_kill.Location = new System.Drawing.Point(88, 77);
            this.auto_kill.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.auto_kill.Name = "auto_kill";
            this.auto_kill.Size = new System.Drawing.Size(114, 22);
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
            this.topest_with_timer.Font = new System.Drawing.Font("得意黑", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.topest_with_timer.Location = new System.Drawing.Point(88, 31);
            this.topest_with_timer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.topest_with_timer.Name = "topest_with_timer";
            this.topest_with_timer.Size = new System.Drawing.Size(102, 22);
            this.topest_with_timer.TabIndex = 0;
            this.topest_with_timer.Text = "应用强制置顶";
            this.topest_with_timer.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Font = new System.Drawing.Font("黑体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(858, 646);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(84, 28);
            this.button1.TabIndex = 2;
            this.button1.Text = "关于";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // config_TabControl
            // 
            this.config_TabControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.config_TabControl.Controls.Add(this.tabPage1);
            this.config_TabControl.Controls.Add(this.tabPage2);
            this.config_TabControl.Font = new System.Drawing.Font("得意黑", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.config_TabControl.Location = new System.Drawing.Point(561, 4);
            this.config_TabControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.config_TabControl.Name = "config_TabControl";
            this.config_TabControl.SelectedIndex = 0;
            this.config_TabControl.Size = new System.Drawing.Size(344, 580);
            this.config_TabControl.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.topest_with_timer);
            this.tabPage1.Controls.Add(this.auto_kill);
            this.tabPage1.Controls.Add(this.attached_target);
            this.tabPage1.Controls.Add(this.auto_hide);
            this.tabPage1.Location = new System.Drawing.Point(4, 27);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Size = new System.Drawing.Size(336, 549);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "主选项";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // attached_target
            // 
            this.attached_target.AutoSize = true;
            this.attached_target.Font = new System.Drawing.Font("得意黑", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.attached_target.Location = new System.Drawing.Point(88, 166);
            this.attached_target.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.attached_target.Name = "attached_target";
            this.attached_target.Size = new System.Drawing.Size(78, 22);
            this.attached_target.TabIndex = 2;
            this.attached_target.Text = "劫持窗口";
            this.attached_target.UseVisualStyleBackColor = true;
            this.attached_target.CheckedChanged += new System.EventHandler(this.attached_target_CheckedChanged);
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
            this.tabPage2.Location = new System.Drawing.Point(4, 27);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage2.Size = new System.Drawing.Size(336, 549);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "自定义";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // normal_window_class
            // 
            this.normal_window_class.Font = new System.Drawing.Font("阿里妈妈数黑体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.normal_window_class.Location = new System.Drawing.Point(120, 124);
            this.normal_window_class.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.normal_window_class.Name = "normal_window_class";
            this.normal_window_class.Size = new System.Drawing.Size(205, 26);
            this.normal_window_class.TabIndex = 9;
            this.normal_window_class.Text = "RedEagle.Monitor";
            // 
            // full_window_class
            // 
            this.full_window_class.Font = new System.Drawing.Font("阿里妈妈数黑体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.full_window_class.Location = new System.Drawing.Point(120, 74);
            this.full_window_class.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.full_window_class.Name = "full_window_class";
            this.full_window_class.Size = new System.Drawing.Size(205, 26);
            this.full_window_class.TabIndex = 8;
            this.full_window_class.Text = "DIBFullViewClass";
            // 
            // n_w_c_l
            // 
            this.n_w_c_l.AutoSize = true;
            this.n_w_c_l.Font = new System.Drawing.Font("阿里妈妈数黑体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.n_w_c_l.Location = new System.Drawing.Point(3, 130);
            this.n_w_c_l.Name = "n_w_c_l";
            this.n_w_c_l.Size = new System.Drawing.Size(98, 18);
            this.n_w_c_l.TabIndex = 7;
            this.n_w_c_l.Text = "普通窗口类名";
            this.n_w_c_l.DoubleClick += new System.EventHandler(this.n_w_c_l_DoubleClick);
            // 
            // f_w_c_l
            // 
            this.f_w_c_l.AutoSize = true;
            this.f_w_c_l.Font = new System.Drawing.Font("阿里妈妈数黑体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.f_w_c_l.Location = new System.Drawing.Point(3, 80);
            this.f_w_c_l.Name = "f_w_c_l";
            this.f_w_c_l.Size = new System.Drawing.Size(98, 18);
            this.f_w_c_l.TabIndex = 6;
            this.f_w_c_l.Text = "全屏窗口类名";
            this.f_w_c_l.DoubleClick += new System.EventHandler(this.f_w_c_l_DoubleClick);
            // 
            // target_panel
            // 
            this.target_panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.target_panel.Location = new System.Drawing.Point(3, 4);
            this.target_panel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.target_panel.Name = "target_panel";
            this.target_panel.Size = new System.Drawing.Size(902, 582);
            this.target_panel.TabIndex = 0;
            // 
            // main_tabcontrol
            // 
            this.main_tabcontrol.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.main_tabcontrol.Controls.Add(this.tabPage7);
            this.main_tabcontrol.Controls.Add(this.attach_page);
            this.main_tabcontrol.Location = new System.Drawing.Point(14, 14);
            this.main_tabcontrol.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.main_tabcontrol.Name = "main_tabcontrol";
            this.main_tabcontrol.SelectedIndex = 0;
            this.main_tabcontrol.Size = new System.Drawing.Size(918, 624);
            this.main_tabcontrol.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.main_tabcontrol.TabIndex = 8;
            this.main_tabcontrol.SelectedIndexChanged += new System.EventHandler(this.main_tabcontrol_SelectedIndexChanged);
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.group_running_stat);
            this.tabPage7.Controls.Add(this.group_log);
            this.tabPage7.Controls.Add(this.config_TabControl);
            this.tabPage7.Font = new System.Drawing.Font("幼圆", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabPage7.Location = new System.Drawing.Point(4, 27);
            this.tabPage7.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage7.Size = new System.Drawing.Size(910, 593);
            this.tabPage7.TabIndex = 0;
            this.tabPage7.Text = "主界面";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // group_log
            // 
            this.group_log.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.group_log.AutoSize = true;
            this.group_log.Controls.Add(this.textbox_log);
            this.group_log.Font = new System.Drawing.Font("HarmonyOS Sans SC", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.group_log.Location = new System.Drawing.Point(3, 167);
            this.group_log.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.group_log.MinimumSize = new System.Drawing.Size(287, 0);
            this.group_log.Name = "group_log";
            this.group_log.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.group_log.Size = new System.Drawing.Size(556, 417);
            this.group_log.TabIndex = 3;
            this.group_log.TabStop = false;
            this.group_log.Text = "日志";
            // 
            // textbox_log
            // 
            this.textbox_log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textbox_log.Location = new System.Drawing.Point(3, 24);
            this.textbox_log.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textbox_log.MinimumSize = new System.Drawing.Size(286, 59);
            this.textbox_log.Multiline = true;
            this.textbox_log.Name = "textbox_log";
            this.textbox_log.ReadOnly = true;
            this.textbox_log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textbox_log.Size = new System.Drawing.Size(550, 389);
            this.textbox_log.TabIndex = 0;
            // 
            // attach_page
            // 
            this.attach_page.Controls.Add(this.target_panel);
            this.attach_page.Font = new System.Drawing.Font("幼圆", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.attach_page.Location = new System.Drawing.Point(4, 27);
            this.attach_page.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.attach_page.Name = "attach_page";
            this.attach_page.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.attach_page.Size = new System.Drawing.Size(910, 593);
            this.attach_page.TabIndex = 1;
            this.attach_page.Text = "附加窗口";
            this.attach_page.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(946, 683);
            this.Controls.Add(this.main_tabcontrol);
            this.Controls.Add(this.button1);
            this.Font = new System.Drawing.Font("阿里妈妈数黑体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(397, 381);
            this.Name = "Form1";
            this.Opacity = 0.95D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "红蜘蛛终结者";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.ResizeEnd += new System.EventHandler(this.Form1_ResizeEnd);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.group_running_stat.ResumeLayout(false);
            this.group_running_stat.PerformLayout();
            this.config_TabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.main_tabcontrol.ResumeLayout(false);
            this.tabPage7.ResumeLayout(false);
            this.tabPage7.PerformLayout();
            this.group_log.ResumeLayout(false);
            this.group_log.PerformLayout();
            this.attach_page.ResumeLayout(false);
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
        private System.Windows.Forms.TabControl config_TabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label n_w_c_l;
        private System.Windows.Forms.Label f_w_c_l;
        private System.Windows.Forms.TextBox normal_window_class;
        private System.Windows.Forms.TextBox full_window_class;
        private System.Windows.Forms.CheckBox attached_target;
        private System.Windows.Forms.Panel target_panel;
        private System.Windows.Forms.TabControl main_tabcontrol;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.TabPage attach_page;
        private System.Windows.Forms.GroupBox group_log;
        private System.Windows.Forms.TextBox textbox_log;
    }
}

