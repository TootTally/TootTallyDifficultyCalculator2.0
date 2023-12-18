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
            checkboxAllSpeed = new CheckBox();
            FilterMapName = new TextBox();
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
            FilterPlayerName = new TextBox();
            label4 = new Label();
            ButtonSaveTo = new Button();
            AimNum = new NumericUpDown();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            TapNum = new NumericUpDown();
            AccNum = new NumericUpDown();
            AimEndNote = new NumericUpDown();
            TapEndMult = new NumericUpDown();
            AimEndSlider = new NumericUpDown();
            AimEndMult = new NumericUpDown();
            RatingOffset = new NumericUpDown();
            label8 = new Label();
            label9 = new Label();
            label10 = new Label();
            label11 = new Label();
            label12 = new Label();
            label13 = new Label();
            EndDrain = new NumericUpDown();
            MaxTime = new NumericUpDown();
            label14 = new Label();
            FilterModifierOnly = new CheckBox();
            BiasMult = new NumericUpDown();
            label15 = new Label();
            ((System.ComponentModel.ISupportInitialize)FilterMinTT).BeginInit();
            ((System.ComponentModel.ISupportInitialize)FilterMaxTT).BeginInit();
            ((System.ComponentModel.ISupportInitialize)AimNum).BeginInit();
            ((System.ComponentModel.ISupportInitialize)TapNum).BeginInit();
            ((System.ComponentModel.ISupportInitialize)AccNum).BeginInit();
            ((System.ComponentModel.ISupportInitialize)AimEndNote).BeginInit();
            ((System.ComponentModel.ISupportInitialize)TapEndMult).BeginInit();
            ((System.ComponentModel.ISupportInitialize)AimEndSlider).BeginInit();
            ((System.ComponentModel.ISupportInitialize)AimEndMult).BeginInit();
            ((System.ComponentModel.ISupportInitialize)RatingOffset).BeginInit();
            ((System.ComponentModel.ISupportInitialize)EndDrain).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MaxTime).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BiasMult).BeginInit();
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
            ButtonForceRefresh.Click += OnForceRefreshClick;
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
            // FilterMapName
            // 
            FilterMapName.BackColor = Color.DarkRed;
            FilterMapName.Font = new Font("Courier New", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            FilterMapName.ForeColor = Color.White;
            FilterMapName.Location = new Point(263, 12);
            FilterMapName.Name = "FilterMapName";
            FilterMapName.Size = new Size(281, 22);
            FilterMapName.TabIndex = 4;
            FilterMapName.KeyPress += OnTextBoxTextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            label1.ForeColor = Color.White;
            label1.Location = new Point(161, 14);
            label1.Name = "label1";
            label1.Size = new Size(96, 20);
            label1.TabIndex = 5;
            label1.Text = "MAP NAME:";
            // 
            // ButtonSwitchBox
            // 
            ButtonSwitchBox.BackColor = Color.Crimson;
            ButtonSwitchBox.FlatStyle = FlatStyle.Popup;
            ButtonSwitchBox.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            ButtonSwitchBox.ForeColor = Color.White;
            ButtonSwitchBox.Location = new Point(983, 216);
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
            label2.Location = new Point(214, 43);
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
            label3.Location = new Point(395, 46);
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
            FilterMinTT.Location = new Point(264, 43);
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
            FilterMaxTT.Location = new Point(449, 43);
            FilterMaxTT.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            FilterMaxTT.Name = "FilterMaxTT";
            FilterMaxTT.Size = new Size(96, 23);
            FilterMaxTT.TabIndex = 11;
            FilterMaxTT.Value = new decimal(new int[] { 999999, 0, 0, 0 });
            FilterMaxTT.ValueChanged += OnValueBoxTextChanged;
            // 
            // FilterPlayerName
            // 
            FilterPlayerName.BackColor = Color.DarkRed;
            FilterPlayerName.Font = new Font("Courier New", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            FilterPlayerName.ForeColor = Color.White;
            FilterPlayerName.Location = new Point(264, 72);
            FilterPlayerName.Name = "FilterPlayerName";
            FilterPlayerName.Size = new Size(281, 22);
            FilterPlayerName.TabIndex = 4;
            FilterPlayerName.KeyPress += OnTextBoxTextChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            label4.ForeColor = Color.White;
            label4.Location = new Point(190, 72);
            label4.Name = "label4";
            label4.Size = new Size(67, 20);
            label4.TabIndex = 5;
            label4.Text = "PLAYER:";
            // 
            // ButtonSaveTo
            // 
            ButtonSaveTo.BackColor = Color.Crimson;
            ButtonSaveTo.FlatStyle = FlatStyle.Popup;
            ButtonSaveTo.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            ButtonSaveTo.ForeColor = Color.White;
            ButtonSaveTo.Location = new Point(865, 216);
            ButtonSaveTo.Name = "ButtonSaveTo";
            ButtonSaveTo.Size = new Size(112, 25);
            ButtonSaveTo.TabIndex = 1;
            ButtonSaveTo.Text = "SAVE TO";
            ButtonSaveTo.UseVisualStyleBackColor = false;
            ButtonSaveTo.Click += OnSaveToButtonPress;
            // 
            // AimNum
            // 
            AimNum.BackColor = Color.DarkRed;
            AimNum.ForeColor = Color.White;
            AimNum.Location = new Point(56, 107);
            AimNum.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            AimNum.Name = "AimNum";
            AimNum.Size = new Size(96, 23);
            AimNum.TabIndex = 11;
            AimNum.Value = new decimal(new int[] { 22, 0, 0, 0 });
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            label5.ForeColor = Color.White;
            label5.Location = new Point(6, 107);
            label5.Name = "label5";
            label5.Size = new Size(43, 20);
            label5.TabIndex = 5;
            label5.Text = "AIM:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            label6.ForeColor = Color.White;
            label6.Location = new Point(6, 145);
            label6.Name = "label6";
            label6.Size = new Size(41, 20);
            label6.TabIndex = 5;
            label6.Text = "TAP:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            label7.ForeColor = Color.White;
            label7.Location = new Point(6, 181);
            label7.Name = "label7";
            label7.Size = new Size(42, 20);
            label7.TabIndex = 5;
            label7.Text = "ACC:";
            // 
            // TapNum
            // 
            TapNum.BackColor = Color.DarkRed;
            TapNum.ForeColor = Color.White;
            TapNum.Location = new Point(56, 142);
            TapNum.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            TapNum.Name = "TapNum";
            TapNum.Size = new Size(96, 23);
            TapNum.TabIndex = 11;
            TapNum.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // AccNum
            // 
            AccNum.BackColor = Color.DarkRed;
            AccNum.ForeColor = Color.White;
            AccNum.Location = new Point(56, 178);
            AccNum.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            AccNum.Name = "AccNum";
            AccNum.Size = new Size(96, 23);
            AccNum.TabIndex = 11;
            AccNum.Value = new decimal(new int[] { 11, 0, 0, 0 });
            // 
            // AimEndNote
            // 
            AimEndNote.BackColor = Color.DarkRed;
            AimEndNote.ForeColor = Color.White;
            AimEndNote.Location = new Point(204, 104);
            AimEndNote.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            AimEndNote.Name = "AimEndNote";
            AimEndNote.Size = new Size(96, 23);
            AimEndNote.TabIndex = 11;
            AimEndNote.Value = new decimal(new int[] { 22, 0, 0, 0 });
            // 
            // TapEndMult
            // 
            TapEndMult.BackColor = Color.DarkRed;
            TapEndMult.ForeColor = Color.White;
            TapEndMult.Location = new Point(204, 142);
            TapEndMult.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            TapEndMult.Name = "TapEndMult";
            TapEndMult.Size = new Size(96, 23);
            TapEndMult.TabIndex = 11;
            TapEndMult.Value = new decimal(new int[] { 15, 0, 0, 0 });
            // 
            // AimEndSlider
            // 
            AimEndSlider.BackColor = Color.DarkRed;
            AimEndSlider.ForeColor = Color.White;
            AimEndSlider.Location = new Point(331, 104);
            AimEndSlider.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            AimEndSlider.Name = "AimEndSlider";
            AimEndSlider.Size = new Size(96, 23);
            AimEndSlider.TabIndex = 11;
            AimEndSlider.Value = new decimal(new int[] { 4, 0, 0, 0 });
            // 
            // AimEndMult
            // 
            AimEndMult.BackColor = Color.DarkRed;
            AimEndMult.ForeColor = Color.White;
            AimEndMult.Location = new Point(462, 104);
            AimEndMult.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            AimEndMult.Name = "AimEndMult";
            AimEndMult.Size = new Size(96, 23);
            AimEndMult.TabIndex = 11;
            AimEndMult.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // RatingOffset
            // 
            RatingOffset.BackColor = Color.DarkRed;
            RatingOffset.ForeColor = Color.White;
            RatingOffset.Location = new Point(56, 216);
            RatingOffset.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            RatingOffset.Name = "RatingOffset";
            RatingOffset.Size = new Size(96, 23);
            RatingOffset.TabIndex = 11;
            RatingOffset.Value = new decimal(new int[] { 34, 0, 0, 0 });
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            label8.ForeColor = Color.White;
            label8.Location = new Point(6, 218);
            label8.Name = "label8";
            label8.Size = new Size(40, 20);
            label8.TabIndex = 5;
            label8.Text = "OFF:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            label9.ForeColor = Color.White;
            label9.Location = new Point(165, 107);
            label9.Name = "label9";
            label9.Size = new Size(33, 20);
            label9.TabIndex = 5;
            label9.Text = "EN:";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            label10.ForeColor = Color.White;
            label10.Location = new Point(165, 142);
            label10.Name = "label10";
            label10.Size = new Size(30, 20);
            label10.TabIndex = 5;
            label10.Text = "ET:";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            label11.ForeColor = Color.White;
            label11.Location = new Point(300, 105);
            label11.Name = "label11";
            label11.Size = new Size(29, 20);
            label11.TabIndex = 5;
            label11.Text = "ES:";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            label12.ForeColor = Color.White;
            label12.Location = new Point(428, 105);
            label12.Name = "label12";
            label12.Size = new Size(35, 20);
            label12.TabIndex = 5;
            label12.Text = "EM:";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            label13.ForeColor = Color.White;
            label13.Location = new Point(165, 181);
            label13.Name = "label13";
            label13.Size = new Size(32, 20);
            label13.TabIndex = 5;
            label13.Text = "ED:";
            // 
            // EndDrain
            // 
            EndDrain.BackColor = Color.DarkRed;
            EndDrain.DecimalPlaces = 4;
            EndDrain.ForeColor = Color.White;
            EndDrain.Location = new Point(201, 183);
            EndDrain.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            EndDrain.Name = "EndDrain";
            EndDrain.Size = new Size(96, 23);
            EndDrain.TabIndex = 11;
            EndDrain.Value = new decimal(new int[] { 1025, 0, 0, 196608 });
            // 
            // MaxTime
            // 
            MaxTime.BackColor = Color.DarkRed;
            MaxTime.DecimalPlaces = 2;
            MaxTime.ForeColor = Color.White;
            MaxTime.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            MaxTime.Location = new Point(201, 218);
            MaxTime.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            MaxTime.Minimum = new decimal(new int[] { 1, 0, 0, 131072 });
            MaxTime.Name = "MaxTime";
            MaxTime.Size = new Size(96, 23);
            MaxTime.TabIndex = 11;
            MaxTime.Value = new decimal(new int[] { 15, 0, 0, 131072 });
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            label14.ForeColor = Color.White;
            label14.Location = new Point(163, 219);
            label14.Name = "label14";
            label14.Size = new Size(35, 20);
            label14.TabIndex = 5;
            label14.Text = "MS:";
            // 
            // FilterModifierOnly
            // 
            FilterModifierOnly.AutoSize = true;
            FilterModifierOnly.ForeColor = Color.White;
            FilterModifierOnly.Location = new Point(551, 74);
            FilterModifierOnly.Name = "FilterModifierOnly";
            FilterModifierOnly.Size = new Size(104, 19);
            FilterModifierOnly.TabIndex = 3;
            FilterModifierOnly.Text = "Modifiers Only";
            FilterModifierOnly.UseVisualStyleBackColor = true;
            FilterModifierOnly.CheckedChanged += OnValueBoxTextChanged;
            // 
            // BiasMult
            // 
            BiasMult.BackColor = Color.DarkRed;
            BiasMult.DecimalPlaces = 2;
            BiasMult.ForeColor = Color.White;
            BiasMult.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            BiasMult.Location = new Point(359, 218);
            BiasMult.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            BiasMult.Minimum = new decimal(new int[] { 1, 0, 0, 131072 });
            BiasMult.Name = "BiasMult";
            BiasMult.Size = new Size(96, 23);
            BiasMult.TabIndex = 11;
            BiasMult.Value = new decimal(new int[] { 5, 0, 0, 65536 });
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            label15.ForeColor = Color.White;
            label15.Location = new Point(306, 218);
            label15.Name = "label15";
            label15.Size = new Size(47, 20);
            label15.TabIndex = 5;
            label15.Text = "BIAS:";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Desktop;
            ClientSize = new Size(1109, 965);
            Controls.Add(FilterMaxTT);
            Controls.Add(RatingOffset);
            Controls.Add(BiasMult);
            Controls.Add(MaxTime);
            Controls.Add(EndDrain);
            Controls.Add(TapEndMult);
            Controls.Add(AccNum);
            Controls.Add(TapNum);
            Controls.Add(AimEndMult);
            Controls.Add(AimEndSlider);
            Controls.Add(AimEndNote);
            Controls.Add(AimNum);
            Controls.Add(FilterMinTT);
            Controls.Add(LoadingLabel);
            Controls.Add(ProgressBarLoading);
            Controls.Add(TextBoxLeaderboardData);
            Controls.Add(TextBoxChartData);
            Controls.Add(label3);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label12);
            Controls.Add(label11);
            Controls.Add(label15);
            Controls.Add(label14);
            Controls.Add(label13);
            Controls.Add(label10);
            Controls.Add(label9);
            Controls.Add(label5);
            Controls.Add(label2);
            Controls.Add(label4);
            Controls.Add(label1);
            Controls.Add(FilterPlayerName);
            Controls.Add(FilterMapName);
            Controls.Add(FilterModifierOnly);
            Controls.Add(checkboxAllSpeed);
            Controls.Add(ButtonSaveTo);
            Controls.Add(ButtonSwitchBox);
            Controls.Add(ButtonForceRefresh);
            Name = "MainForm";
            Text = "Main";
            Shown += OnFormShown;
            ((System.ComponentModel.ISupportInitialize)FilterMinTT).EndInit();
            ((System.ComponentModel.ISupportInitialize)FilterMaxTT).EndInit();
            ((System.ComponentModel.ISupportInitialize)AimNum).EndInit();
            ((System.ComponentModel.ISupportInitialize)TapNum).EndInit();
            ((System.ComponentModel.ISupportInitialize)AccNum).EndInit();
            ((System.ComponentModel.ISupportInitialize)AimEndNote).EndInit();
            ((System.ComponentModel.ISupportInitialize)TapEndMult).EndInit();
            ((System.ComponentModel.ISupportInitialize)AimEndSlider).EndInit();
            ((System.ComponentModel.ISupportInitialize)AimEndMult).EndInit();
            ((System.ComponentModel.ISupportInitialize)RatingOffset).EndInit();
            ((System.ComponentModel.ISupportInitialize)EndDrain).EndInit();
            ((System.ComponentModel.ISupportInitialize)MaxTime).EndInit();
            ((System.ComponentModel.ISupportInitialize)BiasMult).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button ButtonForceRefresh;
        private CheckBox checkboxAllSpeed;
        private TextBox FilterMapName;
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
        private TextBox FilterPlayerName;
        private Label label4;
        private Button ButtonSaveTo;
        private NumericUpDown AimNum;
        private Label label5;
        private Label label6;
        private Label label7;
        private NumericUpDown TapNum;
        private NumericUpDown AccNum;
        private NumericUpDown AimEndNote;
        private NumericUpDown TapEndMult;
        private NumericUpDown AimEndSlider;
        private NumericUpDown AimEndMult;
        private NumericUpDown RatingOffset;
        private Label label8;
        private Label label9;
        private Label label10;
        private Label label11;
        private Label label12;
        private Label label13;
        private NumericUpDown EndDrain;
        private NumericUpDown MaxTime;
        private Label label14;
        private CheckBox FilterModifierOnly;
        private NumericUpDown BiasMult;
        private Label label15;
    }
}