
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
            System.Windows.Forms.Label label1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.listener_timer = new System.Windows.Forms.Timer(this.components);
            this.group_running_stat = new System.Windows.Forms.GroupBox();
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
            this.screenHookStatusLabel = new System.Windows.Forms.Label();
            this.keyboardGuardInTime = new System.Windows.Forms.CheckBox();
            this.screenHookCheckBox = new System.Windows.Forms.CheckBox();
            this.attached_target = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.normal_window_class = new System.Windows.Forms.TextBox();
            this.full_window_class = new System.Windows.Forms.TextBox();
            this.n_w_c_l = new System.Windows.Forms.Label();
            this.f_w_c_l = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.capture_drag_area = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.insert_captured = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.captured_window = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.open_captured_directory = new System.Windows.Forms.PictureBox();
            this.captured_executable = new System.Windows.Forms.TextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ratioHeight = new System.Windows.Forms.NumericUpDown();
            this.ratioWidth = new System.Windows.Forms.NumericUpDown();
            this.keepRatio = new System.Windows.Forms.CheckBox();
            this.target_panel = new System.Windows.Forms.Panel();
            this.to_smallest_btn = new System.Windows.Forms.PictureBox();
            this.move_window = new System.Windows.Forms.PictureBox();
            this.resize_window = new System.Windows.Forms.PictureBox();
            this.main_tabcontrol = new System.Windows.Forms.TabControl();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.group_log = new System.Windows.Forms.GroupBox();
            this.textbox_log = new System.Windows.Forms.TextBox();
            this.immersive_mode = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            label1 = new System.Windows.Forms.Label();
            this.group_running_stat.SuspendLayout();
            this.config_TabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.capture_drag_area)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.open_captured_directory)).BeginInit();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ratioHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ratioWidth)).BeginInit();
            this.target_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.to_smallest_btn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.move_window)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.resize_window)).BeginInit();
            this.main_tabcontrol.SuspendLayout();
            this.tabPage7.SuspendLayout();
            this.group_log.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("阿里妈妈数黑体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            label1.Location = new System.Drawing.Point(288, 6);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(17, 18);
            label1.TabIndex = 1;
            label1.Text = "x";
            // 
            // listener_timer
            // 
            this.listener_timer.Tick += new System.EventHandler(this.Listen);
            // 
            // group_running_stat
            // 
            this.group_running_stat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.group_running_stat.Controls.Add(this.label_process_pid);
            this.group_running_stat.Controls.Add(this.label_running_stat);
            this.group_running_stat.Controls.Add(this.label_1);
            this.group_running_stat.Font = new System.Drawing.Font("阿里妈妈数黑体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.group_running_stat.Location = new System.Drawing.Point(7, 6);
            this.group_running_stat.MinimumSize = new System.Drawing.Size(287, 127);
            this.group_running_stat.Name = "group_running_stat";
            this.group_running_stat.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.group_running_stat.Size = new System.Drawing.Size(589, 127);
            this.group_running_stat.TabIndex = 0;
            this.group_running_stat.TabStop = false;
            this.group_running_stat.Text = "运行状态";
            // 
            // label_process_pid
            // 
            this.label_process_pid.AcceptsReturn = true;
            this.label_process_pid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label_process_pid.Font = new System.Drawing.Font("阿里妈妈数黑体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_process_pid.Location = new System.Drawing.Point(435, 26);
            this.label_process_pid.Multiline = true;
            this.label_process_pid.Name = "label_process_pid";
            this.label_process_pid.ReadOnly = true;
            this.label_process_pid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.label_process_pid.Size = new System.Drawing.Size(147, 91);
            this.label_process_pid.TabIndex = 4;
            this.label_process_pid.Text = "None";
            // 
            // label_running_stat
            // 
            this.label_running_stat.AutoSize = true;
            this.label_running_stat.Font = new System.Drawing.Font("阿里妈妈数黑体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_running_stat.ForeColor = System.Drawing.Color.Red;
            this.label_running_stat.Location = new System.Drawing.Point(94, 49);
            this.label_running_stat.Name = "label_running_stat";
            this.label_running_stat.Size = new System.Drawing.Size(23, 18);
            this.label_running_stat.TabIndex = 2;
            this.label_running_stat.Text = "否";
            // 
            // label_1
            // 
            this.label_1.AutoSize = true;
            this.label_1.Font = new System.Drawing.Font("阿里妈妈数黑体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_1.Location = new System.Drawing.Point(7, 49);
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
            this.name_conflict_err.Location = new System.Drawing.Point(78, 47);
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
            this.label2.Location = new System.Drawing.Point(3, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 18);
            this.label2.TabIndex = 4;
            this.label2.Text = "目标进程名";
            this.label2.DoubleClick += new System.EventHandler(this.label2_DoubleClick);
            // 
            // process_name
            // 
            this.process_name.Font = new System.Drawing.Font("阿里妈妈数黑体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.process_name.Location = new System.Drawing.Point(120, 14);
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
            this.auto_hide.Location = new System.Drawing.Point(6, 105);
            this.auto_hide.Name = "auto_hide";
            this.auto_hide.Size = new System.Drawing.Size(102, 22);
            this.auto_hide.TabIndex = 2;
            this.auto_hide.Text = "自动隐藏目标";
            this.auto_hide.UseVisualStyleBackColor = true;
            this.auto_hide.CheckedChanged += new System.EventHandler(this.auto_hide_CheckedChanged);
            // 
            // auto_kill
            // 
            this.auto_kill.AutoSize = true;
            this.auto_kill.Font = new System.Drawing.Font("得意黑", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.auto_kill.Location = new System.Drawing.Point(6, 77);
            this.auto_kill.Name = "auto_kill";
            this.auto_kill.Size = new System.Drawing.Size(102, 22);
            this.auto_kill.TabIndex = 1;
            this.auto_kill.Text = "自动关闭目标";
            this.auto_kill.UseVisualStyleBackColor = true;
            this.auto_kill.CheckedChanged += new System.EventHandler(this.auto_kill_CheckedChanged);
            // 
            // topest_with_timer
            // 
            this.topest_with_timer.AutoSize = true;
            this.topest_with_timer.Checked = true;
            this.topest_with_timer.CheckState = System.Windows.Forms.CheckState.Checked;
            this.topest_with_timer.Font = new System.Drawing.Font("得意黑", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.topest_with_timer.Location = new System.Drawing.Point(6, 21);
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
            this.button1.Location = new System.Drawing.Point(854, 578);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(84, 23);
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
            this.config_TabControl.Controls.Add(this.tabPage3);
            this.config_TabControl.Controls.Add(this.tabPage4);
            this.config_TabControl.Font = new System.Drawing.Font("得意黑", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.config_TabControl.Location = new System.Drawing.Point(602, 6);
            this.config_TabControl.Name = "config_TabControl";
            this.config_TabControl.SelectedIndex = 0;
            this.config_TabControl.Size = new System.Drawing.Size(340, 566);
            this.config_TabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.config_TabControl.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.screenHookStatusLabel);
            this.tabPage1.Controls.Add(this.topest_with_timer);
            this.tabPage1.Controls.Add(this.auto_kill);
            this.tabPage1.Controls.Add(this.keyboardGuardInTime);
            this.tabPage1.Controls.Add(this.screenHookCheckBox);
            this.tabPage1.Controls.Add(this.attached_target);
            this.tabPage1.Controls.Add(this.auto_hide);
            this.tabPage1.Font = new System.Drawing.Font("得意黑", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabPage1.Location = new System.Drawing.Point(4, 27);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(332, 535);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "主选项";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // screenHookStatusLabel
            // 
            this.screenHookStatusLabel.AutoSize = true;
            this.screenHookStatusLabel.ForeColor = System.Drawing.Color.Gray;
            this.screenHookStatusLabel.Location = new System.Drawing.Point(90, 164);
            this.screenHookStatusLabel.Name = "screenHookStatusLabel";
            this.screenHookStatusLabel.Size = new System.Drawing.Size(37, 16);
            this.screenHookStatusLabel.TabIndex = 3;
            this.screenHookStatusLabel.Text = "未注入";
            // 
            // keyboardGuardInTime
            // 
            this.keyboardGuardInTime.AutoSize = true;
            this.keyboardGuardInTime.Font = new System.Drawing.Font("得意黑", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.keyboardGuardInTime.Location = new System.Drawing.Point(6, 49);
            this.keyboardGuardInTime.Name = "keyboardGuardInTime";
            this.keyboardGuardInTime.Size = new System.Drawing.Size(102, 22);
            this.keyboardGuardInTime.TabIndex = 2;
            this.keyboardGuardInTime.Text = "实时键盘守护";
            this.keyboardGuardInTime.UseVisualStyleBackColor = true;
            this.keyboardGuardInTime.CheckedChanged += new System.EventHandler(this.attached_target_CheckedChanged);
            // 
            // screenHookCheckBox
            // 
            this.screenHookCheckBox.AutoSize = true;
            this.screenHookCheckBox.Font = new System.Drawing.Font("得意黑", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.screenHookCheckBox.Location = new System.Drawing.Point(6, 161);
            this.screenHookCheckBox.Name = "screenHookCheckBox";
            this.screenHookCheckBox.Size = new System.Drawing.Size(78, 22);
            this.screenHookCheckBox.TabIndex = 2;
            this.screenHookCheckBox.Text = "屏幕守护";
            this.screenHookCheckBox.UseVisualStyleBackColor = true;
            this.screenHookCheckBox.CheckedChanged += new System.EventHandler(this.ScreenHookCheckBox_CheckedChanged);
            // 
            // attached_target
            // 
            this.attached_target.AutoSize = true;
            this.attached_target.Font = new System.Drawing.Font("得意黑", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.attached_target.Location = new System.Drawing.Point(6, 133);
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
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(332, 535);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "自定义";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // normal_window_class
            // 
            this.normal_window_class.Font = new System.Drawing.Font("阿里妈妈数黑体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.normal_window_class.Location = new System.Drawing.Point(120, 103);
            this.normal_window_class.Name = "normal_window_class";
            this.normal_window_class.Size = new System.Drawing.Size(205, 26);
            this.normal_window_class.TabIndex = 9;
            this.normal_window_class.Text = "RedEagle.Monitor";
            // 
            // full_window_class
            // 
            this.full_window_class.Font = new System.Drawing.Font("阿里妈妈数黑体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.full_window_class.Location = new System.Drawing.Point(120, 62);
            this.full_window_class.Name = "full_window_class";
            this.full_window_class.Size = new System.Drawing.Size(205, 26);
            this.full_window_class.TabIndex = 8;
            this.full_window_class.Text = "DIBFullViewClass";
            // 
            // n_w_c_l
            // 
            this.n_w_c_l.AutoSize = true;
            this.n_w_c_l.Font = new System.Drawing.Font("阿里妈妈数黑体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.n_w_c_l.Location = new System.Drawing.Point(3, 108);
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
            this.f_w_c_l.Location = new System.Drawing.Point(3, 67);
            this.f_w_c_l.Name = "f_w_c_l";
            this.f_w_c_l.Size = new System.Drawing.Size(98, 18);
            this.f_w_c_l.TabIndex = 6;
            this.f_w_c_l.Text = "全屏窗口类名";
            this.f_w_c_l.DoubleClick += new System.EventHandler(this.f_w_c_l_DoubleClick);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.capture_drag_area);
            this.tabPage3.Controls.Add(this.groupBox1);
            this.tabPage3.Location = new System.Drawing.Point(4, 27);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(332, 535);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "增强功能：窗口捕获";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // capture_drag_area
            // 
            this.capture_drag_area.Image = global::FuckRedSpider.Properties.Resources.鼠标;
            this.capture_drag_area.Location = new System.Drawing.Point(145, 77);
            this.capture_drag_area.Name = "capture_drag_area";
            this.capture_drag_area.Size = new System.Drawing.Size(30, 26);
            this.capture_drag_area.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.capture_drag_area.TabIndex = 2;
            this.capture_drag_area.TabStop = false;
            this.capture_drag_area.MouseDown += new System.Windows.Forms.MouseEventHandler(this.capture_drag_area_MouseDown);
            this.capture_drag_area.MouseMove += new System.Windows.Forms.MouseEventHandler(this.capture_drag_area_MouseMove);
            this.capture_drag_area.MouseUp += new System.Windows.Forms.MouseEventHandler(this.capture_drag_area_MouseUp);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.insert_captured);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Location = new System.Drawing.Point(3, 109);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(330, 194);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "捕获结果";
            // 
            // insert_captured
            // 
            this.insert_captured.Location = new System.Drawing.Point(130, 165);
            this.insert_captured.Name = "insert_captured";
            this.insert_captured.Size = new System.Drawing.Size(75, 23);
            this.insert_captured.TabIndex = 3;
            this.insert_captured.Text = "应用结果";
            this.insert_captured.UseVisualStyleBackColor = true;
            this.insert_captured.Click += new System.EventHandler(this.insert_captured_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.captured_window);
            this.groupBox3.Location = new System.Drawing.Point(6, 88);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(318, 71);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "捕获窗口类名";
            // 
            // captured_window
            // 
            this.captured_window.Location = new System.Drawing.Point(7, 25);
            this.captured_window.Name = "captured_window";
            this.captured_window.ReadOnly = true;
            this.captured_window.Size = new System.Drawing.Size(305, 25);
            this.captured_window.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.open_captured_directory);
            this.groupBox2.Controls.Add(this.captured_executable);
            this.groupBox2.Location = new System.Drawing.Point(6, 23);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(318, 71);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "可执行文件路径";
            // 
            // open_captured_directory
            // 
            this.open_captured_directory.Image = global::FuckRedSpider.Properties.Resources.文件夹;
            this.open_captured_directory.Location = new System.Drawing.Point(282, 24);
            this.open_captured_directory.Name = "open_captured_directory";
            this.open_captured_directory.Size = new System.Drawing.Size(30, 26);
            this.open_captured_directory.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.open_captured_directory.TabIndex = 1;
            this.open_captured_directory.TabStop = false;
            this.open_captured_directory.DoubleClick += new System.EventHandler(this.open_captured_directory_DoubleClick);
            // 
            // captured_executable
            // 
            this.captured_executable.Location = new System.Drawing.Point(7, 25);
            this.captured_executable.Name = "captured_executable";
            this.captured_executable.ReadOnly = true;
            this.captured_executable.Size = new System.Drawing.Size(269, 25);
            this.captured_executable.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.dataGridView1);
            this.tabPage4.Location = new System.Drawing.Point(4, 27);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(332, 535);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "按键监听";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 27;
            this.dataGridView1.Size = new System.Drawing.Size(332, 535);
            this.dataGridView1.TabIndex = 0;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "名称";
            this.Column1.MinimumWidth = 6;
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 125;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "主键";
            this.Column2.Items.AddRange(new object[] {
            "Shift",
            "Ctrl",
            "Alt"});
            this.Column2.MinimumWidth = 6;
            this.Column2.Name = "Column2";
            this.Column2.Width = 125;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "副键";
            this.Column3.MinimumWidth = 6;
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Width = 125;
            // 
            // ratioHeight
            // 
            this.ratioHeight.Enabled = false;
            this.ratioHeight.Font = new System.Drawing.Font("阿里妈妈数黑体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ratioHeight.Location = new System.Drawing.Point(311, 3);
            this.ratioHeight.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.ratioHeight.Name = "ratioHeight";
            this.ratioHeight.Size = new System.Drawing.Size(77, 26);
            this.ratioHeight.TabIndex = 3;
            this.ratioHeight.Value = new decimal(new int[] {
            1080,
            0,
            0,
            0});
            // 
            // ratioWidth
            // 
            this.ratioWidth.Enabled = false;
            this.ratioWidth.Font = new System.Drawing.Font("阿里妈妈数黑体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ratioWidth.Location = new System.Drawing.Point(205, 3);
            this.ratioWidth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.ratioWidth.Name = "ratioWidth";
            this.ratioWidth.Size = new System.Drawing.Size(77, 26);
            this.ratioWidth.TabIndex = 3;
            this.ratioWidth.Value = new decimal(new int[] {
            1920,
            0,
            0,
            0});
            // 
            // keepRatio
            // 
            this.keepRatio.AutoSize = true;
            this.keepRatio.Enabled = false;
            this.keepRatio.Font = new System.Drawing.Font("得意黑", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.keepRatio.Location = new System.Drawing.Point(121, 4);
            this.keepRatio.Name = "keepRatio";
            this.keepRatio.Size = new System.Drawing.Size(78, 22);
            this.keepRatio.TabIndex = 2;
            this.keepRatio.Text = "保持比例";
            this.keepRatio.UseVisualStyleBackColor = true;
            this.keepRatio.CheckedChanged += new System.EventHandler(this.keepRatio_CheckedChanged);
            // 
            // target_panel
            // 
            this.target_panel.Controls.Add(this.to_smallest_btn);
            this.target_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.target_panel.Location = new System.Drawing.Point(0, 0);
            this.target_panel.Name = "target_panel";
            this.target_panel.Size = new System.Drawing.Size(955, 636);
            this.target_panel.TabIndex = 0;
            this.target_panel.Visible = false;
            // 
            // to_smallest_btn
            // 
            this.to_smallest_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.to_smallest_btn.Image = global::FuckRedSpider.Properties.Resources.最小化;
            this.to_smallest_btn.InitialImage = global::FuckRedSpider.Properties.Resources.移动;
            this.to_smallest_btn.Location = new System.Drawing.Point(932, 0);
            this.to_smallest_btn.Name = "to_smallest_btn";
            this.to_smallest_btn.Size = new System.Drawing.Size(25, 25);
            this.to_smallest_btn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.to_smallest_btn.TabIndex = 0;
            this.to_smallest_btn.TabStop = false;
            this.to_smallest_btn.Click += new System.EventHandler(this.to_smallest_btn_Click);
            this.to_smallest_btn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.move_window_MouseDown);
            this.to_smallest_btn.MouseMove += new System.Windows.Forms.MouseEventHandler(this.move_window_MouseMove);
            this.to_smallest_btn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.move_window_MouseUp);
            // 
            // move_window
            // 
            this.move_window.Image = global::FuckRedSpider.Properties.Resources.移动;
            this.move_window.InitialImage = global::FuckRedSpider.Properties.Resources.移动;
            this.move_window.Location = new System.Drawing.Point(0, 0);
            this.move_window.Name = "move_window";
            this.move_window.Size = new System.Drawing.Size(23, 25);
            this.move_window.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.move_window.TabIndex = 0;
            this.move_window.TabStop = false;
            this.move_window.MouseDown += new System.Windows.Forms.MouseEventHandler(this.move_window_MouseDown);
            this.move_window.MouseMove += new System.Windows.Forms.MouseEventHandler(this.move_window_MouseMove);
            this.move_window.MouseUp += new System.Windows.Forms.MouseEventHandler(this.move_window_MouseUp);
            // 
            // resize_window
            // 
            this.resize_window.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.resize_window.Image = global::FuckRedSpider.Properties.Resources.调整大小;
            this.resize_window.InitialImage = global::FuckRedSpider.Properties.Resources.调整大小;
            this.resize_window.Location = new System.Drawing.Point(930, 607);
            this.resize_window.Name = "resize_window";
            this.resize_window.Size = new System.Drawing.Size(25, 25);
            this.resize_window.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.resize_window.TabIndex = 1;
            this.resize_window.TabStop = false;
            this.resize_window.MouseDown += new System.Windows.Forms.MouseEventHandler(this.resize_window_MouseDown);
            this.resize_window.MouseMove += new System.Windows.Forms.MouseEventHandler(this.resize_window_MouseMove);
            this.resize_window.MouseUp += new System.Windows.Forms.MouseEventHandler(this.resize_window_MouseUp);
            // 
            // main_tabcontrol
            // 
            this.main_tabcontrol.Controls.Add(this.tabPage7);
            this.main_tabcontrol.Dock = System.Windows.Forms.DockStyle.Fill;
            this.main_tabcontrol.Location = new System.Drawing.Point(0, 0);
            this.main_tabcontrol.Name = "main_tabcontrol";
            this.main_tabcontrol.SelectedIndex = 0;
            this.main_tabcontrol.Size = new System.Drawing.Size(955, 636);
            this.main_tabcontrol.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.main_tabcontrol.TabIndex = 8;
            this.main_tabcontrol.SelectedIndexChanged += new System.EventHandler(this.main_tabcontrol_SelectedIndexChanged);
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.group_running_stat);
            this.tabPage7.Controls.Add(this.button1);
            this.tabPage7.Controls.Add(this.group_log);
            this.tabPage7.Controls.Add(this.config_TabControl);
            this.tabPage7.Font = new System.Drawing.Font("幼圆", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabPage7.Location = new System.Drawing.Point(4, 25);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage7.Size = new System.Drawing.Size(947, 607);
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
            this.group_log.Location = new System.Drawing.Point(3, 139);
            this.group_log.MinimumSize = new System.Drawing.Size(287, 0);
            this.group_log.Name = "group_log";
            this.group_log.Size = new System.Drawing.Size(596, 581);
            this.group_log.TabIndex = 3;
            this.group_log.TabStop = false;
            this.group_log.Text = "日志";
            // 
            // textbox_log
            // 
            this.textbox_log.Location = new System.Drawing.Point(3, 23);
            this.textbox_log.MinimumSize = new System.Drawing.Size(286, 50);
            this.textbox_log.Multiline = true;
            this.textbox_log.Name = "textbox_log";
            this.textbox_log.ReadOnly = true;
            this.textbox_log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textbox_log.Size = new System.Drawing.Size(587, 440);
            this.textbox_log.TabIndex = 0;
            // 
            // immersive_mode
            // 
            this.immersive_mode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.immersive_mode.Font = new System.Drawing.Font("得意黑", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.immersive_mode.Location = new System.Drawing.Point(3, 4);
            this.immersive_mode.Name = "immersive_mode";
            this.immersive_mode.Size = new System.Drawing.Size(112, 22);
            this.immersive_mode.TabIndex = 2;
            this.immersive_mode.Text = "沉浸式窗口";
            this.immersive_mode.UseVisualStyleBackColor = true;
            this.immersive_mode.CheckedChanged += new System.EventHandler(this.immersive_mode_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(label1);
            this.panel1.Controls.Add(this.ratioHeight);
            this.panel1.Controls.Add(this.immersive_mode);
            this.panel1.Controls.Add(this.keepRatio);
            this.panel1.Controls.Add(this.ratioWidth);
            this.panel1.Location = new System.Drawing.Point(315, 595);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(390, 29);
            this.panel1.TabIndex = 9;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.resize_window);
            this.panel2.Controls.Add(this.move_window);
            this.panel2.Controls.Add(this.target_panel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(955, 636);
            this.panel2.TabIndex = 10;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(955, 636);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.main_tabcontrol);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("幼圆", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(397, 325);
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
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.capture_drag_area)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.open_captured_directory)).EndInit();
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ratioHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ratioWidth)).EndInit();
            this.target_panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.to_smallest_btn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.move_window)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.resize_window)).EndInit();
            this.main_tabcontrol.ResumeLayout(false);
            this.tabPage7.ResumeLayout(false);
            this.tabPage7.PerformLayout();
            this.group_log.ResumeLayout(false);
            this.group_log.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer listener_timer;
        private System.Windows.Forms.GroupBox group_running_stat;
        private System.Windows.Forms.Label label_1;
        private System.Windows.Forms.Label label_running_stat;
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
        private System.Windows.Forms.GroupBox group_log;
        private System.Windows.Forms.TextBox textbox_log;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox captured_executable;
        private System.Windows.Forms.PictureBox open_captured_directory;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox captured_window;
        private System.Windows.Forms.PictureBox capture_drag_area;
        private System.Windows.Forms.Button insert_captured;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewComboBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.CheckBox keyboardGuardInTime;
        private System.Windows.Forms.NumericUpDown ratioHeight;
        private System.Windows.Forms.NumericUpDown ratioWidth;
        private System.Windows.Forms.CheckBox keepRatio;
        private System.Windows.Forms.CheckBox immersive_mode;
        private System.Windows.Forms.PictureBox move_window;
        private System.Windows.Forms.PictureBox resize_window;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox screenHookCheckBox;
        private System.Windows.Forms.Label screenHookStatusLabel;
        private System.Windows.Forms.PictureBox to_smallest_btn;
    }
}

