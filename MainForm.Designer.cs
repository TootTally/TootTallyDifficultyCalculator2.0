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
            ComboBoxSongName = new ComboBox();
            ButtonLoadChart = new Button();
            ListboxMapData = new ListBox();
            ComboBoxReplay = new ComboBox();
            ButtonLoadReplay = new Button();
            checkboxAllSpeed = new CheckBox();
            textBox1 = new TextBox();
            label1 = new Label();
            SuspendLayout();
            // 
            // ComboBoxSongName
            // 
            ComboBoxSongName.BackColor = Color.DarkRed;
            ComboBoxSongName.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBoxSongName.FlatStyle = FlatStyle.Popup;
            ComboBoxSongName.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            ComboBoxSongName.ForeColor = SystemColors.Window;
            ComboBoxSongName.FormattingEnabled = true;
            ComboBoxSongName.Location = new Point(130, 12);
            ComboBoxSongName.Name = "ComboBoxSongName";
            ComboBoxSongName.Size = new Size(281, 25);
            ComboBoxSongName.TabIndex = 0;
            ComboBoxSongName.SelectedValueChanged += OnDropDownSongNameValueChange;
            // 
            // ButtonLoadChart
            // 
            ButtonLoadChart.BackColor = Color.Crimson;
            ButtonLoadChart.FlatStyle = FlatStyle.Popup;
            ButtonLoadChart.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            ButtonLoadChart.ForeColor = Color.White;
            ButtonLoadChart.Location = new Point(12, 12);
            ButtonLoadChart.Name = "ButtonLoadChart";
            ButtonLoadChart.Size = new Size(112, 25);
            ButtonLoadChart.TabIndex = 1;
            ButtonLoadChart.Text = "LOAD CHART";
            ButtonLoadChart.UseVisualStyleBackColor = false;
            ButtonLoadChart.Visible = false;
            ButtonLoadChart.Click += OnLoadChartButtonClick;
            // 
            // ListboxMapData
            // 
            ListboxMapData.BackColor = Color.DarkRed;
            ListboxMapData.BorderStyle = BorderStyle.FixedSingle;
            ListboxMapData.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            ListboxMapData.ForeColor = Color.White;
            ListboxMapData.FormattingEnabled = true;
            ListboxMapData.ItemHeight = 17;
            ListboxMapData.Location = new Point(12, 247);
            ListboxMapData.Name = "ListboxMapData";
            ListboxMapData.ScrollAlwaysVisible = true;
            ListboxMapData.Size = new Size(1085, 699);
            ListboxMapData.TabIndex = 2;
            ListboxMapData.Visible = false;
            // 
            // ComboBoxReplay
            // 
            ComboBoxReplay.BackColor = Color.DarkRed;
            ComboBoxReplay.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBoxReplay.FlatStyle = FlatStyle.Popup;
            ComboBoxReplay.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            ComboBoxReplay.ForeColor = SystemColors.Window;
            ComboBoxReplay.FormattingEnabled = true;
            ComboBoxReplay.Location = new Point(417, 12);
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
            ButtonLoadReplay.Location = new Point(704, 12);
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
            checkboxAllSpeed.Location = new Point(12, 110);
            checkboxAllSpeed.Name = "checkboxAllSpeed";
            checkboxAllSpeed.Size = new Size(75, 19);
            checkboxAllSpeed.TabIndex = 3;
            checkboxAllSpeed.Text = "All Speed";
            checkboxAllSpeed.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(130, 43);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(281, 23);
            textBox1.TabIndex = 4;
            textBox1.KeyPress += OnTextBoxTextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            label1.ForeColor = Color.White;
            label1.Location = new Point(40, 46);
            label1.Name = "label1";
            label1.Size = new Size(60, 20);
            label1.TabIndex = 5;
            label1.Text = "FILTER:";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Desktop;
            ClientSize = new Size(1109, 965);
            Controls.Add(label1);
            Controls.Add(textBox1);
            Controls.Add(checkboxAllSpeed);
            Controls.Add(ListboxMapData);
            Controls.Add(ButtonLoadReplay);
            Controls.Add(ButtonLoadChart);
            Controls.Add(ComboBoxReplay);
            Controls.Add(ComboBoxSongName);
            Name = "MainForm";
            Text = "Main";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox ComboBoxSongName;
        private Button ButtonLoadChart;
        private ListBox ListboxMapData;
        private ComboBox ComboBoxReplay;
        private Button ButtonLoadReplay;
        private CheckBox checkboxAllSpeed;
        private TextBox textBox1;
        private Label label1;
    }
}