namespace TootTallyDifficultyCalculator2._0
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ButtonForceRefresh = new Button();
            ComboBoxReplay = new ComboBox();
            ButtonLoadReplay = new Button();
            checkboxAllSpeed = new CheckBox();
            textBox1 = new TextBox();
            label1 = new Label();
            ButtonSwitchBox = new Button();
            TextBoxChartData = new TextBox();
            TextBoxLeaderboardData = new TextBox();
            ProgressBarLoading = new ProgressBar();
            LoadingLabel = new Label();
            label2 = new Label();
            label3 = new Label();
            FilterMinTT = new NumericUpDown();
            FilterMaxTT = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)FilterMinTT).BeginInit();
            ((System.ComponentModel.ISupportInitialize)FilterMaxTT).BeginInit();
            SuspendLayout();
            // 
            // ButtonForceRefresh
            // 
            ButtonForceRefresh.BackColor = Color.Crimson;
            ButtonForceRefresh.FlatStyle = FlatStyle.Popup;
            ButtonForceRefresh.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            ButtonForceRefresh.ForeColor = Color.White;
            ButtonForceRefresh.Location = new Point(12, 12);
            ButtonForceRefresh.Name = "ButtonForceRefresh";
            ButtonForceRefresh.Size = new Size(140, 25);
            ButtonForceRefresh.TabIndex = 1;
            ButtonForceRefresh.Text = "FORCE REFRESH";
            ButtonForceRefresh.UseVisualStyleBackColor = false;
            ButtonForceRefresh.Click += OnDisplayChartsButtonClick;
            // 
            // ComboBoxReplay
            // 
            ComboBoxReplay.BackColor = Color.DarkRed;
            ComboBoxReplay.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBoxReplay.FlatStyle = FlatStyle.Popup;
            ComboBoxReplay.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            ComboBoxReplay.ForeColor = SystemColors.Window;
            ComboBoxReplay.FormattingEnabled = true;
            ComboBoxReplay.Location = new Point(514, 10);
            ComboBoxReplay.Name = "ComboBoxReplay";
            ComboBoxReplay.Size = new Size(281, 25);
            ComboBoxReplay.TabIndex = 0;
            ComboBoxReplay.SelectedValueChanged += OnDropDownReplayValueChange;
            // 
            // ButtonLoadReplay
            // 
            ButtonLoadReplay.BackColor = Color.Crimson;
            ButtonLoadReplay.FlatStyle = FlatStyle.Popup;
            ButtonLoadReplay.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            ButtonLoadReplay.ForeColor = Color.White;
            ButtonLoadReplay.Location = new Point(801, 10);
            ButtonLoadReplay.Name = "ButtonLoadReplay";
            ButtonLoadReplay.Size = new Size(112, 25);
            ButtonLoadReplay.TabIndex = 1;
            ButtonLoadReplay.Text = "LOAD REPLAY";
            ButtonLoadReplay.UseVisualStyleBackColor = false;
            ButtonLoadReplay.Visible = false;
            ButtonLoadReplay.Click += OnLoadReplayButtonClick;
            // 
            // checkboxAllSpeed
            // 
            checkboxAllSpeed.AutoSize = true;
            checkboxAllSpeed.Checked = true;
            checkboxAllSpeed.CheckState = CheckState.Checked;
            checkboxAllSpeed.ForeColor = Color.White;
            checkboxAllSpeed.Location = new Point(12, 43);
            checkboxAllSpeed.Name = "checkboxAllSpeed";
            checkboxAllSpeed.Size = new Size(75, 19);
            checkboxAllSpeed.TabIndex = 3;
            checkboxAllSpeed.Text = "All Speed";
            checkboxAllSpeed.UseVisualStyleBackColor = true;
            checkboxAllSpeed.CheckedChanged += OnValueBoxTextChanged;
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.DarkRed;
            textBox1.Font = new Font("Courier New", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            textBox1.ForeColor = Color.White;
            textBox1.Location = new Point(227, 12);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(281, 22);
            textBox1.TabIndex = 4;
            textBox1.KeyPress += OnTextBoxTextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            label1.ForeColor = Color.White;
            label1.Location = new Point(161, 14);
            label1.Name = "label1";
            label1.Size = new Size(60, 20);
            label1.TabIndex = 5;
            label1.Text = "FILTER:";
            // 
            // ButtonSwitchBox
            // 
            ButtonSwitchBox.BackColor = Color.Crimson;
            ButtonSwitchBox.FlatStyle = FlatStyle.Popup;
            ButtonSwitchBox.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            ButtonSwitchBox.ForeColor = Color.White;
            ButtonSwitchBox.Location = new Point(985, 216);
            ButtonSwitchBox.Name = "ButtonSwitchBox";
            ButtonSwitchBox.Size = new Size(112, 25);
            ButtonSwitchBox.TabIndex = 1;
            ButtonSwitchBox.Text = "SWITCH BOX";
            ButtonSwitchBox.UseVisualStyleBackColor = false;
            ButtonSwitchBox.Click += OnSwitchBoxClick;
            // 
            // TextBoxChartData
            // 
            TextBoxChartData.BackColor = Color.DarkRed;
            TextBoxChartData.ForeColor = Color.White;
            TextBoxChartData.Location = new Point(12, 247);
            TextBoxChartData.Multiline = true;
            TextBoxChartData.Name = "TextBoxChartData";
            TextBoxChartData.ReadOnly = true;
            TextBoxChartData.ScrollBars = ScrollBars.Vertical;
            TextBoxChartData.Size = new Size(1085, 699);
            TextBoxChartData.TabIndex = 7;
            // 
            // TextBoxLeaderboardData
            // 
            TextBoxLeaderboardData.BackColor = Color.DarkRed;
            TextBoxLeaderboardData.Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point);
            TextBoxLeaderboardData.ForeColor = Color.White;
            TextBoxLeaderboardData.Location = new Point(12, 247);
            TextBoxLeaderboardData.Multiline = true;
            TextBoxLeaderboardData.Name = "TextBoxLeaderboardData";
            TextBoxLeaderboardData.ReadOnly = true;
            TextBoxLeaderboardData.ScrollBars = ScrollBars.Vertical;
            TextBoxLeaderboardData.Size = new Size(1085, 699);
            TextBoxLeaderboardData.TabIndex = 8;
            TextBoxLeaderboardData.Text = "Test Font";
            TextBoxLeaderboardData.Visible = false;
            // 
            // ProgressBarLoading
            // 
            ProgressBarLoading.BackColor = Color.White;
            ProgressBarLoading.Location = new Point(300, 141);
            ProgressBarLoading.Name = "ProgressBarLoading";
            ProgressBarLoading.Size = new Size(451, 24);
            ProgressBarLoading.Step = 1;
            ProgressBarLoading.Style = ProgressBarStyle.Continuous;
            ProgressBarLoading.TabIndex = 9;
            // 
            // LoadingLabel
            // 
            LoadingLabel.AutoSize = true;
            LoadingLabel.BackColor = Color.DarkRed;
            LoadingLabel.Font = new Font("Segoe UI", 48F, FontStyle.Regular, GraphicsUnit.Point);
            LoadingLabel.ForeColor = Color.White;
            LoadingLabel.Location = new Point(300, 265);
            LoadingLabel.Name = "LoadingLabel";
            LoadingLabel.Size = new Size(306, 86);
            LoadingLabel.TabIndex = 10;
            LoadingLabel.Text = "LOADING";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            label2.ForeColor = Color.White;
            label2.Location = new Point(177, 44);
            label2.Name = "label2";
            label2.Size = new Size(44, 20);
            label2.TabIndex = 5;
            label2.Text = "MIN:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            label3.ForeColor = Color.White;
            label3.Location = new Point(358, 47);
            label3.Name = "label3";
            label3.Size = new Size(48, 20);
            label3.TabIndex = 5;
            label3.Text = "MAX:";
            // 
            // FilterMinTT
            // 
            FilterMinTT.BackColor = Color.DarkRed;
            FilterMinTT.ForeColor = Color.White;
            FilterMinTT.Increment = new decimal(new int[] { 10, 0, 0, 0 });
            FilterMinTT.Location = new Point(227, 44);
            FilterMinTT.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            FilterMinTT.Name = "FilterMinTT";
            FilterMinTT.Size = new Size(96, 23);
            FilterMinTT.TabIndex = 11;
            FilterMinTT.ValueChanged += OnValueBoxTextChanged;
            // 
            // FilterMaxTT
            // 
            FilterMaxTT.BackColor = Color.DarkRed;
            FilterMaxTT.ForeColor = Color.White;
            FilterMaxTT.Location = new Point(412, 44);
            FilterMaxTT.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            FilterMaxTT.Name = "FilterMaxTT";
            FilterMaxTT.Size = new Size(96, 23);
            FilterMaxTT.TabIndex = 11;
            FilterMaxTT.Value = new decimal(new int[] { 9999, 0, 0, 0 });
            FilterMaxTT.ValueChanged += OnValueBoxTextChanged;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Desktop;
            ClientSize = new Size(1109, 965);
            Controls.Add(FilterMaxTT);
            Controls.Add(FilterMinTT);
            Controls.Add(LoadingLabel);
            Controls.Add(ProgressBarLoading);
            Controls.Add(TextBoxLeaderboardData);
            Controls.Add(TextBoxChartData);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(textBox1);
            Controls.Add(checkboxAllSpeed);
            Controls.Add(ButtonSwitchBox);
            Controls.Add(ButtonLoadReplay);
            Controls.Add(ButtonForceRefresh);
            Controls.Add(ComboBoxReplay);
            Name = "MainForm";
            Text = "Main";
            Shown += OnFormShown;
            ((System.ComponentModel.ISupportInitialize)FilterMinTT).EndInit();
            ((System.ComponentModel.ISupportInitialize)FilterMaxTT).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button ButtonForceRefresh;
        private ComboBox ComboBoxReplay;
        private Button ButtonLoadReplay;
        private CheckBox checkboxAllSpeed;
        private TextBox textBox1;
        private Label label1;
        private Button ButtonSwitchBox;
        private TextBox TextBoxChartData;
        private TextBox TextBoxLeaderboardData;
        private ProgressBar ProgressBarLoading;
        private Label LoadingLabel;
        private Label label2;
        private Label label3;
        private NumericUpDown FilterMinTT;
        private NumericUpDown FilterMaxTT;
    }
}