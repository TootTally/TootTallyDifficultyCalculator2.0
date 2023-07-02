namespace TootTallyDifficultyCalculator2._0
{
    partial class Graph
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
            ChartData = new Syncfusion.Windows.Forms.Chart.ChartControl();
            SuspendLayout();
            // 
            // ChartData
            // 
            ChartData.ChartArea.CursorLocation = new Point(0, 0);
            ChartData.ChartArea.CursorReDraw = false;
            ChartData.IsWindowLess = false;
            // 
            // 
            // 
            ChartData.Legend.Location = new Point(1046, 75);
            ChartData.Localize = null;
            ChartData.Location = new Point(-1, -4);
            ChartData.Name = "ChartData";
            ChartData.PrimaryXAxis.LogLabelsDisplayMode = Syncfusion.Windows.Forms.Chart.LogLabelsDisplayMode.Default;
            ChartData.PrimaryXAxis.Margin = true;
            ChartData.PrimaryYAxis.LogLabelsDisplayMode = Syncfusion.Windows.Forms.Chart.LogLabelsDisplayMode.Default;
            ChartData.PrimaryYAxis.Margin = true;
            ChartData.Size = new Size(1155, 612);
            ChartData.TabIndex = 0;
            ChartData.Text = "chartControl1";
            // 
            // 
            // 
            ChartData.Title.Name = "Default";
            ChartData.Titles.Add(ChartData.Title);
            ChartData.VisualTheme = "";
            // 
            // Graph
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1154, 609);
            Controls.Add(ChartData);
            Name = "Graph";
            Text = "Graph";
            ResumeLayout(false);
        }

        #endregion

        private Syncfusion.Windows.Forms.Chart.ChartControl ChartData;
    }
}