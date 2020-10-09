namespace MelonLoader
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.ThemeManager = new MetroFramework.Components.MetroStyleManager(this.components);
            this.InstallerVersion = new MetroFramework.Controls.MetroLabel();
            this.Link_Wiki = new System.Windows.Forms.PictureBox();
            this.Link_GitHub = new System.Windows.Forms.PictureBox();
            this.Link_Twitter = new System.Windows.Forms.PictureBox();
            this.Link_Discord = new System.Windows.Forms.PictureBox();
            this.ML_Text = new System.Windows.Forms.PictureBox();
            this.ML_Logo = new System.Windows.Forms.PictureBox();
            this.PageManager = new MetroFramework.Controls.MetroTabControl();
            this.Tab_Automated = new MetroFramework.Controls.MetroTabPage();
            this.Automated_x64Only = new MetroFramework.Controls.MetroLabel();
            this.Automated_Uninstall = new MetroFramework.Controls.MetroButton();
            this.Automated_Install = new MetroFramework.Controls.MetroButton();
            this.Automated_Arch_AutoDetect = new MetroFramework.Controls.MetroCheckBox();
            this.Automated_Arch_Selection = new MetroFramework.Controls.MetroComboBox();
            this.Automated_Arch_Text = new MetroFramework.Controls.MetroLabel();
            this.Automated_Version_Latest = new MetroFramework.Controls.MetroCheckBox();
            this.Automated_Version_Selection = new MetroFramework.Controls.MetroComboBox();
            this.Automated_Version_Text = new MetroFramework.Controls.MetroLabel();
            this.Automated_UnityGame_Select = new MetroFramework.Controls.MetroButton();
            this.Automated_UnityGame_Display = new System.Windows.Forms.TextBox();
            this.Automated_UnityGame_Text = new MetroFramework.Controls.MetroLabel();
            this.Automated_Divider = new MetroFramework.Controls.MetroLabel();
            this.Tab_Output = new MetroFramework.Controls.MetroTabPage();
            this.Output_Current_Operation = new MetroFramework.Controls.MetroLabel();
            this.Output_Divider = new MetroFramework.Controls.MetroLabel();
            this.Output_Total_Progress_Text_Label = new MetroFramework.Controls.MetroLabel();
            this.Output_Current_Progress_Text_Label = new MetroFramework.Controls.MetroLabel();
            this.Output_Current_Progress_Display = new MetroFramework.Controls.MetroProgressBar();
            this.Output_Current_Text = new MetroFramework.Controls.MetroLabel();
            this.Output_Total_Progress_Display = new MetroFramework.Controls.MetroProgressBar();
            this.Output_Total_Text = new MetroFramework.Controls.MetroLabel();
            this.Output_Total_Progress_Text = new MetroFramework.Controls.MetroLabel();
            this.Output_Current_Progress_Text = new MetroFramework.Controls.MetroLabel();
            this.Tab_ManualZip = new MetroFramework.Controls.MetroTabPage();
            this.ManualZip_ZipArchive_Select = new MetroFramework.Controls.MetroButton();
            this.ManualZip_ZipArchive_Display = new System.Windows.Forms.TextBox();
            this.ManualZip_ZipArchive_Text = new MetroFramework.Controls.MetroLabel();
            this.ManualZip_UnityGame_Select = new MetroFramework.Controls.MetroButton();
            this.ManualZip_UnityGame_Display = new System.Windows.Forms.TextBox();
            this.ManualZip_UnityGame_Text = new MetroFramework.Controls.MetroLabel();
            this.ManualZip_Uninstall = new MetroFramework.Controls.MetroButton();
            this.ManualZip_Install = new MetroFramework.Controls.MetroButton();
            this.ManualZip_Divider = new MetroFramework.Controls.MetroLabel();
            this.Tab_Settings = new MetroFramework.Controls.MetroTabPage();
            this.Settings_CloseAfterCompletion = new MetroFramework.Controls.MetroCheckBox();
            this.Settings_AutoUpdateInstaller = new MetroFramework.Controls.MetroCheckBox();
            this.Settings_Theme_Selection = new MetroFramework.Controls.MetroComboBox();
            this.Settings_Theme_Text = new MetroFramework.Controls.MetroLabel();
            this.Tab_PleaseWait = new MetroFramework.Controls.MetroTabPage();
            this.PleaseWait_PleaseWait = new MetroFramework.Controls.MetroLabel();
            this.PleaseWait_Text = new MetroFramework.Controls.MetroLabel();
            this.Tab_Error = new MetroFramework.Controls.MetroTabPage();
            this.Error_Retry = new MetroFramework.Controls.MetroButton();
            this.Error_Error = new MetroFramework.Controls.MetroLabel();
            this.Error_Text = new MetroFramework.Controls.MetroLabel();
            this.InstallerUpdateNotice = new MetroFramework.Controls.MetroLabel();
            ((System.ComponentModel.ISupportInitialize)(this.ThemeManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Link_Wiki)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Link_GitHub)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Link_Twitter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Link_Discord)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ML_Text)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ML_Logo)).BeginInit();
            this.PageManager.SuspendLayout();
            this.Tab_Automated.SuspendLayout();
            this.Tab_Output.SuspendLayout();
            this.Tab_ManualZip.SuspendLayout();
            this.Tab_Settings.SuspendLayout();
            this.Tab_PleaseWait.SuspendLayout();
            this.Tab_Error.SuspendLayout();
            this.SuspendLayout();
            // 
            // ThemeManager
            // 
            this.ThemeManager.Owner = this;
            this.ThemeManager.Style = MetroFramework.MetroColorStyle.Red;
            this.ThemeManager.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // InstallerVersion
            // 
            this.InstallerVersion.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.InstallerVersion.Location = new System.Drawing.Point(7, 42);
            this.InstallerVersion.Name = "InstallerVersion";
            this.InstallerVersion.Size = new System.Drawing.Size(115, 23);
            this.InstallerVersion.TabIndex = 8;
            this.InstallerVersion.Text = "Installer v0.0.0";
            this.InstallerVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.InstallerVersion.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // Link_Wiki
            // 
            this.Link_Wiki.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Link_Wiki.Image = global::MelonLoader.Properties.Resources.Wiki;
            this.Link_Wiki.Location = new System.Drawing.Point(97, 14);
            this.Link_Wiki.Name = "Link_Wiki";
            this.Link_Wiki.Size = new System.Drawing.Size(25, 25);
            this.Link_Wiki.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Link_Wiki.TabIndex = 9;
            this.Link_Wiki.TabStop = false;
            this.Link_Wiki.Click += new System.EventHandler(this.Link_Wiki_Click);
            // 
            // Link_GitHub
            // 
            this.Link_GitHub.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Link_GitHub.Image = global::MelonLoader.Properties.Resources.GitHub_Dark;
            this.Link_GitHub.Location = new System.Drawing.Point(67, 14);
            this.Link_GitHub.Name = "Link_GitHub";
            this.Link_GitHub.Size = new System.Drawing.Size(25, 25);
            this.Link_GitHub.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Link_GitHub.TabIndex = 7;
            this.Link_GitHub.TabStop = false;
            this.Link_GitHub.Click += new System.EventHandler(this.Link_GitHub_Click);
            // 
            // Link_Twitter
            // 
            this.Link_Twitter.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Link_Twitter.Image = global::MelonLoader.Properties.Resources.Twitter;
            this.Link_Twitter.Location = new System.Drawing.Point(37, 14);
            this.Link_Twitter.Name = "Link_Twitter";
            this.Link_Twitter.Size = new System.Drawing.Size(25, 25);
            this.Link_Twitter.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Link_Twitter.TabIndex = 6;
            this.Link_Twitter.TabStop = false;
            this.Link_Twitter.Click += new System.EventHandler(this.Link_Twitter_Click);
            // 
            // Link_Discord
            // 
            this.Link_Discord.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Link_Discord.Image = global::MelonLoader.Properties.Resources.Discord;
            this.Link_Discord.Location = new System.Drawing.Point(3, 11);
            this.Link_Discord.Name = "Link_Discord";
            this.Link_Discord.Size = new System.Drawing.Size(32, 32);
            this.Link_Discord.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Link_Discord.TabIndex = 5;
            this.Link_Discord.TabStop = false;
            this.Link_Discord.Click += new System.EventHandler(this.Link_Discord_Click);
            // 
            // ML_Text
            // 
            this.ML_Text.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.ML_Text.Image = global::MelonLoader.Properties.Resources.ML_Text;
            this.ML_Text.Location = new System.Drawing.Point(23, 134);
            this.ML_Text.Name = "ML_Text";
            this.ML_Text.Size = new System.Drawing.Size(437, 63);
            this.ML_Text.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ML_Text.TabIndex = 1;
            this.ML_Text.TabStop = false;
            // 
            // ML_Logo
            // 
            this.ML_Logo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.ML_Logo.Image = global::MelonLoader.Properties.Resources.ML_Icon;
            this.ML_Logo.Location = new System.Drawing.Point(184, 20);
            this.ML_Logo.Name = "ML_Logo";
            this.ML_Logo.Size = new System.Drawing.Size(120, 109);
            this.ML_Logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ML_Logo.TabIndex = 0;
            this.ML_Logo.TabStop = false;
            // 
            // PageManager
            // 
            this.PageManager.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.PageManager.Controls.Add(this.Tab_Automated);
            this.PageManager.Controls.Add(this.Tab_ManualZip);
            this.PageManager.Controls.Add(this.Tab_Output);
            this.PageManager.Controls.Add(this.Tab_Settings);
            this.PageManager.Controls.Add(this.Tab_PleaseWait);
            this.PageManager.Controls.Add(this.Tab_Error);
            this.PageManager.Cursor = System.Windows.Forms.Cursors.Default;
            this.PageManager.FontWeight = MetroFramework.MetroTabControlWeight.Bold;
            this.PageManager.ItemSize = new System.Drawing.Size(141, 34);
            this.PageManager.Location = new System.Drawing.Point(21, 203);
            this.PageManager.Name = "PageManager";
            this.PageManager.SelectedIndex = 2;
            this.PageManager.Size = new System.Drawing.Size(439, 222);
            this.PageManager.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.PageManager.Style = MetroFramework.MetroColorStyle.Red;
            this.PageManager.TabIndex = 10;
            this.PageManager.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.PageManager.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // Tab_Automated
            // 
            this.Tab_Automated.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Tab_Automated.Controls.Add(this.Automated_x64Only);
            this.Tab_Automated.Controls.Add(this.Automated_Uninstall);
            this.Tab_Automated.Controls.Add(this.Automated_Install);
            this.Tab_Automated.Controls.Add(this.Automated_Arch_AutoDetect);
            this.Tab_Automated.Controls.Add(this.Automated_Arch_Selection);
            this.Tab_Automated.Controls.Add(this.Automated_Arch_Text);
            this.Tab_Automated.Controls.Add(this.Automated_Version_Latest);
            this.Tab_Automated.Controls.Add(this.Automated_Version_Selection);
            this.Tab_Automated.Controls.Add(this.Automated_Version_Text);
            this.Tab_Automated.Controls.Add(this.Automated_UnityGame_Select);
            this.Tab_Automated.Controls.Add(this.Automated_UnityGame_Display);
            this.Tab_Automated.Controls.Add(this.Automated_UnityGame_Text);
            this.Tab_Automated.Controls.Add(this.Automated_Divider);
            this.Tab_Automated.Cursor = System.Windows.Forms.Cursors.Default;
            this.Tab_Automated.HorizontalScrollbarBarColor = true;
            this.Tab_Automated.Location = new System.Drawing.Point(4, 38);
            this.Tab_Automated.Name = "Tab_Automated";
            this.Tab_Automated.Size = new System.Drawing.Size(431, 180);
            this.Tab_Automated.Style = MetroFramework.MetroColorStyle.Red;
            this.Tab_Automated.TabIndex = 0;
            this.Tab_Automated.Text = "Automated   ";
            this.Tab_Automated.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Tab_Automated.VerticalScrollbarBarColor = true;
            // 
            // Automated_x64Only
            // 
            this.Automated_x64Only.AutoSize = true;
            this.Automated_x64Only.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.Automated_x64Only.Location = new System.Drawing.Point(182, 88);
            this.Automated_x64Only.Name = "Automated_x64Only";
            this.Automated_x64Only.Size = new System.Drawing.Size(64, 19);
            this.Automated_x64Only.TabIndex = 15;
            this.Automated_x64Only.Text = "x64 Only";
            this.Automated_x64Only.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Automated_x64Only.Visible = false;
            // 
            // Automated_Uninstall
            // 
            this.Automated_Uninstall.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Automated_Uninstall.Location = new System.Drawing.Point(218, 129);
            this.Automated_Uninstall.Name = "Automated_Uninstall";
            this.Automated_Uninstall.Size = new System.Drawing.Size(206, 44);
            this.Automated_Uninstall.TabIndex = 14;
            this.Automated_Uninstall.Text = "UN-INSTALL";
            this.Automated_Uninstall.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Automated_Uninstall.Visible = false;
            this.Automated_Uninstall.Click += new System.EventHandler(this.Automated_Uninstall_Click);
            // 
            // Automated_Install
            // 
            this.Automated_Install.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Automated_Install.Enabled = false;
            this.Automated_Install.Location = new System.Drawing.Point(5, 129);
            this.Automated_Install.Name = "Automated_Install";
            this.Automated_Install.Size = new System.Drawing.Size(206, 44);
            this.Automated_Install.TabIndex = 13;
            this.Automated_Install.Text = "INSTALL";
            this.Automated_Install.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Automated_Install.Click += new System.EventHandler(this.Automated_Install_Click);
            // 
            // Automated_Arch_AutoDetect
            // 
            this.Automated_Arch_AutoDetect.AutoSize = true;
            this.Automated_Arch_AutoDetect.Checked = true;
            this.Automated_Arch_AutoDetect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Automated_Arch_AutoDetect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Automated_Arch_AutoDetect.Location = new System.Drawing.Point(253, 90);
            this.Automated_Arch_AutoDetect.Name = "Automated_Arch_AutoDetect";
            this.Automated_Arch_AutoDetect.Size = new System.Drawing.Size(88, 15);
            this.Automated_Arch_AutoDetect.Style = MetroFramework.MetroColorStyle.Green;
            this.Automated_Arch_AutoDetect.TabIndex = 10;
            this.Automated_Arch_AutoDetect.Text = "Auto-Detect";
            this.Automated_Arch_AutoDetect.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Automated_Arch_AutoDetect.UseVisualStyleBackColor = true;
            this.Automated_Arch_AutoDetect.CheckedChanged += new System.EventHandler(this.Automated_Arch_AutoDetect_CheckedChanged);
            // 
            // Automated_Arch_Selection
            // 
            this.Automated_Arch_Selection.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Automated_Arch_Selection.Enabled = false;
            this.Automated_Arch_Selection.FormattingEnabled = true;
            this.Automated_Arch_Selection.ItemHeight = 23;
            this.Automated_Arch_Selection.Items.AddRange(new object[] {
            "x86",
            "x64"});
            this.Automated_Arch_Selection.Location = new System.Drawing.Point(187, 83);
            this.Automated_Arch_Selection.Name = "Automated_Arch_Selection";
            this.Automated_Arch_Selection.Size = new System.Drawing.Size(55, 29);
            this.Automated_Arch_Selection.TabIndex = 9;
            this.Automated_Arch_Selection.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // Automated_Arch_Text
            // 
            this.Automated_Arch_Text.AutoSize = true;
            this.Automated_Arch_Text.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.Automated_Arch_Text.Location = new System.Drawing.Point(142, 88);
            this.Automated_Arch_Text.Name = "Automated_Arch_Text";
            this.Automated_Arch_Text.Size = new System.Drawing.Size(40, 19);
            this.Automated_Arch_Text.TabIndex = 8;
            this.Automated_Arch_Text.Text = "Arch:";
            this.Automated_Arch_Text.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // Automated_Version_Latest
            // 
            this.Automated_Version_Latest.AutoSize = true;
            this.Automated_Version_Latest.Checked = true;
            this.Automated_Version_Latest.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Automated_Version_Latest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Automated_Version_Latest.Location = new System.Drawing.Point(287, 50);
            this.Automated_Version_Latest.Name = "Automated_Version_Latest";
            this.Automated_Version_Latest.Size = new System.Drawing.Size(54, 15);
            this.Automated_Version_Latest.Style = MetroFramework.MetroColorStyle.Green;
            this.Automated_Version_Latest.TabIndex = 7;
            this.Automated_Version_Latest.Text = "Latest";
            this.Automated_Version_Latest.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Automated_Version_Latest.UseVisualStyleBackColor = true;
            this.Automated_Version_Latest.CheckedChanged += new System.EventHandler(this.Automated_Version_Latest_CheckedChanged);
            // 
            // Automated_Version_Selection
            // 
            this.Automated_Version_Selection.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Automated_Version_Selection.Enabled = false;
            this.Automated_Version_Selection.FormattingEnabled = true;
            this.Automated_Version_Selection.ItemHeight = 23;
            this.Automated_Version_Selection.Location = new System.Drawing.Point(155, 43);
            this.Automated_Version_Selection.Name = "Automated_Version_Selection";
            this.Automated_Version_Selection.Size = new System.Drawing.Size(121, 29);
            this.Automated_Version_Selection.TabIndex = 6;
            this.Automated_Version_Selection.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Automated_Version_Selection.SelectedValueChanged += new System.EventHandler(this.Automated_Version_Selection_SelectedValueChanged);
            // 
            // Automated_Version_Text
            // 
            this.Automated_Version_Text.AutoSize = true;
            this.Automated_Version_Text.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.Automated_Version_Text.Location = new System.Drawing.Point(95, 49);
            this.Automated_Version_Text.Name = "Automated_Version_Text";
            this.Automated_Version_Text.Size = new System.Drawing.Size(57, 19);
            this.Automated_Version_Text.TabIndex = 5;
            this.Automated_Version_Text.Text = "Version:";
            this.Automated_Version_Text.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // Automated_UnityGame_Select
            // 
            this.Automated_UnityGame_Select.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Automated_UnityGame_Select.Location = new System.Drawing.Point(91, 12);
            this.Automated_UnityGame_Select.Name = "Automated_UnityGame_Select";
            this.Automated_UnityGame_Select.Size = new System.Drawing.Size(60, 19);
            this.Automated_UnityGame_Select.TabIndex = 4;
            this.Automated_UnityGame_Select.Text = "SELECT";
            this.Automated_UnityGame_Select.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Automated_UnityGame_Select.Click += new System.EventHandler(this.Automated_UnityGame_Select_Click);
            // 
            // Automated_UnityGame_Display
            // 
            this.Automated_UnityGame_Display.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Automated_UnityGame_Display.Location = new System.Drawing.Point(159, 11);
            this.Automated_UnityGame_Display.Name = "Automated_UnityGame_Display";
            this.Automated_UnityGame_Display.ReadOnly = true;
            this.Automated_UnityGame_Display.Size = new System.Drawing.Size(260, 20);
            this.Automated_UnityGame_Display.TabIndex = 3;
            this.Automated_UnityGame_Display.Text = "Please Select your Unity Game...";
            // 
            // Automated_UnityGame_Text
            // 
            this.Automated_UnityGame_Text.AutoSize = true;
            this.Automated_UnityGame_Text.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.Automated_UnityGame_Text.Location = new System.Drawing.Point(4, 12);
            this.Automated_UnityGame_Text.Name = "Automated_UnityGame_Text";
            this.Automated_UnityGame_Text.Size = new System.Drawing.Size(85, 19);
            this.Automated_UnityGame_Text.TabIndex = 2;
            this.Automated_UnityGame_Text.Text = "Unity Game:";
            this.Automated_UnityGame_Text.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // Automated_Divider
            // 
            this.Automated_Divider.AutoSize = true;
            this.Automated_Divider.Location = new System.Drawing.Point(1, 106);
            this.Automated_Divider.Name = "Automated_Divider";
            this.Automated_Divider.Size = new System.Drawing.Size(429, 19);
            this.Automated_Divider.TabIndex = 11;
            this.Automated_Divider.Text = "______________________________________________________________________";
            this.Automated_Divider.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Automated_Divider.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // Tab_Output
            // 
            this.Tab_Output.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Tab_Output.Controls.Add(this.Output_Current_Operation);
            this.Tab_Output.Controls.Add(this.Output_Divider);
            this.Tab_Output.Controls.Add(this.Output_Total_Progress_Text_Label);
            this.Tab_Output.Controls.Add(this.Output_Current_Progress_Text_Label);
            this.Tab_Output.Controls.Add(this.Output_Current_Progress_Display);
            this.Tab_Output.Controls.Add(this.Output_Current_Text);
            this.Tab_Output.Controls.Add(this.Output_Total_Progress_Display);
            this.Tab_Output.Controls.Add(this.Output_Total_Text);
            this.Tab_Output.Controls.Add(this.Output_Total_Progress_Text);
            this.Tab_Output.Controls.Add(this.Output_Current_Progress_Text);
            this.Tab_Output.Cursor = System.Windows.Forms.Cursors.Default;
            this.Tab_Output.HorizontalScrollbarBarColor = true;
            this.Tab_Output.Location = new System.Drawing.Point(4, 38);
            this.Tab_Output.Name = "Tab_Output";
            this.Tab_Output.Size = new System.Drawing.Size(431, 180);
            this.Tab_Output.TabIndex = 4;
            this.Tab_Output.Text = "Output   ";
            this.Tab_Output.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Tab_Output.VerticalScrollbarBarColor = true;
            // 
            // Output_Current_Operation
            // 
            this.Output_Current_Operation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Output_Current_Operation.CustomForeColor = true;
            this.Output_Current_Operation.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.Output_Current_Operation.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.Output_Current_Operation.ForeColor = System.Drawing.SystemColors.Highlight;
            this.Output_Current_Operation.Location = new System.Drawing.Point(5, 5);
            this.Output_Current_Operation.Name = "Output_Current_Operation";
            this.Output_Current_Operation.Size = new System.Drawing.Size(419, 74);
            this.Output_Current_Operation.TabIndex = 13;
            this.Output_Current_Operation.Text = "Current Operation";
            this.Output_Current_Operation.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Output_Current_Operation.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // Output_Divider
            // 
            this.Output_Divider.AutoSize = true;
            this.Output_Divider.Location = new System.Drawing.Point(0, 69);
            this.Output_Divider.Name = "Output_Divider";
            this.Output_Divider.Size = new System.Drawing.Size(429, 19);
            this.Output_Divider.TabIndex = 12;
            this.Output_Divider.Text = "______________________________________________________________________";
            this.Output_Divider.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Output_Divider.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // Output_Total_Progress_Text_Label
            // 
            this.Output_Total_Progress_Text_Label.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Output_Total_Progress_Text_Label.AutoSize = true;
            this.Output_Total_Progress_Text_Label.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.Output_Total_Progress_Text_Label.Location = new System.Drawing.Point(405, 142);
            this.Output_Total_Progress_Text_Label.Name = "Output_Total_Progress_Text_Label";
            this.Output_Total_Progress_Text_Label.Size = new System.Drawing.Size(20, 19);
            this.Output_Total_Progress_Text_Label.TabIndex = 9;
            this.Output_Total_Progress_Text_Label.Text = "%";
            this.Output_Total_Progress_Text_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Output_Total_Progress_Text_Label.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // Output_Current_Progress_Text_Label
            // 
            this.Output_Current_Progress_Text_Label.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Output_Current_Progress_Text_Label.AutoSize = true;
            this.Output_Current_Progress_Text_Label.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.Output_Current_Progress_Text_Label.Location = new System.Drawing.Point(405, 105);
            this.Output_Current_Progress_Text_Label.Name = "Output_Current_Progress_Text_Label";
            this.Output_Current_Progress_Text_Label.Size = new System.Drawing.Size(20, 19);
            this.Output_Current_Progress_Text_Label.TabIndex = 6;
            this.Output_Current_Progress_Text_Label.Text = "%";
            this.Output_Current_Progress_Text_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Output_Current_Progress_Text_Label.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // Output_Current_Progress_Display
            // 
            this.Output_Current_Progress_Display.Location = new System.Drawing.Point(64, 103);
            this.Output_Current_Progress_Display.Name = "Output_Current_Progress_Display";
            this.Output_Current_Progress_Display.Size = new System.Drawing.Size(312, 23);
            this.Output_Current_Progress_Display.Style = MetroFramework.MetroColorStyle.Green;
            this.Output_Current_Progress_Display.TabIndex = 5;
            this.Output_Current_Progress_Display.Value = 100;
            // 
            // Output_Current_Text
            // 
            this.Output_Current_Text.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.Output_Current_Text.Location = new System.Drawing.Point(3, 105);
            this.Output_Current_Text.Name = "Output_Current_Text";
            this.Output_Current_Text.Size = new System.Drawing.Size(61, 23);
            this.Output_Current_Text.TabIndex = 4;
            this.Output_Current_Text.Text = "Current:";
            this.Output_Current_Text.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // Output_Total_Progress_Display
            // 
            this.Output_Total_Progress_Display.Location = new System.Drawing.Point(64, 140);
            this.Output_Total_Progress_Display.Name = "Output_Total_Progress_Display";
            this.Output_Total_Progress_Display.Size = new System.Drawing.Size(312, 23);
            this.Output_Total_Progress_Display.Style = MetroFramework.MetroColorStyle.Green;
            this.Output_Total_Progress_Display.TabIndex = 2;
            this.Output_Total_Progress_Display.Value = 100;
            // 
            // Output_Total_Text
            // 
            this.Output_Total_Text.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.Output_Total_Text.Location = new System.Drawing.Point(20, 142);
            this.Output_Total_Text.Name = "Output_Total_Text";
            this.Output_Total_Text.Size = new System.Drawing.Size(50, 20);
            this.Output_Total_Text.TabIndex = 3;
            this.Output_Total_Text.Text = "Total:";
            this.Output_Total_Text.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // Output_Total_Progress_Text
            // 
            this.Output_Total_Progress_Text.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Output_Total_Progress_Text.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.Output_Total_Progress_Text.Location = new System.Drawing.Point(374, 142);
            this.Output_Total_Progress_Text.Name = "Output_Total_Progress_Text";
            this.Output_Total_Progress_Text.Size = new System.Drawing.Size(40, 19);
            this.Output_Total_Progress_Text.TabIndex = 10;
            this.Output_Total_Progress_Text.Text = "100";
            this.Output_Total_Progress_Text.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Output_Total_Progress_Text.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // Output_Current_Progress_Text
            // 
            this.Output_Current_Progress_Text.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Output_Current_Progress_Text.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.Output_Current_Progress_Text.Location = new System.Drawing.Point(374, 105);
            this.Output_Current_Progress_Text.Name = "Output_Current_Progress_Text";
            this.Output_Current_Progress_Text.Size = new System.Drawing.Size(40, 19);
            this.Output_Current_Progress_Text.TabIndex = 14;
            this.Output_Current_Progress_Text.Text = "100";
            this.Output_Current_Progress_Text.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Output_Current_Progress_Text.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // Tab_ManualZip
            // 
            this.Tab_ManualZip.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Tab_ManualZip.Controls.Add(this.ManualZip_ZipArchive_Select);
            this.Tab_ManualZip.Controls.Add(this.ManualZip_ZipArchive_Display);
            this.Tab_ManualZip.Controls.Add(this.ManualZip_ZipArchive_Text);
            this.Tab_ManualZip.Controls.Add(this.ManualZip_UnityGame_Select);
            this.Tab_ManualZip.Controls.Add(this.ManualZip_UnityGame_Display);
            this.Tab_ManualZip.Controls.Add(this.ManualZip_UnityGame_Text);
            this.Tab_ManualZip.Controls.Add(this.ManualZip_Uninstall);
            this.Tab_ManualZip.Controls.Add(this.ManualZip_Install);
            this.Tab_ManualZip.Controls.Add(this.ManualZip_Divider);
            this.Tab_ManualZip.Cursor = System.Windows.Forms.Cursors.Default;
            this.Tab_ManualZip.HorizontalScrollbarBarColor = true;
            this.Tab_ManualZip.Location = new System.Drawing.Point(4, 38);
            this.Tab_ManualZip.Name = "Tab_ManualZip";
            this.Tab_ManualZip.Size = new System.Drawing.Size(431, 180);
            this.Tab_ManualZip.TabIndex = 1;
            this.Tab_ManualZip.Text = "Manual Zip  ";
            this.Tab_ManualZip.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Tab_ManualZip.VerticalScrollbarBarColor = true;
            // 
            // ManualZip_ZipArchive_Select
            // 
            this.ManualZip_ZipArchive_Select.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ManualZip_ZipArchive_Select.Location = new System.Drawing.Point(91, 75);
            this.ManualZip_ZipArchive_Select.Name = "ManualZip_ZipArchive_Select";
            this.ManualZip_ZipArchive_Select.Size = new System.Drawing.Size(60, 19);
            this.ManualZip_ZipArchive_Select.TabIndex = 22;
            this.ManualZip_ZipArchive_Select.Text = "SELECT";
            this.ManualZip_ZipArchive_Select.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.ManualZip_ZipArchive_Select.Click += new System.EventHandler(this.ManualZip_ZipArchive_Select_Click);
            // 
            // ManualZip_ZipArchive_Display
            // 
            this.ManualZip_ZipArchive_Display.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ManualZip_ZipArchive_Display.Location = new System.Drawing.Point(159, 74);
            this.ManualZip_ZipArchive_Display.Name = "ManualZip_ZipArchive_Display";
            this.ManualZip_ZipArchive_Display.ReadOnly = true;
            this.ManualZip_ZipArchive_Display.Size = new System.Drawing.Size(260, 20);
            this.ManualZip_ZipArchive_Display.TabIndex = 21;
            this.ManualZip_ZipArchive_Display.Text = "Please Select your MelonLoader Zip Archive...";
            // 
            // ManualZip_ZipArchive_Text
            // 
            this.ManualZip_ZipArchive_Text.AutoSize = true;
            this.ManualZip_ZipArchive_Text.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.ManualZip_ZipArchive_Text.Location = new System.Drawing.Point(9, 74);
            this.ManualZip_ZipArchive_Text.Name = "ManualZip_ZipArchive_Text";
            this.ManualZip_ZipArchive_Text.Size = new System.Drawing.Size(80, 19);
            this.ManualZip_ZipArchive_Text.TabIndex = 20;
            this.ManualZip_ZipArchive_Text.Text = "Zip Archive:";
            this.ManualZip_ZipArchive_Text.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // ManualZip_UnityGame_Select
            // 
            this.ManualZip_UnityGame_Select.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ManualZip_UnityGame_Select.Location = new System.Drawing.Point(91, 27);
            this.ManualZip_UnityGame_Select.Name = "ManualZip_UnityGame_Select";
            this.ManualZip_UnityGame_Select.Size = new System.Drawing.Size(60, 19);
            this.ManualZip_UnityGame_Select.TabIndex = 19;
            this.ManualZip_UnityGame_Select.Text = "SELECT";
            this.ManualZip_UnityGame_Select.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.ManualZip_UnityGame_Select.Click += new System.EventHandler(this.ManualZip_UnityGame_Select_Click);
            // 
            // ManualZip_UnityGame_Display
            // 
            this.ManualZip_UnityGame_Display.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ManualZip_UnityGame_Display.Location = new System.Drawing.Point(159, 26);
            this.ManualZip_UnityGame_Display.Name = "ManualZip_UnityGame_Display";
            this.ManualZip_UnityGame_Display.ReadOnly = true;
            this.ManualZip_UnityGame_Display.Size = new System.Drawing.Size(260, 20);
            this.ManualZip_UnityGame_Display.TabIndex = 18;
            this.ManualZip_UnityGame_Display.Text = "Please Select your Unity Game...";
            // 
            // ManualZip_UnityGame_Text
            // 
            this.ManualZip_UnityGame_Text.AutoSize = true;
            this.ManualZip_UnityGame_Text.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.ManualZip_UnityGame_Text.Location = new System.Drawing.Point(4, 27);
            this.ManualZip_UnityGame_Text.Name = "ManualZip_UnityGame_Text";
            this.ManualZip_UnityGame_Text.Size = new System.Drawing.Size(85, 19);
            this.ManualZip_UnityGame_Text.TabIndex = 17;
            this.ManualZip_UnityGame_Text.Text = "Unity Game:";
            this.ManualZip_UnityGame_Text.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // ManualZip_Uninstall
            // 
            this.ManualZip_Uninstall.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ManualZip_Uninstall.Location = new System.Drawing.Point(218, 129);
            this.ManualZip_Uninstall.Name = "ManualZip_Uninstall";
            this.ManualZip_Uninstall.Size = new System.Drawing.Size(206, 44);
            this.ManualZip_Uninstall.TabIndex = 16;
            this.ManualZip_Uninstall.Text = "UN-INSTALL";
            this.ManualZip_Uninstall.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.ManualZip_Uninstall.Visible = false;
            this.ManualZip_Uninstall.Click += new System.EventHandler(this.ManualZip_Uninstall_Click);
            // 
            // ManualZip_Install
            // 
            this.ManualZip_Install.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ManualZip_Install.Enabled = false;
            this.ManualZip_Install.Location = new System.Drawing.Point(5, 129);
            this.ManualZip_Install.Name = "ManualZip_Install";
            this.ManualZip_Install.Size = new System.Drawing.Size(206, 44);
            this.ManualZip_Install.TabIndex = 15;
            this.ManualZip_Install.Text = "INSTALL";
            this.ManualZip_Install.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.ManualZip_Install.Click += new System.EventHandler(this.ManualZip_Install_Click);
            // 
            // ManualZip_Divider
            // 
            this.ManualZip_Divider.AutoSize = true;
            this.ManualZip_Divider.Location = new System.Drawing.Point(1, 106);
            this.ManualZip_Divider.Name = "ManualZip_Divider";
            this.ManualZip_Divider.Size = new System.Drawing.Size(429, 19);
            this.ManualZip_Divider.TabIndex = 12;
            this.ManualZip_Divider.Text = "______________________________________________________________________";
            this.ManualZip_Divider.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // Tab_Settings
            // 
            this.Tab_Settings.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Tab_Settings.Controls.Add(this.Settings_CloseAfterCompletion);
            this.Tab_Settings.Controls.Add(this.Settings_AutoUpdateInstaller);
            this.Tab_Settings.Controls.Add(this.Settings_Theme_Selection);
            this.Tab_Settings.Controls.Add(this.Settings_Theme_Text);
            this.Tab_Settings.Cursor = System.Windows.Forms.Cursors.Default;
            this.Tab_Settings.HorizontalScrollbarBarColor = true;
            this.Tab_Settings.Location = new System.Drawing.Point(4, 38);
            this.Tab_Settings.Name = "Tab_Settings";
            this.Tab_Settings.Size = new System.Drawing.Size(431, 180);
            this.Tab_Settings.TabIndex = 2;
            this.Tab_Settings.Text = "Settings  ";
            this.Tab_Settings.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Tab_Settings.VerticalScrollbarBarColor = true;
            // 
            // Settings_CloseAfterCompletion
            // 
            this.Settings_CloseAfterCompletion.AutoSize = true;
            this.Settings_CloseAfterCompletion.Checked = true;
            this.Settings_CloseAfterCompletion.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Settings_CloseAfterCompletion.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Settings_CloseAfterCompletion.Location = new System.Drawing.Point(158, 124);
            this.Settings_CloseAfterCompletion.Name = "Settings_CloseAfterCompletion";
            this.Settings_CloseAfterCompletion.Size = new System.Drawing.Size(147, 15);
            this.Settings_CloseAfterCompletion.Style = MetroFramework.MetroColorStyle.Green;
            this.Settings_CloseAfterCompletion.TabIndex = 5;
            this.Settings_CloseAfterCompletion.Text = "Close After Completion";
            this.Settings_CloseAfterCompletion.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Settings_CloseAfterCompletion.UseVisualStyleBackColor = true;
            this.Settings_CloseAfterCompletion.CheckedChanged += new System.EventHandler(this.Settings_CloseAfterCompletion_CheckedChanged);
            // 
            // Settings_AutoUpdateInstaller
            // 
            this.Settings_AutoUpdateInstaller.AutoSize = true;
            this.Settings_AutoUpdateInstaller.Checked = true;
            this.Settings_AutoUpdateInstaller.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Settings_AutoUpdateInstaller.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Settings_AutoUpdateInstaller.Location = new System.Drawing.Point(158, 84);
            this.Settings_AutoUpdateInstaller.Name = "Settings_AutoUpdateInstaller";
            this.Settings_AutoUpdateInstaller.Size = new System.Drawing.Size(136, 15);
            this.Settings_AutoUpdateInstaller.Style = MetroFramework.MetroColorStyle.Green;
            this.Settings_AutoUpdateInstaller.TabIndex = 4;
            this.Settings_AutoUpdateInstaller.Text = "Auto-Update Installer";
            this.Settings_AutoUpdateInstaller.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Settings_AutoUpdateInstaller.UseVisualStyleBackColor = true;
            this.Settings_AutoUpdateInstaller.CheckedChanged += new System.EventHandler(this.Settings_AutoUpdateInstaller_CheckedChanged);
            // 
            // Settings_Theme_Selection
            // 
            this.Settings_Theme_Selection.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Settings_Theme_Selection.FormattingEnabled = true;
            this.Settings_Theme_Selection.ItemHeight = 23;
            this.Settings_Theme_Selection.Items.AddRange(new object[] {
            "Dark",
            "Light"});
            this.Settings_Theme_Selection.Location = new System.Drawing.Point(217, 30);
            this.Settings_Theme_Selection.Name = "Settings_Theme_Selection";
            this.Settings_Theme_Selection.Size = new System.Drawing.Size(61, 29);
            this.Settings_Theme_Selection.TabIndex = 3;
            this.Settings_Theme_Selection.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Settings_Theme_Selection.SelectedIndexChanged += new System.EventHandler(this.ThemeChanged);
            // 
            // Settings_Theme_Text
            // 
            this.Settings_Theme_Text.AutoSize = true;
            this.Settings_Theme_Text.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.Settings_Theme_Text.Location = new System.Drawing.Point(158, 35);
            this.Settings_Theme_Text.Name = "Settings_Theme_Text";
            this.Settings_Theme_Text.Size = new System.Drawing.Size(53, 19);
            this.Settings_Theme_Text.TabIndex = 2;
            this.Settings_Theme_Text.Text = "Theme:";
            this.Settings_Theme_Text.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // Tab_PleaseWait
            // 
            this.Tab_PleaseWait.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Tab_PleaseWait.Controls.Add(this.PleaseWait_PleaseWait);
            this.Tab_PleaseWait.Controls.Add(this.PleaseWait_Text);
            this.Tab_PleaseWait.Cursor = System.Windows.Forms.Cursors.Default;
            this.Tab_PleaseWait.HorizontalScrollbarBarColor = true;
            this.Tab_PleaseWait.Location = new System.Drawing.Point(4, 38);
            this.Tab_PleaseWait.Name = "Tab_PleaseWait";
            this.Tab_PleaseWait.Size = new System.Drawing.Size(431, 180);
            this.Tab_PleaseWait.TabIndex = 5;
            this.Tab_PleaseWait.Text = "Please Wait    ";
            this.Tab_PleaseWait.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Tab_PleaseWait.VerticalScrollbarBarColor = true;
            // 
            // PleaseWait_PleaseWait
            // 
            this.PleaseWait_PleaseWait.AutoSize = true;
            this.PleaseWait_PleaseWait.CustomForeColor = true;
            this.PleaseWait_PleaseWait.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.PleaseWait_PleaseWait.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.PleaseWait_PleaseWait.ForeColor = System.Drawing.SystemColors.Highlight;
            this.PleaseWait_PleaseWait.Location = new System.Drawing.Point(161, 36);
            this.PleaseWait_PleaseWait.Name = "PleaseWait_PleaseWait";
            this.PleaseWait_PleaseWait.Size = new System.Drawing.Size(127, 25);
            this.PleaseWait_PleaseWait.TabIndex = 5;
            this.PleaseWait_PleaseWait.Text = "PLEASE WAIT";
            this.PleaseWait_PleaseWait.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.PleaseWait_PleaseWait.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // PleaseWait_Text
            // 
            this.PleaseWait_Text.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.PleaseWait_Text.Location = new System.Drawing.Point(127, 79);
            this.PleaseWait_Text.Name = "PleaseWait_Text";
            this.PleaseWait_Text.Size = new System.Drawing.Size(200, 22);
            this.PleaseWait_Text.TabIndex = 4;
            this.PleaseWait_Text.Text = "Checking for Updates...";
            this.PleaseWait_Text.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.PleaseWait_Text.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // Tab_Error
            // 
            this.Tab_Error.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Tab_Error.Controls.Add(this.Error_Retry);
            this.Tab_Error.Controls.Add(this.Error_Error);
            this.Tab_Error.Controls.Add(this.Error_Text);
            this.Tab_Error.Cursor = System.Windows.Forms.Cursors.Default;
            this.Tab_Error.HorizontalScrollbarBarColor = true;
            this.Tab_Error.Location = new System.Drawing.Point(4, 38);
            this.Tab_Error.Name = "Tab_Error";
            this.Tab_Error.Size = new System.Drawing.Size(431, 180);
            this.Tab_Error.Style = MetroFramework.MetroColorStyle.Red;
            this.Tab_Error.TabIndex = 3;
            this.Tab_Error.Text = "Error";
            this.Tab_Error.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Tab_Error.VerticalScrollbarBarColor = true;
            // 
            // Error_Retry
            // 
            this.Error_Retry.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Error_Retry.Location = new System.Drawing.Point(183, 122);
            this.Error_Retry.Name = "Error_Retry";
            this.Error_Retry.Size = new System.Drawing.Size(75, 23);
            this.Error_Retry.TabIndex = 4;
            this.Error_Retry.Text = "RETRY";
            this.Error_Retry.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Error_Retry.Click += new System.EventHandler(this.Error_Retry_Click);
            // 
            // Error_Error
            // 
            this.Error_Error.AutoSize = true;
            this.Error_Error.CustomForeColor = true;
            this.Error_Error.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.Error_Error.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.Error_Error.ForeColor = System.Drawing.Color.Red;
            this.Error_Error.Location = new System.Drawing.Point(184, 36);
            this.Error_Error.Name = "Error_Error";
            this.Error_Error.Size = new System.Drawing.Size(72, 25);
            this.Error_Error.TabIndex = 3;
            this.Error_Error.Text = "ERROR";
            this.Error_Error.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Error_Error.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // Error_Text
            // 
            this.Error_Text.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Error_Text.AutoSize = true;
            this.Error_Text.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.Error_Text.Location = new System.Drawing.Point(94, 79);
            this.Error_Text.Name = "Error_Text";
            this.Error_Text.Size = new System.Drawing.Size(266, 19);
            this.Error_Text.TabIndex = 2;
            this.Error_Text.Text = "Failed to get List of Releases from GitHub!";
            this.Error_Text.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Error_Text.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // InstallerUpdateNotice
            // 
            this.InstallerUpdateNotice.Cursor = System.Windows.Forms.Cursors.Hand;
            this.InstallerUpdateNotice.CustomForeColor = true;
            this.InstallerUpdateNotice.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.InstallerUpdateNotice.ForeColor = System.Drawing.Color.Lime;
            this.InstallerUpdateNotice.Location = new System.Drawing.Point(22, 65);
            this.InstallerUpdateNotice.Name = "InstallerUpdateNotice";
            this.InstallerUpdateNotice.Size = new System.Drawing.Size(85, 41);
            this.InstallerUpdateNotice.Style = MetroFramework.MetroColorStyle.Green;
            this.InstallerUpdateNotice.TabIndex = 11;
            this.InstallerUpdateNotice.Text = "New Update\r\nAvailable!";
            this.InstallerUpdateNotice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.InstallerUpdateNotice.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.InstallerUpdateNotice.Visible = false;
            this.InstallerUpdateNotice.Click += new System.EventHandler(this.InstallerUpdateNotice_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(481, 441);
            this.Controls.Add(this.InstallerUpdateNotice);
            this.Controls.Add(this.PageManager);
            this.Controls.Add(this.Link_Wiki);
            this.Controls.Add(this.InstallerVersion);
            this.Controls.Add(this.Link_GitHub);
            this.Controls.Add(this.Link_Twitter);
            this.Controls.Add(this.Link_Discord);
            this.Controls.Add(this.ML_Text);
            this.Controls.Add(this.ML_Logo);
            this.DisplayHeader = false;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(20, 30, 20, 20);
            this.Resizable = false;
            this.Style = MetroFramework.MetroColorStyle.Green;
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ThemeManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Link_Wiki)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Link_GitHub)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Link_Twitter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Link_Discord)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ML_Text)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ML_Logo)).EndInit();
            this.PageManager.ResumeLayout(false);
            this.Tab_Automated.ResumeLayout(false);
            this.Tab_Automated.PerformLayout();
            this.Tab_Output.ResumeLayout(false);
            this.Tab_Output.PerformLayout();
            this.Tab_ManualZip.ResumeLayout(false);
            this.Tab_ManualZip.PerformLayout();
            this.Tab_Settings.ResumeLayout(false);
            this.Tab_Settings.PerformLayout();
            this.Tab_PleaseWait.ResumeLayout(false);
            this.Tab_PleaseWait.PerformLayout();
            this.Tab_Error.ResumeLayout(false);
            this.Tab_Error.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox ML_Logo;
        private System.Windows.Forms.PictureBox ML_Text;
        private MetroFramework.Components.MetroStyleManager ThemeManager;
        private System.Windows.Forms.PictureBox Link_Discord;
        private System.Windows.Forms.PictureBox Link_Twitter;
        private System.Windows.Forms.PictureBox Link_GitHub;
        private MetroFramework.Controls.MetroLabel InstallerVersion;
        private System.Windows.Forms.PictureBox Link_Wiki;
        private MetroFramework.Controls.MetroTabControl PageManager;
        private MetroFramework.Controls.MetroTabPage Tab_Automated;
        private MetroFramework.Controls.MetroTabPage Tab_ManualZip;
        private MetroFramework.Controls.MetroTabPage Tab_Settings;
        private MetroFramework.Controls.MetroComboBox Settings_Theme_Selection;
        private MetroFramework.Controls.MetroLabel Settings_Theme_Text;
        private MetroFramework.Controls.MetroCheckBox Settings_AutoUpdateInstaller;
        private MetroFramework.Controls.MetroCheckBox Settings_CloseAfterCompletion;
        private MetroFramework.Controls.MetroTabPage Tab_Error;
        private MetroFramework.Controls.MetroButton Error_Retry;
        private MetroFramework.Controls.MetroLabel Error_Text;
        private MetroFramework.Controls.MetroLabel Error_Error;
        private MetroFramework.Controls.MetroLabel Automated_UnityGame_Text;
        private System.Windows.Forms.TextBox Automated_UnityGame_Display;
        private MetroFramework.Controls.MetroButton Automated_UnityGame_Select;
        private MetroFramework.Controls.MetroLabel Automated_Version_Text;
        private MetroFramework.Controls.MetroCheckBox Automated_Version_Latest;
        private MetroFramework.Controls.MetroComboBox Automated_Version_Selection;
        private MetroFramework.Controls.MetroCheckBox Automated_Arch_AutoDetect;
        private MetroFramework.Controls.MetroComboBox Automated_Arch_Selection;
        private MetroFramework.Controls.MetroLabel Automated_Arch_Text;
        private MetroFramework.Controls.MetroLabel Automated_Divider;
        private MetroFramework.Controls.MetroLabel ManualZip_Divider;
        private MetroFramework.Controls.MetroButton Automated_Install;
        private MetroFramework.Controls.MetroButton Automated_Uninstall;
        private MetroFramework.Controls.MetroTabPage Tab_Output;
        private MetroFramework.Controls.MetroButton ManualZip_Uninstall;
        private MetroFramework.Controls.MetroButton ManualZip_Install;
        private MetroFramework.Controls.MetroButton ManualZip_ZipArchive_Select;
        private System.Windows.Forms.TextBox ManualZip_ZipArchive_Display;
        private MetroFramework.Controls.MetroLabel ManualZip_ZipArchive_Text;
        private MetroFramework.Controls.MetroButton ManualZip_UnityGame_Select;
        private System.Windows.Forms.TextBox ManualZip_UnityGame_Display;
        private MetroFramework.Controls.MetroLabel ManualZip_UnityGame_Text;
        private MetroFramework.Controls.MetroLabel InstallerUpdateNotice;
        private MetroFramework.Controls.MetroTabPage Tab_PleaseWait;
        private MetroFramework.Controls.MetroLabel PleaseWait_PleaseWait;
        private MetroFramework.Controls.MetroLabel PleaseWait_Text;
        private MetroFramework.Controls.MetroLabel Output_Total_Progress_Text_Label;
        internal MetroFramework.Controls.MetroLabel Output_Total_Progress_Text;
        private MetroFramework.Controls.MetroLabel Output_Current_Progress_Text_Label;
        internal MetroFramework.Controls.MetroProgressBar Output_Current_Progress_Display;
        private MetroFramework.Controls.MetroLabel Output_Current_Text;
        private MetroFramework.Controls.MetroLabel Output_Total_Text;
        internal MetroFramework.Controls.MetroProgressBar Output_Total_Progress_Display;
        private MetroFramework.Controls.MetroLabel Output_Divider;
        internal MetroFramework.Controls.MetroLabel Output_Current_Operation;
        internal MetroFramework.Controls.MetroLabel Output_Current_Progress_Text;
        private MetroFramework.Controls.MetroLabel Automated_x64Only;
    }
}