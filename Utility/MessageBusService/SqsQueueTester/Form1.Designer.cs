namespace SqsQueueTester
{
    partial class Form1
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
            this.btnPush = new System.Windows.Forms.Button();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.btnHeartbeat = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnPush
            // 
            this.btnPush.Location = new System.Drawing.Point(12, 25);
            this.btnPush.Name = "btnPush";
            this.btnPush.Size = new System.Drawing.Size(138, 23);
            this.btnPush.TabIndex = 0;
            this.btnPush.Text = "Push";
            this.btnPush.UseVisualStyleBackColor = true;
            this.btnPush.Click += new System.EventHandler(this.btnPush_Click);
            // 
            // txtResult
            // 
            this.txtResult.Location = new System.Drawing.Point(12, 71);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResult.Size = new System.Drawing.Size(988, 417);
            this.txtResult.TabIndex = 1;
            // 
            // btnHeartbeat
            // 
            this.btnHeartbeat.Location = new System.Drawing.Point(195, 25);
            this.btnHeartbeat.Name = "btnHeartbeat";
            this.btnHeartbeat.Size = new System.Drawing.Size(147, 23);
            this.btnHeartbeat.TabIndex = 2;
            this.btnHeartbeat.Text = "Heart Beat";
            this.btnHeartbeat.UseVisualStyleBackColor = true;
            this.btnHeartbeat.Click += new System.EventHandler(this.btnHeartbeat_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1012, 500);
            this.Controls.Add(this.btnHeartbeat);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.btnPush);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPush;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Button btnHeartbeat;
    }
}

