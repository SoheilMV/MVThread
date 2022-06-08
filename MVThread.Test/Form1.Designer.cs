
namespace MVThread.Test
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.numBot = new System.Windows.Forms.NumericUpDown();
            this.listData = new System.Windows.Forms.ListBox();
            this.pbChecked = new System.Windows.Forms.ProgressBar();
            this.lblPercentage = new System.Windows.Forms.Label();
            this.lblChecked = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.numBot)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(12, 12);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(93, 12);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(174, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Bot :";
            // 
            // numBot
            // 
            this.numBot.Location = new System.Drawing.Point(211, 12);
            this.numBot.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numBot.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numBot.Name = "numBot";
            this.numBot.Size = new System.Drawing.Size(130, 23);
            this.numBot.TabIndex = 3;
            this.numBot.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // listData
            // 
            this.listData.FormattingEnabled = true;
            this.listData.ItemHeight = 15;
            this.listData.Location = new System.Drawing.Point(12, 41);
            this.listData.Name = "listData";
            this.listData.Size = new System.Drawing.Size(329, 199);
            this.listData.TabIndex = 4;
            // 
            // pbChecked
            // 
            this.pbChecked.Location = new System.Drawing.Point(12, 246);
            this.pbChecked.Name = "pbChecked";
            this.pbChecked.Size = new System.Drawing.Size(288, 23);
            this.pbChecked.TabIndex = 5;
            // 
            // lblPercentage
            // 
            this.lblPercentage.AutoSize = true;
            this.lblPercentage.Location = new System.Drawing.Point(306, 250);
            this.lblPercentage.Name = "lblPercentage";
            this.lblPercentage.Size = new System.Drawing.Size(23, 15);
            this.lblPercentage.TabIndex = 2;
            this.lblPercentage.Text = "0%";
            // 
            // lblChecked
            // 
            this.lblChecked.AutoSize = true;
            this.lblChecked.Location = new System.Drawing.Point(12, 282);
            this.lblChecked.Name = "lblChecked";
            this.lblChecked.Size = new System.Drawing.Size(68, 15);
            this.lblChecked.TabIndex = 2;
            this.lblChecked.Text = "Checked : 0";
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(353, 306);
            this.Controls.Add(this.pbChecked);
            this.Controls.Add(this.listData);
            this.Controls.Add(this.numBot);
            this.Controls.Add(this.lblPercentage);
            this.Controls.Add(this.lblChecked);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Test";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numBot)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numBot;
        private System.Windows.Forms.ListBox listData;
        private System.Windows.Forms.ProgressBar pbChecked;
        private System.Windows.Forms.Label lblPercentage;
        private System.Windows.Forms.Label lblChecked;
        private System.Windows.Forms.Timer timer;
    }
}

