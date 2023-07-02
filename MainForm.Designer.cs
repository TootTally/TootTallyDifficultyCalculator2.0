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
            ComboBoxSongName.Location = new Point(12, 12);
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
            ButtonLoadChart.Location = new Point(299, 12);
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
            ListboxMapData.Location = new Point(12, 43);
            ListboxMapData.Name = "ListboxMapData";
            ListboxMapData.ScrollAlwaysVisible = true;
            ListboxMapData.Size = new Size(638, 393);
            ListboxMapData.TabIndex = 2;
            ListboxMapData.Visible = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Desktop;
            ClientSize = new Size(656, 447);
            Controls.Add(ListboxMapData);
            Controls.Add(ButtonLoadChart);
            Controls.Add(ComboBoxSongName);
            Name = "MainForm";
            Text = "Main";
            ResumeLayout(false);
        }

        #endregion

        private ComboBox ComboBoxSongName;
        private Button ButtonLoadChart;
        private ListBox ListboxMapData;
    }
}